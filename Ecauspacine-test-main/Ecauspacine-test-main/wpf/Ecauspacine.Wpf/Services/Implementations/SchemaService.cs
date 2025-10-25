
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Attributes;
using Ecauspacine.Contracts.EntityTypes;
using Ecauspacine.Wpf.Services.Interfaces;

namespace Ecauspacine.Wpf.Services.Implementations;

public class SchemaService : ISchemaService
{
    private readonly IApiClient _api;

    public SchemaService(IApiClient api) => _api = api;

    public async Task<IReadOnlyList<EntityTypeDto>> GetEntityTypesAsync(CancellationToken ct = default)
    {
        return await _api.GetAsync<IReadOnlyList<EntityTypeDto>>("/api/entity-types", ct) ?? new List<EntityTypeDto>();
    }

    public async Task<EntityTypeDto> CreateEntityTypeAsync(string code, string label, string? description, CancellationToken ct = default)
    {
        var dto = new EntityTypeCreateDto(code, label, description);
        return await _api.PostAsync<EntityTypeCreateDto, EntityTypeDto>("/api/entity-types", dto, ct)
            ?? throw new System.InvalidOperationException("Création d'entité échouée");
    }

    public Task<EntityTypeDto?> UpdateEntityTypeAsync(long id, EntityTypeUpdateDto dto, CancellationToken ct = default)
        => _api.PutAsync<EntityTypeUpdateDto, EntityTypeDto>($"/api/entity-types/{id}", dto, ct);

    public Task DeleteEntityTypeAsync(long id, CancellationToken ct = default)
        => _api.DeleteAsync($"/api/entity-types/{id}", ct);

    public async Task<IReadOnlyList<AttributeDefDto>> GetAttributeDefinitionsAsync(CancellationToken ct = default)
    {
        return await _api.GetAsync<IReadOnlyList<AttributeDefDto>>("/api/attribute-definitions", ct) ?? new List<AttributeDefDto>();
    }

    public async Task<AttributeDefDto> CreateAttributeDefinitionAsync(AttributeDefCreateDto dto, CancellationToken ct = default)
        => await _api.PostAsync<AttributeDefCreateDto, AttributeDefDto>("/api/attribute-definitions", dto, ct)
            ?? throw new System.InvalidOperationException("Création d'attribut échouée");

    public Task<AttributeDefDto?> UpdateAttributeDefinitionAsync(long id, AttributeDefUpdateDto dto, CancellationToken ct = default)
        => _api.PutAsync<AttributeDefUpdateDto, AttributeDefDto>($"/api/attribute-definitions/{id}", dto, ct);

    public async Task<IReadOnlyList<AttributeRuleDto>> GetAttributeRulesAsync(long entityTypeId, CancellationToken ct = default)
    {
        return await _api.GetAsync<IReadOnlyList<AttributeRuleDto>>($"/api/entity-types/{entityTypeId}/attributes", ct) ?? new List<AttributeRuleDto>();
    }

    public async Task<AttributeRuleDto> CreateAttributeRuleAsync(AttributeRuleCreateDto dto, CancellationToken ct = default)
        => await _api.PostAsync<AttributeRuleCreateDto, AttributeRuleDto>($"/api/entity-types/{dto.EntityTypeId}/attributes", dto, ct)
            ?? throw new System.InvalidOperationException("Création de règle échouée");

    public Task<AttributeRuleDto?> UpdateAttributeRuleAsync(long entityTypeId, long ruleId, AttributeRuleUpdateDto dto, CancellationToken ct = default)
        => _api.PutAsync<AttributeRuleUpdateDto, AttributeRuleDto>($"/api/entity-types/{entityTypeId}/attributes/{ruleId}", dto, ct);

    public Task DeleteAttributeRuleAsync(long entityTypeId, long ruleId, CancellationToken ct = default)
        => _api.DeleteAsync($"/api/entity-types/{entityTypeId}/attributes/{ruleId}", ct);
}
