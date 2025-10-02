using Ecauspacine.Api.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1) Bootstrap léger (charge .env en dev, fixe l'URL Kestrel)
AppBootstrap.LoadEnvIfDev(builder);
AppBootstrap.ConfigureKestrelUrl(builder); // http://0.0.0.0:5000

// 2) Enregistrer tous les services via des extensions parlantes
builder.Services
    .AddApiBasics()           // Controllers, Swagger, HealthChecks
    .AddCorsFromEnv()         // CORS_ORIGINS ou "*"
    .AddMySqlDbContext(builder.Configuration, builder.Environment) // Connexion MySQL (env/config)
    .AddFeatureServices();    // Services par Feature (EntityTypes, etc.)

var app = builder.Build();

// 3) Pipeline HTTP standard
app.UseApiBasics();           // CORS + Swagger UI
app.MapControllers();
// HealthChecks avec writer JSON
app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        var json = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString().ToLowerInvariant()
        });
        await ctx.Response.WriteAsync(json);
    }
});

app.Run();
