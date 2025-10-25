namespace Ecauspacine.Contracts.Common;

/// <summary>
/// Codes « data_kind » utilisés par les attributs EAV.
/// </summary>
public static class DataKindCodes
{
    /// <summary>Valeur libre sous forme de texte.</summary>
    public const string Text = "text";

    /// <summary>Alias historique pour les valeurs texte.</summary>
    public const string String = Text;

    /// <summary>Entier signé 32 bits.</summary>
    public const string Int = "int";

    /// <summary>Entier signé 64 bits.</summary>
    public const string Long = "long";

    /// <summary>Flottant simple précision.</summary>
    public const string Float = "float";

    /// <summary>Flottant double précision.</summary>
    public const string Double = "double";

    /// <summary>Booléen.</summary>
    public const string Bool = "bool";

    /// <summary>Entier signé 8 bits.</summary>
    public const string SByte = "sbyte";

    /// <summary>Entier non signé 8 bits.</summary>
    public const string Byte = "byte";

    /// <summary>Entier signé 16 bits.</summary>
    public const string Short = "short";

    /// <summary>Entier non signé 16 bits.</summary>
    public const string UShort = "ushort";

    /// <summary>Entier non signé 32 bits.</summary>
    public const string UInt = "uint";

    /// <summary>Enumération référencée dans la table des listes de valeurs.</summary>
    public const string Enum = "enum";

    /// <summary>Valeur liée à un référentiel simple.</summary>
    public const string Lookup = "lookup";

    /// <summary>Valeur libre sérialisée en JSON.</summary>
    public const string Json = "json";

    /// <summary>Référence vers une entité (code historique utilisé par l'API).</summary>
    public const string RefEntity = "ref_entity";

    /// <summary>Alias lisible pour <see cref="RefEntity"/>.</summary>
    public const string EntityReference = RefEntity;

    /// <summary>Référence vers un attribut (utilisé pour les métadonnées avancées).</summary>
    public const string RefAttribute = "ref_attribute";
}
