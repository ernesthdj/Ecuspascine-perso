
using Ecauspacine.Wpf.ViewModels.Base;

namespace Ecauspacine.Wpf.ViewModels.Dashboard;

public class DashboardTabViewModel : ViewModelBase
{
    public DashboardTabViewModel(string title, ViewModelBase content)
    {
        Title = title;
        Content = content;
    }

    public string Title { get; }
    public ViewModelBase Content { get; }

    private bool _isInitialized;
    public bool IsInitialized
    {
        get => _isInitialized;
        set => SetProperty(ref _isInitialized, value);
    }
}
