
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ecauspacine.Wpf.Helpers;
using Ecauspacine.Wpf.ViewModels.Base;

namespace Ecauspacine.Wpf.ViewModels.Dashboard;

public class DashboardViewModel : ViewModelBase, IInitializable
{
    private readonly SchemaViewModel _schema;
    private readonly DataViewModel _data;
    private readonly ExportViewModel _export;

    public DashboardViewModel(SchemaViewModel schema, DataViewModel data, ExportViewModel export)
    {
        _schema = schema;
        _data = data;
        _export = export;

        Tabs = new ObservableCollection<DashboardTabViewModel>
        {
            new("Schémas", _schema),
            new("Données", _data),
            new("Génération", _export)
        };

        ShowSchemaCommand = new RelayCommand(_ => SelectedTab = Tabs.FirstOrDefault(t => t.Content is SchemaViewModel), _ => Tabs.Any());
        ShowExportCommand = new RelayCommand(_ => SelectedTab = Tabs.FirstOrDefault(t => t.Content is ExportViewModel), _ => Tabs.Any());

        foreach (var tab in Tabs)
        {
            if (tab.Content is DataViewModel dataVm)
                dataVm.RequestEntityReload += OnEntityReloadRequested;
        }
    }

    public ObservableCollection<DashboardTabViewModel> Tabs { get; }

    private DashboardTabViewModel? _selectedTab;
    public DashboardTabViewModel? SelectedTab
    {
        get => _selectedTab;
        set
        {
            if (SetProperty(ref _selectedTab, value) && value is not null)
                _ = EnsureTabInitializedAsync(value);
        }
    }

    public ICommand ShowSchemaCommand { get; }
    public ICommand ShowExportCommand { get; }

    public async Task InitializeAsync()
    {
        if (!Tabs.Any()) return;
        SelectedTab = Tabs.First();
        await EnsureTabInitializedAsync(SelectedTab!);
    }

    private async Task EnsureTabInitializedAsync(DashboardTabViewModel tab)
    {
        if (tab.IsInitialized) return;
        if (tab.Content is IInitializable init)
            await init.InitializeAsync();
        tab.IsInitialized = true;
    }

    public void Reset()
    {
        foreach (var tab in Tabs)
        {
            tab.IsInitialized = false;
            if (tab.Content is IInitializable init)
                init.Reset();
        }
        _selectedTab = null;
        OnPropertyChanged(nameof(SelectedTab));
    }

    private void OnEntityReloadRequested()
    {
        if (Tabs.FirstOrDefault()?.Content is SchemaViewModel schemaVm)
        {
            _ = schemaVm.InitializeAsync();
        }
    }
}
