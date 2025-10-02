namespace Ecauspacine.Contracts.Common;

/// <summary>
/// Codes "data_kind" (r�f�rentiel) utilis�s par les attributs EAV.
/// On passe ces codes c�t� API pour �viter d'exposer les IDs.
/// </summary>
public static class DataKindCodes
{
    public const string Int = "int";
    public const string Float = "float";
    public const string Bool = "bool";
    public const string Text = "text";
    public const string Lookup = "lookup";
    public const string Json = "json";
    public const string RefEntity = "ref_entity";
    public const string RefAttribute = "ref_attribute";
}
