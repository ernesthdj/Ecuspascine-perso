using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ecauspacine.Contracts.Entities;
using Ecauspacine.Wpf.Helpers;
using Ecauspacine.Wpf.Services.Interfaces;
using Ecauspacine.Wpf.ViewModels.Base;

namespace Ecauspacine.Wpf.ViewModels.Dashboard;

public class DataViewModel : ViewModelBase, IInitializable
{
    private readonly ISchemaService _schemaService;
    private readonly IDataService _dataService;
    private readonly IRecordJsonConverter _jsonConverter;

    public DataViewModel(ISchemaService schemaService, IDataService dataService, IRecordJsonConverter jsonConverter)
    {
        _schemaService = schemaService;
        _dataService = dataService;
        _jsonConverter = jsonConverter;

        EntityTypes = new ObservableCollection<EntityTypeDto>();
        Records = new ObservableCollection<RecordSummary>();

        RefreshCommand = new RelayCommand(async _ => await RefreshAsync(), _ => SelectedEntityType is not null && !IsBusy);
        CreateRecordCommand = new RelayCommand(async _ => await CreateRecordAsync(), _ => SelectedEntityType is not null && !IsBusy);
        SaveRecordCommand = new RelayCommand(async _ => await SaveRecordAsync(), _ => SelectedRecord is not null && !IsBusy);
        DeleteRecordCommand = new RelayCommand(async _ => await DeleteRecordAsync(), _ => SelectedRecord is not null && !IsBusy);
    }

    public event Action? RequestEntityReload;

    public ObservableCollection<EntityTypeDto> EntityTypes { get; }
    public ObservableCollection<RecordSummary> Records { get; }

    private EntityTypeDto? _selectedEntityType;
    public EntityTypeDto? SelectedEntityType
    {
        get => _selectedEntityType;
        set
        {
            if (SetProperty(ref _selectedEntityType, value))
            {
                _ = RefreshAsync();
            }
        }
    }

    private RecordSummary? _selectedRecord;
    public RecordSummary? SelectedRecord
    {
        get => _selectedRecord;
        set
        {
            if (SetProperty(ref _selectedRecord, value))
            {
                EditRecordJson = value?.Json;
            }
        }
    }

    private string? _search;
    public string? Search
    {
        get => _search;
        set => SetProperty(ref _search, value);
    }

    private string _newRecordJson = "{}";
    public string NewRecordJson
    {
        get => _newRecordJson;
        set => SetProperty(ref _newRecordJson, value);
    }

    private string? _editRecordJson;
    public string? EditRecordJson
    {
        get => _editRecordJson;
        set => SetProperty(ref _editRecordJson, value);
    }

    private string? _sortAttribute;
    public string? SortAttribute
    {
        get => _sortAttribute;
        set => SetProperty(ref _sortAttribute, value);
    }

    private bool _sortDesc;
    public bool SortDescending
    {
        get => _sortDesc;
        set => SetProperty(ref _sortDesc, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            SetProperty(ref _isBusy, value);
            (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CreateRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (SaveRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand CreateRecordCommand { get; }
    public ICommand SaveRecordCommand { get; }
    public ICommand DeleteRecordCommand { get; }

    public async Task InitializeAsync()
    {
        if (EntityTypes.Count > 0) return;
        await LoadEntityTypesAsync();
    }

    public void Reset()
    {
        EntityTypes.Clear();
        Records.Clear();
        SelectedEntityType = null;
        SelectedRecord = null;
        NewRecordJson = "{}";
        EditRecordJson = null;
        ErrorMessage = null;
    }

    private async Task LoadEntityTypesAsync()
    {
        var types = await _schemaService.GetEntityTypesAsync();
        EntityTypes.Clear();
        foreach (var t in types)
            EntityTypes.Add(t);
        SelectedEntityType = EntityTypes.FirstOrDefault();
    }

    private async Task RefreshAsync()
    {
        if (SelectedEntityType is null)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var records = await _dataService.GetRecordsAsync(SelectedEntityType.Id, Search, SortAttribute, SortDescending);
            Records.Clear();
            foreach (var record in records)
            {
                var json = await _jsonConverter.BuildRecordJsonAsync(record, SelectedEntityType.Id);
                Records.Add(new RecordSummary(record, json));
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

    private async Task CreateRecordAsync()
    {
        if (SelectedEntityType is null) return;
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            var payload = await _jsonConverter.BuildRecordPayloadAsync(SelectedEntityType.Id, NewRecordJson);
            var dto = new EntityRecordCreateDto(SelectedEntityType.Id, null, payload);
            var created = await _dataService.CreateRecordAsync(dto);
            var json = await _jsonConverter.BuildRecordJsonAsync(created, SelectedEntityType.Id);
            Records.Add(new RecordSummary(created, json));
            RequestEntityReload?.Invoke();
            NewRecordJson = "{}";
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

    private async Task SaveRecordAsync()
    {
        if (SelectedEntityType is null || SelectedRecord is null || string.IsNullOrWhiteSpace(EditRecordJson))
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            var payload = await _jsonConverter.BuildRecordPayloadAsync(SelectedEntityType.Id, EditRecordJson);
            var dto = new EntityRecordUpdateDto(null, payload);
            var updated = await _dataService.UpdateRecordAsync(SelectedEntityType.Id, SelectedRecord.Dto.Id, dto);
            if (updated is not null)
            {
                var json = await _jsonConverter.BuildRecordJsonAsync(updated, SelectedEntityType.Id);
                var summary = new RecordSummary(updated, json);
                var index = Records.IndexOf(SelectedRecord);
                Records[index] = summary;
                SelectedRecord = summary;
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

    private async Task DeleteRecordAsync()
    {
        if (SelectedEntityType is null || SelectedRecord is null)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await _dataService.DeleteRecordAsync(SelectedEntityType.Id, SelectedRecord.Dto.Id);
            Records.Remove(SelectedRecord);
            SelectedRecord = null;
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
}
