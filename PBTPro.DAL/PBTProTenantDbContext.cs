using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PBTPro.DAL.Services;
using PBTPro.DAL.Models;

namespace PBTPro.DAL;

public partial class PBTProTenantDbContext : DbContext
{
    public virtual DbSet<mst_ahlidun> mst_ahliduns { get; set; }

    public virtual DbSet<mst_ahlimajli> mst_ahlimajlis { get; set; }

    public virtual DbSet<mst_ahliparl> mst_ahliparls { get; set; }

    public virtual DbSet<mst_area> mst_areas { get; set; }

    public virtual DbSet<mst_device_bind> mst_device_binds { get; set; }

    public virtual DbSet<mst_dun> mst_duns { get; set; }

    public virtual DbSet<mst_license_premis_tax> mst_license_premis_taxes { get; set; }

    public virtual DbSet<mst_licensee> mst_licensees { get; set; }

    public virtual DbSet<mst_lot> mst_lots { get; set; }

    public virtual DbSet<mst_owner_licensee> mst_owner_licensees { get; set; }

    public virtual DbSet<mst_owner_premi> mst_owner_premis { get; set; }

    public virtual DbSet<mst_parliament> mst_parliaments { get; set; }

    public virtual DbSet<mst_patrol_schedule> mst_patrol_schedules { get; set; }

    public virtual DbSet<mst_pic_licensee> mst_pic_licensees { get; set; }

    public virtual DbSet<mst_premis> mst_premis { get; set; }

    public virtual DbSet<mst_taxholder> mst_taxholders { get; set; }

    public virtual DbSet<mst_zon> mst_zons { get; set; }

    public virtual DbSet<ref_cfsc_inventory> ref_cfsc_inventories { get; set; }

    public virtual DbSet<ref_cfsc_invtype> ref_cfsc_invtypes { get; set; }

    public virtual DbSet<ref_cfsc_scenario> ref_cfsc_scenarios { get; set; }

    public virtual DbSet<ref_cfsc_type> ref_cfsc_types { get; set; }

    public virtual DbSet<ref_cmpd_type> ref_cmpd_types { get; set; }

    public virtual DbSet<ref_deliver> ref_delivers { get; set; }

    public virtual DbSet<ref_department> ref_departments { get; set; }

    public virtual DbSet<ref_division> ref_divisions { get; set; }

    public virtual DbSet<ref_doc> ref_docs { get; set; }

    public virtual DbSet<ref_license_cat> ref_license_cats { get; set; }

    public virtual DbSet<ref_license_op> ref_license_ops { get; set; }

    public virtual DbSet<ref_license_status> ref_license_statuses { get; set; }

    public virtual DbSet<ref_license_type> ref_license_types { get; set; }

    public virtual DbSet<ref_note_type> ref_note_types { get; set; }

    public virtual DbSet<ref_notice_duration> ref_notice_durations { get; set; }

    public virtual DbSet<ref_notice_type> ref_notice_types { get; set; }

    public virtual DbSet<ref_ntc_duration> ref_ntc_durations { get; set; }

    public virtual DbSet<ref_patrol_status> ref_patrol_statuses { get; set; }

    public virtual DbSet<ref_patrol_type> ref_patrol_types { get; set; }

    public virtual DbSet<ref_relationship> ref_relationships { get; set; }

    public virtual DbSet<ref_tax_cat> ref_tax_cats { get; set; }

    public virtual DbSet<ref_tax_status> ref_tax_statuses { get; set; }

    public virtual DbSet<ref_tax_type> ref_tax_types { get; set; }

    public virtual DbSet<ref_trn_status> ref_trn_statuses { get; set; }

    public virtual DbSet<ref_unit> ref_units { get; set; }

    public virtual DbSet<trn_cfsc> trn_cfscs { get; set; }

    public virtual DbSet<trn_cfsc_img> trn_cfsc_imgs { get; set; }

    public virtual DbSet<trn_cfsc_item> trn_cfsc_items { get; set; }

    public virtual DbSet<trn_cmpd> trn_cmpds { get; set; }

    public virtual DbSet<trn_cmpd_img> trn_cmpd_imgs { get; set; }

    public virtual DbSet<trn_inspect> trn_inspects { get; set; }

    public virtual DbSet<trn_inspect_img> trn_inspect_imgs { get; set; }

    public virtual DbSet<trn_notice> trn_notices { get; set; }

    public virtual DbSet<trn_notice_img> trn_notice_imgs { get; set; }

    public virtual DbSet<trn_patrol_officer> trn_patrol_officers { get; set; }

    public virtual DbSet<trn_premis_visit> trn_premis_visits { get; set; }

