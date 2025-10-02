using System.Collections.Generic;

namespace Ecauspacine.Contracts.Common;

/// <summary>
/// Détail d'erreur pour les cas HTTP 400/422/500,
/// avec éventuelles erreurs de validation champ-à-champ.
/// </summary>
public record ApiError(string Message, string? Code = null, IDictionary<string, string[]>? Validation = null);
