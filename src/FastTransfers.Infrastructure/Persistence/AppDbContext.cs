using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastTransfers.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User>           Users           => Set<User>();
    public DbSet<Project>        Projects        => Set<Project>();
    public DbSet<Folder>         Folders         => Set<Folder>();
    public DbSet<SchemaTemplate> SchemaTemplates => Set<SchemaTemplate>();
    public DbSet<AppFile>        AppFiles        => Set<AppFile>();
    public DbSet<AppFileMetadata> AppFileMetadata => Set<AppFileMetadata>();
    public DbSet<FileContent>    FileContents    => Set<FileContent>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await base.SaveChangesAsync(ct);
}
