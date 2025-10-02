using System.Collections.Generic;

namespace Ecauspacine.Contracts.Common;

/// <summary>
/// D�tail d'erreur pour les cas HTTP 400/422/500,
/// avec �ventuelles erreurs de validation champ-�-champ.
/// </summary>
public record ApiError(string Message, string? Code = null, IDictionary<string, string[]>? Validation = null);
