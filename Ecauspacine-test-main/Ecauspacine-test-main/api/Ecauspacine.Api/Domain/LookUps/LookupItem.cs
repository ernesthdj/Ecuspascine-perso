using Ecauspacine.Api.Domain.Common;

namespace Ecauspacine.Api.Domain.Lookups;

/// <summary>
/// Élément d'un groupe de référentiel (valeur d'énum).
/// </summary>
public class LookupItem : CodeLabelDescriptionEntity
{
    public long GroupId { get; set; }
    public LookupGroup Group { get; set; } = default!;

    /// <summary>Valeur numérique optionnelle (utile pour des flags/codes).</summary>
    public int? Value { get; set; }

    /// <summary>Ordre d'affichage (1 par défaut).</summary>
    public int OrderIndex { get; set; } = 1;
}
