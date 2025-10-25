using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ecauspacine.Contracts.EntityTypes;
using Ecauspacine.Wpf.Helpers;
using Ecauspacine.Wpf.Services.Interfaces;
using Ecauspacine.Wpf.ViewModels.Base;

namespace Ecauspacine.Wpf.ViewModels.Dashboard.Schema;

/// <summary>
/// ViewModel responsible for managing EntityTypes (CRUD operations)
/// </summary>
public class EntityTypeManagementViewModel : ViewModelBase
{
    private readonly ISchemaService _schemaService;

    public EntityTypeManagementViewModel(ISchemaService schemaService)
    {
        _schemaService = schemaService;

        EntityTypes = new ObservableCollection<EntityTypeDto>();

        CreateEntityCommand = new RelayCommand(async _ => await CreateEntityAsync(), _ => !IsBusy);
        SaveEntityCommand = new RelayCommand(async _ => await SaveEntityAsync(), _ => SelectedEntityType is not null && !IsBusy);
        DeleteEntityCommand = new RelayCommand(async _ => await DeleteEntityAsync(), _ => SelectedEntityType is not null && !IsBusy);
        RefreshEntitiesCommand = new RelayCommand(async _ => await LoadEntityTypesAsync(), _ => !IsBusy);
    }

    public ObservableCollection<EntityTypeDto> EntityTypes { get; }

    private EntityTypeDto? _selectedEntityType;
    public EntityTypeDto? SelectedEntityType
    {
        get => _selectedEntityType;
        set
        {
            if (SetProperty(ref _selectedEntityType, value))
            {
                UpdateEntityFields();
                EntityTypeChanged?.Invoke(value);
            }
        }
    }

    public event Action<EntityTypeDto?>? EntityTypeChanged;

    // Create entity properties
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

    // Edit entity properties
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

    public ICommand CreateEntityCommand { get; }
    public ICommand SaveEntityCommand { get; }
    public ICommand DeleteEntityCommand { get; }
    public ICommand RefreshEntitiesCommand { get; }

    public async Task LoadEntityTypesAsync()
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
            ClearNewEntityFields();
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

    private void ClearNewEntityFields()
    {
        NewEntityCode = string.Empty;
        NewEntityLabel = string.Empty;
        NewEntityDescription = null;
    }

    public void Reset()
    {
        EntityTypes.Clear();
        SelectedEntityType = null;
        ErrorMessage = null;
        ClearNewEntityFields();
    }

    private void RefreshCommandStates()
    {
        (CreateEntityCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveEntityCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteEntityCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RefreshEntitiesCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
