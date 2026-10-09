using System.Collections;
using System.Globalization;
using System.Resources;
using System.Text;

namespace AIExplorer_App.Localization;

public sealed class AppLocalizer
{
    private static readonly ResourceManager ResourceManager = new("AIExplorer_App.Resources.Strings", typeof(AppLocalizer).Assembly);
    private readonly Dictionary<string, string> _literalToKey = new(StringComparer.Ordinal);
    private readonly object _gate = new();
    private CultureInfo _culture = CultureInfo.GetCultureInfo(AppLanguage.EnglishUnitedStates);

    public static AppLocalizer Instance { get; } = new();

    public event EventHandler? LanguageChanged;

    public string CurrentLanguage => _culture.Name;

    private AppLocalizer()
    {
        RebuildLiteralMap();
    }

    public string ApplyLanguage(string? language)
    {
        var resolved = AppLanguage.NormalizeOrDefault(language);
        var next = CultureInfo.GetCultureInfo(resolved);
        if (string.Equals(_culture.Name, next.Name, StringComparison.OrdinalIgnoreCase))
        {
            return _culture.Name;
        }

        lock (_gate)
        {
            _culture = next;
            CultureInfo.CurrentUICulture = next;
            CultureInfo.CurrentCulture = next;
            RebuildLiteralMap();
        }

        LanguageChanged?.Invoke(this, EventArgs.Empty);
        return _culture.Name;
    }

    public string Get(string key)
    {
        var text = ResourceManager.GetString(key, _culture);
        if (!string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        text = ResourceManager.GetString(key, CultureInfo.GetCultureInfo(AppLanguage.EnglishUnitedStates));
        if (!string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        System.Diagnostics.Debug.WriteLine($"[Localization] Missing key: {key}");
        return string.Empty;
    }

    public string Format(string key, params object[] args)
    {
        var template = Get(key);
        return string.IsNullOrEmpty(template) ? string.Empty : string.Format(_culture, template, args);
    }

    public string TranslateLiteral(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text ?? string.Empty;
        }

        if (_literalToKey.TryGetValue(text, out var key))
        {
            var translated = Get(key);
            if (!string.IsNullOrWhiteSpace(translated))
            {
                return translated;
            }
        }

        return text;
    }

    public IReadOnlyList<string> ValidateTranslationCoverage(string language)
    {
        var target = CultureInfo.GetCultureInfo(AppLanguage.NormalizeOrDefault(language));
        var english = CultureInfo.GetCultureInfo(AppLanguage.EnglishUnitedStates);

        var enSet = ResourceManager.GetResourceSet(english, true, true);
        var targetSet = ResourceManager.GetResourceSet(target, true, true);
        if (enSet is null || targetSet is null)
        {
            return ["Unable to load resource sets."];
        }

        var enKeys = new HashSet<string>(EnumerateKeys(enSet), StringComparer.Ordinal);
        var targetKeys = new HashSet<string>(EnumerateKeys(targetSet), StringComparer.Ordinal);

        return enKeys
            .Where(k => !targetKeys.Contains(k))
            .OrderBy(k => k, StringComparer.Ordinal)
            .ToArray();
    }

    private static IEnumerable<string> EnumerateKeys(ResourceSet set)
    {
        foreach (DictionaryEntry entry in set)
        {
            if (entry.Key is string key)
            {
                yield return key;
            }
        }
    }

    private void RebuildLiteralMap()
    {
        _literalToKey.Clear();
        AddCultureLiteralMap(CultureInfo.GetCultureInfo(AppLanguage.EnglishUnitedStates));
        AddCultureLiteralMap(CultureInfo.GetCultureInfo(AppLanguage.ChineseSimplified));
    }

    private void AddCultureLiteralMap(CultureInfo culture)
    {
        var set = ResourceManager.GetResourceSet(culture, true, true);
        if (set is null)
        {
            return;
        }

        foreach (DictionaryEntry entry in set)
        {
            if (entry.Key is not string key ||
                entry.Value is not string value ||
                !key.StartsWith("Literal_", StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            _literalToKey[value] = key;
        }
    }
}
