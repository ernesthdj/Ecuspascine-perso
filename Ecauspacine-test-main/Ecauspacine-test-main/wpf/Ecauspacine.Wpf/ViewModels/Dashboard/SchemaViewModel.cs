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
using Ecauspacine.Wpf.ViewModels.Dashboard.Schema;

namespace Ecauspacine.Wpf.ViewModels.Dashboard;

public class SchemaViewModel : ViewModelBase, IInitializable
{
    private readonly ISchemaService _schemaService;
    private readonly ILookupClient _lookupClient;

    private readonly Dictionary<long, AttributeDefDto> _definitionCache = new();

    public SchemaViewModel(ISchemaService schemaService, ILookupClient lookupClient)
    {
        _schemaService = schemaService;
        _lookupClient = lookupClient;

        EntityTypes = new ObservableCollection<EntityTypeDto>();
        Attributes = new ObservableCollection<AttributeRuleDisplayViewModel>();
        LookupGroups = new ObservableCollection<LookupGroupDto>();
        AvailableLookupItems = new ObservableCollection<LookupItemDto>();

        CreateEntityCommand = new RelayCommand(async _ => await CreateEntityAsync(), _ => !IsBusy);
        SaveEntityCommand = new RelayCommand(async _ => await SaveEntityAsync(), _ => SelectedEntityType is not null && !IsBusy);
        DeleteEntityCommand = new RelayCommand(async _ => await DeleteEntityAsync(), _ => SelectedEntityType is not null && !IsBusy);
        AddAttributeCommand = new RelayCommand(async _ => await AddAttributeAsync(), _ => SelectedEntityType is not null && !IsBusy);
        SaveAttributeCommand = new RelayCommand(async _ => await SaveAttributeAsync(), _ => SelectedAttribute is not null && !IsBusy);
        DeleteAttributeCommand = new RelayCommand(async _ => await DeleteAttributeAsync(), _ => SelectedAttribute is not null && !IsBusy);
        RefreshEntitiesCommand = new RelayCommand(async _ => await LoadEntityTypesAsync(), _ => !IsBusy);

        SelectedAccessMode = SchemaOptions.AccessModes.First();
        SelectedDataKind = SchemaOptions.DataKinds.First();
        NewAttributeOrder = 1;
    }

    public ObservableCollection<EntityTypeDto> EntityTypes { get; }
    public ObservableCollection<AttributeRuleDisplayViewModel> Attributes { get; }
    public ObservableCollection<LookupGroupDto> LookupGroups { get; }
    public ObservableCollection<LookupItemDto> AvailableLookupItems { get; }

    public IReadOnlyList<DataKindOption> DataKinds => SchemaOptions.DataKinds;
    public IReadOnlyList<AccessModeOption> AccessModes => SchemaOptions.AccessModes;

    private EntityTypeDto? _selectedEntityType;
    public EntityTypeDto? SelectedEntityType
    {
        get => _selectedEntityType;
        set
        {
            if (SetProperty(ref _selectedEntityType, value))
            {
                UpdateEntityFields();
                _ = LoadAttributesAsync();
            }
        }
    }

    private AttributeRuleDisplayViewModel? _selectedAttribute;
    public AttributeRuleDisplayViewModel? SelectedAttribute
    {
        get => _selectedAttribute;
        set => SetProperty(ref _selectedAttribute, value);
    }

    private string _newEntityCode = string.Empty;
    public string NewEntityCode
    {
        get => _newEntityCode;
        set => SetProperty(ref _newEntityCode, value);
    }

    private string _newEntityLabel = string.Empty;
    public string NewEntityLabel
    {
        get => _newEntityLabel;
        set => SetProperty(ref _newEntityLabel, value);
    }

    private string? _newEntityDescription;
    public string? NewEntityDescription
    {
        get => _newEntityDescription;
        set => SetProperty(ref _newEntityDescription, value);
    }

    private string _editEntityCode = string.Empty;
    public string EditEntityCode
    {
        get => _editEntityCode;
        set => SetProperty(ref _editEntityCode, value);
    }

    private string _editEntityLabel = string.Empty;
    public string EditEntityLabel
    {
        get => _editEntityLabel;
        set => SetProperty(ref _editEntityLabel, value);
    }

    private string? _editEntityDescription;
    public string? EditEntityDescription
    {
        get => _editEntityDescription;
        set => SetProperty(ref _editEntityDescription, value);
    }

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

