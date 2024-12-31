using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PBTPro.DAL.Models;

public partial class TempContext : DbContext
{
    public TempContext(DbContextOptions<TempContext> options)
        : base(options)
    {
    }

    public virtual DbSet<permission> permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<permission>(entity =>
        {
            entity.HasKey(e => e.permission_id).HasName("permissions_pkey");

            entity.ToTable("permissions", "core", tb => tb.HasComment("The core.permission table manages permissions for various features within the system. It associates roles with specific features and defines the level of access each role has for those features."));

            entity.Property(e => e.permission_id).HasComment("Unique identifier for each core.permission record.");
            entity.Property(e => e.can_add)
                .HasDefaultValue(true)
                .HasComment("Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.");
            entity.Property(e => e.can_approve_changes)
                .HasDefaultValue(true)
                .HasComment("In systems where changes need approval before implementation, this column can specify whether users with the specified role can approve proposed changes.");
            entity.Property(e => e.can_authorize)
                .HasDefaultValue(true)
                .HasComment("If your system includes approval processes, this column specifies whether users with the specified role have the authority to approve or authorize certain actions or transactions for themselves or as supervisors.");
            entity.Property(e => e.can_delete)
                .HasDefaultValue(true)
                .HasComment("Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.");
            entity.Property(e => e.can_download)
                .HasDefaultValue(true)
                .HasComment("Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.");
            entity.Property(e => e.can_edit)
                .HasDefaultValue(true)
                .HasComment("Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.");
            entity.Property(e => e.can_execute)
                .HasDefaultValue(true)
                .HasComment("This column indicates whether a user with the specified role can execute or perform actions associated with a feature. It can be useful for scenarios where viewing, adding, editing, or deleting actions need to be restricted separately.");
            entity.Property(e => e.can_export_data)
                .HasDefaultValue(true)
                .HasComment("If your system involves exporting data, this column can specify whether users with the specified role can export data from the system.");
            entity.Property(e => e.can_import_data)
                .HasDefaultValue(false)
                .HasComment("Similarly, if data import functionality exists, this column can determine whether users with the specified role can import data into the system");
            entity.Property(e => e.can_print)
                .HasDefaultValue(true)
                .HasComment("Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.");
            entity.Property(e => e.can_upload)
                .HasDefaultValue(true)
                .HasComment("Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.");
            entity.Property(e => e.can_view)
                .HasDefaultValue(true)
                .HasComment("Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.");
            entity.Property(e => e.can_view_sensitive)
                .HasDefaultValue(true)
                .HasComment("For systems dealing with sensitive information, this column can control whether users with the specified role are allowed to view sensitive data.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the row was created.");
            entity.Property(e => e.creator_id).HasComment("User ID of the creator ");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the row has been logically deleted (soft deleted). 0 represents not deleted, and 1 represents deleted.");
            entity.Property(e => e.menu_id).HasComment("Identifier for the feature to which the core.permission applies.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the user record was last modified.");
            entity.Property(e => e.modifier_id).HasComment("User ID of the modifier");
            entity.Property(e => e.role_id).HasComment("Identifier for the role associated with the core.permission.");
        });
        modelBuilder.HasSequence("auditlog_archive_id_seq", "audit");
        modelBuilder.HasSequence("patrol_info_id_seq", "patrol");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
