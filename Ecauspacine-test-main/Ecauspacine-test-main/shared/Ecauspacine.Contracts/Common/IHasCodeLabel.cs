namespace Ecauspacine.Contracts.Common;

/// <summary>
/// Convention "catalogue": �l�ments identifi�s par un Code pour l'export .cs + un Label.
/// (ex: lookup items, types d'entit�, d�finitions d'attributs)
/// </summary>
public interface IHasCodeLabel
{
    string Code { get; }
    string Label { get; }
}
