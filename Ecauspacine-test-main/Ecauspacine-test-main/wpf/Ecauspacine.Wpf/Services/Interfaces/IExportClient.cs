using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Exports;

namespace Ecauspacine.Wpf.Services.Interfaces;

public interface IExportClient
{
    Task<ExportResultDto> ExportAsync(long entityTypeId, IReadOnlyList<ExportFormat> formats, string targetDirectory, CancellationToken ct = default);
}
