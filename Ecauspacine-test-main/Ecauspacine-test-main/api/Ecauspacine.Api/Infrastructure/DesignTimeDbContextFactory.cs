using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text;

namespace Ecauspacine.Api.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EcauspacineDbContext>
{
    public EcauspacineDbContext CreateDbContext(string[] args)
    {
        // Permet "dotnet ef ..." en lisant .env si présent
        try 
        { 
            Env.Load(); 
        } 
        catch 
        { 
            /* ignore */ 
        }

        var builder = new DbContextOptionsBuilder<EcauspacineDbContext>();
        var conn = Environment.GetEnvironmentVariable("ConnectionStrings__Db")
                   ?? BuildFromDbEnv()
                   ?? throw new InvalidOperationException("Configure ConnectionStrings__Db ou DB_* pour les migrations EF.");

        var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
        builder.UseMySql(conn, serverVersion, my => my.MigrationsAssembly(typeof(EcauspacineDbContext).Assembly.FullName));
        return new EcauspacineDbContext(builder.Options);
    }

    private static string? BuildFromDbEnv()
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
