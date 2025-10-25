
using System.Windows.Input;
using Ecauspacine.Wpf.Helpers;
using Ecauspacine.Wpf.Services.Interfaces;
using Ecauspacine.Wpf.ViewModels.Base;

namespace Ecauspacine.Wpf.ViewModels;

public class ShellViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authService;
    private readonly LoginViewModel _loginViewModel;
    private readonly DashboardViewModel _dashboardViewModel;
    private ViewModelBase _currentView;

    public ShellViewModel(IAuthenticationService authService, LoginViewModel loginViewModel, DashboardViewModel dashboardViewModel)
    {
        _authService = authService;
        _loginViewModel = loginViewModel;
        _dashboardViewModel = dashboardViewModel;

        _loginViewModel.LoginSucceeded += HandleLoginSucceeded;
        _currentView = _loginViewModel;

        LogoutCommand = new RelayCommand(_ => Logout(), _ => IsAuthenticated);
    }

    public ViewModelBase CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    private bool _isAuthenticated;
    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        private set
        {
            SetProperty(ref _isAuthenticated, value);
            (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public ICommand LogoutCommand { get; }

    private void HandleLoginSucceeded()
    {
        IsAuthenticated = true;
        CurrentView = _dashboardViewModel;
        _ = _dashboardViewModel.InitializeAsync();
    }

    private void Logout()
    {
        _authService.Logout();
        _dashboardViewModel.Reset();
        _loginViewModel.Reset();
        IsAuthenticated = false;
        CurrentView = _loginViewModel;
    }
}
