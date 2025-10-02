using Ecauspacine.Api.Domain.Common;

namespace Ecauspacine.Api.Domain;

/// <summary>
/// Type d'entité (E de EAV). Sert à définir des "classes" dynamiques côté métier.
/// Hérite du modèle standard Code/Label/Description + timestamps.
/// </summary>
public class EntityType : CodeLabelDescriptionEntity
{

}
