namespace Ecauspacine.Contracts.Common;

/// <summary>
/// Marqueur simple pour les DTOs qui possèdent un identifiant unique (DB PK).
/// Utile pour des contraintes génériques côté client.
/// </summary>
public interface IHasId
{
    long Id { get; }
}
