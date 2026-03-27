using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastTransfers.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.HasKey(u => u.Id);

        b.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        b.HasIndex(u => u.Email)
            .IsUnique();

        b.Property(u => u.PasswordHash)
            .IsRequired();

        b.HasMany(u => u.Projects)
            .WithOne(p => p.Owner)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> b)
    {
        b.HasKey(p => p.Id);

        b.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        b.HasMany(p => p.Folders)
            .WithOne(f => f.Project)
            .HasForeignKey(f => f.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> b)
    {
        b.HasKey(f => f.Id);

        b.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(200);

        b.HasOne(f => f.Schema)
            .WithOne(s => s.Folder)
            .HasForeignKey<SchemaTemplate>(s => s.FolderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(f => f.Files)
            .WithOne(file => file.Folder)
            .HasForeignKey(file => file.FolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SchemaTemplateConfiguration : IEntityTypeConfiguration<SchemaTemplate>
{
    public void Configure(EntityTypeBuilder<SchemaTemplate> b)
    {
        b.HasKey(s => s.Id);

        b.Property(s => s.SchemaYaml)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        b.Property(s => s.TemplateHtml)
            .HasColumnType("nvarchar(max)");

        b.Property(s => s.TemplateCss)
            .HasColumnType("nvarchar(max)");
    }
}

public class AppFileConfiguration : IEntityTypeConfiguration<AppFile>
{
    public void Configure(EntityTypeBuilder<AppFile> b)
    {
        b.HasKey(f => f.Id);

        b.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(300);

        b.Property(f => f.StorageKey)
            .IsRequired()
            .HasMaxLength(500);

        b.Property(f => f.StorageProvider)
            .IsRequired()
            .HasConversion<string>()  // Store as "Database", "AzureBlob" etc.
            .HasMaxLength(50);

        b.Property(f => f.ContentType)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("text/html");

        b.HasMany(f => f.Metadata)
            .WithOne(m => m.File)
            .HasForeignKey(m => m.FileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AppFileMetadataConfiguration : IEntityTypeConfiguration<AppFileMetadata>
{
    public void Configure(EntityTypeBuilder<AppFileMetadata> b)
    {
        b.HasKey(m => m.Id);

        b.Property(m => m.Key)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(m => m.Value)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        // One file can't have duplicate keys
        b.HasIndex(m => new { m.FileId, m.Key })
            .IsUnique();
    }
}

public class FileContentConfiguration : IEntityTypeConfiguration<FileContent>
{
    public void Configure(EntityTypeBuilder<FileContent> b)
    {
        b.HasKey(f => f.Id);

        b.Property(f => f.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)");
    }
}
