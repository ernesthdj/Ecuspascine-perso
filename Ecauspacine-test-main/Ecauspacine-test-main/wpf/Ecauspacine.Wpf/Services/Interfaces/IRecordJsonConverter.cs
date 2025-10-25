using System.Collections.Generic;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Attributes;
using Ecauspacine.Contracts.Entities;

namespace Ecauspacine.Wpf.Services.Interfaces;

/// <summary>
/// Service for converting entity records to/from JSON representation
/// </summary>
public interface IRecordJsonConverter
{
    /// <summary>
    /// Builds attribute value payload from JSON string
    /// </summary>
    Task<IReadOnlyList<AttributeValueUpsertDto>> BuildRecordPayloadAsync(long entityTypeId, string? json);

    /// <summary>
    /// Builds JSON representation of an entity record
    /// </summary>
    Task<string> BuildRecordJsonAsync(EntityRecordDto record, long entityTypeId);
}