    private EntityTypeDto? _selectedReferenceType;
    public EntityTypeDto? SelectedReferenceType
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
            SetProperty(ref _isBusy, value);
            (CreateEntityCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (SaveEntityCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteEntityCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (AddAttributeCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (SaveAttributeCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteAttributeCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RefreshEntitiesCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public ICommand CreateEntityCommand { get; }
    public ICommand SaveEntityCommand { get; }
    public ICommand DeleteEntityCommand { get; }
    public ICommand AddAttributeCommand { get; }
    public ICommand SaveAttributeCommand { get; }
    public ICommand DeleteAttributeCommand { get; }
    public ICommand RefreshEntitiesCommand { get; }

    public async Task InitializeAsync()
    {
        if (EntityTypes.Count == 0)
        {
            await LoadLookupGroupsAsync();
            await LoadDefinitionsAsync();
            await LoadEntityTypesAsync();
        }
    }

    public void Reset()
    {
        EntityTypes.Clear();
        Attributes.Clear();
        LookupGroups.Clear();
        AvailableLookupItems.Clear();
        _definitionCache.Clear();
        SelectedEntityType = null;
        SelectedAttribute = null;
        ErrorMessage = null;
    }

    private async Task LoadEntityTypesAsync()
    {
        try
        {
            IsBusy = true;
            var list = await _schemaService.GetEntityTypesAsync();
            EntityTypes.Clear();
            foreach (var item in list)
            {
                EntityTypes.Add(item);
            }

            if (SelectedEntityType is null && EntityTypes.Count > 0)
                SelectedEntityType = EntityTypes.First();
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
        if (SelectedEntityType is null)
            return;

        var rules = await _schemaService.GetAttributeRulesAsync(SelectedEntityType.Id);
        foreach (var rule in rules)
        {
            if (!_definitionCache.TryGetValue(rule.AttributeDefId, out var def))
                continue;
            var lookup = def.LookupGroupId.HasValue ? LookupGroups.FirstOrDefault(g => g.Id == def.LookupGroupId.Value) : null;
            Attributes.Add(new AttributeRuleDisplayViewModel(rule, def, lookup));
        }
    }

    private async Task CreateEntityAsync()
    {
        if (string.IsNullOrWhiteSpace(NewEntityCode) || string.IsNullOrWhiteSpace(NewEntityLabel))
        {
            ErrorMessage = "Code et Label sont requis.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            var created = await _schemaService.CreateEntityTypeAsync(NewEntityCode.Trim(), NewEntityLabel.Trim(), NewEntityDescription, default);
            EntityTypes.Add(created);
            SelectedEntityType = created;
            NewEntityCode = string.Empty;
            NewEntityLabel = string.Empty;
            NewEntityDescription = null;
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

    private async Task SaveEntityAsync()
    {
        if (SelectedEntityType is null)
            return;

        var dto = new EntityTypeUpdateDto(EditEntityCode, EditEntityLabel, EditEntityDescription);
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            var updated = await _schemaService.UpdateEntityTypeAsync(SelectedEntityType.Id, dto, default);
            if (updated is not null)
            {
                var index = EntityTypes.IndexOf(SelectedEntityType);
                EntityTypes[index] = updated;
                SelectedEntityType = updated;
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

    private async Task DeleteEntityAsync()
    {
        if (SelectedEntityType is null)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await _schemaService.DeleteEntityTypeAsync(SelectedEntityType.Id, default);
            EntityTypes.Remove(SelectedEntityType);
            SelectedEntityType = EntityTypes.FirstOrDefault();
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

    private async Task AddAttributeAsync()
    {
        if (SelectedEntityType is null || SelectedDataKind is null)
            return;

        if (string.IsNullOrWhiteSpace(NewAttributeCode) || string.IsNullOrWhiteSpace(NewAttributeLabel))
        {
            ErrorMessage = "Code et Label de l'attribut sont requis.";
            return;
        }

        long? lookupGroupId = null;
        if (SelectedDataKind.Code == Ecauspacine.Contracts.Common.DataKindCodes.Enum)
        {
            lookupGroupId = SelectedLookupGroup?.Id;
            if (lookupGroupId is null)
            {
                ErrorMessage = "Sélectionnez un groupe d'options.";
                return;
            }
        }

        long? refEntityTypeId = null;
        if (SelectedDataKind.Code == Ecauspacine.Contracts.Common.DataKindCodes.EntityReference)
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
            else if (SelectedDataKind.Code == Ecauspacine.Contracts.Common.DataKindCodes.Enum && SelectedLookupItem is not null)
            {
                defaultValue = JsonDocument.Parse(SelectedLookupItem.Id.ToString()).RootElement;
            }

            var ruleDto = new AttributeRuleCreateDto(SelectedEntityType.Id, def.Id, SelectedAccessMode?.Code ?? "read_write", NewAttributeIsRequired, NewAttributeOrder <= 0 ? 1 : NewAttributeOrder, defaultValue, null);
            var rule = await _schemaService.CreateAttributeRuleAsync(ruleDto, default);

            var lookup = lookupGroupId.HasValue ? LookupGroups.FirstOrDefault(g => g.Id == lookupGroupId.Value) : null;
            var display = new AttributeRuleDisplayViewModel(rule, def, lookup);
            Attributes.Add(display);
            SelectedAttribute = display;

            NewAttributeCode = string.Empty;
            NewAttributeLabel = string.Empty;
            NewAttributeDescription = null;
            NewAttributeDefault = null;
            NewAttributeOrder = 1;
            SelectedLookupItem = null;
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
        if (SelectedAttribute is null || SelectedEntityType is null)
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
            var updated = await _schemaService.UpdateAttributeRuleAsync(SelectedEntityType.Id, SelectedAttribute.Rule.Id, update, default);
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
        if (SelectedAttribute is null || SelectedEntityType is null)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await _schemaService.DeleteAttributeRuleAsync(SelectedEntityType.Id, SelectedAttribute.Rule.Id, default);
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

    private void UpdateEntityFields()
    {
        if (SelectedEntityType is null)
        {
            EditEntityCode = string.Empty;
            EditEntityLabel = string.Empty;
            EditEntityDescription = null;
        }
        else
        {
            EditEntityCode = SelectedEntityType.Code;
            EditEntityLabel = SelectedEntityType.Label;
            EditEntityDescription = SelectedEntityType.Description;
        }
    }

    private void UpdateDataKindDependencies()
    {
        if (SelectedDataKind?.Code != Ecauspacine.Contracts.Common.DataKindCodes.Enum)
        {
            SelectedLookupGroup = null;
            AvailableLookupItems.Clear();
            SelectedLookupItem = null;
        }

        if (SelectedDataKind?.Code != Ecauspacine.Contracts.Common.DataKindCodes.EntityReference)
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
}
