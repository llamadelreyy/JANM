using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PBTPro.DAL.Models;

namespace PBTPro.DAL;

public partial class PBTProDbContext : IdentityDbContext<ApplicationUser>
{
    //public PBTProDbContext(DbContextOptions<PBTProDbContext> options)
    //    : base(options)
    //{
    //}

    public virtual DbSet<AppEmailQueue> AppEmailQueues { get; set; }

    public virtual DbSet<AppEmailTemplate> AppEmailTemplates { get; set; }

    public virtual DbSet<AppSystemMessage> AppSystemMessages { get; set; }

    public virtual DbSet<MstLot> MstLots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<AppEmailQueue>(entity =>
        {
            entity.HasKey(e => e.RecId).HasName("app_email_queue_pkey");

            entity.ToTable("app_email_queue");

            entity.Property(e => e.RecId)
                .HasDefaultValueSql("nextval('app_email_queue_recid_seq'::regclass)")
                .HasColumnName("rec_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_dtm");
            entity.Property(e => e.DateSent)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_sent");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_dtm");
            entity.Property(e => e.Remark).HasColumnName("remark");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'New'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasMaxLength(255)
                .HasColumnName("subject");
            entity.Property(e => e.ToEmail)
                .HasMaxLength(255)
                .HasColumnName("to_email");
        });

        modelBuilder.Entity<AppEmailTemplate>(entity =>
        {
            entity.HasKey(e => e.RecId).HasName("app_email_templates_pkey");

            entity.ToTable("app_email_template");

            entity.Property(e => e.RecId)
                .HasDefaultValueSql("nextval('app_email_templates_recid_seq'::regclass)")
                .HasColumnName("rec_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_dtm");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_dtm");
            entity.Property(e => e.Subject)
                .HasMaxLength(255)
                .HasColumnName("subject");
        });

        modelBuilder.Entity<AppSystemMessage>(entity =>
        {
            entity.HasKey(e => e.RecId).HasName("app_system_message_pkey");

            entity.ToTable("app_system_message");

            entity.Property(e => e.RecId).HasColumnName("rec_id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_dtm");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_dtm");
            entity.Property(e => e.Module)
                .HasMaxLength(50)
                .HasColumnName("module");
            entity.Property(e => e.Type)
                .HasMaxLength(1)
                .HasColumnName("type");
        });

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
