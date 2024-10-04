using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using SimpleStore.Core.Enums;
using SimpleStore.Core.Interfaces;
using SimpleStore.Core.ViewModels;
using SimpleStore.Maui.Client.Messages;
using SimpleStore.Maui.Client.Services;

namespace SimpleStore.Maui.Client.ViewModels;

public class SettingsViewModel : ViewModel
{
    private readonly IUserPreferences _userPreferences;
    private readonly AppState _appState;
    public ObservableCollection<Currency> Currencies { get; } = [];
    private Currency _selectedCurrency;

    public Currency SelectedCurrency
    {
        get => _selectedCurrency;
        set
        {
            _appState.Currency = value;

            if (_selectedCurrency == value)
                return;

            _selectedCurrency = value;
            OnPropertyChanged();
            _userPreferences.SaveCurrency(_selectedCurrency);
            
            WeakReferenceMessenger.Default.Send(new CurrencyChangedMessage(_selectedCurrency));
        }
    }

    public SettingsViewModel(IUserPreferences userPreferences, AppState appState)
    {
        _userPreferences = userPreferences;
        _appState = appState;

        foreach (Currency currency in Enum.GetValues(typeof(Currency)))
        {
            Currencies.Add(currency);
        }

        MainThread.BeginInvokeOnMainThread(() => SelectedCurrency = _userPreferences.LoadCurrency());
    }
}