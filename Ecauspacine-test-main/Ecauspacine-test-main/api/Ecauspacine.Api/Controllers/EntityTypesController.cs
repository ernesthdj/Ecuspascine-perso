using Ecauspacine.Api.Features.EntityTypes;
using Ecauspacine.Contracts.EntityTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecauspacine.Api.Controllers;

/// <summary>
/// Endpoints CRUD basiques pour les types d'entité (liste / création / mise à jour).
/// </summary>
[ApiController]
[Route("api/entity-types")]
public class EntityTypesController : ControllerBase
{
    private readonly IEntityTypeService _service;

    public EntityTypesController(IEntityTypeService service) => _service = service;

    /// <summary>Retourne la liste triée par Code.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EntityTypeDto>>> List(CancellationToken ct)
        => Ok(await _service.ListAsync(ct));

    /// <summary>Crée un nouveau type d'entité (Code unique requis).</summary>
    [HttpPost]
    public async Task<ActionResult<EntityTypeDto>> Create([FromBody] EntityTypeCreateDto body, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateAsync(body, ct);
            return CreatedAtAction(nameof(List), new { id = created.Id }, created);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        catch (DbUpdateException) { return Conflict(new { error = "Un EntityType avec ce Code existe déjà." }); }
    }

    /// <summary>Met à jour le Code/Label d'un type (vérifie unicité du Code si changé).</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<EntityTypeDto>> Update([FromRoute] long id, [FromBody] EntityTypeUpdateDto body, CancellationToken ct)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, body, ct);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        catch (DbUpdateException) { return Conflict(new { error = "Conflit de contrainte (unicité Code ?)." }); }
    }

    /// <summary>Supprime un type d'entité par Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken ct)
    {
        try
        {
            var ok = await _service.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
        
        catch (DbUpdateException ex)
        {
            return Conflict(new { error = "Impossible de supprimer ce type car il est référencé.", detail = ex.Message });
        }
    }
}
