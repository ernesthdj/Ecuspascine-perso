using Ecauspacine.Api.Domain.Common;

namespace Ecauspacine.Api.Domain.Lookups;

/// <summary>
/// Groupe de référentiel (équiv. un "enum" côté C#).
/// </summary>
public class LookupGroup : CodeLabelDescriptionEntity
{
    public ICollection<LookupItem> Items { get; set; } = new List<LookupItem>();
}
