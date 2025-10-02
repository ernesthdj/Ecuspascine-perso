using System;
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ecauspacine.Wpf.Services.Interfaces;
using Ecauspacine.Wpf.Services.Http;
using Ecauspacine.Wpf.Services.Implementations;
using Ecauspacine.Wpf.ViewModels;

namespace Ecauspacine.Wpf;

/// <summary>
/// Point d\'entrée WPF (côté code). On utilise Microsoft.Extensions.Hosting pour
/// bénéficier d\'un container DI moderne, comme en ASP.NET Core.
/// </summary>
public partial class App : Application
{
    /// <summary>Host global (IoC container + cycle de vie).</summary>
    public static IHost AppHost { get; private set; } = null!;

    public App()
    {
        // Crée le Host et enregistre tous les services nécessaires à l'application.
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((ctx, services) =>
            {
                // HttpClient configuré avec la base address de l'API (tunnel DEV).
                services.AddSingleton(new HttpClient { BaseAddress = new Uri("http://localhost:5001") });

                // Services applicatifs
                services.AddSingleton<IApiClient, ApiClient>();
                services.AddSingleton<IHealthService, HealthService>();

                // ViewModels : Transient = une instance par utilisation
                services.AddTransient<HealthViewModel>();

                // Windows
                services.AddTransient<MainWindow>();
            })
            .Build();
    }

    /// <summary>
    /// Démarre l\'application : on résout MainWindow via le container et on l\'affiche.
    /// </summary>
    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();

        // Résout MainWindow via DI (permet constructeur avec paramètres)
        var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    /// <summary>Arrêt propre du Host pour libérer les ressources.</summary>
    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        base.OnExit(e);
    }
}
