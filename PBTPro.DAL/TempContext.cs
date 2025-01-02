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

    public virtual DbSet<menu> menus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<menu>(entity =>
        {
            entity.HasKey(e => e.menu_id).HasName("menus_pkey");

            entity.ToTable("menus", "core", tb => tb.HasComment("This table stores information about the menus available in the system. It can be hierarchical, where each core.menu item may have a parent core.menu."));

            entity.Property(e => e.menu_id).HasComment("Identifier for each core.menu item.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the row was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("User ID of the creator.");
            entity.Property(e => e.icon_path)
                .HasMaxLength(50)
                .HasDefaultValueSql("'0'::character varying")
                .HasComment("Path to the icon associated with the core.menu item.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the row has been logically deleted (soft deleted). 0 represents not deleted, and 1 represents deleted.");
            entity.Property(e => e.is_tenant)
                .HasDefaultValue(false)
                .HasComment("Indicates whether the core.menu is associated with tenant modules.");
            entity.Property(e => e.menu_name)
                .HasMaxLength(50)
                .HasDefaultValueSql("'0'::character varying")
                .HasComment("Name of the core.menu item.");
            entity.Property(e => e.menu_path)
                .HasMaxLength(50)
                .HasComment("Path/Route to the UI with the core.menu item.");
            entity.Property(e => e.menu_sequence)
                .HasDefaultValue(0)
                .HasComment("Sequence number indicating the order of the core.menu item.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the user record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("User ID of the modifier.");
            entity.Property(e => e.module_id).HasComment("Identifier for the module associated with the core.menu item.");
            entity.Property(e => e.parent_id)
                .HasDefaultValue(0)
                .HasComment("Identifier linking the core.menu item to its parent core.menu item (if applicable).");
        });
        modelBuilder.HasSequence("auditlog_archive_id_seq", "audit");
        modelBuilder.HasSequence("patrol_info_id_seq", "patrol");
        modelBuilder.HasSequence("ref_department_dept_id_seq", "tenant");
        modelBuilder.HasSequence("ref_division_div_id_seq", "tenant");
        modelBuilder.HasSequence("ref_unit_unit_id_seq", "tenant");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