    public virtual DbSet<trn_witness> trn_witnesses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<mst_ahlidun>(entity =>
        {
            entity.HasKey(e => e.ahlidun_id).HasName("tn_ahlidun_pk");

            entity.ToTable("mst_ahlidun", "tenant", tb => tb.HasComment("This table stores information about Ahli Majlis under DUN."));

            entity.Property(e => e.ahlidun_id)
                .HasDefaultValueSql("nextval('tenant.tn_ahlidun_ahlidun_id_seq'::regclass)")
                .HasComment("Unique identifier for each Ahli Majlis record (Primary Key).");
            entity.Property(e => e.ahlidun_name)
                .HasMaxLength(100)
                .HasComment("Name of Ahli Majlis (e.g., En Abd Razak).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.dun_id).HasComment("DUN ID that this member represents (FK to tn_dun).");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");

            entity.HasOne(d => d.dun).WithMany(p => p.mst_ahliduns)
                .HasForeignKey(d => d.dun_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_tn_ahlidun_refer_to_dun");
        });

        modelBuilder.Entity<mst_ahlimajli>(entity =>
        {
            entity.HasKey(e => e.ahlimj_id).HasName("pbt_ahlimajlis_pk");

            entity.ToTable("mst_ahlimajlis", "tenant", tb => tb.HasComment("This table stores information about Ahli Majlis under Zon."));

            entity.Property(e => e.ahlimj_id)
                .HasDefaultValueSql("nextval('tenant.pbt_ahlimajlis_ahlimajlis_id_seq'::regclass)")
                .HasComment("Unique identifier for each Ahli Majlis record (Primary Key).");
            entity.Property(e => e.ahlimj_name)
                .HasMaxLength(100)
                .HasComment("Name of Ahli Majlis (e.g., En Abd Razak).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.zon_id).HasComment("ZON ID that this member represents (FK to pbt_zon).");

            entity.HasOne(d => d.zon).WithMany(p => p.mst_ahlimajlis)
                .HasForeignKey(d => d.zon_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_pbt_ahlimajlis_zon");
        });

        modelBuilder.Entity<mst_ahliparl>(entity =>
        {
            entity.HasKey(e => e.ahliparl_id).HasName("pbt_ahliparl_pk");

            entity.ToTable("mst_ahliparl", "tenant", tb => tb.HasComment("This table stores information about ahli parliament."));

            entity.Property(e => e.ahliparl_id)
                .HasDefaultValueSql("nextval('tenant.pbt_ahliparl_ahliparl_id_seq'::regclass)")
                .HasComment("Unique identifier for each ahli parliament record (Primary Key).");
            entity.Property(e => e.ahliparl_name)
                .HasMaxLength(40)
                .HasComment("Name of ahli parliament (e.g., Tn Tuan Ganabatirau a/l Veraman).");
            entity.Property(e => e.ahliparl_party)
                .HasMaxLength(20)
                .HasComment("Party name of ahli parliament (e.g., Bebas).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.parl_id).HasComment("ID for the parliament under this ahli (FK to tn_parliament).");
            entity.Property(e => e.term_end)
                .HasComment("End date of the term in office.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.term_start)
                .HasComment("Start date of the term in office.")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.parl).WithMany(p => p.mst_ahliparls)
                .HasForeignKey(d => d.parl_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_pbt_ahliparl_belong_to_parl");
        });

        modelBuilder.Entity<mst_area>(entity =>
        {
            entity.HasKey(e => e.gid).HasName("mst_area_pkey");

            entity.ToTable("mst_area", "tenant");

            entity.HasIndex(e => e.geom, "mst_area_geom_idx").HasMethod("gist");

            entity.Property(e => e.area).HasMaxLength(254);
            entity.Property(e => e.geom).HasColumnType("geometry(MultiPolygon,4326)");
        });

        modelBuilder.Entity<mst_device_bind>(entity =>
        {
            entity.HasKey(e => e.device_bind_id).HasName("device_binding_pkey");

            entity.ToTable("mst_device_bind", "tenant", tb => tb.HasComment("This table stores information about devices bound to the PBT Pro system. When a user logs into the application, it captures details such as the unique device identifier, device type, and the status of the binding, linking the login account to the device used for login"));

            entity.Property(e => e.device_bind_id)
                .ValueGeneratedNever()
                .HasComment("Unique identifier for each device binding record (Primary Key)");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created");
            entity.Property(e => e.creator_id).HasComment("User who created the record");
            entity.Property(e => e.device_id)
                .HasMaxLength(100)
                .HasComment("The unique identifier of the device (e.g., Android ID, IMEI, or MAC address)");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was updated");
            entity.Property(e => e.modifier_id).HasComment("User who updated the record");
        });

        modelBuilder.Entity<mst_dun>(entity =>
        {
            entity.HasKey(e => e.dun_id).HasName("tn_dun_pk");

            entity.ToTable("mst_dun", "tenant", tb => tb.HasComment("This table stores information related to the Dewan Undangan Negeri (DUN) in Malaysia, which refers to the highest legislative body at the state level. Each DUN functions as a sub-division within a parliamentary constituency and is responsible for law-making and overseeing the implementation of state government policies."));

            entity.HasIndex(e => e.dun_code, "tn_dun_dun_code_key").IsUnique();

            entity.Property(e => e.dun_id)
                .HasDefaultValueSql("nextval('tenant.tn_dun_dun_id_seq'::regclass)")
                .HasComment("Unique identifier for each DUN record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.dun_code)
                .HasMaxLength(10)
                .HasComment("Code for the DUN (e.g., N45).");
            entity.Property(e => e.dun_name)
                .HasMaxLength(40)
                .HasComment("Name of the DUN (e.g., Dun Batu Tiga).");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.parl_id).HasComment("Parliament ID that this DUN falls under (FK to tn_parliament).");

            entity.HasOne(d => d.parl).WithMany(p => p.mst_duns)
                .HasForeignKey(d => d.parl_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_tn_dun_belong_to_parl");
        });

        modelBuilder.Entity<mst_license_premis_tax>(entity =>
        {
            entity.HasKey(e => e.license_premis_tax_id).HasName("mst_license_premis_tax_pkey");

            entity.ToTable("mst_license_premis_tax", "tenant");

            entity.Property(e => e.license_premis_tax_id).HasDefaultValueSql("nextval('tenant.mst_license_premis_id_seq'::regclass)");
            entity.Property(e => e.codeid_premis).HasColumnType("character varying");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.floor_building).HasColumnType("character varying");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.license_accno).HasColumnType("character varying");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.tax_accno).HasColumnType("character varying");

            entity.HasOne(d => d.codeid_premisNavigation).WithMany(p => p.mst_license_premis_taxes)
                .HasPrincipalKey(p => p.codeid_premis)
                .HasForeignKey(d => d.codeid_premis)
                .HasConstraintName("codeid_premis");

            entity.HasOne(d => d.license).WithMany(p => p.mst_license_premis_taxlicenses)
                .HasForeignKey(d => d.license_id)
                .HasConstraintName("fk_license_id");

            entity.HasOne(d => d.licensee).WithMany(p => p.mst_license_premis_taxlicensees)
                .HasForeignKey(d => d.licensee_id)
                .HasConstraintName("fk_licensee_id");

            entity.HasOne(d => d.status_lesen).WithMany(p => p.mst_license_premis_taxes)
                .HasForeignKey(d => d.status_lesen_id)
                .HasConstraintName("status_lesen_id");

            entity.HasOne(d => d.status_tax).WithMany(p => p.mst_license_premis_taxes)
                .HasForeignKey(d => d.status_tax_id)
                .HasConstraintName("status_tax_id");

            entity.HasOne(d => d.taxholder).WithMany(p => p.mst_license_premis_taxes)
                .HasForeignKey(d => d.taxholder_id)
                .HasConstraintName("fk_taxholder_id");
        });

        modelBuilder.Entity<mst_licensee>(entity =>
        {
            entity.HasKey(e => e.licensee_id).HasName("tn_licensee_pkey");

            entity.ToTable("mst_licensees", "tenant", tb => tb.HasComment("This table stores information about individuals or entities that hold licenses, including details such as the license holder identity, the business they operate, and relevant contact information."));

            entity.Property(e => e.licensee_id).HasComment("Unique identifier for each license holder (Primary Key).");
            entity.Property(e => e.business_addr)
                .HasMaxLength(255)
                .HasComment("Physical address of the business operated by the license holder.");
            entity.Property(e => e.business_name)
                .HasMaxLength(100)
                .HasComment("Name of the business operated by the license holder.");
            entity.Property(e => e.cat_id).HasComment("Code representing the type of license issued.");
            entity.Property(e => e.codeid_premis).HasColumnType("character varying");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.district_code)
                .HasMaxLength(10)
                .HasComment("Code representing the district where the business is located.");
            entity.Property(e => e.doc_support).HasColumnType("character varying");
            entity.Property(e => e.end_date).HasComment("End date of the current licensing period.");
            entity.Property(e => e.g_activity_1).HasColumnType("character varying");
            entity.Property(e => e.g_activity_2).HasColumnType("character varying");
            entity.Property(e => e.g_activity_3).HasColumnType("character varying");
            entity.Property(e => e.g_signbboard_1).HasColumnType("character varying");
            entity.Property(e => e.g_signbboard_2).HasColumnType("character varying");
            entity.Property(e => e.g_signbboard_3).HasColumnType("character varying");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.license_accno)
                .HasMaxLength(40)
                .HasComment("Unique account number assigned to the license holder.");
            entity.Property(e => e.license_duration)
                .HasDefaultValueSql("1")
                .HasComment("Duration of the license validity in years or intervals.")
                .HasColumnType("character varying");
            entity.Property(e => e.license_type).HasColumnType("character varying");
            entity.Property(e => e.lot).HasColumnType("character varying");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.owner_icno)
                .HasMaxLength(40)
                .HasComment("Identification card number of the owner (foreign key).");
            entity.Property(e => e.ssm_no).HasColumnType("character varying");
            entity.Property(e => e.start_date).HasComment("Start date of the current licensing period.");
            entity.Property(e => e.state_code)
                .HasMaxLength(10)
                .HasComment("Code representing the state where the business is located.");
            entity.Property(e => e.status_id)
                .HasDefaultValue(1)
                .HasComment("Current status of the license (FK to status reference).");

            entity.HasOne(d => d.cat).WithMany(p => p.mst_licensees)
                .HasForeignKey(d => d.cat_id)
                .HasConstraintName("fk_cat_id");

            entity.HasOne(d => d.codeid_premisNavigation).WithMany(p => p.mst_licensees)
                .HasPrincipalKey(p => p.codeid_premis)
                .HasForeignKey(d => d.codeid_premis)
                .HasConstraintName("fk_codeid_premis");

            entity.HasOne(d => d.status).WithMany(p => p.mst_licensees)
                .HasForeignKey(d => d.status_id)
                .HasConstraintName("fk_license_status_id");
        });

        modelBuilder.Entity<mst_lot>(entity =>
        {
            entity.HasKey(e => e.gid).HasName("mst_lot_pkey");

            entity.ToTable("mst_lot", "tenant");

            entity.HasIndex(e => e.geom, "mst_lot_geom_idx").HasMethod("gist");

            entity.Property(e => e.apdate).HasMaxLength(8);
            entity.Property(e => e.cls).HasMaxLength(1);
            entity.Property(e => e.daerah).HasMaxLength(2);
            entity.Property(e => e.entrymode).HasMaxLength(1);
            entity.Property(e => e.geom).HasColumnType("geometry(MultiPolygon,4326)");
            entity.Property(e => e.guid).HasMaxLength(32);
            entity.Property(e => e.landtitlec).HasMaxLength(2);
            entity.Property(e => e.landusecod).HasMaxLength(2);
            entity.Property(e => e.lot).HasMaxLength(7);
            entity.Property(e => e.mukim).HasMaxLength(2);
            entity.Property(e => e.negeri).HasMaxLength(2);
            entity.Property(e => e.pa).HasMaxLength(15);
            entity.Property(e => e.refplan).HasMaxLength(15);
            entity.Property(e => e.seksyen).HasMaxLength(3);
            entity.Property(e => e.unit).HasMaxLength(1);
            entity.Property(e => e.upi).HasMaxLength(16);
        });

