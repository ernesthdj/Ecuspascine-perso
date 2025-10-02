using Ecauspacine.Api.Features.EntityType;
using Ecauspacine.Api.Features.EntityTypes;
using Ecauspacine.Api.Features.Lookups;
using Ecauspacine.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Ecauspacine.Api.Configuration;

/// <summary>
/// Regroupe les enregistrements de services (DI) pour garder Program.cs concis.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre le minimum pour une Web API : Controllers, Swagger, HealthChecks.
    /// </summary>
    public static IServiceCollection AddApiBasics(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();
        return services;
    }

    /// <summary>
    /// Configure CORS en lisant CORS_ORIGINS (CSV). Si vide/absent, autorise tout (*).
    /// </summary>
    public static IServiceCollection AddCorsFromEnv(this IServiceCollection services)
    {
        const string CorsPolicyName = "CorsPolicy";
        var raw = Environment.GetEnvironmentVariable("CORS_ORIGINS");
        string[]? origins = raw?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                if (origins is { Length: > 0 })
                {
                    policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
                }
                else
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Enregistre le DbContext.
    /// - DEV: si pas de connection string, fallback InMemory (DevFallback).
    /// - PROD: si pas de connection string, on jette une exception explicite.
    /// </summary>
    public static IServiceCollection AddMySqlDbContext(
        this IServiceCollection services,
        IConfiguration config,
        IHostEnvironment env)
    {
        string? connStr = config.GetConnectionString("Db");
        connStr ??= BuildMySqlConnectionStringFromEnv();

        if (string.IsNullOrWhiteSpace(connStr))
        {
            if (env.IsDevelopment())
            {
                Console.WriteLine("Aucune connection string fournie. Fallback DEV: InMemory database 'DevFallback'.");
                services.AddDbContext<EcauspacineDbContext>(opt =>
                {
                    opt.UseInMemoryDatabase("DevFallback");
                });
                return services;
            }

            throw new InvalidOperationException(
                "Aucune connection string fournie pour MySQL. " +
                "Configurez 'ConnectionStrings__Db' ou les variables DB_HOST/DB_NAME/DB_USER/DB_PASSWORD.");
        }

        var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
        services.AddDbContext<EcauspacineDbContext>(opt =>
        {
            opt.UseMySql(connStr, serverVersion, my =>
                my.MigrationsAssembly(typeof(EcauspacineDbContext).Assembly.FullName));
        });

        return services;
    }

    /// <summary>
    /// Enregistre les services applicatifs par "Feature".
    /// </summary>
    public static IServiceCollection AddFeatureServices(this IServiceCollection services)
    {
        // EntityTypes
        services.AddScoped<IEntityTypeService, EntityTypeService>();
        // LookupGroups & LookupItems
        services.AddScoped<ILookupService, LookupService>();

        // Autres services par feature à ajouter ici...
        return services;
    }

    private static string? BuildMySqlConnectionStringFromEnv()
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
        var db = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var pwd = Environment.GetEnvironmentVariable("DB_PASSWORD");

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(db) ||
            string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pwd))
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.Append($"Server={host};Port={port};Database={db};User Id={user};Password={pwd};");
        sb.Append("TreatTinyAsBoolean=true;CharSet=utf8mb4;SslMode=None");
        return sb.ToString();
    }
}
