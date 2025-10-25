using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Ecauspacine.Contracts.Attributes;
using Ecauspacine.Contracts.Entities;
using Ecauspacine.Wpf.Helpers;
using Ecauspacine.Wpf.Services.Interfaces;
using Ecauspacine.Wpf.ViewModels.Base;

namespace Ecauspacine.Wpf.ViewModels.Dashboard;

public class DataViewModel : ViewModelBase, IInitializable
{
    private readonly ISchemaService _schemaService;
    private readonly IDataService _dataService;

    private readonly Dictionary<long, AttributeDefDto> _definitions = new();
    private readonly Dictionary<long, IReadOnlyList<AttributeRuleDto>> _rulesByEntity = new();

    public DataViewModel(ISchemaService schemaService, IDataService dataService)
    {
        _schemaService = schemaService;
        _dataService = dataService;

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
        _definitions.Clear();
        _rulesByEntity.Clear();
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

            if (!_rulesByEntity.TryGetValue(SelectedEntityType.Id, out var rules))
            {
                rules = await _schemaService.GetAttributeRulesAsync(SelectedEntityType.Id);
                _rulesByEntity[SelectedEntityType.Id] = rules;
            }

            foreach (var rule in rules)
            {
                if (!_definitions.ContainsKey(rule.AttributeDefId))
                {
                    var defs = await _schemaService.GetAttributeDefinitionsAsync();
                    foreach (var def in defs)
                        _definitions[def.Id] = def;
                    break;
                }
            }

            var records = await _dataService.GetRecordsAsync(SelectedEntityType.Id, Search, SortAttribute, SortDescending);
            Records.Clear();
            foreach (var record in records)
            {
                Records.Add(new RecordSummary(record, BuildRecordJson(record, rules)));
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
            var payload = await BuildRecordPayloadAsync(SelectedEntityType.Id, NewRecordJson);
            var dto = new EntityRecordCreateDto(SelectedEntityType.Id, null, payload);
            var created = await _dataService.CreateRecordAsync(dto);
            if (_rulesByEntity.TryGetValue(SelectedEntityType.Id, out var rules))
            {
                Records.Add(new RecordSummary(created, BuildRecordJson(created, rules)));
            }
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
            var payload = await BuildRecordPayloadAsync(SelectedEntityType.Id, EditRecordJson);
            var dto = new EntityRecordUpdateDto(null, payload);
            var updated = await _dataService.UpdateRecordAsync(SelectedEntityType.Id, SelectedRecord.Dto.Id, dto);
            if (updated is not null && _rulesByEntity.TryGetValue(SelectedEntityType.Id, out var rules))
            {
                var summary = new RecordSummary(updated, BuildRecordJson(updated, rules));
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

    private async Task<IReadOnlyList<AttributeValueUpsertDto>> BuildRecordPayloadAsync(long entityTypeId, string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException("Fournissez un JSON valide.");

        var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException("Le JSON doit représenter un objet.");

        if (!_rulesByEntity.TryGetValue(entityTypeId, out var rules))
        {
            rules = await _schemaService.GetAttributeRulesAsync(entityTypeId);
            _rulesByEntity[entityTypeId] = rules;
        }

        var list = new List<AttributeValueUpsertDto>();
        foreach (var rule in rules)
        {
            if (!_definitions.TryGetValue(rule.AttributeDefId, out var def))
                continue;

            if (!doc.RootElement.TryGetProperty(def.Code, out var valueElement))
                continue;

            var upsert = rule.AttributeCode switch
            {
                _ when def.DataKind == Ecauspacine.Contracts.Common.DataKindCodes.Enum => new AttributeValueUpsertDto(0, def.Id, def.DataKind, null, valueElement.GetInt64(), null),
                _ when def.DataKind == Ecauspacine.Contracts.Common.DataKindCodes.EntityReference => new AttributeValueUpsertDto(0, def.Id, def.DataKind, null, null, valueElement.GetInt64()),
                _ => new AttributeValueUpsertDto(0, def.Id, def.DataKind, valueElement, null, null)
            };

            list.Add(upsert);
        }

        return list;
    }

    private string BuildRecordJson(EntityRecordDto record, IReadOnlyList<AttributeRuleDto> rules)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var rule in rules)
        {
            if (!_definitions.TryGetValue(rule.AttributeDefId, out var def))
                continue;
            var val = record.Values.FirstOrDefault(v => v.AttributeDefId == rule.AttributeDefId);
            if (val is null)
                continue;

            object? value = val.DataKind switch
            {
                Ecauspacine.Contracts.Common.DataKindCodes.Enum => val.LookupItemId,
                Ecauspacine.Contracts.Common.DataKindCodes.EntityReference => val.RefEntityId,
                _ => val.JsonValue?.ToString()
            };
            dict[def.Code] = value;
        }

        return JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
    }

    public class RecordSummary
    {
        public RecordSummary(EntityRecordDto dto, string json)
        {
            Dto = dto;
            Json = json;
        }

        public EntityRecordDto Dto { get; }
        public string Json { get; }
        public override string ToString() => Json;
    }
}
