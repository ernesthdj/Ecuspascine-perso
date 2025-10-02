namespace Ecauspacine.Contracts.Common;

/// <summary>
/// Requête de pagination "classique".
/// </summary>
public record PagedRequest(int Page = 1, int PageSize = 20, string? Search = null, string? OrderBy = null);

/// <summary>
/// Résultat paginé pour listes.
/// </summary>
public record PagedResult<T>(int Page, int PageSize, int Total, IReadOnlyList<T> Items);