        modelBuilder.Entity<mst_owner_licensee>(entity =>
        {
            entity.HasKey(e => e.owner_id).HasName("mst_owner_licensees_pkey");

            entity.ToTable("mst_owner_licensees", "tenant", tb => tb.HasComment("This table stores information about license owners"));

            entity.HasIndex(e => e.owner_icno, "unique_owner_licensees_icno").IsUnique();

            entity.Property(e => e.owner_id)
                .HasDefaultValueSql("nextval('tenant.tn_owner_licensee_owner_id_seq'::regclass)")
                .HasComment("Unique identifier for each owner.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.district_code)
                .HasMaxLength(20)
                .HasComment("District where the owner resides.");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasDefaultValue(0);
            entity.Property(e => e.owner_addr)
                .HasMaxLength(255)
                .HasComment("Address of the owner.");
            entity.Property(e => e.owner_email)
                .HasMaxLength(100)
                .HasComment("Email address.");
            entity.Property(e => e.owner_icno)
                .HasMaxLength(20)
                .HasComment("IC number (must be unique).");
            entity.Property(e => e.owner_name)
                .HasMaxLength(100)
                .HasComment("Owner's name.");
            entity.Property(e => e.owner_telno)
                .HasMaxLength(40)
                .HasComment("Phone number of the owner.");
            entity.Property(e => e.state_code)
                .HasMaxLength(10)
                .HasComment("State where the owner resides.");
        });

        modelBuilder.Entity<mst_owner_premi>(entity =>
        {
            entity.HasKey(e => e.owner_id).HasName("mst_owner_premis_pkey");

            entity.ToTable("mst_owner_premis", "tenant");

            entity.HasIndex(e => e.owner_icno, "unique_owner_premis_icno").IsUnique();

            entity.Property(e => e.owner_id).HasComment("Unique identifier for each owner.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.district_code)
                .HasMaxLength(20)
                .HasComment("District where the owner resides.");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasDefaultValue(0);
            entity.Property(e => e.owner_addr)
                .HasMaxLength(255)
                .HasComment("Address of the owner.");
            entity.Property(e => e.owner_email)
                .HasMaxLength(100)
                .HasComment("Email address.");
            entity.Property(e => e.owner_icno)
                .HasMaxLength(20)
                .HasComment("IC number (must be unique).");
            entity.Property(e => e.owner_name)
                .HasMaxLength(100)
                .HasComment("Owner's name.");
            entity.Property(e => e.owner_telno)
                .HasMaxLength(40)
                .HasComment("Phone number of the owner.");
            entity.Property(e => e.state_code)
                .HasMaxLength(10)
                .HasComment("State where the owner resides.");
        });

        modelBuilder.Entity<mst_parliament>(entity =>
        {
            entity.HasKey(e => e.parl_id).HasName("tn_parliament_pk");

            entity.ToTable("mst_parliament", "tenant", tb => tb.HasComment("This table stores information about parliaments, including their codes and names."));

            entity.HasIndex(e => e.parl_code, "tn_parliament_parl_code_key").IsUnique();

            entity.Property(e => e.parl_id)
                .HasDefaultValueSql("nextval('tenant.tn_parliament_parl_id_seq'::regclass)")
                .HasComment("Unique identifier for each parliament record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.parl_code)
                .HasMaxLength(10)
                .HasComment("Code assigned to each parliament (e.g., P110).");
            entity.Property(e => e.parl_name)
                .HasMaxLength(200)
                .HasComment("Name of the parliament (e.g., Parliament Klang).");
        });

        modelBuilder.Entity<mst_patrol_schedule>(entity =>
        {
            entity.HasKey(e => e.schedule_id).HasName("mst_patrol_schedule_pkey");

            entity.ToTable("mst_patrol_schedule", "tenant");

            entity.HasIndex(e => e.dept_id, "fki_dept_id_refers_to_dept_id");

            entity.Property(e => e.cnt_cmpd).HasDefaultValue(0);
            entity.Property(e => e.cnt_notes).HasDefaultValue(0);
            entity.Property(e => e.cnt_notice).HasDefaultValue(0);
            entity.Property(e => e.cnt_seizure).HasDefaultValue(0);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.district_code).HasMaxLength(10);
            entity.Property(e => e.end_location).HasColumnType("geometry(Point,4326)");
            entity.Property(e => e.end_time).HasColumnType("timestamp without time zone");
            entity.Property(e => e.idno).HasMaxLength(50);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.is_scheduled).HasDefaultValue(true);
            entity.Property(e => e.loc_name).HasMaxLength(255);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasDefaultValue(0);
            entity.Property(e => e.start_location).HasColumnType("geometry(Point,4326)");
            entity.Property(e => e.start_time).HasColumnType("timestamp without time zone");
            entity.Property(e => e.town_code).HasMaxLength(10);

            entity.HasOne(d => d.dept).WithMany(p => p.mst_patrol_schedules)
                .HasForeignKey(d => d.dept_id)
                .HasConstraintName("dept_id_refers_to_dept_id");

            entity.HasOne(d => d.status).WithMany(p => p.mst_patrol_schedules)
                .HasForeignKey(d => d.status_id)
                .HasConstraintName("status_id_refers_to_patrol_status_id");

            entity.HasOne(d => d.type).WithMany(p => p.mst_patrol_schedules)
                .HasForeignKey(d => d.type_id)
                .HasConstraintName("type_id_refers_to_patrol_type_id");
        });

        modelBuilder.Entity<mst_pic_licensee>(entity =>
        {
            entity.HasKey(e => e.pic_id).HasName("pk_pic_id");

            entity.ToTable("mst_pic_licensees", "tenant", tb => tb.HasComment("This table stores about person in charge on the premise, based on license"));

            entity.HasIndex(e => e.pic_icno, "unique_pic_icno").IsUnique();

            entity.Property(e => e.codeid_premis).HasColumnType("character varying");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.license_accno).HasColumnType("character varying");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasDefaultValue(0);
            entity.Property(e => e.pic_addr).HasMaxLength(255);
            entity.Property(e => e.pic_icno).HasMaxLength(14);
            entity.Property(e => e.pic_name).HasMaxLength(50);
            entity.Property(e => e.pic_telno).HasMaxLength(14);

            entity.HasOne(d => d.codeid_premisNavigation).WithMany(p => p.mst_pic_licensees)
                .HasPrincipalKey(p => p.codeid_premis)
                .HasForeignKey(d => d.codeid_premis)
                .HasConstraintName("fk_codeid_premis");

            entity.HasOne(d => d.licensee).WithMany(p => p.mst_pic_licensees)
                .HasForeignKey(d => d.licensee_id)
                .HasConstraintName("fk_license_id");

            entity.HasOne(d => d.relation).WithMany(p => p.mst_pic_licensees)
                .HasForeignKey(d => d.relation_id)
                .HasConstraintName("fk_relation_id");
        });

