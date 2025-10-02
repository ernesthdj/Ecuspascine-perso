using System;
using System.Threading.Tasks;
using Ecauspacine.Wpf.ViewModels.Base;
using Ecauspacine.Wpf.Services.Interfaces;
// Exemple (quand tu auras des DTO dans Contracts) :
// using Ecauspacine.Contracts.Health;

namespace Ecauspacine.Wpf.ViewModels;

/// <summary>
/// ViewModel dédié au contrôle de santé de l\'API.
/// </summary>
public class HealthViewModel : ViewModelBase
{
    private readonly IHealthService _healthService;

    private string _statusText = "Unknown";
    /// <summary>Texte affiché (résultat JSON brut ou erreur).</summary>
    public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

    private bool _isBusy;
    /// <summary>Indique si un appel réseau est en cours (lié à la ProgressBar).</summary>
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public HealthViewModel(IHealthService healthService)
    {
        _healthService = healthService;
    }

    /// <summary>Appelle l\'API via le service pour récupérer l\'état de santé.</summary>
    public async Task CheckAsync()
    {
        try
        {
            IsBusy = true;
            StatusText = await _healthService.CheckAsync();
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally { IsBusy = false; }
    }
}
