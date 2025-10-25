
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Exports;
using Ecauspacine.Wpf.Services.Interfaces;

namespace Ecauspacine.Wpf.Services.Implementations;

public class ExportClient : IExportClient
{
    private readonly IApiClient _api;

    public ExportClient(IApiClient api) => _api = api;

    public async Task<ExportResultDto> ExportAsync(long entityTypeId, IReadOnlyList<ExportFormat> formats, string targetDirectory, CancellationToken ct = default)
    {
        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        var request = new ExportRequestDto(entityTypeId, formats);
        var (content, headers) = await _api.PostForBytesWithHeadersAsync("/api/export", request, ct);

        var archiveName = headers.TryGetValue("X-Export-Archive", out var values)
            ? values.FirstOrDefault() ?? $"export_{entityTypeId}.zip"
            : $"export_{entityTypeId}.zip";

        var filePath = Path.Combine(targetDirectory, archiveName);
        await File.WriteAllBytesAsync(filePath, content, ct);

        var generated = headers.TryGetValue("X-Export-Files", out var files)
            ? files.FirstOrDefault()?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>()
            : Array.Empty<string>();

        return new ExportResultDto(archiveName, generated);
    }
}
