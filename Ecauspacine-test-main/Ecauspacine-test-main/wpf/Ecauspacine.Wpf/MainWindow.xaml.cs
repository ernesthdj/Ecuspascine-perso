using System.Windows;
using Ecauspacine.Wpf.ViewModels;

namespace Ecauspacine.Wpf;

/// <summary>
/// Fenêtre principale. Le ViewModel est injecté par DI (cf. App.xaml.cs).
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(HealthViewModel healthVm)
    {
        InitializeComponent();

        // Affecte le VM au DataContext de la fenêtre (hérite à HealthView)
        DataContext = healthVm;

        // Récupère le bouton dans la HealthView et lui assigne un handler simple.
        var btn = (System.Windows.Controls.Button)HealthView.FindName("CheckButton");
        btn.Click += async (_, __) => await healthVm.CheckAsync();
    }
}
