using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Ecauspacine.Contracts.EntityTypes;
using Ecauspacine.Contracts.Exports;
using Ecauspacine.Wpf.Helpers;
using Ecauspacine.Wpf.Services.Interfaces;
using Ecauspacine.Wpf.ViewModels.Base;

namespace Ecauspacine.Wpf.ViewModels.Dashboard;

public class ExportViewModel : ViewModelBase, IInitializable
{
    private readonly ISchemaService _schemaService;
    private readonly IExportClient _exportClient;

    public ExportViewModel(ISchemaService schemaService, IExportClient exportClient)
    {
        _schemaService = schemaService;
        _exportClient = exportClient;

        EntityTypes = new ObservableCollection<EntityTypeDto>();
        GeneratedFiles = new ObservableCollection<string>();
        TargetDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EddsExports");

        BrowseCommand = new RelayCommand(_ => BrowseForDirectory());
        ExportCommand = new RelayCommand(async _ => await ExportAsync(), _ => CanExport());
    }

    public ObservableCollection<EntityTypeDto> EntityTypes { get; }
    public ObservableCollection<string> GeneratedFiles { get; }

    private EntityTypeDto? _selectedEntityType;
    public EntityTypeDto? SelectedEntityType
    {
        get => _selectedEntityType;
        set
        {
            if (SetProperty(ref _selectedEntityType, value))
                RaiseCanExecute();
        }
    }

    private bool _includeCsv = true;
    public bool IncludeCsv
    {
        get => _includeCsv;
        set
        {
            if (SetProperty(ref _includeCsv, value))
                RaiseCanExecute();
        }
    }

    private bool _includeJson = true;
    public bool IncludeJson
    {
        get => _includeJson;
        set
        {
            if (SetProperty(ref _includeJson, value))
                RaiseCanExecute();
        }
    }

    private bool _includeXml;
    public bool IncludeXml
    {
        get => _includeXml;
        set
        {
            if (SetProperty(ref _includeXml, value))
                RaiseCanExecute();
        }
    }

    private string _targetDirectory;
    public string TargetDirectory
    {
        get => _targetDirectory;
        set
        {
            if (SetProperty(ref _targetDirectory, value))
                RaiseCanExecute();
        }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            SetProperty(ref _isBusy, value);
            RaiseCanExecute();
        }
    }

    private string? _statusMessage;
    public string? StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public ICommand BrowseCommand { get; }
    public ICommand ExportCommand { get; }

    public async Task InitializeAsync()
    {
        if (EntityTypes.Count > 0)
            return;

        var entities = await _schemaService.GetEntityTypesAsync();
        EntityTypes.Clear();
        foreach (var entity in entities)
            EntityTypes.Add(entity);
        SelectedEntityType = EntityTypes.FirstOrDefault();
    }

    public void Reset()
    {
        GeneratedFiles.Clear();
        StatusMessage = null;
        ErrorMessage = null;
        IsBusy = false;
    }

    private void BrowseForDirectory()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Sélectionnez le dossier d'export",
            SelectedPath = Directory.Exists(TargetDirectory) ? TargetDirectory : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            TargetDirectory = dialog.SelectedPath;
        }
    }

    private bool CanExport()
    {
        return !IsBusy
               && SelectedEntityType is not null
               && !string.IsNullOrWhiteSpace(TargetDirectory)
               && (IncludeCsv || IncludeJson || IncludeXml);
    }

    private async Task ExportAsync()
    {
        if (!CanExport())
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            StatusMessage = null;
            GeneratedFiles.Clear();

            var formats = new[]
            {
                IncludeCsv ? ExportFormat.Csv : (ExportFormat?)null,
                IncludeJson ? ExportFormat.Json : (ExportFormat?)null,
                IncludeXml ? ExportFormat.Xml : (ExportFormat?)null
            }
            .Where(f => f.HasValue)
            .Select(f => f!.Value)
            .ToList();

            var result = await _exportClient.ExportAsync(SelectedEntityType!.Id, formats, TargetDirectory);
            StatusMessage = $"Archive générée : {result.ArchiveFileName}";
            foreach (var file in result.GeneratedFiles)
                GeneratedFiles.Add(file);
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

    private void RaiseCanExecute()
    {
        (ExportCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
