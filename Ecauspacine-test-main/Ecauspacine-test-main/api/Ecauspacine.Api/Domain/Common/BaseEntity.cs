namespace Ecauspacine.Api.Domain.Common;

/// <summary>
/// Entité de base avec identifiant (long) + traces temporelles standard.
/// </summary>
public abstract class BaseEntity
{
    public long Id { get; set; }
}
