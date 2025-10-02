using System.Text.Json;
using Ecauspacine.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecauspacine.Api.Controllers;

[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    private readonly EcauspacineDbContext _db;
    private readonly IWebHostEnvironment _env;

    public DebugController(EcauspacineDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet("db")]
    public async Task<IActionResult> Db(CancellationToken ct)
    {
        var provider = _db.Database.ProviderName ?? "(unknown)";
        var isRelational = _db.Database.IsRelational();
        string? databaseFromConn = null;
        string? dataSourceFromConn = null;

        if (isRelational)
        {
            try
            {
                var conn = _db.Database.GetDbConnection();
                databaseFromConn = conn.Database;
                dataSourceFromConn = conn.DataSource;
            }
            catch (Exception ex)
            {
                dataSourceFromConn = $"(error reading connection): {ex.Message}";
            }
        }

        int? entityTypeCount = null;
        string? entityTypeError = null;

        try
        {
            entityTypeCount = await _db.EntityTypes.CountAsync(ct);
        }
        catch (Exception ex)
        {
            entityTypeError = ex.GetType().Name + ": " + ex.Message;
        }

        var payload = new
        {
            environment = _env.EnvironmentName,
            provider,
            isRelational,
            connection = new
            {
                database = databaseFromConn,
                dataSource = dataSourceFromConn
            },
            entityType = new
            {
                count = entityTypeCount,
                error = entityTypeError
            }
        };

        // On écrit en JSON bien lisible
        return Content(JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }), "application/json");
    }
}
