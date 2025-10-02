using DotNetEnv;

namespace Ecauspacine.Api.Configuration;

/// <summary>
/// Petites opérations de démarrage (lisibles et testables)
/// </summary>
public static class AppBootstrap
{
    /// <summary>
    /// Charge le fichier .env uniquement en développement (ignore s'il est absent).
    /// </summary>
    public static void LoadEnvIfDev(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            try { Env.Load(); } catch { /* pas d'exception bloquante */ }
        }
    }

    /// <summary>
    /// Force Kestrel à écouter sur http://0.0.0.0:5000 (contexte VPS/Docker).
    /// </summary>
    public static void ConfigureKestrelUrl(WebApplicationBuilder builder)
    {
        builder.WebHost.UseUrls("http://0.0.0.0:5000");
    }
}
