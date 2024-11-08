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

    public virtual DbSet<MstArea> MstAreas { get; set; }

    public virtual DbSet<MstDaerah> MstDaerahs { get; set; }

    public virtual DbSet<MstDistrict> MstDistricts { get; set; }

    public virtual DbSet<MstLot> MstLots { get; set; }

    public virtual DbSet<MstMukim> MstMukims { get; set; }

    public virtual DbSet<ParFormField> ParFormFields { get; set; }

    public virtual DbSet<TbAuditlog> TbAuditlogs { get; set; }

    public virtual DbSet<TbFaq> TbFaqs { get; set; }

    public virtual DbSet<TbHubungikami> TbHubungikamis { get; set; }

    public virtual DbSet<TrnPatrol> TrnPatrols { get; set; }

    public virtual DbSet<TrnPatrolDet> TrnPatrolDets { get; set; }   

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
            entity.Property(e => e.CntRetry)
                .HasDefaultValue(0)
                .HasColumnName("cnt_retry");
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
       
        modelBuilder.Entity<AppFormField>(entity =>
        {
            entity.HasKey(e => e.RecId).HasName("app_form_field_pkey");

            entity.ToTable("app_form_field");

            entity.Property(e => e.RecId).HasColumnName("rec_id");
            entity.Property(e => e.ApiSeeded)
                .HasDefaultValue(false)
                .HasColumnName("api_seeded");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_dtm");
            entity.Property(e => e.FormType)
                .HasMaxLength(50)
                .HasColumnName("form_type");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Label)
                .HasMaxLength(50)
                .HasColumnName("label");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_dtm");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Option).HasColumnName("option");
            entity.Property(e => e.Orders)
                .HasDefaultValue(0)
                .HasColumnName("orders");
            entity.Property(e => e.Required)
                .HasDefaultValue(false)
                .HasColumnName("required");
            entity.Property(e => e.SourceUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("source_url");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");
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
            entity.Property(e => e.Feature)
                .HasMaxLength(50)
                .HasColumnName("feature");
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
            entity.Property(e => e.Type)
                .HasMaxLength(1)
                .HasColumnName("type");
        });

        modelBuilder.Entity<MstArea>(entity =>
        {
            entity.HasKey(e => e.Gid).HasName("mst_area_pkey");

            entity.ToTable("mst_area");

            entity.HasIndex(e => e.Geom, "mst_area_geom_idx").HasMethod("gist");

            entity.Property(e => e.Gid).HasColumnName("gid");
            entity.Property(e => e.Acc)
                .HasMaxLength(50)
                .HasColumnName("acc");
            entity.Property(e => e.Ark)
                .HasMaxLength(50)
                .HasColumnName("ark");
            entity.Property(e => e.Bds)
                .HasMaxLength(50)
                .HasColumnName("bds");
            entity.Property(e => e.Fcd)
                .HasMaxLength(7)
                .HasColumnName("fcd");
            entity.Property(e => e.Fnm)
                .HasMaxLength(100)
                .HasColumnName("fnm");
            entity.Property(e => e.Geom)
                .HasColumnType("geometry(MultiPolygon,3375)")
                .HasColumnName("geom");
            entity.Property(e => e.Keluasan)
                .HasMaxLength(50)
                .HasColumnName("keluasan");
            entity.Property(e => e.Kemaskini)
                .HasMaxLength(50)
                .HasColumnName("kemaskini");
            entity.Property(e => e.KodDaerah)
                .HasMaxLength(50)
                .HasColumnName("kod_daerah");
            entity.Property(e => e.KodNegeri)
                .HasMaxLength(50)
                .HasColumnName("kod_negeri");
            entity.Property(e => e.Nam)
                .HasMaxLength(50)
                .HasColumnName("nam");
            entity.Property(e => e.Objectid).HasColumnName("objectid");
            entity.Property(e => e.ShapeArea).HasColumnName("shape_area");
            entity.Property(e => e.ShapeLeng).HasColumnName("shape_leng");
        });

        modelBuilder.Entity<MstDaerah>(entity =>
        {
            entity.HasKey(e => e.Gid).HasName("mst_daerah_pkey");

            entity.ToTable("mst_daerah");

            entity.HasIndex(e => e.Geom, "mst_daerah_geom_idx").HasMethod("gist");

            entity.Property(e => e.Gid).HasColumnName("gid");
            entity.Property(e => e.Acc)
                .HasMaxLength(50)
                .HasColumnName("acc");
            entity.Property(e => e.Ark)
                .HasMaxLength(50)
                .HasColumnName("ark");
            entity.Property(e => e.Bds)
                .HasMaxLength(50)
                .HasColumnName("bds");
            entity.Property(e => e.Fcd)
                .HasMaxLength(7)
                .HasColumnName("fcd");
            entity.Property(e => e.Fnm)
                .HasMaxLength(100)
                .HasColumnName("fnm");
            entity.Property(e => e.Geom)
                .HasColumnType("geometry(MultiPolygon,3375)")
                .HasColumnName("geom");
            entity.Property(e => e.Keluasan)
                .HasMaxLength(50)
                .HasColumnName("keluasan");
            entity.Property(e => e.Kemaskini)
                .HasMaxLength(50)
                .HasColumnName("kemaskini");
            entity.Property(e => e.KodDaerah)
                .HasMaxLength(50)
                .HasColumnName("kod_daerah");
            entity.Property(e => e.KodNegeri)
                .HasMaxLength(50)
                .HasColumnName("kod_negeri");
            entity.Property(e => e.Nam)
                .HasMaxLength(50)
                .HasColumnName("nam");
            entity.Property(e => e.Objectid).HasColumnName("objectid");
            entity.Property(e => e.ShapeArea).HasColumnName("shape_area");
            entity.Property(e => e.ShapeLeng).HasColumnName("shape_leng");
        });

        modelBuilder.Entity<MstDistrict>(entity =>
        {
            entity.HasKey(e => e.Gid).HasName("mst_district_pkey");

            entity.ToTable("mst_district");

            entity.HasIndex(e => e.Geom, "mst_district_geom_idx").HasMethod("gist");

            entity.Property(e => e.Gid).HasColumnName("gid");
            entity.Property(e => e.Acc)
                .HasMaxLength(50)
                .HasColumnName("acc");
            entity.Property(e => e.Ark)
                .HasMaxLength(50)
                .HasColumnName("ark");
            entity.Property(e => e.Bds)
                .HasMaxLength(50)
                .HasColumnName("bds");
            entity.Property(e => e.Dasdas)
                .HasMaxLength(50)
                .HasColumnName("dasdas");
            entity.Property(e => e.Fcd)
                .HasMaxLength(10)
                .HasColumnName("fcd");
            entity.Property(e => e.Fnm)
                .HasMaxLength(50)
                .HasColumnName("fnm");
            entity.Property(e => e.Geom)
                .HasColumnType("geometry(MultiPolygon,3375)")
                .HasColumnName("geom");
            entity.Property(e => e.Globalid)
                .HasMaxLength(38)
                .HasColumnName("globalid");
            entity.Property(e => e.Keluasan)
                .HasMaxLength(50)
                .HasColumnName("keluasan");
            entity.Property(e => e.KodDaerah)
                .HasMaxLength(50)
                .HasColumnName("kod_daerah");
            entity.Property(e => e.KodMukim)
                .HasMaxLength(50)
                .HasColumnName("kod_mukim");
            entity.Property(e => e.KodNegeri)
                .HasMaxLength(50)
                .HasColumnName("kod_negeri");
            entity.Property(e => e.Nam)
                .HasMaxLength(50)
                .HasColumnName("nam");
            entity.Property(e => e.Objectid).HasColumnName("objectid");
            entity.Property(e => e.ShapeArea).HasColumnName("shape_area");
            entity.Property(e => e.ShapeLeng).HasColumnName("shape_leng");
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

        modelBuilder.Entity<MstMukim>(entity =>
        {
            entity.HasKey(e => e.Gid).HasName("mst_mukim_pkey");

            entity.ToTable("mst_mukim");

            entity.HasIndex(e => e.Geom, "mst_mukim_geom_idx").HasMethod("gist");

            entity.Property(e => e.Gid).HasColumnName("gid");
            entity.Property(e => e.Acc)
                .HasMaxLength(50)
                .HasColumnName("acc");
            entity.Property(e => e.Ark)
                .HasMaxLength(50)
                .HasColumnName("ark");
            entity.Property(e => e.Bds)
                .HasMaxLength(50)
                .HasColumnName("bds");
            entity.Property(e => e.Dasdas)
                .HasMaxLength(50)
                .HasColumnName("dasdas");
            entity.Property(e => e.Fcd)
                .HasMaxLength(10)
                .HasColumnName("fcd");
            entity.Property(e => e.Fnm)
                .HasMaxLength(50)
                .HasColumnName("fnm");
            entity.Property(e => e.Geom)
                .HasColumnType("geometry(MultiPolygon,3375)")
                .HasColumnName("geom");
            entity.Property(e => e.Globalid)
                .HasMaxLength(38)
                .HasColumnName("globalid");
            entity.Property(e => e.Keluasan)
                .HasMaxLength(50)
                .HasColumnName("keluasan");
            entity.Property(e => e.KodDaerah)
                .HasMaxLength(50)
                .HasColumnName("kod_daerah");
            entity.Property(e => e.KodMukim)
                .HasMaxLength(50)
                .HasColumnName("kod_mukim");
            entity.Property(e => e.KodNegeri)
                .HasMaxLength(50)
                .HasColumnName("kod_negeri");
            entity.Property(e => e.Nam)
                .HasMaxLength(50)
                .HasColumnName("nam");
            entity.Property(e => e.Objectid).HasColumnName("objectid");
            entity.Property(e => e.ShapeArea).HasColumnName("shape_area");
            entity.Property(e => e.ShapeLeng).HasColumnName("shape_leng");
        });

        modelBuilder.Entity<ParFormField>(entity =>
        {
            entity.HasKey(e => e.RecId).HasName("par_form_field_pkey");

            entity.ToTable("par_form_field");

            entity.Property(e => e.RecId).HasColumnName("rec_id");
            entity.Property(e => e.ApiSeeded)
                .HasDefaultValue(false)
                .HasColumnName("api_seeded");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_dtm");
            entity.Property(e => e.FormType)
                .HasMaxLength(50)
                .HasColumnName("form_type");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Label)
                .HasMaxLength(50)
                .HasColumnName("label");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_dtm");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Option).HasColumnName("option");
            entity.Property(e => e.Orders)
                .HasDefaultValue(0)
                .HasColumnName("orders");
            entity.Property(e => e.Required)
                .HasDefaultValue(false)
                .HasColumnName("required");
            entity.Property(e => e.SourceUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("source_url");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");
        });

        modelBuilder.Entity<TbAuditlog>(entity =>
        {
            entity.HasKey(e => e.Auditid).HasName("tb_auditlog_pkey");

            entity.ToTable("tb_auditlog");

            entity.Property(e => e.Auditid).HasColumnName("auditid");
            entity.Property(e => e.Catatan).HasColumnName("catatan");
            entity.Property(e => e.Jenisaudit)
                .HasMaxLength(255)
                .HasColumnName("jenisaudit");
            entity.Property(e => e.Method)
                .HasMaxLength(10)
                .HasColumnName("method");
            entity.Property(e => e.Namamodule)
                .HasMaxLength(255)
                .HasColumnName("namamodule");
            entity.Property(e => e.Perananid).HasColumnName("perananid");
            entity.Property(e => e.Rekcipta)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("rekcipta");
            entity.Property(e => e.Rekciptauserid).HasColumnName("rekciptauserid");
        });

        modelBuilder.Entity<TbFaq>(entity =>
        {
            entity.HasKey(e => e.Faqid).HasName("tb_faq_pkey");

            entity.ToTable("tb_faq");

            entity.Property(e => e.Faqid).HasColumnName("faqid");
            entity.Property(e => e.Jawapanfaq).HasColumnName("jawapanfaq");
            entity.Property(e => e.Kategorifaq)
                .HasMaxLength(255)
                .HasColumnName("kategorifaq");
            entity.Property(e => e.Rekcipta)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("rekcipta");
            entity.Property(e => e.Rekciptauserid).HasColumnName("rekciptauserid");
            entity.Property(e => e.Rekstatus)
                .HasMaxLength(50)
                .HasColumnName("rekstatus");
            entity.Property(e => e.Rekubah)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("rekubah");
            entity.Property(e => e.Rekubahuserid).HasColumnName("rekubahuserid");
            entity.Property(e => e.Soalanfaq).HasColumnName("soalanfaq");
        });

        modelBuilder.Entity<TbHubungikami>(entity =>
        {
            entity.HasKey(e => e.Hubkamiid).HasName("tb_hubungikami_pkey");

            entity.ToTable("tb_hubungikami");

            entity.Property(e => e.Hubkamiid).HasColumnName("hubkamiid");
            entity.Property(e => e.Catatan).HasColumnName("catatan");
            entity.Property(e => e.Emailpenghantar)
                .HasMaxLength(255)
                .HasColumnName("emailpenghantar");
            entity.Property(e => e.Namapenerima)
                .HasMaxLength(255)
                .HasColumnName("namapenerima");
            entity.Property(e => e.Namapenghantar)
                .HasMaxLength(255)
                .HasColumnName("namapenghantar");
            entity.Property(e => e.Rekcipta)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("rekcipta");
            entity.Property(e => e.Rekciptauserid).HasColumnName("rekciptauserid");
            entity.Property(e => e.Rekubah)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("rekubah");
            entity.Property(e => e.Rekubahuserid).HasColumnName("rekubahuserid");
            entity.Property(e => e.Telnopenghantar)
                .HasMaxLength(20)
                .HasColumnName("telnopenghantar");
            entity.Property(e => e.Tiketid)
                .HasMaxLength(50)
                .HasColumnName("tiketid");
        });

        modelBuilder.Entity<TrnPatrol>(entity =>
        {
            entity.HasKey(e => e.RecId).HasName("trn_patrol_pkey");

            entity.ToTable("trn_patrol");

            entity.Property(e => e.RecId).HasColumnName("rec_id");
            entity.Property(e => e.CntCompound)
                .HasDefaultValue(0)
                .HasColumnName("cnt_compound");
            entity.Property(e => e.CntNotes)
                .HasDefaultValue(0)
                .HasColumnName("cnt_notes");
            entity.Property(e => e.CntNotice)
                .HasDefaultValue(0)
                .HasColumnName("cnt_notice");
            entity.Property(e => e.CntSeizure)
                .HasDefaultValue(0)
                .HasColumnName("cnt_seizure");
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
            entity.Property(e => e.StartDtm)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_dtm");
            entity.Property(e => e.StartLocation)
                .HasColumnType("geometry(Point,3375)")
                .HasColumnName("start_location");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'New'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.StopDtm)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("stop_dtm");
            entity.Property(e => e.StopLocation)
                .HasColumnType("geometry(Point,3375)")
                .HasColumnName("stop_location");
        });

        modelBuilder.Entity<TrnPatrolDet>(entity =>
        {
            entity.HasKey(e => e.RecId).HasName("trn_patrol_det_pkey");

            entity.ToTable("trn_patrol_det");

            entity.Property(e => e.RecId).HasColumnName("rec_id");
            entity.Property(e => e.CntCompound)
                .HasDefaultValue(0)
                .HasColumnName("cnt_compound");
            entity.Property(e => e.CntNotes)
                .HasDefaultValue(0)
                .HasColumnName("cnt_notes");
            entity.Property(e => e.CntNotice)
                .HasDefaultValue(0)
                .HasColumnName("cnt_notice");
            entity.Property(e => e.CntSeizure)
                .HasDefaultValue(0)
                .HasColumnName("cnt_seizure");
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
            entity.Property(e => e.Isleader)
                .HasDefaultValue(false)
                .HasColumnName("isleader");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDtm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_dtm");
            entity.Property(e => e.PatrolId)
                .HasDefaultValueSql("nextval('trn_patrol_det_parol_id_seq'::regclass)")
                .HasColumnName("patrol_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
