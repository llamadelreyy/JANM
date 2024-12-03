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

    public virtual DbSet<config_system_param> config_system_params { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<config_system_param>(entity =>
        {
            entity.HasKey(e => e.param_id).HasName("config_system_param_pkey");

            entity.ToTable("config_system_param", "config");

            entity.Property(e => e.active_flag).HasDefaultValue(true);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.param_app_layer).HasMaxLength(10);
            entity.Property(e => e.param_group).HasMaxLength(50);
            entity.Property(e => e.param_name).HasMaxLength(50);
            entity.Property(e => e.param_value).HasMaxLength(1000);
            entity.Property(e => e.update_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.updated_by).HasDefaultValue(0);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
