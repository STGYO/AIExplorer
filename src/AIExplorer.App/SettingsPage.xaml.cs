using AIExplorer_App.ViewModels;
using AIExplorer_App.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AIExplorer_App;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        ViewModel = App.Services.GetRequiredService<SettingsViewModel>();
        InitializeComponent();
        DataContext = ViewModel;
        ViewModel.Reload();
        Loaded += (_, _) => ApplyLocalization();
        AppLocalizer.Instance.LanguageChanged += (_, _) => ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        VisualLocalizer.Apply(this);
    }

    private void OnBackClick(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
        else
        {
            Frame.Navigate(typeof(MainPage));
        }
    }

    private async void OnSaveClick(object sender, RoutedEventArgs e)
    {
        await ViewModel.SaveCommand.ExecuteAsync(null);
        LanguageAppliedText.Visibility = Visibility.Visible;
        var dialog = new ContentDialog
        {
            Title = AppLocalizer.Instance.Get("Settings_Saved_Title"),
            Content = AppLocalizer.Instance.Get("Settings_Saved_Content"),
            CloseButtonText = AppLocalizer.Instance.Get("Settings_Close"),
            XamlRoot = XamlRoot,
        };
        VisualLocalizer.LocalizeDialog(dialog);
        await dialog.ShowAsync();
    }
}
