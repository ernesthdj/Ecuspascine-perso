using System.Windows;
using Ecauspacine.Wpf.ViewModels;

namespace Ecauspacine.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(ShellViewModel shellViewModel)
    {
        InitializeComponent();
        DataContext = shellViewModel;
    }
}
