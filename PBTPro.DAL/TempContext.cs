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

    public virtual DbSet<MstLot> MstLots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<MstLot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Sample_Lot_pkey");

            entity.ToTable("mst_lot");

            entity.HasIndex(e => e.Geom, "sidx_Sample_Lot_geom").HasMethod("gist");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Apdate)
                .HasMaxLength(8)
                .HasColumnName("apdate");
            entity.Property(e => e.Cls)
                .HasMaxLength(1)
                .HasColumnName("cls");
            entity.Property(e => e.Daerah)
                .HasMaxLength(2)
                .HasColumnName("daerah");
            entity.Property(e => e.Entrymode)
                .HasMaxLength(1)
                .HasColumnName("entrymode");
            entity.Property(e => e.GArea).HasColumnName("g_area");
            entity.Property(e => e.Geom)
                .HasColumnType("geometry(MultiPolygon,3375)")
                .HasColumnName("geom");
            entity.Property(e => e.Guid)
                .HasMaxLength(32)
                .HasColumnName("guid");
            entity.Property(e => e.Landtitlec)
                .HasMaxLength(2)
                .HasColumnName("landtitlec");
            entity.Property(e => e.Landusecod)
                .HasMaxLength(2)
                .HasColumnName("landusecod");
            entity.Property(e => e.Lot)
                .HasMaxLength(7)
                .HasColumnName("lot");
            entity.Property(e => e.MArea).HasColumnName("m_area");
            entity.Property(e => e.MiPrinx).HasColumnName("mi_prinx");
            entity.Property(e => e.Mukim)
                .HasMaxLength(2)
                .HasColumnName("mukim");
            entity.Property(e => e.Negeri)
                .HasMaxLength(2)
                .HasColumnName("negeri");
            entity.Property(e => e.Objectid).HasColumnName("objectid");
            entity.Property(e => e.Pa)
                .HasMaxLength(15)
                .HasColumnName("pa");
            entity.Property(e => e.Refplan)
                .HasMaxLength(15)
                .HasColumnName("refplan");
            entity.Property(e => e.SArea).HasColumnName("s_area");
            entity.Property(e => e.Seksyen)
                .HasMaxLength(3)
                .HasColumnName("seksyen");
            entity.Property(e => e.ShapeArea).HasColumnName("shape_area");
            entity.Property(e => e.ShapeLeng).HasColumnName("shape_leng");
            entity.Property(e => e.Unit)
                .HasMaxLength(1)
                .HasColumnName("unit");
            entity.Property(e => e.Updated).HasColumnName("updated");
            entity.Property(e => e.Upi)
                .HasMaxLength(16)
                .HasColumnName("upi");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
