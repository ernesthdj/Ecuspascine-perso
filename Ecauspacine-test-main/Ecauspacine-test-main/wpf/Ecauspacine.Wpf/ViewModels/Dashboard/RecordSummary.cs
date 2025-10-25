using Ecauspacine.Contracts.Entities;

namespace Ecauspacine.Wpf.ViewModels.Dashboard;

/// <summary>
/// Represents a summary view of an entity record with its JSON representation
/// </summary>
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
