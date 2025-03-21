using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using SimpleStore.Core.Interfaces;
using SimpleStore.Core.Models;
using SimpleStore.Core.ViewModels;
using SimpleStore.Maui.Client.Messages;
using SimpleStore.Maui.Client.Services;

namespace SimpleStore.Maui.Client.ViewModels;

public class AuthViewModel :
    ViewModel,
    IRecipient<CheckoutCompletedMessage>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IDialogService _dialogService;
    private readonly AppState _appState;

    public event Action<bool>? AuthStateChanged;

    private bool _isAuthenticated;

    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        private set
        {
            if (_isAuthenticated == value)
                return;

            _isAuthenticated = value;
            OnPropertyChanged();
            AuthStateChanged?.Invoke(_isAuthenticated);
        }
    }

    private string _username = string.Empty;

    public string Username
    {
        get => _username;
        set
        {
            if (_username == value)
                return;

            _username = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsFormValid));
        }
    }

    private string _password = string.Empty;

    public string Password
    {
        get => _password;
        set
        {
            if (_password == value)
                return;

            _password = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsFormValid));
        }
    }

    public bool IsFormValid => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

    public ICommand SignInCommand { get; }
    public ICommand SignUpCommand { get; }

    public AuthViewModel(
        IAuthenticationService authenticationService,
        IDialogService dialogService,
        AppState appState)
    {
        _authenticationService = authenticationService;
        _dialogService = dialogService;
        _appState = appState;

        SignInCommand = new Command(SignIn);
        SignUpCommand = new Command(SignUp);

        WeakReferenceMessenger.Default.Register(this);
    }

    private async void SignIn()
    {
        Customer? customer = _authenticationService.SignIn(Username, Password);
        if (customer is null)
        {
            await _dialogService.ShowAlertAsync("Sign in failed", "Invalid username or password.", "OK");
            return;
        }

        OnAuthenticated(customer);
    }

    private async void SignUp()
    {
        Customer? customer = _authenticationService.SignUp(Username, Password);
        if (customer is null)
        {
            await _dialogService.ShowAlertAsync("Sign up failed", "Username taken.", "OK");
            return;
        }

        OnAuthenticated(customer);
    }

    public void SignOut()
    {
        _appState.Customer = null;
        WeakReferenceMessenger.Default.Send(new ClearCartMessage());

        MainThread.BeginInvokeOnMainThread(() => IsAuthenticated = false);
    }

    private void OnAuthenticated(Customer customer)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _appState.Customer = customer;
            IsAuthenticated = true;
            ClearEntries();
        });
    }

    private void ClearEntries()
    {
        Username = string.Empty;
        Password = string.Empty;
    }

    #region IRecipient<CheckoutCompletedMessage>

    public void Receive(CheckoutCompletedMessage message)
    {
        if (_appState.Customer is null)
            return;

        _authenticationService.UpdateCustomer(_appState.Customer);
    }

    #endregion
}