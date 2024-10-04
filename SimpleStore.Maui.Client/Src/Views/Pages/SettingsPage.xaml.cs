using SimpleStore.Maui.Client.ViewModels;

namespace SimpleStore.Maui.Client.Views.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly AuthViewModel _authViewModel;

    public SettingsPage(SettingsViewModel settingsViewModel, AuthViewModel authViewModel)
    {
        InitializeComponent();
        BindingContext = settingsViewModel;
        _authViewModel = authViewModel;
    }

    private void OnSignOutClicked(object? sender, EventArgs e)
    {
        _authViewModel.SignOut();
    }
}