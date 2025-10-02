namespace Ecauspacine.Api.Configuration;

/// <summary>
/// Extensions pour le pipeline HTTP (middleware) pour garder Program.cs propre.
/// </summary>
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApiBasics(this IApplicationBuilder app)
    {
        // CORS : même nom de policy que dans AddCorsFromEnv()
        app.UseCors("CorsPolicy");

        // Swagger toujours actif (simplifie le debug et le déploiement)
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
