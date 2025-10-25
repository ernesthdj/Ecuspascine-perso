using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Ecauspacine.Contracts.Attributes;
using Ecauspacine.Contracts.EntityTypes;
using Ecauspacine.Contracts.Lookups;
using Ecauspacine.Wpf.Helpers;
using Ecauspacine.Wpf.Services.Interfaces;
using Ecauspacine.Wpf.ViewModels.Base;

namespace Ecauspacine.Wpf.ViewModels.Dashboard.Schema;

/// <summary>
/// ViewModel responsible for managing Attributes for a selected EntityType
/// </summary>
public class AttributeManagementViewModel : ViewModelBase
{
    private readonly ISchemaService _schemaService;
    private readonly ILookupClient _lookupClient;
    private readonly Dictionary<long, AttributeDefDto> _definitionCache = new();

    public AttributeManagementViewModel(ISchemaService schemaService, ILookupClient lookupClient)
    {
        _schemaService = schemaService;
        _lookupClient = lookupClient;

        Attributes = new ObservableCollection<AttributeRuleDisplayViewModel>();
        LookupGroups = new ObservableCollection<LookupGroupDto>();
        AvailableLookupItems = new ObservableCollection<LookupItemDto>();

        AddAttributeCommand = new RelayCommand(async _ => await AddAttributeAsync(), _ => CanAddAttribute());
        SaveAttributeCommand = new RelayCommand(async _ => await SaveAttributeAsync(), _ => SelectedAttribute is not null && !IsBusy);
        DeleteAttributeCommand = new RelayCommand(async _ => await DeleteAttributeAsync(), _ => SelectedAttribute is not null && !IsBusy);

        SelectedAccessMode = SchemaOptions.AccessModes.First();
        SelectedDataKind = SchemaOptions.DataKinds.First();
        NewAttributeOrder = 1;
    }

    public ObservableCollection<AttributeRuleDisplayViewModel> Attributes { get; }
    public ObservableCollection<LookupGroupDto> LookupGroups { get; }
    public ObservableCollection<LookupItemDto> AvailableLookupItems { get; }

    public IReadOnlyList<DataKindOption> DataKinds => SchemaOptions.DataKinds;
    public IReadOnlyList<AccessModeOption> AccessModes => SchemaOptions.AccessModes;

    private long? _currentEntityTypeId;

    private AttributeRuleDisplayViewModel? _selectedAttribute;
    public AttributeRuleDisplayViewModel? SelectedAttribute
    {
        get => _selectedAttribute;
        set => SetProperty(ref _selectedAttribute, value);
    }

    // New attribute properties
    private string _newAttributeCode = string.Empty;
    public string NewAttributeCode
    {
        get => _newAttributeCode;
        set => SetProperty(ref _newAttributeCode, value);
    }

    private string _newAttributeLabel = string.Empty;
    public string NewAttributeLabel
    {
        get => _newAttributeLabel;
        set => SetProperty(ref _newAttributeLabel, value);
    }

    private string? _newAttributeDescription;
    public string? NewAttributeDescription
    {
        get => _newAttributeDescription;
        set => SetProperty(ref _newAttributeDescription, value);
    }

    private DataKindOption? _selectedDataKind;
    public DataKindOption? SelectedDataKind
    {
        get => _selectedDataKind;
        set
        {
            if (SetProperty(ref _selectedDataKind, value))
            {
                UpdateDataKindDependencies();
            }
        }
    }

    private AccessModeOption? _selectedAccessMode;
    public AccessModeOption? SelectedAccessMode
    {
        get => _selectedAccessMode;
        set => SetProperty(ref _selectedAccessMode, value);
    }

    private bool _newAttributeIsRequired;
    public bool NewAttributeIsRequired
    {
        get => _newAttributeIsRequired;
        set => SetProperty(ref _newAttributeIsRequired, value);
    }

    private int _newAttributeOrder;
    public int NewAttributeOrder
    {
        get => _newAttributeOrder;
        set => SetProperty(ref _newAttributeOrder, value);
    }

    private string? _newAttributeDefault;
    public string? NewAttributeDefault
    {
        get => _newAttributeDefault;
        set => SetProperty(ref _newAttributeDefault, value);
    }

