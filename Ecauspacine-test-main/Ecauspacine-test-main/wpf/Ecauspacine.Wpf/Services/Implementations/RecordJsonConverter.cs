using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Attributes;
using Ecauspacine.Contracts.Entities;
using Ecauspacine.Wpf.Services.Interfaces;

namespace Ecauspacine.Wpf.Services.Implementations;

/// <summary>
/// Service responsible for converting entity records to/from JSON representation
/// </summary>
public class RecordJsonConverter : IRecordJsonConverter
{
    private readonly ISchemaService _schemaService;
    private readonly Dictionary<long, AttributeDefDto> _definitions = new();
    private readonly Dictionary<long, IReadOnlyList<AttributeRuleDto>> _rulesByEntity = new();

    public RecordJsonConverter(ISchemaService schemaService)
    {
        _schemaService = schemaService;
    }

    public async Task<IReadOnlyList<AttributeValueUpsertDto>> BuildRecordPayloadAsync(long entityTypeId, string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException("Fournissez un JSON valide.");

        var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException("Le JSON doit représenter un objet.");

        var rules = await GetRulesAsync(entityTypeId);
        await EnsureDefinitionsLoadedAsync(rules);

        var list = new List<AttributeValueUpsertDto>();
        foreach (var rule in rules)
        {
            if (!_definitions.TryGetValue(rule.AttributeDefId, out var def))
                continue;

            if (!doc.RootElement.TryGetProperty(def.Code, out var valueElement))
                continue;

            var upsert = CreateAttributeValueUpsert(def, valueElement);
            list.Add(upsert);
        }

        return list;
    }

    public async Task<string> BuildRecordJsonAsync(EntityRecordDto record, long entityTypeId)
    {
        var rules = await GetRulesAsync(entityTypeId);
        await EnsureDefinitionsLoadedAsync(rules);

        var dict = new Dictionary<string, object?>();
        foreach (var rule in rules)
        {
            if (!_definitions.TryGetValue(rule.AttributeDefId, out var def))
                continue;

            var val = record.Values.FirstOrDefault(v => v.AttributeDefId == rule.AttributeDefId);
            if (val is null)
                continue;

            dict[def.Code] = ExtractValueForJson(val);
        }

        return JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
    }

    private async Task<IReadOnlyList<AttributeRuleDto>> GetRulesAsync(long entityTypeId)
    {
        if (!_rulesByEntity.TryGetValue(entityTypeId, out var rules))
        {
            rules = await _schemaService.GetAttributeRulesAsync(entityTypeId);
            _rulesByEntity[entityTypeId] = rules;
        }
        return rules;
    }

    private async Task EnsureDefinitionsLoadedAsync(IReadOnlyList<AttributeRuleDto> rules)
    {
        foreach (var rule in rules)
        {
            if (!_definitions.ContainsKey(rule.AttributeDefId))
            {
                var defs = await _schemaService.GetAttributeDefinitionsAsync();
                foreach (var def in defs)
                    _definitions[def.Id] = def;
                break;
            }
        }
    }

    private static AttributeValueUpsertDto CreateAttributeValueUpsert(AttributeDefDto def, JsonElement valueElement)
    {
        return def.DataKind switch
        {
            Contracts.Common.DataKindCodes.Enum => new AttributeValueUpsertDto(0, def.Id, def.DataKind, null, valueElement.GetInt64(), null),
            Contracts.Common.DataKindCodes.EntityReference => new AttributeValueUpsertDto(0, def.Id, def.DataKind, null, null, valueElement.GetInt64()),
            _ => new AttributeValueUpsertDto(0, def.Id, def.DataKind, valueElement, null, null)
        };
    }

    private static object? ExtractValueForJson(AttributeValueDto val)
    {
        return val.DataKind switch
        {
            Contracts.Common.DataKindCodes.Enum => val.LookupItemId,
            Contracts.Common.DataKindCodes.EntityReference => val.RefEntityId,
            _ => val.JsonValue?.ToString()
        };
    }
}
