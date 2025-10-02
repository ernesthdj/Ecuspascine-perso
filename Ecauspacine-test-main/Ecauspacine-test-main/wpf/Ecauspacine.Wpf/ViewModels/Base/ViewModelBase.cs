using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ecauspacine.Wpf.ViewModels.Base;

/// <summary>
/// Base de tous les ViewModels pour supporter le binding WPF.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Met à jour le champ et notifie l\'UI si la valeur change.
    /// </summary>
    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>Déclenche manuellement la notification de propriété.</summary>
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
