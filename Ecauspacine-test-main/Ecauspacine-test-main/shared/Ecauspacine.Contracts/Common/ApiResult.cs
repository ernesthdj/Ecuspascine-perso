namespace Ecauspacine.Contracts.Common;

/// <summary>
/// Enveloppe standard pour les réponses d'API
/// (facile à consommer côté WinForms, avec Success/Data/Message).
/// </summary>
public record ApiResult<T>(bool Success, T? Data, string? Message = null, string? ErrorCode = null)
{
	public static ApiResult<T> Ok(T data, string? msg = null) => new(true, data, msg, null);
	public static ApiResult<T> Fail(string msg, string? code = null) => new(false, default, msg, code);
}