    private LookupGroupDto? _selectedLookupGroup;
    public LookupGroupDto? SelectedLookupGroup
    {
        get => _selectedLookupGroup;
        set
        {
            if (SetProperty(ref _selectedLookupGroup, value))
            {
                RefreshLookupItems();
            }
        }
    }

    private LookupItemDto? _selectedLookupItem;
    public LookupItemDto? SelectedLookupItem
    {
        get => _selectedLookupItem;
        set => SetProperty(ref _selectedLookupItem, value);
    }

    private ObservableCollection<Contracts.EntityTypes.EntityTypeDto>? _entityTypes;
    public ObservableCollection<Contracts.EntityTypes.EntityTypeDto>? EntityTypes
    {
        get => _entityTypes;
        set => SetProperty(ref _entityTypes, value);
    }

    private Contracts.EntityTypes.EntityTypeDto? _selectedReferenceType;
    public Contracts.EntityTypes.EntityTypeDto? SelectedReferenceType
    {
        get => _selectedReferenceType;
        set => SetProperty(ref _selectedReferenceType, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
            {
                RefreshCommandStates();
            }
        }
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public ICommand AddAttributeCommand { get; }
    public ICommand SaveAttributeCommand { get; }
    public ICommand DeleteAttributeCommand { get; }

    public async Task InitializeAsync()
    {
        await LoadLookupGroupsAsync();
        await LoadDefinitionsAsync();
    }

    public async Task LoadAttributesForEntityAsync(long entityTypeId)
    {
        _currentEntityTypeId = entityTypeId;
        await LoadAttributesAsync();
    }

    private async Task LoadLookupGroupsAsync()
    {
        var groups = await _lookupClient.GetGroupsAsync(includeItems: true);
        LookupGroups.Clear();
        foreach (var group in groups)
            LookupGroups.Add(group);
    }

    private async Task LoadDefinitionsAsync()
    {
        _definitionCache.Clear();
        var definitions = await _schemaService.GetAttributeDefinitionsAsync();
        foreach (var def in definitions)
            _definitionCache[def.Id] = def;
    }

    private async Task LoadAttributesAsync()
    {
        Attributes.Clear();
        if (!_currentEntityTypeId.HasValue)
            return;

        var rules = await _schemaService.GetAttributeRulesAsync(_currentEntityTypeId.Value);
        foreach (var rule in rules)
        {
            if (!_definitionCache.TryGetValue(rule.AttributeDefId, out var def))
                continue;
            var lookup = def.LookupGroupId.HasValue ? LookupGroups.FirstOrDefault(g => g.Id == def.LookupGroupId.Value) : null;
            Attributes.Add(new AttributeRuleDisplayViewModel(rule, def, lookup));
        }
    }

    private async Task AddAttributeAsync()
    {
        if (!_currentEntityTypeId.HasValue || SelectedDataKind is null)
            return;

        if (string.IsNullOrWhiteSpace(NewAttributeCode) || string.IsNullOrWhiteSpace(NewAttributeLabel))
        {
            ErrorMessage = "Code et Label de l'attribut sont requis.";
            return;
        }

        long? lookupGroupId = null;
        if (SelectedDataKind.Code == Contracts.Common.DataKindCodes.Enum)
        {
            lookupGroupId = SelectedLookupGroup?.Id;
            if (lookupGroupId is null)
            {
                ErrorMessage = "Sélectionnez un groupe d'options.";
                return;
            }
        }

        long? refEntityTypeId = null;
        if (SelectedDataKind.Code == Contracts.Common.DataKindCodes.EntityReference)
            refEntityTypeId = SelectedReferenceType?.Id;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var defDto = new AttributeDefCreateDto(NewAttributeCode.Trim(), NewAttributeLabel.Trim(), SelectedDataKind.Code, null, refEntityTypeId, lookupGroupId, NewAttributeDescription);
            var def = await _schemaService.CreateAttributeDefinitionAsync(defDto, default);
            _definitionCache[def.Id] = def;

            JsonElement? defaultValue = null;
            if (!string.IsNullOrWhiteSpace(NewAttributeDefault))
            {
                defaultValue = JsonDocument.Parse(NewAttributeDefault).RootElement;
            }
            else if (SelectedDataKind.Code == Contracts.Common.DataKindCodes.Enum && SelectedLookupItem is not null)
            {
                defaultValue = JsonDocument.Parse(SelectedLookupItem.Id.ToString()).RootElement;
            }

            var ruleDto = new AttributeRuleCreateDto(_currentEntityTypeId.Value, def.Id, SelectedAccessMode?.Code ?? "read_write", NewAttributeIsRequired, NewAttributeOrder <= 0 ? 1 : NewAttributeOrder, defaultValue, null);
            var rule = await _schemaService.CreateAttributeRuleAsync(ruleDto, default);

            var lookup = lookupGroupId.HasValue ? LookupGroups.FirstOrDefault(g => g.Id == lookupGroupId.Value) : null;
            var display = new AttributeRuleDisplayViewModel(rule, def, lookup);
            Attributes.Add(display);
            SelectedAttribute = display;

            ClearNewAttributeFields();
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

    private async Task SaveAttributeAsync()
    {
        if (SelectedAttribute is null || !_currentEntityTypeId.HasValue)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            JsonElement? defaultValue = null;
            if (!string.IsNullOrWhiteSpace(SelectedAttribute.DefaultValue))
            {
                defaultValue = JsonDocument.Parse(SelectedAttribute.DefaultValue).RootElement;
            }

            var update = new AttributeRuleUpdateDto(SelectedAttribute.AccessMode, SelectedAttribute.IsRequired, SelectedAttribute.OrderIndex, defaultValue, null);
            var updated = await _schemaService.UpdateAttributeRuleAsync(_currentEntityTypeId.Value, SelectedAttribute.Rule.Id, update, default);
            if (updated is not null)
            {
                var def = _definitionCache[updated.AttributeDefId];
                var lookup = def.LookupGroupId.HasValue ? LookupGroups.FirstOrDefault(g => g.Id == def.LookupGroupId.Value) : null;
                var vm = new AttributeRuleDisplayViewModel(updated, def, lookup);
                var index = Attributes.IndexOf(SelectedAttribute);
                Attributes[index] = vm;
                SelectedAttribute = vm;
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

    private async Task DeleteAttributeAsync()
    {
        if (SelectedAttribute is null || !_currentEntityTypeId.HasValue)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await _schemaService.DeleteAttributeRuleAsync(_currentEntityTypeId.Value, SelectedAttribute.Rule.Id, default);
            Attributes.Remove(SelectedAttribute);
            SelectedAttribute = null;
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

    private void UpdateDataKindDependencies()
    {
        if (SelectedDataKind?.Code != Contracts.Common.DataKindCodes.Enum)
        {
            SelectedLookupGroup = null;
            AvailableLookupItems.Clear();
            SelectedLookupItem = null;
        }

        if (SelectedDataKind?.Code != Contracts.Common.DataKindCodes.EntityReference)
        {
            SelectedReferenceType = null;
        }
    }

    private void RefreshLookupItems()
    {
        AvailableLookupItems.Clear();
        if (SelectedLookupGroup is null) return;
        foreach (var item in SelectedLookupGroup.Items)
            AvailableLookupItems.Add(item);
    }

    private void ClearNewAttributeFields()
    {
        NewAttributeCode = string.Empty;
        NewAttributeLabel = string.Empty;
        NewAttributeDescription = null;
        NewAttributeDefault = null;
        NewAttributeOrder = 1;
        SelectedLookupItem = null;
    }

    private bool CanAddAttribute()
    {
        return _currentEntityTypeId.HasValue && !IsBusy;
    }

    public void Reset()
    {
        Attributes.Clear();
        LookupGroups.Clear();
        AvailableLookupItems.Clear();
        _definitionCache.Clear();
        SelectedAttribute = null;
        ErrorMessage = null;
        _currentEntityTypeId = null;
        ClearNewAttributeFields();
    }

    private void RefreshCommandStates()
    {
        (AddAttributeCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveAttributeCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteAttributeCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
