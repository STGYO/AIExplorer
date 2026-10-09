using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AIExplorer_App.Localization;

public static class VisualLocalizer
{
    public static void Apply(DependencyObject? root)
    {
        if (root is null)
        {
            return;
        }

        TranslateSelf(root);
        TranslateFlyout(root);

        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            Apply(VisualTreeHelper.GetChild(root, i));
        }
    }

    public static void LocalizeDialog(ContentDialog dialog)
    {
        dialog.Title = TranslateObject(dialog.Title);
        dialog.Content = TranslateObject(dialog.Content);
        dialog.PrimaryButtonText = AppLocalizer.Instance.TranslateLiteral(dialog.PrimaryButtonText);
        dialog.SecondaryButtonText = AppLocalizer.Instance.TranslateLiteral(dialog.SecondaryButtonText);
        dialog.CloseButtonText = AppLocalizer.Instance.TranslateLiteral(dialog.CloseButtonText);
    }

    private static object? TranslateObject(object? value) => value switch
    {
        string text => AppLocalizer.Instance.TranslateLiteral(text),
        DependencyObject dependency => dependency,
        _ => value,
    };

    private static void TranslateFlyout(DependencyObject root)
    {
        if (root is FrameworkElement fe && fe.ContextFlyout is MenuFlyout menu)
        {
            LocalizeMenuFlyout(menu);
        }
    }

    private static void LocalizeMenuFlyout(MenuFlyout menu)
    {
        foreach (var item in menu.Items)
        {
            switch (item)
            {
                case MenuFlyoutItem mi:
                    mi.Text = AppLocalizer.Instance.TranslateLiteral(mi.Text);
                    break;
                case ToggleMenuFlyoutItem tmi:
                    tmi.Text = AppLocalizer.Instance.TranslateLiteral(tmi.Text);
                    break;
                case MenuFlyoutSubItem sub:
                    sub.Text = AppLocalizer.Instance.TranslateLiteral(sub.Text);
                    foreach (var subItem in sub.Items)
                    {
                        if (subItem is MenuFlyoutItem nested)
                        {
                            nested.Text = AppLocalizer.Instance.TranslateLiteral(nested.Text);
                        }
                        else if (subItem is ToggleMenuFlyoutItem nestedToggle)
                        {
                            nestedToggle.Text = AppLocalizer.Instance.TranslateLiteral(nestedToggle.Text);
                        }
                    }
                    break;
            }
        }
    }

    private static void TranslateSelf(DependencyObject root)
    {
        switch (root)
        {
            case TextBlock tb:
                tb.Text = AppLocalizer.Instance.TranslateLiteral(tb.Text);
                break;
            case Button button:
                button.Content = TranslateObject(button.Content);
                break;
            case ToggleSwitch toggle:
                toggle.Header = TranslateObject(toggle.Header);
                toggle.OnContent = TranslateObject(toggle.OnContent);
                toggle.OffContent = TranslateObject(toggle.OffContent);
                break;
            case CheckBox checkBox:
                checkBox.Content = TranslateObject(checkBox.Content);
                break;
            case ComboBox comboBox:
                comboBox.Header = TranslateObject(comboBox.Header);
                break;
            case TextBox textBox:
                textBox.Header = TranslateObject(textBox.Header);
                textBox.PlaceholderText = AppLocalizer.Instance.TranslateLiteral(textBox.PlaceholderText);
                break;
            case AutoSuggestBox autoSuggestBox:
                autoSuggestBox.PlaceholderText = AppLocalizer.Instance.TranslateLiteral(autoSuggestBox.PlaceholderText);
                break;
            case MenuFlyoutItem menuFlyoutItem:
                menuFlyoutItem.Text = AppLocalizer.Instance.TranslateLiteral(menuFlyoutItem.Text);
                break;
            case ToggleMenuFlyoutItem toggleItem:
                toggleItem.Text = AppLocalizer.Instance.TranslateLiteral(toggleItem.Text);
                break;
            case MenuFlyoutSubItem subItem:
                subItem.Text = AppLocalizer.Instance.TranslateLiteral(subItem.Text);
                break;
        }

        if (root is FrameworkElement fe)
        {
            var tooltip = ToolTipService.GetToolTip(fe);
            if (tooltip is string text)
            {
                ToolTipService.SetToolTip(fe, AppLocalizer.Instance.TranslateLiteral(text));
            }
        }
    }
}
