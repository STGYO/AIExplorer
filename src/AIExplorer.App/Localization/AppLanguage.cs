namespace AIExplorer_App.Localization;

public static class AppLanguage
{
    public const string EnglishUnitedStates = "en-US";
    public const string ChineseSimplified = "zh-CN";

    public static IReadOnlyList<string> Supported { get; } = [EnglishUnitedStates, ChineseSimplified];

    public static string NormalizeOrDefault(string? language) =>
        Supported.Contains(language, StringComparer.OrdinalIgnoreCase)
            ? Supported.First(x => string.Equals(x, language, StringComparison.OrdinalIgnoreCase))
            : EnglishUnitedStates;
}
