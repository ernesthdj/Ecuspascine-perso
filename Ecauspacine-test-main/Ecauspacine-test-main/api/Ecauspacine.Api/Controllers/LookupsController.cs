using Ecauspacine.Api.Features.Lookups;
using Ecauspacine.Contracts.Lookups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecauspacine.Api.Controllers;

/// <summary>
/// Gestion des groupes & items de référentiel (enum côté C#).
/// </summary>
[ApiController]
[Route("api/lookups")]
public class LookupsController : ControllerBase
{
    private readonly ILookupService _service;
    public LookupsController(ILookupService service) => _service = service;

    // ===== GROUPS =====

    /// <summary>Liste les groupes. includeItems=true pour inclure les items.</summary>
    [HttpGet("groups")]
    public async Task<ActionResult<IReadOnlyList<LookupGroupDto>>> ListGroups([FromQuery] bool includeItems, CancellationToken ct)
        => Ok(await _service.ListGroupsAsync(includeItems, ct));

    [HttpPost("groups")]
    public async Task<ActionResult<LookupGroupDto>> CreateGroup([FromBody] LookupGroupCreateDto body, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateGroupAsync(body, ct);
            return CreatedAtAction(nameof(ListGroups), new { id = created.Id }, created);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPut("groups/{id:long}")]
    public async Task<ActionResult<LookupGroupDto>> UpdateGroup(long id, [FromBody] LookupGroupUpdateDto body, CancellationToken ct)
    {
        try
        {
            var updated = await _service.UpdateGroupAsync(id, body, ct);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpDelete("groups/{id:long}")]
    public async Task<IActionResult> DeleteGroup(long id, CancellationToken ct)
    {
        try
        {
            var ok = await _service.DeleteGroupAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { error = "Impossible de supprimer un groupe contenant des items.", detail = ex.Message });
        }
    }

    // ===== ITEMS =====

    [HttpGet("groups/{groupId:long}/items")]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> ListItems(long groupId, CancellationToken ct)
        => Ok(await _service.ListItemsAsync(groupId, ct));

    /// <summary>
    /// Crée un item (GroupId dans le body, conformément au contrat).
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<LookupItemDto>> CreateItem([FromBody] LookupItemCreateDto body, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateItemAsync(body, ct);
            return CreatedAtAction(nameof(ListItems), new { groupId = created.GroupId, id = created.Id }, created);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPut("items/{itemId:long}")]
    public async Task<ActionResult<LookupItemDto>> UpdateItem(long itemId, [FromBody] LookupItemUpdateDto body, CancellationToken ct)
    {
        try
        {
            var updated = await _service.UpdateItemAsync(itemId, body, ct);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpDelete("items/{itemId:long}")]
    public async Task<IActionResult> DeleteItem(long itemId, CancellationToken ct)
    {
        var ok = await _service.DeleteItemAsync(itemId, ct);
        return ok ? NoContent() : NotFound();
    }
}
