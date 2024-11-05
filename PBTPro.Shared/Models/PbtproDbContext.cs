using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PBTPro.Shared.Models;

public partial class PbtproDbContext : DbContext
{
    public PbtproDbContext()
    {
    }

    public PbtproDbContext(DbContextOptions<PbtproDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Action> Actions { get; set; }

    public virtual DbSet<Codeaction> Codeactions { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Offense> Offenses { get; set; }

    public virtual DbSet<Version> Versions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=pbtpro;Username=postgres;Password=Password1");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>(entity =>
        {
            entity.HasKey(e => e.Actionid).HasName("actions_pkey");

            entity.ToTable("actions");

            entity.Property(e => e.Actionid).HasColumnName("actionid");
            entity.Property(e => e.Actiondescription).HasColumnName("actiondescription");
            entity.Property(e => e.Actionenabled).HasColumnName("actionenabled");
            entity.Property(e => e.Actionname)
                .HasMaxLength(255)
                .HasColumnName("actionname");
        });

        modelBuilder.Entity<Codeaction>(entity =>
        {
            entity.HasKey(e => e.Codeactid).HasName("codeactions_pkey");

            entity.ToTable("codeactions");

            entity.Property(e => e.Codeactid).HasColumnName("codeactid");
            entity.Property(e => e.Codeactname)
                .HasMaxLength(255)
                .HasColumnName("codeactname");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Deptid).HasName("departments_pkey");

            entity.ToTable("departments");

            entity.Property(e => e.Deptid).HasColumnName("deptid");
            entity.Property(e => e.Deptdescription).HasColumnName("deptdescription");
            entity.Property(e => e.Deptname)
                .HasMaxLength(255)
                .HasColumnName("deptname");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Locid).HasName("locations_pkey");

            entity.ToTable("locations");

            entity.Property(e => e.Locid).HasColumnName("locid");
            entity.Property(e => e.Locname)
                .HasMaxLength(255)
                .HasColumnName("locname");
        });

        modelBuilder.Entity<Offense>(entity =>
        {
            entity.HasKey(e => e.Offenseid).HasName("offenses_pkey");

            entity.ToTable("offenses");

            entity.Property(e => e.Offenseid).HasColumnName("offenseid");
            entity.Property(e => e.Offensedescription).HasColumnName("offensedescription");
            entity.Property(e => e.Offenseenabled).HasColumnName("offenseenabled");
            entity.Property(e => e.Offensename)
                .HasMaxLength(255)
                .HasColumnName("offensename");
        });

        modelBuilder.Entity<Version>(entity =>
        {
            entity.HasKey(e => e.Versionid).HasName("versions_pkey");

            entity.ToTable("versions");

            entity.Property(e => e.Versionid)
                .ValueGeneratedNever()
                .HasColumnName("versionid");
            entity.Property(e => e.Versiondescription).HasColumnName("versiondescription");
            entity.Property(e => e.Versionname)
                .HasMaxLength(255)
                .HasColumnName("versionname");
            entity.Property(e => e.Versionnumber)
                .HasMaxLength(255)
                .HasColumnName("versionnumber");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
