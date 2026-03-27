using FastTransfers.Application.Common;
using FastTransfers.Domain.Interfaces;
using FastTransfers.Infrastructure.Identity;
using FastTransfers.Infrastructure.Persistence;
using FastTransfers.Infrastructure.Persistence.Repositories;
using FastTransfers.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FastTransfers.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                                        IConfiguration config)
    {
        // ── Database ──────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // ── Repositories ──────────────────────────────────────
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IFolderRepository, FolderRepository>();
        services.AddScoped<ISchemaTemplateRepository, SchemaTemplateRepository>();
        services.AddScoped<IAppFileRepository, AppFileRepository>();
        services.AddScoped<IFileContentRepository, FileContentRepository>();

        // ── Storage ───────────────────────────────────────────
        // Switch provider via Storage:Provider in appsettings.json:
        //   "Database" → SQL Server FileContents table
        //   "Local"    → local filesystem (uploads folder)
        //   "MongoDB"  → MongoDB collection
        // ── Storage ───────────────────────────────────────────
        var provider = config["Storage:Provider"] ?? "Database";

        if (provider == "Local")
        {
            services.AddScoped<IFileStorageService, LocalStorageService>();
        }
        else if (provider == "MongoDB")
        {
            // This is the important part for MongoDB
            var mongoConnectionString = config["Storage:MongoDB:ConnectionString"];

            if (string.IsNullOrWhiteSpace(mongoConnectionString))
            {
                throw new InvalidOperationException("MongoDB ConnectionString is missing from configuration. Check .env or environment variables.");
            }

            // You can also get DatabaseName and CollectionName if needed
            var mongoDatabaseName = config["Storage:MongoDB:DatabaseName"] ?? "Formify";
            var mongoCollectionName = config["Storage:MongoDB:CollectionName"] ?? "fileContents";

            // Register MongoDB services here (example)
            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
            services.AddScoped<IFileStorageService, MongoDbStorageService>();
        }
        else
        {
            services.AddScoped<IFileStorageService, DatabaseStorageService>();
        }

        // ── Identity ──────────────────────────────────────────
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}