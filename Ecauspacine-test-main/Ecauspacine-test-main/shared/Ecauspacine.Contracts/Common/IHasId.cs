namespace Ecauspacine.Contracts.Common;

/// <summary>
/// Marqueur simple pour les DTOs qui poss�dent un identifiant unique (DB PK).
/// Utile pour des contraintes g�n�riques c�t� client.
/// </summary>
public interface IHasId
{
    long Id { get; }
}
