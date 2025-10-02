namespace Ecauspacine.Api.Domain.Common;

/// <summary>
/// Modèle “catalogue” standard : Code (technique), Label (affichage), Description (longue).
/// - Code est pensé pour l’export C# (ASCII/PascalCase), unicité décidée par table.
/// - Label est requis (affichage UI).
/// - Description est optionnelle.
/// </summary>
public abstract class CodeLabelDescriptionEntity : BaseEntity
{
    public string Code { get; set; } = default!;  // longueur max 255
    public string Label { get; set; } = default!; // requis, max 255
    public string? Description { get; set; }      // TEXT
}
