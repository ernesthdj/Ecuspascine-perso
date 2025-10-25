
using System.Collections.Generic;
using Ecauspacine.Contracts.Common;

namespace Ecauspacine.Wpf.ViewModels.Dashboard.Schema;

public record DataKindOption(string Code, string DisplayName);
public record AccessModeOption(string Code, string DisplayName);

public static class SchemaOptions
{
    public static IReadOnlyList<DataKindOption> DataKinds { get; } = new List<DataKindOption>
    {
        new(DataKindCodes.String, "Texte"),
        new(DataKindCodes.Int, "Entier (int)"),
        new(DataKindCodes.Long, "Entier long"),
        new(DataKindCodes.Float, "Flottant (float)"),
        new(DataKindCodes.Double, "Flottant (double)"),
        new(DataKindCodes.Bool, "Booléen"),
        new(DataKindCodes.SByte, "Entier (sbyte)"),
        new(DataKindCodes.Byte, "Entier (byte)"),
        new(DataKindCodes.Short, "Entier (short)"),
        new(DataKindCodes.UShort, "Entier (ushort)"),
        new(DataKindCodes.UInt, "Entier (uint)"),
        new(DataKindCodes.Enum, "Enuméré"),
        new(DataKindCodes.EntityReference, "Référence d'entité")
    };

    public static IReadOnlyList<AccessModeOption> AccessModes { get; } = new List<AccessModeOption>
    {
        new("read_write", "Lecture & écriture"),
        new("read_only", "Lecture seule")
    };
}
