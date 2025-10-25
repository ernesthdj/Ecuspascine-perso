using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Attributes;
using Ecauspacine.Contracts.EntityTypes;

namespace Ecauspacine.Wpf.Services.Interfaces;

public interface ISchemaService
{
    Task<IReadOnlyList<EntityTypeDto>> GetEntityTypesAsync(CancellationToken ct = default);
    Task<EntityTypeDto> CreateEntityTypeAsync(string code, string label, string? description, CancellationToken ct = default);
    Task<EntityTypeDto?> UpdateEntityTypeAsync(long id, EntityTypeUpdateDto dto, CancellationToken ct = default);
    Task DeleteEntityTypeAsync(long id, CancellationToken ct = default);

    Task<IReadOnlyList<AttributeDefDto>> GetAttributeDefinitionsAsync(CancellationToken ct = default);
    Task<AttributeDefDto> CreateAttributeDefinitionAsync(AttributeDefCreateDto dto, CancellationToken ct = default);
    Task<AttributeDefDto?> UpdateAttributeDefinitionAsync(long id, AttributeDefUpdateDto dto, CancellationToken ct = default);

    Task<IReadOnlyList<AttributeRuleDto>> GetAttributeRulesAsync(long entityTypeId, CancellationToken ct = default);
    Task<AttributeRuleDto> CreateAttributeRuleAsync(AttributeRuleCreateDto dto, CancellationToken ct = default);
    Task<AttributeRuleDto?> UpdateAttributeRuleAsync(long entityTypeId, long ruleId, AttributeRuleUpdateDto dto, CancellationToken ct = default);
    Task DeleteAttributeRuleAsync(long entityTypeId, long ruleId, CancellationToken ct = default);
}
