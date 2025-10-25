using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Entities;

namespace Ecauspacine.Wpf.Services.Interfaces;

public interface IDataService
{
    Task<IReadOnlyList<EntityRecordDto>> GetRecordsAsync(long entityTypeId, string? search, string? sort, bool desc, CancellationToken ct = default);
    Task<EntityRecordDto> CreateRecordAsync(EntityRecordCreateDto dto, CancellationToken ct = default);
    Task<EntityRecordDto?> UpdateRecordAsync(long entityTypeId, long recordId, EntityRecordUpdateDto dto, CancellationToken ct = default);
    Task DeleteRecordAsync(long entityTypeId, long recordId, CancellationToken ct = default);
}
