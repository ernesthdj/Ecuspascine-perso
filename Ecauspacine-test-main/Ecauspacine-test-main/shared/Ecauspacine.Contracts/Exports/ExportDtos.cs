using System.Collections.Generic;

namespace Ecauspacine.Contracts.Exports;

/// <summary>
/// Format d'export disponible
/// </summary>
public enum ExportFormat
{
    Csv,
    Json,
    Xml
}

/// <summary>
/// Résultat d'une opération d'export
/// </summary>
public record ExportResultDto(
    string ArchiveFileName,
    IReadOnlyList<string> GeneratedFiles);
