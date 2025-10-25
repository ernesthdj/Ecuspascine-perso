
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ecauspacine.Wpf.Helpers;
using Ecauspacine.Wpf.Services.Interfaces;
using Ecauspacine.Wpf.ViewModels.Base;

namespace Ecauspacine.Wpf.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authService;

    public LoginViewModel(IAuthenticationService authService)
    {
        _authService = authService;
        LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => !IsBusy);
    }

    public event Action? LoginSucceeded;

    private string _username = "admin";
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _password = "admin";
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            SetProperty(ref _isBusy, value);
            (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var ok = await _authService.LoginAsync(Username, Password, CancellationToken.None);
            if (ok)
            {
                LoginSucceeded?.Invoke();
            }
            else
            {
                ErrorMessage = "Identifiants invalides.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void Reset()
    {
        Password = string.Empty;
        ErrorMessage = null;
    }
}
