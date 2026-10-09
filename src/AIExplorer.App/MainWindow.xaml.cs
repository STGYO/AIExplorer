using AIExplorer.Core.Settings;
using AIExplorer_App.Localization;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;

namespace AIExplorer_App;

public sealed partial class MainWindow : Window
{
    public bool IsSecondary { get; }

    public MainPage? Page => RootFrame.Content as MainPage;

    public MainWindow(bool secondary = false)
    {
        IsSecondary = secondary;
        InitializeComponent();

        ApplyBackdrop(WindowBackdropOptions.Mica);
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        ApplyWindowIcon();
        ApplyLocalization();
        AppLocalizer.Instance.LanguageChanged += OnLanguageChanged;

        ApplyDefaultWindowBounds();
        RootFrame.Navigate(typeof(MainPage), secondary);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        ApplyLocalization();
        if (RootFrame.Content is FrameworkElement element)
        {
            VisualLocalizer.Apply(element);
        }
    }

    private void ApplyLocalization()
    {
        Title = AppLocalizer.Instance.Get("App_Title");
        AppTitleText.Text = AppLocalizer.Instance.Get("App_Title");
    }

    private void ApplyWindowIcon()
    {
        try
        {
            var ico = Path.Combine(AppContext.BaseDirectory, "Assets", "AIExplorer.ico");
            if (!File.Exists(ico))
            {
                ico = Path.Combine(AppContext.BaseDirectory, "AIExplorer.ico");
            }

            if (File.Exists(ico))
            {
                AppWindow.SetIcon(ico);
            }
        }
        catch
        {
        }
    }

    /// <summary>默认占当前屏幕工作区的一半，并居中。</summary>
    private void ApplyDefaultWindowBounds()
    {
        var displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary)
                          ?? DisplayArea.Primary;
        var work = displayArea.WorkArea;

        var width = Math.Max(640, work.Width / 2);
        var height = Math.Max(480, work.Height / 2);
        AppWindow.Resize(new SizeInt32(width, height));
        AppWindow.Move(new PointInt32(
            work.X + (work.Width - width) / 2,
            work.Y + (work.Height - height) / 2));
    }

    /// <summary>切换 Mica / 亚克力 / 纯色背景（对齐 Allen 窗口背景能力的精简版）。</summary>
    public void ApplyBackdrop(string? backdrop)
    {
        var kind = WindowBackdropOptions.IsKnown(backdrop) ? backdrop! : WindowBackdropOptions.Mica;
        SystemBackdrop = kind switch
        {
            WindowBackdropOptions.Acrylic => new DesktopAcrylicBackdrop(),
            WindowBackdropOptions.None => null,
            _ => new MicaBackdrop(),
        };

        // None 时补不透明底，避免桌面内容透进来影响可读性。
        RootGrid.Background = kind == WindowBackdropOptions.None
            ? (Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"]
            : new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    }
}
