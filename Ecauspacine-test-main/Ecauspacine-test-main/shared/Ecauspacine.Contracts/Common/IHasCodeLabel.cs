namespace Ecauspacine.Contracts.Common;

/// <summary>
/// Convention "catalogue": éléments identifiés par un Code pour l'export .cs + un Label.
/// (ex: lookup items, types d'entité, définitions d'attributs)
/// </summary>
public interface IHasCodeLabel
{
    string Code { get; }
    string Label { get; }
}
