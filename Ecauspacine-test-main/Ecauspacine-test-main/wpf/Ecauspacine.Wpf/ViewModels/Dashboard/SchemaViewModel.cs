using System.Threading.Tasks;
using Ecauspacine.Wpf.Services.Interfaces;
using Ecauspacine.Wpf.ViewModels.Base;
using Ecauspacine.Wpf.ViewModels.Dashboard.Schema;

namespace Ecauspacine.Wpf.ViewModels.Dashboard;

/// <summary>
/// Coordinator ViewModel for Schema management.
/// Delegates EntityType and Attribute management to specialized ViewModels.
/// </summary>
public class SchemaViewModel : ViewModelBase, IInitializable
{
    public SchemaViewModel(EntityTypeManagementViewModel entityTypeVM, AttributeManagementViewModel attributeVM)
    {
        EntityTypeManagement = entityTypeVM;
        AttributeManagement = attributeVM;

        // Subscribe to EntityType changes to reload attributes
        EntityTypeManagement.EntityTypeChanged += async (entityType) =>
        {
            if (entityType is not null)
            {
                await AttributeManagement.LoadAttributesForEntityAsync(entityType.Id);
            }
            else
            {
                AttributeManagement.Reset();
            }
        };

        // Share EntityTypes collection for references
        AttributeManagement.EntityTypes = EntityTypeManagement.EntityTypes;
    }

    public EntityTypeManagementViewModel EntityTypeManagement { get; }
    public AttributeManagementViewModel AttributeManagement { get; }

    public async Task InitializeAsync()
    {
        if (EntityTypeManagement.EntityTypes.Count == 0)
        {
            await AttributeManagement.InitializeAsync();
            await EntityTypeManagement.LoadEntityTypesAsync();
        }
    }

    public void Reset()
    {
        EntityTypeManagement.Reset();
        AttributeManagement.Reset();
    }
}