        modelBuilder.Entity<mst_premis>(entity =>
        {
            entity.HasKey(e => e.id).HasName("mst_premis_pkey");

            entity.ToTable("mst_premis", "tenant");

            entity.HasIndex(e => e.codeid_premis, "unique_codeid_premis").IsUnique();

            entity.Property(e => e.codeid_premis).HasMaxLength(100);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.geom).HasColumnType("geometry(PointZ,4326)");
            entity.Property(e => e.gkeseluruh).HasMaxLength(200);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.lot).HasColumnType("character varying");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<mst_taxholder>(entity =>
        {
            entity.HasKey(e => e.taxholder_id).HasName("mst_taxholder_pkey");

            entity.ToTable("mst_taxholders", "tenant", tb => tb.HasComment("Table to store information about tax holders, including their business details and associated statuses."));

            entity.Property(e => e.taxholder_id).HasComment("Unique identifier for each tax holder.");
            entity.Property(e => e.alamat).HasColumnType("character varying");
            entity.Property(e => e.codeid_premis).HasColumnType("character varying");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("ID of the user who created this record.");
            entity.Property(e => e.district_code)
                .HasMaxLength(10)
                .HasComment("Code representing the district where the business is located.");
            entity.Property(e => e.dun_id).HasComment("Identifier for state assembly representation related to taxation.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("ID of the user who last modified this record.");
            entity.Property(e => e.owner_icno)
                .HasMaxLength(40)
                .HasComment("Identification number of the owner, linked to the owners table.");
            entity.Property(e => e.parliment_id).HasComment("Identifier for parliamentary representation related to taxation.");
            entity.Property(e => e.state_code)
                .HasMaxLength(10)
                .HasComment("Code representing the state where the business is located.");
            entity.Property(e => e.status_id).HasComment("Status of the tax holder, linked to a reference status table.");
            entity.Property(e => e.tax_accno)
                .HasMaxLength(40)
                .HasComment("Tax account number associated with the tax holder.");
            //entity.Property(e => e.tax_duration)
            //    .HasDefaultValueSql("'1 year'::interval")
            //    .HasComment("Duration of the tax obligation, defaulting to 1 year.");
            entity.Property(e => e.tax_end_date).HasComment("Date when the tax obligation ends.");
            entity.Property(e => e.tax_start_date).HasComment("Date when the tax obligation starts.");
            entity.Property(e => e.zon_id).HasComment("Identifier for zoning related to taxation.");

            entity.HasOne(d => d.dun).WithMany(p => p.mst_taxholders)
                .HasForeignKey(d => d.dun_id)
                .HasConstraintName("fk_dun_id");

            entity.HasOne(d => d.parliment).WithMany(p => p.mst_taxholders)
                .HasForeignKey(d => d.parliment_id)
                .HasConstraintName("fk_parliment_id");

            entity.HasOne(d => d.status).WithMany(p => p.mst_taxholders)
                .HasForeignKey(d => d.status_id)
                .HasConstraintName("fk_status_id");

            entity.HasOne(d => d.zon).WithMany(p => p.mst_taxholders)
                .HasForeignKey(d => d.zon_id)
                .HasConstraintName("fk_zon_id");
        });

        modelBuilder.Entity<mst_zon>(entity =>
        {
            entity.HasKey(e => e.zon_id).HasName("tn_zon_pk");

            entity.ToTable("mst_zon", "tenant", tb => tb.HasComment("This table stores information related to zone information under DUN. Example: Zon Teluk Gong di Klang."));

            entity.Property(e => e.zon_id)
                .HasDefaultValueSql("nextval('tenant.tn_zon_zon_id_seq'::regclass)")
                .HasComment("Unique identifier for each zone record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.dun_id).HasComment("DUN ID that this zone is associated with (FK to tn_dun).");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.zon_code)
                .HasMaxLength(40)
                .HasComment("Code for the zone (e.g., Zon N42B).");
            entity.Property(e => e.zon_list).HasComment("List of locations under the zone (e.g., Taman Kapar, Taman Aman, etc.).");

            entity.HasOne(d => d.dun).WithMany(p => p.mst_zons)
                .HasForeignKey(d => d.dun_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_tn_zon_belong_to_dun");
        });

        modelBuilder.Entity<ref_cfsc_inventory>(entity =>
        {
            entity.HasKey(e => e.inv_id).HasName("ref_cfsc_items_pkey");

            entity.ToTable("ref_cfsc_inventory", "tenant", tb => tb.HasComment("Table to store confiscated items along with their types."));

            entity.Property(e => e.inv_id)
                .HasDefaultValueSql("nextval('tenant.ref_cfsc_items_item_id_seq'::regclass)")
                .HasComment("Unique identifier for each confiscated item.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record.");
            entity.Property(e => e.inv_name)
                .HasMaxLength(100)
                .HasComment("Name of the confiscated item.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.item_type).HasComment("Type of the confiscated item (Mudah Disita or Tidak Mudah Disita).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who modified this record.");
        });

        modelBuilder.Entity<ref_cfsc_invtype>(entity =>
        {
            entity.HasKey(e => e.inv_type_id).HasName("ref_confiscation_item_types_pkey");

            entity.ToTable("ref_cfsc_invtypes", "tenant", tb => tb.HasComment("Table to store types of confiscated items, such as easy to confiscate and hard to confiscate."));

            entity.Property(e => e.inv_type_id)
                .HasDefaultValueSql("nextval('tenant.ref_confiscation_item_types_item_type_id_seq'::regclass)")
                .HasComment("Unique identifier for each type of confiscated item.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record (default is 0).");
            entity.Property(e => e.inv_type_desc)
                .HasMaxLength(100)
                .HasComment("Description of the type of confiscated item.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record (default is 0).");
        });

        modelBuilder.Entity<ref_cfsc_scenario>(entity =>
        {
            entity.HasKey(e => e.scen_id).HasName("ref_cfsc_scenarios_pkey");

            entity.ToTable("ref_cfsc_scenarios", "tenant", tb => tb.HasComment("Table to store different scenarios related to confiscation cases."));

            entity.Property(e => e.scen_id).HasComment("Unique identifier for each confiscation scenario.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record.");
            entity.Property(e => e.scen_desc)
                .HasMaxLength(255)
                .HasComment("Description of the confiscation scenario.");
            entity.Property(e => e.scen_name).HasMaxLength(50);
        });

        modelBuilder.Entity<ref_cfsc_type>(entity =>
        {
            entity.HasKey(e => e.cfsc_type_id).HasName("cfsc_type_pk");

            entity.ToTable("ref_cfsc_types", "tenant", tb => tb.HasComment("Table to store different types of confiscations, including their codes, names, and descriptions."));

            entity.Property(e => e.cfsc_type_id).HasComment("Unique identifier for each confiscation type.");
            entity.Property(e => e.cfsc_type_code)
                .HasMaxLength(10)
                .HasComment("Code representing the confiscation type, must be unique.");
            entity.Property(e => e.cfsc_type_desc)
                .HasMaxLength(255)
                .HasComment("Description of the confiscation type.");
            entity.Property(e => e.cfsc_type_name)
                .HasMaxLength(40)
                .HasComment("Name of the confiscation type.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("ID of the user who created this record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("ID of the user who last modified this record.");
        });

        modelBuilder.Entity<ref_cmpd_type>(entity =>
        {
            entity.HasKey(e => e.cmpd_type_id).HasName("cmpd_type_pk");

            entity.ToTable("ref_cmpd_types", "tenant", tb => tb.HasComment("This table stores types of compounds available under PBT (e.g., Kompaun Atas Premis, Kompaun Lesen)."));

            entity.HasIndex(e => e.cmpd_type_code, "cmpd_type_cmpd_type_code_key").IsUnique();

            entity.Property(e => e.cmpd_type_id)
                .HasDefaultValueSql("nextval('tenant.cmpd_type_cmpd_type_id_seq'::regclass)")
                .HasComment("Unique identifier for each type of compound record (Primary Key).");
            entity.Property(e => e.cmpd_type_code)
                .HasMaxLength(10)
                .HasComment("Code for the compound type (e.g., K12 - Kompaun Lesen).");
            entity.Property(e => e.cmpd_type_desc)
                .HasMaxLength(255)
                .HasComment("Description about the compound type.");
            entity.Property(e => e.cmpd_type_name)
                .HasMaxLength(40)
                .HasComment("Name of the compound type (e.g., Kompaun Lesen).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
        });

        modelBuilder.Entity<ref_deliver>(entity =>
        {
            entity.HasKey(e => e.deliver_id).HasName("deliver_id");

            entity.ToTable("ref_deliver", "tenant");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.deliver_name).HasMaxLength(40);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasDefaultValue(0);
        });

        modelBuilder.Entity<ref_department>(entity =>
        {
            entity.HasKey(e => e.dept_id).HasName("ref_department_pkey");

            entity.ToTable("ref_department", "tenant", tb => tb.HasComment("This table stores information about departments under PBT (e.g., Jabatan Pelesenan)."));

            entity.HasIndex(e => e.dept_code, "ref_department_dept_code_key").IsUnique();

            entity.Property(e => e.dept_id).HasComment("Unique identifier for each department record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.dept_code)
                .HasMaxLength(10)
                .HasComment("Code of the department (e.g., PL).");
            entity.Property(e => e.dept_desc).HasComment("Description about the department (e.g., Roles, Job Description, etc.).");
            entity.Property(e => e.dept_email).HasMaxLength(100);
            entity.Property(e => e.dept_name)
                .HasMaxLength(40)
                .HasComment("Name of the department (e.g., Jabatan Pelesenan).");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
        });

        modelBuilder.Entity<ref_division>(entity =>
        {
            entity.HasKey(e => e.div_id).HasName("ref_division_pkey");

            entity.ToTable("ref_division", "tenant", tb => tb.HasComment("This table stores information about divisions under departments in PBT (e.g., Bahagian TRED dan Perniagaan dan Industri)."));

            entity.HasIndex(e => new { e.div_code, e.dept_name }, "ref_division_div_code_key").IsUnique();

            entity.Property(e => e.div_id).HasComment("Unique identifier for each division record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.dept_name).HasMaxLength(100);
            entity.Property(e => e.div_code)
                .HasMaxLength(10)
                .HasComment("Code of the division (e.g., PL-TR).");
            entity.Property(e => e.div_desc).HasComment("Description about the division (e.g., Roles, Job Description, etc.).");
            entity.Property(e => e.div_name)
                .HasMaxLength(40)
                .HasComment("Name of division (e.g., Bahagian TRED dan Perniagaan dan Industri).");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");

            entity.HasOne(d => d.dept).WithMany(p => p.ref_divisions)
                .HasForeignKey(d => d.dept_id)
                .HasConstraintName("fk_div_id_belongs_to_dept_id");
        });

        modelBuilder.Entity<ref_doc>(entity =>
        {
            entity.HasKey(e => e.doc_id).HasName("ref_doc_pkey");

            entity.ToTable("ref_doc", "tenant");

            entity.Property(e => e.cnt_download).HasDefaultValue(0);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.description).HasColumnType("character varying");
            entity.Property(e => e.doc_cat).HasColumnType("character varying");
            entity.Property(e => e.filename).HasColumnType("character varying");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.pathurl).HasColumnType("character varying");
            entity.Property(e => e.title).HasColumnType("character varying");
        });

        modelBuilder.Entity<ref_license_cat>(entity =>
        {
            entity.HasKey(e => e.cat_id).HasName("pk_license_cat_id");

            entity.ToTable("ref_license_cat", "tenant", tb => tb.HasComment("This table stores types of licenses."));

            entity.Property(e => e.cat_id)
                .HasDefaultValueSql("nextval('tenant.ref_license_type_license_type_id_seq'::regclass)")
                .HasComment("Unique identifier for each license type.");
            entity.Property(e => e.cat_code)
                .HasMaxLength(100)
                .HasComment("Code representing the license type.");
            entity.Property(e => e.cat_name)
                .HasMaxLength(100)
                .HasComment("Name of the license type.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the record was created.");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Indicates if the record is marked as deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the record was last modified.");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified the record.");
            entity.Property(e => e.type_id).HasComment("Code representing the category of the license.");

            entity.HasOne(d => d.type).WithMany(p => p.ref_license_cats)
                .HasForeignKey(d => d.type_id)
                .HasConstraintName("type_id_refers_to_type_id");
        });

        modelBuilder.Entity<ref_license_op>(entity =>
        {
            entity.HasKey(e => e.ops_id).HasName("pk_license_ops_id");

            entity.ToTable("ref_license_ops", "tenant", tb => tb.HasComment("This table stores operations related to licenses."));

            entity.Property(e => e.ops_id)
                .HasDefaultValueSql("nextval('tenant.ref_license_ops_license_ops_id_seq'::regclass)")
                .HasComment("Unique identifier for each license operation.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Indicates if the record is marked as deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified the record.");
            entity.Property(e => e.ops_code)
                .HasMaxLength(100)
                .HasComment("Code representing the license operation.");
            entity.Property(e => e.ops_name)
                .HasMaxLength(300)
                .HasComment("Name of the license operation.");
            entity.Property(e => e.type_id).HasComment("Code representing the category of the license.");

            entity.HasOne(d => d.type).WithMany(p => p.ref_license_ops)
                .HasForeignKey(d => d.type_id)
                .HasConstraintName("type_id_refers_to_type_id");
        });

        modelBuilder.Entity<ref_license_status>(entity =>
        {
            entity.HasKey(e => e.status_id).HasName("license_status_pk");

            entity.ToTable("ref_license_status", "tenant", tb => tb.HasComment("This table stores the different statuses for each license (e.g., Aktif, Tidak Aktif, Batal)."));

            entity.Property(e => e.status_id)
                .HasDefaultValueSql("nextval('tenant.license_status_license_status_id_seq'::regclass)")
                .HasComment("Unique identifier for each license status record (Primary Key).");
            entity.Property(e => e.color)
                .HasMaxLength(40)
                .HasDefaultValueSql("'black'::character varying");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating if the status record is active or inactive.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.priority).HasDefaultValue(1);
            entity.Property(e => e.status_name)
                .HasMaxLength(40)
                .HasComment("Name of the license status (e.g., Aktif, Tidak Aktif, Batal).");
        });

        modelBuilder.Entity<ref_license_type>(entity =>
        {
            entity.HasKey(e => e.type_id).HasName("license_type_id");

            entity.ToTable("ref_license_types", "tenant", tb => tb.HasComment("This table stores categories of licenses."));

            entity.Property(e => e.type_id)
                .HasDefaultValueSql("nextval('tenant.ref_license_cat_license_cat_id_seq'::regclass)")
                .HasComment("Unique identifier for each license category.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the record was created.");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Indicates if the record is marked as deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the record was last modified.");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified the record.");
            entity.Property(e => e.type_code)
                .HasMaxLength(100)
                .HasComment("Code representing the license category.");
            entity.Property(e => e.type_name)
                .HasMaxLength(255)
                .HasComment("Name of the license category.");
        });

        modelBuilder.Entity<ref_note_type>(entity =>
        {
            entity.HasKey(e => e.note_type_id).HasName("note_type_pk");

            entity.ToTable("ref_note_types", "tenant", tb => tb.HasComment("This table stores types of inspection notes available under PBT (e.g., Nota Pemeriksaan Lesen, Nota Pemeriksaan Individual)."));

            entity.HasIndex(e => e.note_type_code, "note_type_note_type_code_key").IsUnique();

            entity.Property(e => e.note_type_id)
                .HasDefaultValueSql("nextval('tenant.note_type_note_type_id_seq'::regclass)")
                .HasComment("Unique identifier for each type of inspection note record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the inspection note is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.note_type_code)
                .HasMaxLength(10)
                .HasComment("Code for the note type (e.g., NP12 - Nota Pemeriksaan Lesen).");
            entity.Property(e => e.note_type_desc)
                .HasMaxLength(255)
                .HasComment("Description about the note type.");
            entity.Property(e => e.note_type_name)
                .HasMaxLength(40)
                .HasComment("Name of the inspection note type (e.g., Nota Pemeriksaan Lesen).");
        });

        modelBuilder.Entity<ref_notice_duration>(entity =>
        {
            entity.HasKey(e => e.duration_id).HasName("ref_notice_durations_pkey");

            entity.ToTable("ref_notice_durations", "tenant", tb => tb.HasComment("This table stores duration values for notices (e.g., Serta Merta, 3 hari, 5 hari)."));

            entity.Property(e => e.duration_id).HasComment("Unique identifier for each notice duration record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("User who created the record.");
            entity.Property(e => e.duration_value)
                .HasMaxLength(20)
                .HasComment("Value of the notice duration (e.g., Serta Merta, 3 hari, 5 hari).");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("User who last updated the record.");
        });

        modelBuilder.Entity<ref_notice_type>(entity =>
        {
            entity.HasKey(e => e.notice_type_id).HasName("notice_type_pk");

            entity.ToTable("ref_notice_types", "tenant", tb => tb.HasComment("This table stores types of notices available under PBT (e.g., Notis Atas Premis, Notis Lesen)."));

            entity.HasIndex(e => e.notice_type_code, "notice_type_notice_type_code_key").IsUnique();

            entity.Property(e => e.notice_type_id)
                .HasDefaultValueSql("nextval('tenant.notice_type_notice_type_id_seq'::regclass)")
                .HasComment("Unique identifier for each type of notice record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.notice_type_code)
                .HasMaxLength(10)
                .HasComment("Code for the notice type (e.g., N12 - Notis Lesen).");
            entity.Property(e => e.notice_type_desc)
                .HasMaxLength(255)
                .HasComment("Description about the notice type.");
            entity.Property(e => e.notice_type_name)
                .HasMaxLength(40)
                .HasComment("Name of the notice type (e.g., Notis Lesen).");
        });

        modelBuilder.Entity<ref_ntc_duration>(entity =>
        {
            entity.HasKey(e => e.duration_id).HasName("notice_dur_pk");

            entity.ToTable("ref_ntc_duration", "tenant", tb => tb.HasComment("This table stores duration values for notices (e.g., Serta Merta, 3 hari, 5 hari)."));

            entity.Property(e => e.duration_id)
                .HasDefaultValueSql("nextval('tenant.notice_dur_duration_id_seq'::regclass)")
                .HasComment("Unique identifier for each notice duration record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.duration_value)
                .HasMaxLength(20)
                .HasComment("Value of the notice duration (e.g., Serta Merta, 3 hari, 5 hari).");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
        });

        modelBuilder.Entity<ref_patrol_status>(entity =>
        {
            entity.HasKey(e => e.status_id).HasName("ref_patrol_status_pkey");

            entity.ToTable("ref_patrol_status", "tenant", tb => tb.HasComment("This table stores the status of patrols (e.g., Belum Mula, Dalam Rondaan, Selesai)."));

            entity.Property(e => e.status_id)
                .ValueGeneratedNever()
                .HasComment("Unique identifier for each patrol status.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Record creation timestamp.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Indicates if the record is marked as deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Last modified timestamp.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified the record.");
            entity.Property(e => e.status_code)
                .HasMaxLength(10)
                .HasComment("Code representing the patrol status.");
            entity.Property(e => e.status_desc).HasComment("Description of the patrol status.");
            entity.Property(e => e.status_name)
                .HasMaxLength(50)
                .HasComment("Name of the patrol status.");
        });

        modelBuilder.Entity<ref_patrol_type>(entity =>
        {
            entity.HasKey(e => e.type_id).HasName("pk_patrol_type");

            entity.ToTable("ref_patrol_types", "tenant");

            entity.HasIndex(e => e.type_code, "ref_patrol_type_patrol_type_code_key").IsUnique();

            entity.Property(e => e.type_id).ValueGeneratedNever();
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasDefaultValue(0);
            entity.Property(e => e.type_code).HasMaxLength(10);
            entity.Property(e => e.type_name).HasMaxLength(50);
        });

        modelBuilder.Entity<ref_relationship>(entity =>
        {
            entity.HasKey(e => e.relation_id).HasName("ref_relationships_pkey");

            entity.ToTable("ref_relationships", "tenant", tb => tb.HasComment("This table stores relationships between the recipient to the premise owner e.g., Pekerja, Ahli Keluarga, Tiada kaitan"));

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.relation_name).HasMaxLength(20);
        });

        modelBuilder.Entity<ref_tax_cat>(entity =>
        {
            entity.HasKey(e => e.cat_id).HasName("tax_cat_pk");

            entity.ToTable("ref_tax_cat", "tenant", tb => tb.HasComment("This table stores categories under the tax types available under PBT (e.g., Cukai Taksiran consists of Kediaman, Industri, Pertanian)."));

            entity.HasIndex(e => e.cat_code, "tax_cat_tax_cat_code_key").IsUnique();

            entity.Property(e => e.cat_id).HasComment("Unique identifier for each category of tax record (Primary Key).");
            entity.Property(e => e.cat_code)
                .HasMaxLength(10)
                .HasComment("Code for the tax category (e.g., A1).");
            entity.Property(e => e.cat_name)
                .HasMaxLength(40)
                .HasComment("Name of the tax category (e.g., Kediaman).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.type_id).HasComment("Type of tax that this category belongs to (FK to tax_type).");

            entity.HasOne(d => d.type).WithMany(p => p.ref_tax_cats)
                .HasForeignKey(d => d.type_id)
                .HasConstraintName("tax_cat_id_belongs_to_tax_type");
        });

        modelBuilder.Entity<ref_tax_status>(entity =>
        {
            entity.HasKey(e => e.status_id).HasName("tax_status_pk");

            entity.ToTable("ref_tax_status", "tenant", tb => tb.HasComment("This table stores the different statuses for each tax (status cukai) (e.g., Cukai Dibayar, Cukai Tertunggak)."));

            entity.Property(e => e.status_id)
                .HasDefaultValueSql("nextval('tenant.tax_status_tax_status_id_seq'::regclass)")
                .HasComment("Unique identifier for each tax status record (Primary Key).");
            entity.Property(e => e.color)
                .HasMaxLength(40)
                .HasDefaultValueSql("'black'::character varying");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating if the status record is active or inactive.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.priority).HasDefaultValue(1);
            entity.Property(e => e.status_name)
                .HasMaxLength(100)
                .HasComment("Name of the tax status (e.g., Cukai Dibayar, Cukai Tertunggak).");
        });

        modelBuilder.Entity<ref_tax_type>(entity =>
        {
            entity.HasKey(e => e.type_id).HasName("tax_type_pk");

            entity.ToTable("ref_tax_types", "tenant", tb => tb.HasComment("This table stores types of tax (cukai) that are available under PBT (e.g., Cukai Taksiran)."));

            entity.HasIndex(e => e.type_code, "type_code_unique").IsUnique();

            entity.Property(e => e.type_id)
                .HasDefaultValueSql("nextval('tenant.ref_tax_type_type_id_seq'::regclass)")
                .HasComment("Unique identifier for each type of tax record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.type_code).HasMaxLength(40);
            entity.Property(e => e.type_name)
                .HasMaxLength(40)
                .HasComment("Name of tax type (e.g., Cukai Taksiran).");
        });

        modelBuilder.Entity<ref_trn_status>(entity =>
        {
            entity.HasKey(e => e.status_id).HasName("transaction_status_pkey");

            entity.ToTable("ref_trn_status", "tenant", tb => tb.HasComment("This table stores reference information about transaction statuses (e.g., Kompaun, Notis)."));

            entity.Property(e => e.status_id).HasComment("Unique identifier for each transaction status record (Primary Key).");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User ID of the individual who created this record.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating if the status record is active (False) or deleted (True).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when this record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User ID of the individual who last updated this record.");
            entity.Property(e => e.status_name)
                .HasMaxLength(20)
                .HasComment("Name of the transaction status (e.g., Baru, Dalam Tindakan, Tutup).");
        });

        modelBuilder.Entity<ref_unit>(entity =>
        {
            entity.HasKey(e => e.unit_id).HasName("ref_unit_pkey");

            entity.ToTable("ref_unit", "tenant", tb => tb.HasComment("This table stores information about unit under departments in PBT (e.g., Bahagian TRED dan Perniagaan dan Industri)."));

            entity.HasIndex(e => new { e.unit_code, e.dept_name, e.div_name }, "ref_unit_unit_code_key").IsUnique();

            entity.Property(e => e.unit_id).HasComment("Unique identifier for each unit under division");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User who created the record.");
            entity.Property(e => e.dept_name).HasMaxLength(100);
            entity.Property(e => e.div_name).HasMaxLength(100);
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Logical delete flag indicating if the record is active or deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last updated.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User who last updated the record.");
            entity.Property(e => e.unit_code)
                .HasMaxLength(10)
                .HasComment("Code of the unit (e.g., PL-TR).");
            entity.Property(e => e.unit_desc).HasComment("Description about the unit (e.g., Roles, Job Description, etc.).");
            entity.Property(e => e.unit_name)
                .HasMaxLength(40)
                .HasComment("Name of unit (e.g., Unit Kaunter).");

            entity.HasOne(d => d.dept).WithMany(p => p.ref_units)
                .HasForeignKey(d => d.dept_id)
                .HasConstraintName("fk_div_id_belongs_to_dept_id");

            entity.HasOne(d => d.div).WithMany(p => p.ref_units)
                .HasForeignKey(d => d.div_id)
                .HasConstraintName("fk_unit_id_belongs_to_div_id");
        });

        modelBuilder.Entity<trn_cfsc>(entity =>
        {
            entity.HasKey(e => e.trn_cfsc_id).HasName("trn_cfscs_pkey");

            entity.ToTable("trn_cfscs", "tenant", tb => tb.HasComment("Table to store confiscation transactions, including details about the owner and items confiscated."));

            entity.Property(e => e.trn_cfsc_id).HasComment("Unique identifier for each confiscation transaction.");
            entity.Property(e => e.act_code).HasColumnType("character varying");
            entity.Property(e => e.cfsc_latitude)
                .HasPrecision(9, 6)
                .HasComment("Latitude of the location where the confiscation occurred.");
            entity.Property(e => e.cfsc_longitude)
                .HasPrecision(9, 6)
                .HasComment("Longitude of the location where the confiscation occurred.");
            entity.Property(e => e.cfsc_ref_no)
                .HasMaxLength(50)
                .HasComment("Reference number for tracking this specific confiscation.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record.");
            entity.Property(e => e.doc_name).HasColumnType("character varying");
            entity.Property(e => e.doc_pathurl).HasColumnType("character varying");
            entity.Property(e => e.instruction).HasComment("Instructions regarding actions to be taken related to this confiscation.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record.");
            entity.Property(e => e.offense_code).HasColumnType("character varying");
            entity.Property(e => e.offs_location).HasComment("Location where any offenses occurred during the confiscation.");
            entity.Property(e => e.owner_icno)
                .HasMaxLength(30)
                .HasComment("Identification number of the owner associated with the confiscated items.");
            entity.Property(e => e.recipient_addr).HasMaxLength(255);
            entity.Property(e => e.recipient_icno).HasMaxLength(20);
            entity.Property(e => e.recipient_name).HasMaxLength(50);
            entity.Property(e => e.recipient_sign).HasMaxLength(255);
            entity.Property(e => e.recipient_telno).HasMaxLength(12);
            entity.Property(e => e.recipient_relation_id);
            entity.Property(e => e.scen_id).HasComment("Scenario that happened during confiscation (e.g., Pemilik Tidak Dijumpai, linked to a reference ref_cfsc_scenarios table.");
            entity.Property(e => e.section_code).HasColumnType("character varying");
            entity.Property(e => e.tax_accno)
                .HasMaxLength(40)
                .HasComment("Tax account number associated with the business being inspected.");
            entity.Property(e => e.trnstatus_id)
                .HasDefaultValue(1)
                .HasComment("Status of the confiscation transaction, linked to a reference status table.");
            entity.Property(e => e.uuk_code).HasColumnType("character varying");

            entity.HasOne(d => d.inv).WithMany(p => p.trn_cfscs)
                .HasForeignKey(d => d.inv_id)
                .HasConstraintName("fk_inv_id");

            entity.HasOne(d => d.inv_type).WithMany(p => p.trn_cfscs)
                .HasForeignKey(d => d.inv_type_id)
                .HasConstraintName("fk_inv_type_id");

            entity.HasOne(d => d.scen).WithMany(p => p.trn_cfscs)
                .HasForeignKey(d => d.scen_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_trn_confiscation_scen_id");

            entity.HasOne(d => d.schedule).WithMany(p => p.trn_cfscs)
                .HasForeignKey(d => d.schedule_id)
                .HasConstraintName("fk_schedule_id");

            entity.HasOne(d => d.trnstatus).WithMany(p => p.trn_cfscs)
                .HasForeignKey(d => d.trnstatus_id)
                .HasConstraintName("fk_trnstatus_id");
        });

        modelBuilder.Entity<trn_cfsc_img>(entity =>
        {
            entity.HasKey(e => e.cfsc_img_id).HasName("trn_cfsc_imgs_pkey");

            entity.ToTable("trn_cfsc_imgs", "tenant", tb => tb.HasComment("Stores images associated with confiscation records"));

            entity.Property(e => e.cfsc_img_id).HasComment("Unique identifier for the confiscation image record");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record.");
            entity.Property(e => e.desc).HasComment("Optional description of the image");
            entity.Property(e => e.filename)
                .HasMaxLength(255)
                .HasComment("Original filename of the image");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record.");
            entity.Property(e => e.pathurl)
                .HasMaxLength(255)
                .HasComment("URL or file path where the image is stored");
            entity.Property(e => e.trn_cfsc_id).HasComment("Foreign key to the trn_confiscation table");

            entity.HasOne(d => d.trn_cfsc).WithMany(p => p.trn_cfsc_imgs)
                .HasForeignKey(d => d.trn_cfsc_id)
                .HasConstraintName("fk_trn_cfsc_img_trn_cfsc_id");
        });

        modelBuilder.Entity<trn_cfsc_item>(entity =>
        {
            entity.HasKey(e => e.item_id).HasName("trn_cfsc_items_pkey");

            entity.ToTable("trn_cfsc_items", "tenant", tb => tb.HasComment("Table to store confiscated items along with their types."));

            entity.Property(e => e.item_id).HasComment("Unique identifier for each confiscated item.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record.");
            entity.Property(e => e.description).HasColumnType("character varying");
            entity.Property(e => e.inv_id).HasComment("Type of the confiscated item (Mudah Disita or Tidak Mudah Disita).");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who modified this record.");

            entity.HasOne(d => d.inv).WithMany(p => p.trn_cfsc_items)
                .HasForeignKey(d => d.inv_id)
                .HasConstraintName("fk_inv_id");

            entity.HasOne(d => d.trn_cfsc).WithMany(p => p.trn_cfsc_items)
                .HasForeignKey(d => d.trn_cfsc_id)
                .HasConstraintName("fk_trncfsc_id");
        });

        modelBuilder.Entity<trn_cmpd>(entity =>
        {
            entity.HasKey(e => e.trn_cmpd_id).HasName("trn_cmpds_pkey");

            entity.ToTable("trn_cmpds", "tenant", tb => tb.HasComment("Table to store information about compounds issued, including details about the owner, business, and associated documentation."));

            entity.Property(e => e.trn_cmpd_id).HasComment("Unique identifier for each compound record.");
            entity.Property(e => e.act_code).HasColumnType("character varying");
            entity.Property(e => e.amt_cmpd)
                .HasPrecision(12, 2)
                .HasComment("Amount associated with the compound, stored as a numeric value.");
            entity.Property(e => e.cmpd_latitude)
                .HasPrecision(9, 6)
                .HasComment("Latitude of the location where the compound was issued.");
            entity.Property(e => e.cmpd_longitude)
                .HasPrecision(9, 6)
                .HasComment("Longitude of the location where the compound was issued.");
            entity.Property(e => e.cmpd_ref_no)
                .HasMaxLength(50)
                .HasComment("Reference number for the compound.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record.");
            entity.Property(e => e.deliver_id).HasComment("Identifier for delivery method used for this compound notice.");
            entity.Property(e => e.doc_name).HasColumnType("character varying");
            entity.Property(e => e.doc_pathurl).HasColumnType("character varying");
            entity.Property(e => e.instruction).HasComment("Instructions regarding actions to be taken related to this compound.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record.");
            entity.Property(e => e.offense_code).HasColumnType("character varying");
            entity.Property(e => e.offs_location).HasComment("Location where the offense occurred.");
            entity.Property(e => e.owner_icno)
                .HasMaxLength(30)
                .HasComment("Identification number of the owner.");
            entity.Property(e => e.recipient_addr).HasMaxLength(255);
            entity.Property(e => e.recipient_icno).HasMaxLength(20);
            entity.Property(e => e.recipient_name).HasMaxLength(50);
            entity.Property(e => e.recipient_sign).HasMaxLength(255);
            entity.Property(e => e.recipient_telno).HasMaxLength(12);
            entity.Property(e => e.recipient_relation_id);
            entity.Property(e => e.section_code).HasColumnType("character varying");
            entity.Property(e => e.tax_accno).HasColumnType("character varying");
            entity.Property(e => e.trnstatus_id)
                .HasDefaultValue(4)
                .HasComment("Status of the compound, linked to a reference status table.");
            entity.Property(e => e.uuk_code).HasColumnType("character varying");

            entity.HasOne(d => d.deliver).WithMany(p => p.trn_cmpds)
                .HasForeignKey(d => d.deliver_id)
                .HasConstraintName("fk_deliver_id");

            entity.HasOne(d => d.schedule).WithMany(p => p.trn_cmpds)
                .HasForeignKey(d => d.schedule_id)
                .HasConstraintName("fk_schedule_id");

            entity.HasOne(d => d.trnstatus).WithMany(p => p.trn_cmpds)
                .HasForeignKey(d => d.trnstatus_id)
                .HasConstraintName("fk_trnstatus_id");
        });

        modelBuilder.Entity<trn_cmpd_img>(entity =>
        {
            entity.HasKey(e => e.cmpd_img_id).HasName("trn_cmpd_imgs_pkey");

            entity.ToTable("trn_cmpd_imgs", "tenant", tb => tb.HasComment("Stores images associated with compound records"));

            entity.Property(e => e.cmpd_img_id).HasComment("Unique identifier for the compound image record");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.desc).HasComment("Optional description of the image");
            entity.Property(e => e.filename)
                .HasMaxLength(255)
                .HasComment("Original filename of the image");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasDefaultValue(0);
            entity.Property(e => e.pathurl)
                .HasMaxLength(255)
                .HasComment("URL or file path where the image is stored");
            entity.Property(e => e.trn_cmpd_id).HasComment("Foreign key to the trn_compounds table");

            entity.HasOne(d => d.trn_cmpd).WithMany(p => p.trn_cmpd_imgs)
                .HasForeignKey(d => d.trn_cmpd_id)
                .HasConstraintName("fk_trn_cmpd_id");
        });

        modelBuilder.Entity<trn_inspect>(entity =>
        {
            entity.HasKey(e => e.trn_inspect_id).HasName("trn_inspects_pkey");

            entity.ToTable("trn_inspects", "tenant", tb => tb.HasComment("Table to store inspection records, including details about the inspection type, owner information, and related documentation."));

            entity.Property(e => e.trn_inspect_id).HasComment("Unique identifier for each inspection record.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record.");
            entity.Property(e => e.dept_id).HasComment("Identifier for the department responsible for conducting this inspection.");
            entity.Property(e => e.doc_name).HasColumnType("character varying");
            entity.Property(e => e.doc_pathurl).HasColumnType("character varying");
            entity.Property(e => e.inspect_latitude)
                .HasPrecision(9, 6)
                .HasComment("Latitude of the location where the inspection occurred.");
            entity.Property(e => e.inspect_longitude)
                .HasPrecision(9, 6)
                .HasComment("Longitude of the location where the inspection occurred.");
            entity.Property(e => e.inspect_ref_no)
                .HasMaxLength(50)
                .HasComment("Reference number for tracking this specific inspection.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record.");
            entity.Property(e => e.notes).HasComment("Additional notes or comments regarding the inspection.");
            entity.Property(e => e.offs_location).HasComment("Location where any offenses occurred during the inspection.");
            entity.Property(e => e.owner_icno)
                .HasMaxLength(30)
                .HasComment("Identification number of the owner associated with the inspection.");
            entity.Property(e => e.tax_accno).HasColumnType("character varying");
            entity.Property(e => e.trnstatus_id)
                .HasDefaultValue(1)
                .HasComment("Status of the inspection record, linked to a reference status table.");

            entity.HasOne(d => d.dept).WithMany(p => p.trn_inspects)
                .HasForeignKey(d => d.dept_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("trn_inspect_dept_id_fkey");

            entity.HasOne(d => d.trnstatus).WithMany(p => p.trn_inspects)
                .HasForeignKey(d => d.trnstatus_id)
                .HasConstraintName("fk_trnstatus_id");
        });

        modelBuilder.Entity<trn_inspect_img>(entity =>
        {
            entity.HasKey(e => e.inspect_img_id).HasName("trn_inspect_img_pkey");

            entity.ToTable("trn_inspect_imgs", "tenant", tb => tb.HasComment("Stores images associated with inspection notes records"));

            entity.Property(e => e.inspect_img_id).HasComment("Unique identifier for the compound image record");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record.");
            entity.Property(e => e.desc).HasComment("Optional description of the image");
            entity.Property(e => e.filename)
                .HasMaxLength(255)
                .HasComment("Original filename of the image");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record.");
            entity.Property(e => e.pathurl)
                .HasMaxLength(255)
                .HasComment("URL or file path where the image is stored");
            entity.Property(e => e.trn_inspect_id).HasComment("Foreign key to the trn_compounds table");

            entity.HasOne(d => d.trn_inspect).WithMany(p => p.trn_inspect_imgs)
                .HasForeignKey(d => d.trn_inspect_id)
                .HasConstraintName("fk_trn_inspect_img_trn_inspect_id");
        });

        modelBuilder.Entity<trn_notice>(entity =>
        {
            entity.HasKey(e => e.trn_notice_id).HasName("trn_notices_pkey");

            entity.ToTable("trn_notices", "tenant", tb => tb.HasComment("Table to store information about notices issued, including details about the owner, business, and associated documentation."));

            entity.Property(e => e.trn_notice_id).HasComment("Unique identifier for each notice.");
            entity.Property(e => e.act_code).HasColumnType("character varying");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record.");
            entity.Property(e => e.deliver_id).HasComment("Identifier for delivery method used for this notice.");
            entity.Property(e => e.doc_name).HasColumnType("character varying");
            entity.Property(e => e.doc_pathurl).HasColumnType("character varying");
            entity.Property(e => e.duration_id)
                .HasDefaultValue(1)
                .HasComment("Duration PREMISE OWNER for how long this notice is valid or relevant.");
            entity.Property(e => e.instruction).HasComment("Instructions regarding actions to be taken related to this notice.");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record.");
            entity.Property(e => e.notice_latitude)
                .HasPrecision(9, 6)
                .HasComment("Latitude of the location where the notice was issued.");
            entity.Property(e => e.notice_longitude)
                .HasPrecision(9, 6)
                .HasComment("Longitude of the location where the notice was issued.");
            entity.Property(e => e.notice_ref_no)
                .HasMaxLength(50)
                .HasComment("Reference number for the notice.");
            entity.Property(e => e.offense_code).HasColumnType("character varying");
            entity.Property(e => e.offs_location).HasComment("Location where the offense occurred.");
            entity.Property(e => e.owner_icno)
                .HasMaxLength(30)
                .HasComment("Identification number of the owner.");
            entity.Property(e => e.recipient_addr).HasMaxLength(255);
            entity.Property(e => e.recipient_icno).HasMaxLength(20);
            entity.Property(e => e.recipient_name).HasMaxLength(50);
            entity.Property(e => e.recipient_sign).HasMaxLength(255);
            entity.Property(e => e.recipient_telno).HasMaxLength(12);
            entity.Property(e => e.recipient_relation_id);
            entity.Property(e => e.section_code).HasColumnType("character varying");
            entity.Property(e => e.tax_accno)
                .HasMaxLength(40)
                .HasComment("Tax account number associated with the owner.");
            entity.Property(e => e.trnstatus_id)
                .HasDefaultValue(1)
                .HasComment("Status of the notice, linked to a reference status table.");
            entity.Property(e => e.uuk_code).HasColumnType("character varying");

            entity.HasOne(d => d.deliver).WithMany(p => p.trn_notices)
                .HasForeignKey(d => d.deliver_id)
                .HasConstraintName("fk_deliver_id");

            entity.HasOne(d => d.duration).WithMany(p => p.trn_notices)
                .HasForeignKey(d => d.duration_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("trn_notice_notice_duration_id_fkey");

            entity.HasOne(d => d.schedule).WithMany(p => p.trn_notices)
                .HasForeignKey(d => d.schedule_id)
                .HasConstraintName("fk_schedule_id");

            entity.HasOne(d => d.trnstatus).WithMany(p => p.trn_notices)
                .HasForeignKey(d => d.trnstatus_id)
                .HasConstraintName("fk_trnstatus_id");
        });

        modelBuilder.Entity<trn_notice_img>(entity =>
        {
            entity.HasKey(e => e.notice_img_id).HasName("trn_notice_imgs_pkey");

            entity.ToTable("trn_notice_imgs", "tenant", tb => tb.HasComment("Stores images associated with notices records"));

            entity.Property(e => e.notice_img_id).HasComment("Unique identifier for the compound image record");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record.");
            entity.Property(e => e.desc).HasComment("Optional description of the image");
            entity.Property(e => e.filename)
                .HasMaxLength(255)
                .HasComment("Original filename of the image");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is deleted (soft delete).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record.");
            entity.Property(e => e.pathurl)
                .HasMaxLength(255)
                .HasComment("URL or file path where the image is stored");
            entity.Property(e => e.trn_notice_id).HasComment("Foreign key to the trn_notices table");

            entity.HasOne(d => d.trn_notice).WithMany(p => p.trn_notice_imgs)
                .HasForeignKey(d => d.trn_notice_id)
                .HasConstraintName("fk_trn_notice_img_trn_notice_id");
        });

        modelBuilder.Entity<trn_patrol_officer>(entity =>
        {
            entity.HasKey(e => e.officer_id).HasName("trn_patrol_officers_pkey");

            entity.ToTable("trn_patrol_officers", "tenant");

            entity.Property(e => e.cnt_cmpd).HasDefaultValue(0);
            entity.Property(e => e.cnt_notes).HasDefaultValue(0);
            entity.Property(e => e.cnt_notice).HasDefaultValue(0);
            entity.Property(e => e.cnt_seizure).HasDefaultValue(0);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.end_time).HasColumnType("timestamp without time zone");
            entity.Property(e => e.idno).HasMaxLength(50);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.is_leader).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasDefaultValue(0);
            entity.Property(e => e.start_time).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.schedule).WithMany(p => p.trn_patrol_officers)
                .HasForeignKey(d => d.schedule_id)
                .HasConstraintName("schedule_id_refers_to_schedule_id");

            entity.HasOne(d => d.visit).WithMany(p => p.trn_patrol_officers)
                .HasForeignKey(d => d.visit_id)
                .HasConstraintName("fk_visit_id");
        });

        modelBuilder.Entity<trn_premis_visit>(entity =>
        {
            entity.HasKey(e => e.visit_id).HasName("trn_premis_visit_pkey");

            entity.ToTable("trn_premis_visit", "tenant", tb => tb.HasComment("This table stores status visited premis that has been done by officer"));

            entity.Property(e => e.codeid_premis).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.status_visit).HasDefaultValue(false);

            entity.HasOne(d => d.codeid_premisNavigation).WithMany(p => p.trn_premis_visits)
                .HasPrincipalKey(p => p.codeid_premis)
                .HasForeignKey(d => d.codeid_premis)
                .HasConstraintName("fk_codeid_premis");

            entity.HasOne(d => d.schedule).WithMany(p => p.trn_premis_visits)
                .HasForeignKey(d => d.schedule_id)
                .HasConstraintName("fk_schedule_id");
        });

        modelBuilder.Entity<trn_witness>(entity =>
        {
            entity.HasKey(e => e.witness_id).HasName("trn_witness_pkey");

            entity.ToTable("trn_witness", "tenant");

            entity.Property(e => e.created_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.modified_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.name).HasColumnType("character varying");
            entity.Property(e => e.trn_type).HasColumnType("character varying");
        });
        modelBuilder.HasSequence("mst_patrol_schedule_schedule_id_seq", "tenant");
        modelBuilder.HasSequence("mst_premis_id_seq", "tenant");
        modelBuilder.HasSequence("ref_cfsc_scenarios_scen_id_seq", "tenant");
        modelBuilder.HasSequence("ref_department_dept_id_seq", "tenant");
        modelBuilder.HasSequence("ref_division_div_id_seq", "tenant");
        modelBuilder.HasSequence("ref_doc_doc_id_seq", "tenant");
        modelBuilder.HasSequence("ref_unit_unit_id_seq", "tenant");
        modelBuilder.HasSequence("trn_premis_visit_visit_id_seq", "tenant");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
