using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PBTPro.DAL.Models;

namespace PBTPro.DAL;

public partial class PBTProDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    //public PBTProDbContext(DbContextOptions<PBTProDbContext> options)
    //    : base(options)
    //{
    //}

    public virtual DbSet<auditlog_archive_info> auditlog_archive_infos { get; set; }

    public virtual DbSet<auditlog_info> auditlog_infos { get; set; }

    public virtual DbSet<config_building> config_buildings { get; set; }

    public virtual DbSet<config_department> config_departments { get; set; }

    public virtual DbSet<config_email_template> config_email_templates { get; set; }

    public virtual DbSet<config_form_field> config_form_fields { get; set; }

    public virtual DbSet<config_pbt> config_pbts { get; set; }

    public virtual DbSet<config_system_message> config_system_messages { get; set; }

    public virtual DbSet<config_system_param> config_system_params { get; set; }

    public virtual DbSet<department_info> department_infos { get; set; }

    public virtual DbSet<faq_info> faq_infos { get; set; }

    public virtual DbSet<license_address_swap> license_address_swaps { get; set; }

    public virtual DbSet<license_history> license_histories { get; set; }

    public virtual DbSet<license_holder> license_holders { get; set; }

    public virtual DbSet<license_information> license_informations { get; set; }

    public virtual DbSet<license_location> license_locations { get; set; }

    public virtual DbSet<license_medium> license_media { get; set; }

    public virtual DbSet<license_tax> license_taxes { get; set; }

    public virtual DbSet<license_transaction> license_transactions { get; set; }

    public virtual DbSet<menu> menus { get; set; }

    public virtual DbSet<mst_area> mst_areas { get; set; }

    public virtual DbSet<mst_daerah> mst_daerahs { get; set; }

    public virtual DbSet<mst_district> mst_districts { get; set; }

    public virtual DbSet<mst_lot> mst_lots { get; set; }

    public virtual DbSet<mst_mukim> mst_mukims { get; set; }

    public virtual DbSet<mst_premis> mst_premis { get; set; }

    public virtual DbSet<notification_email_history> notification_email_histories { get; set; }

    public virtual DbSet<notification_email_queue> notification_email_queues { get; set; }

    public virtual DbSet<patrol_info> patrol_infos { get; set; }

    public virtual DbSet<patrol_member> patrol_members { get; set; }

    public virtual DbSet<patrol_scheduler> patrol_schedulers { get; set; }

    public virtual DbSet<permission> permissions { get; set; }

    public virtual DbSet<user_account> user_accounts { get; set; }

    public virtual DbSet<user_profile> user_profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<auditlog_archive_info>(entity =>
        {
            //entity
            //    .HasNoKey()
            //    .ToTable("auditlog_archive_info", "audit");

            entity.HasKey(e => e.archive_id).HasName("auditlog_archive_info_pkey");

            entity.ToTable("auditlog_archive_info", "audit");

            entity.Property(e => e.archive_id).HasDefaultValueSql("nextval('audit.auditlog_audit_id_seq'::regclass)");
            entity.Property(e => e.archive_isarchived).HasDefaultValue(true);
            entity.Property(e => e.archive_method).HasMaxLength(255);
            entity.Property(e => e.archive_module_name)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.archive_role_id).HasDefaultValue(0);
            entity.Property(e => e.archive_username).HasMaxLength(25);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.archive_audit_id).HasDefaultValue(0);
        });

        modelBuilder.Entity<auditlog_info>(entity =>
        {
            entity.HasKey(e => e.audit_id).HasName("auditlog_pkey");

            entity.ToTable("auditlog_info", "audit");

            entity.Property(e => e.audit_id).HasDefaultValueSql("nextval('audit.auditlog_audit_id_seq'::regclass)");
            entity.Property(e => e.audit_isarchived).HasDefaultValue(false);
            entity.Property(e => e.audit_method).HasMaxLength(255);
            entity.Property(e => e.audit_module_name)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.audit_role_id).HasDefaultValue(0);
            entity.Property(e => e.audit_username).HasMaxLength(25);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<config_building>(entity =>
        {
            entity.HasKey(e => e.building_id).HasName("config_building_pkey");

            entity.ToTable("config_building", "config");

            entity.Property(e => e.building_code).HasMaxLength(30);
            entity.Property(e => e.building_name)
                .HasMaxLength(250)
                .IsFixedLength();
            entity.Property(e => e.created_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.updated_date).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<config_department>(entity =>
        {
            entity.HasKey(e => e.config_dept_id).HasName("config_department_pkey");

            entity.ToTable("config_department", "config");

            entity.Property(e => e.config_dept_email).HasMaxLength(150);
            entity.Property(e => e.config_dept_head).HasMaxLength(300);
            entity.Property(e => e.config_dept_name).HasMaxLength(250);
            entity.Property(e => e.config_dept_notel).HasMaxLength(25);

            entity.HasOne(d => d.config_dept_pbtNavigation).WithMany(p => p.config_departments)
                .HasForeignKey(d => d.config_dept_pbt)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("pbt_code");
        });

        modelBuilder.Entity<config_email_template>(entity =>
        {
            entity.HasKey(e => e.template_id).HasName("config_email_template_pkey");

            entity.ToTable("config_email_template", "config");

            entity.Property(e => e.active_flag).HasDefaultValue(true);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.update_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.template_code).HasMaxLength(50);
            entity.Property(e => e.template_subject).HasMaxLength(255);
            entity.Property(e => e.updated_by).HasDefaultValue(0);
        });

        modelBuilder.Entity<config_form_field>(entity =>
        {
            entity.HasKey(e => e.field_id).HasName("config_form_field_pkey");

            entity.ToTable("config_form_field", "config");

            entity.Property(e => e.active_flag).HasDefaultValue(true);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.field_api_seeded).HasDefaultValue(false);
            entity.Property(e => e.field_form_type).HasMaxLength(50);
            entity.Property(e => e.field_label).HasMaxLength(50);
            entity.Property(e => e.field_name).HasMaxLength(50);
            entity.Property(e => e.field_orders).HasDefaultValue(0);
            entity.Property(e => e.field_required).HasDefaultValue(false);
            entity.Property(e => e.field_source_url)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.field_type).HasMaxLength(10);
            entity.Property(e => e.update_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.updated_by).HasDefaultValue(0);
        });

        modelBuilder.Entity<config_pbt>(entity =>
        {
            entity.HasKey(e => e.pbt_id).HasName("config_pbt_pkey");

            entity.ToTable("config_pbt", "config");

            entity.Property(e => e.pbt_address1).HasMaxLength(150);
            entity.Property(e => e.pbt_address2).HasMaxLength(150);
            entity.Property(e => e.pbt_address3).HasMaxLength(150);
            entity.Property(e => e.pbt_code).HasPrecision(5);
            entity.Property(e => e.pbt_email).HasMaxLength(250);
            entity.Property(e => e.pbt_name).HasMaxLength(500);
            entity.Property(e => e.pbt_notel).HasMaxLength(25);
            entity.Property(e => e.pbt_pcode).HasPrecision(5);
            entity.Property(e => e.pbt_state).HasMaxLength(30);
        });

        modelBuilder.Entity<config_system_message>(entity =>
        {
            entity.HasKey(e => e.message_id).HasName("config_system_message_pkey");

            entity.ToTable("config_system_message", "config");

            entity.Property(e => e.active_flag).HasDefaultValue(true);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.message_code).HasMaxLength(255);
            entity.Property(e => e.message_feature).HasMaxLength(50);
            entity.Property(e => e.message_type).HasMaxLength(1);
            entity.Property(e => e.update_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.updated_by).HasDefaultValue(0);
        });

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

        modelBuilder.Entity<department_info>(entity =>
        {
            entity.HasKey(e => e.dept_id).HasName("department_info_pkey");

            entity.ToTable("department_info", "department");

            entity.Property(e => e.dept_id).HasDefaultValueSql("nextval('department.department_info_depart_id_seq'::regclass)");
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.dept_code).HasMaxLength(50);
            entity.Property(e => e.dept_name).HasMaxLength(100);
            entity.Property(e => e.dept_description)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.dept_status).HasMaxLength(30);
            entity.Property(e => e.updated_by).HasDefaultValue(0);
            entity.Property(e => e.updated_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<faq_info>(entity =>
        {
            entity.HasKey(e => e.faq_id).HasName("faq_info_pkey");

            entity.ToTable("faq_info", "faq");

            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.faq_category).HasMaxLength(50);
            entity.Property(e => e.faq_status).HasMaxLength(30);
            entity.Property(e => e.updated_by).HasDefaultValue(0);
            entity.Property(e => e.updated_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<license_address_swap>(entity =>
        {
            entity.HasKey(e => e.swap_license_id).HasName("license_address_swap_pkey");

            entity.ToTable("license_address_swap", "license", tb => tb.HasComment("TABLE BAGI MENYIMPAN ALAMAT LESEN SEDIA ADA DAN BARU"));

            entity.Property(e => e.created_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.swap_current_addr1).HasMaxLength(100);
            entity.Property(e => e.swap_current_addr2).HasMaxLength(150);
            entity.Property(e => e.swap_current_addr3).HasMaxLength(150);
            entity.Property(e => e.swap_current_area).HasMaxLength(50);
            entity.Property(e => e.swap_current_pcode).HasPrecision(5);
            entity.Property(e => e.swap_current_state).HasMaxLength(30);
            entity.Property(e => e.swap_license_account).HasMaxLength(20);
            entity.Property(e => e.swap_new_addr1).HasMaxLength(100);
            entity.Property(e => e.swap_new_addr2).HasMaxLength(150);
            entity.Property(e => e.swap_new_addr3).HasMaxLength(150);
            entity.Property(e => e.swap_new_area).HasMaxLength(50);
            entity.Property(e => e.swap_new_pcode).HasPrecision(5);
            entity.Property(e => e.swap_new_state).HasMaxLength(30);
            entity.Property(e => e.updated_date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.swap_id_infoNavigation).WithMany(p => p.license_address_swaps)
                .HasForeignKey(d => d.swap_id_info)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("swap_id_info");
        });

        modelBuilder.Entity<license_history>(entity =>
        {
            entity.HasKey(e => e.license_hist_id).HasName("license_history_pkey");

            entity.ToTable("license_history", "license", tb => tb.HasComment("TABLE SEJARAH LESEN"));

            entity.Property(e => e.created_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.updated_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.license_hist_account).HasMaxLength(20);
            entity.Property(e => e.license_hist_addr1).HasMaxLength(100);
            entity.Property(e => e.license_hist_addr2).HasMaxLength(150);
            entity.Property(e => e.license_hist_addr3).HasMaxLength(150);
            entity.Property(e => e.license_hist_area).HasMaxLength(50);
            entity.Property(e => e.license_hist_holder).HasMaxLength(20);
            entity.Property(e => e.license_hist_pcode).HasPrecision(5);
            entity.Property(e => e.license_hist_state).HasMaxLength(30);

            entity.HasOne(d => d.hist_id_license).WithMany(p => p.license_history)
               .HasForeignKey(d => d.hist_id_info)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("hist_id_info");
        });

        modelBuilder.Entity<license_holder>(entity =>
        {
            entity.HasKey(e => e.license_holder_id).HasName("license_holder_pkey");

            entity.ToTable("license_holder", "license", tb => tb.HasComment("TABLE MAKLUMAT PELESEN"));

            entity.Property(e => e.created_by).HasMaxLength(10);
            entity.Property(e => e.created_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.license_holder_account).HasMaxLength(20);
            entity.Property(e => e.license_holder_addr1).HasMaxLength(100);
            entity.Property(e => e.license_holder_addr2).HasMaxLength(150);
            entity.Property(e => e.license_holder_addr3).HasMaxLength(150);
            entity.Property(e => e.license_holder_area).HasMaxLength(50);
            entity.Property(e => e.license_holder_custid).HasMaxLength(50);
            entity.Property(e => e.license_holder_email).HasMaxLength(100);
            entity.Property(e => e.license_holder_name).HasMaxLength(250);
            entity.Property(e => e.license_holder_pcode).HasPrecision(5);
            entity.Property(e => e.license_holder_phone).HasPrecision(20);
            entity.Property(e => e.license_holder_state)
                .HasMaxLength(30)
                .IsFixedLength();
            entity.Property(e => e.updated_by).HasMaxLength(10);
            entity.Property(e => e.updated_date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.license_holder_infoNavigation).WithMany(p => p.license_holders)
                .HasForeignKey(d => d.license_holder_info)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("license_holder_info");
        });

        modelBuilder.Entity<license_information>(entity =>
        {
            entity.HasKey(e => e.license_id).HasName("license_information_pkey");

            entity.ToTable("license_information", "license", tb => tb.HasComment("TABLE BAGI MAKLUMAT LESEN"));

            entity.HasIndex(e => e.license_id, "index_license_id")
                .IsUnique()
                .HasAnnotation("Npgsql:StorageParameter:deduplicate_items", "false");

            entity.Property(e => e.created_by).HasMaxLength(10);
            entity.Property(e => e.created_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.license_account_number).HasMaxLength(50);
            entity.Property(e => e.license_amount).HasPrecision(15, 2);
            entity.Property(e => e.license_amount_balance).HasPrecision(15, 2);
            entity.Property(e => e.license_business_addr1).HasMaxLength(100);
            entity.Property(e => e.license_business_addr2).HasMaxLength(150);
            entity.Property(e => e.license_business_addr3).HasMaxLength(150);
            entity.Property(e => e.license_business_area).HasMaxLength(50);
            entity.Property(e => e.license_business_name).HasMaxLength(250);
            entity.Property(e => e.license_business_pcode).HasPrecision(5);
            entity.Property(e => e.license_business_state).HasMaxLength(30);
            entity.Property(e => e.license_latitude).HasMaxLength(250);
            entity.Property(e => e.license_longitud).HasMaxLength(250);
            entity.Property(e => e.license_payment_status).HasMaxLength(30);
            entity.Property(e => e.license_pbt_origin).HasPrecision(5);
            entity.Property(e => e.license_period_status).HasMaxLength(30);
            entity.Property(e => e.license_risk_status).HasMaxLength(30);
            entity.Property(e => e.license_status).HasMaxLength(20);
            entity.Property(e => e.license_type).HasMaxLength(50);
            entity.Property(e => e.updated_by).HasMaxLength(10);
            entity.Property(e => e.updated_date).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<license_location>(entity =>
        {
            entity.HasKey(e => e.license_loc_id).HasName("license_location_pkey");

            entity.ToTable("license_location", "license");

            entity.Property(e => e.created_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.license_loc_latitude).HasMaxLength(100);
            entity.Property(e => e.license_loc_longitude).HasMaxLength(100);
            entity.Property(e => e.license_loc_no).HasMaxLength(30);
            entity.Property(e => e.license_loc_pbt).HasPrecision(5);
            entity.Property(e => e.updated_date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.license_loc_infoNavigation).WithMany(p => p.license_locations)
                .HasForeignKey(d => d.license_loc_info)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("license_loc_info");
        });

        modelBuilder.Entity<license_medium>(entity =>
        {
            entity.HasKey(e => e.media_id).HasName("license_media_pkey");

            entity.ToTable("license_media", "license");

            entity.Property(e => e.media_license_account).HasMaxLength(50);
            entity.Property(e => e.media_url_link).HasMaxLength(500);

            entity.HasOne(d => d.media_id_infoNavigation).WithMany(p => p.license_media)
                .HasForeignKey(d => d.media_id_info)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("media_id_info");
        });

        modelBuilder.Entity<license_tax>(entity =>
        {
            entity.HasKey(e => e.tax_id).HasName("license_tax_pkey");

            entity.ToTable("license_tax", "license", tb => tb.HasComment("TABLE MAKLUMAT CUKAI TAKSIRAN"));

            entity.Property(e => e.created_by).HasMaxLength(10);
            entity.Property(e => e.created_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.tax_license_account).HasMaxLength(20);
            entity.Property(e => e.tax_main_account).HasMaxLength(20);
            entity.Property(e => e.tax_property_address1).HasMaxLength(100);
            entity.Property(e => e.tax_property_address2).HasMaxLength(250);
            entity.Property(e => e.tax_property_amount).HasPrecision(15, 2);
            entity.Property(e => e.tax_property_area).HasMaxLength(50);
            entity.Property(e => e.tax_property_pcode).HasPrecision(5);
            entity.Property(e => e.tax_property_state).HasMaxLength(50);
            entity.Property(e => e.tax_property_status).HasMaxLength(1);
            entity.Property(e => e.updated_by).HasMaxLength(10);
            entity.Property(e => e.updated_date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.tax_license_infoNavigation).WithMany(p => p.license_taxes)
                .HasForeignKey(d => d.tax_license_info)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("tax_license_info");
        });

        modelBuilder.Entity<license_transaction>(entity =>
        {
            entity.HasKey(e => e.license_trans_id).HasName("license_transaction_pkey");

            entity.ToTable("license_transaction", "license", tb => tb.HasComment("TABLE TRANSAKSI LESEN"));

            entity.Property(e => e.created_by).HasMaxLength(10);
            entity.Property(e => e.created_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.license_trans_account).HasMaxLength(20);
            entity.Property(e => e.license_trans_amount).HasPrecision(15, 2);
            entity.Property(e => e.license_trans_code).HasMaxLength(50);
            entity.Property(e => e.license_trans_name).HasMaxLength(250);
            entity.Property(e => e.license_trans_status).HasMaxLength(1);
            entity.Property(e => e.updated_by).HasMaxLength(10);
            entity.Property(e => e.updated_date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.license_trans_infoNavigation).WithMany(p => p.license_transactions)
                .HasForeignKey(d => d.license_trans_info)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("license_trans_info");
        });

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

        modelBuilder.Entity<mst_area>(entity =>
        {
            entity.HasKey(e => e.gid).HasName("mst_area_pkey");

            entity.ToTable("mst_area");

            entity.HasIndex(e => e.geom, "mst_area_geom_idx").HasMethod("gist");

            entity.Property(e => e.acc).HasMaxLength(50);
            entity.Property(e => e.ark).HasMaxLength(50);
            entity.Property(e => e.bds).HasMaxLength(50);
            entity.Property(e => e.fcd).HasMaxLength(7);
            entity.Property(e => e.fnm).HasMaxLength(100);
            entity.Property(e => e.geom).HasColumnType("geometry(MultiPolygon,3375)");
            entity.Property(e => e.keluasan).HasMaxLength(50);
            entity.Property(e => e.kemaskini).HasMaxLength(50);
            entity.Property(e => e.kod_daerah).HasMaxLength(50);
            entity.Property(e => e.kod_negeri).HasMaxLength(50);
            entity.Property(e => e.nam).HasMaxLength(50);
        });

        modelBuilder.Entity<mst_daerah>(entity =>
        {
            entity.HasKey(e => e.gid).HasName("mst_daerah_pkey");

            entity.ToTable("mst_daerah");

            entity.HasIndex(e => e.geom, "mst_daerah_geom_idx").HasMethod("gist");

            entity.Property(e => e.acc).HasMaxLength(50);
            entity.Property(e => e.ark).HasMaxLength(50);
            entity.Property(e => e.bds).HasMaxLength(50);
            entity.Property(e => e.fcd).HasMaxLength(7);
            entity.Property(e => e.fnm).HasMaxLength(100);
            entity.Property(e => e.geom).HasColumnType("geometry(MultiPolygon,3375)");
            entity.Property(e => e.keluasan).HasMaxLength(50);
            entity.Property(e => e.kemaskini).HasMaxLength(50);
            entity.Property(e => e.kod_daerah).HasMaxLength(50);
            entity.Property(e => e.kod_negeri).HasMaxLength(50);
            entity.Property(e => e.nam).HasMaxLength(50);
        });

        modelBuilder.Entity<mst_district>(entity =>
        {
            entity.HasKey(e => e.gid).HasName("mst_district_pkey");

            entity.ToTable("mst_district");

            entity.HasIndex(e => e.geom, "mst_district_geom_idx").HasMethod("gist");

            entity.Property(e => e.acc).HasMaxLength(50);
            entity.Property(e => e.ark).HasMaxLength(50);
            entity.Property(e => e.bds).HasMaxLength(50);
            entity.Property(e => e.dasdas).HasMaxLength(50);
            entity.Property(e => e.fcd).HasMaxLength(10);
            entity.Property(e => e.fnm).HasMaxLength(50);
            entity.Property(e => e.geom).HasColumnType("geometry(MultiPolygon,3375)");
            entity.Property(e => e.globalid).HasMaxLength(38);
            entity.Property(e => e.keluasan).HasMaxLength(50);
            entity.Property(e => e.kod_daerah).HasMaxLength(50);
            entity.Property(e => e.kod_mukim).HasMaxLength(50);
            entity.Property(e => e.kod_negeri).HasMaxLength(50);
            entity.Property(e => e.nam).HasMaxLength(50);
        });

        modelBuilder.Entity<mst_lot>(entity =>
        {
            entity.HasKey(e => e.gid).HasName("mst_lot_pkey");

            entity.ToTable("mst_lot");

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

        modelBuilder.Entity<mst_mukim>(entity =>
        {
            entity.HasKey(e => e.gid).HasName("mst_mukim_pkey");

            entity.ToTable("mst_mukim");

            entity.HasIndex(e => e.geom, "mst_mukim_geom_idx").HasMethod("gist");

            entity.Property(e => e.acc).HasMaxLength(50);
            entity.Property(e => e.ark).HasMaxLength(50);
            entity.Property(e => e.bds).HasMaxLength(50);
            entity.Property(e => e.dasdas).HasMaxLength(50);
            entity.Property(e => e.fcd).HasMaxLength(10);
            entity.Property(e => e.fnm).HasMaxLength(50);
            entity.Property(e => e.geom).HasColumnType("geometry(MultiPolygon,3375)");
            entity.Property(e => e.globalid).HasMaxLength(38);
            entity.Property(e => e.keluasan).HasMaxLength(50);
            entity.Property(e => e.kod_daerah).HasMaxLength(50);
            entity.Property(e => e.kod_mukim).HasMaxLength(50);
            entity.Property(e => e.kod_negeri).HasMaxLength(50);
            entity.Property(e => e.nam).HasMaxLength(50);
        });

        modelBuilder.Entity<mst_premis>(entity =>
        {
            entity.HasKey(e => e.gid).HasName("mst_premis_pkey");

            entity.HasIndex(e => e.geom, "mst_premis_geom_idx").HasMethod("gist");

            entity.Property(e => e.daerah).HasMaxLength(2);
            entity.Property(e => e.gambar1).HasMaxLength(10);
            entity.Property(e => e.gambar2).HasMaxLength(10);
            entity.Property(e => e.geom).HasColumnType("geometry(Point,4326)");
            entity.Property(e => e.lesen).HasMaxLength(20);
            entity.Property(e => e.lot).HasMaxLength(7);
            entity.Property(e => e.mukim).HasMaxLength(2);
            entity.Property(e => e.negeri).HasMaxLength(2);
            entity.Property(e => e.no_akaun).HasMaxLength(20);
            entity.Property(e => e.seksyen).HasMaxLength(3);
        });

        modelBuilder.Entity<notification_email_history>(entity =>
        {
            entity.HasKey(e => e.history_id).HasName("notification_email_history_pkey");

            entity.ToTable("notification_email_history", "notification");

            entity.Property(e => e.active_flag).HasDefaultValue(true);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.history_cnt_retry).HasDefaultValue(0);
            entity.Property(e => e.history_date_sent)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.history_recipient).HasMaxLength(255);
            entity.Property(e => e.history_status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'New'::character varying");
            entity.Property(e => e.history_subject).HasMaxLength(255);
            entity.Property(e => e.update_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.updated_by).HasDefaultValue(0);
        });

        modelBuilder.Entity<notification_email_queue>(entity =>
        {
            entity.HasKey(e => e.queue_id).HasName("notification_email_queue_pkey");

            entity.ToTable("notification_email_queue", "notification");

            entity.Property(e => e.active_flag).HasDefaultValue(true);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.update_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.queue_cnt_retry).HasDefaultValue(0);
            entity.Property(e => e.queue_date_sent)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.queue_recipient).HasMaxLength(255);
            entity.Property(e => e.queue_status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'New'::character varying");
            entity.Property(e => e.queue_subject).HasMaxLength(255);
            entity.Property(e => e.updated_by).HasDefaultValue(0);
        });        

        modelBuilder.Entity<patrol_info>(entity =>
        {

            entity.HasKey(e => e.patrol_id).HasName("patrol_info_pkey");

            entity.ToTable("patrol_info", "patrol");            

            entity.Property(e => e.active_flag).HasDefaultValue(true);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.patrol_cnt_compound).HasDefaultValue(0);
            entity.Property(e => e.patrol_cnt_notes).HasDefaultValue(0);
            entity.Property(e => e.patrol_cnt_notice).HasDefaultValue(0);
            entity.Property(e => e.patrol_cnt_seizure).HasDefaultValue(0);
            entity.Property(e => e.patrol_dept_name).HasColumnType("character varying");
            entity.Property(e => e.patrol_end_dtm).HasColumnType("timestamp without time zone");
            entity.Property(e => e.patrol_end_location).HasColumnType("geometry(Point,4326)");
            entity.Property(e => e.patrol_location).HasColumnType("character varying");
            entity.Property(e => e.patrol_officer_name).HasColumnType("character varying");
            entity.Property(e => e.patrol_scheduled).HasDefaultValue(true);
            entity.Property(e => e.patrol_start_dtm).HasColumnType("timestamp without time zone");
            entity.Property(e => e.patrol_start_location).HasColumnType("geometry(Point,4326)");
            entity.Property(e => e.patrol_status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'New'::character varying");
            entity.Property(e => e.updated_by).HasDefaultValue(0);
            entity.Property(e => e.updated_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });
        
        modelBuilder.Entity<patrol_member>(entity =>
        {
            entity.HasKey(e => e.member_id).HasName("patrol_member_pkey");

            entity.ToTable("patrol_member", "patrol");

            entity.Property(e => e.active_flag).HasDefaultValue(true);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.member_cnt_compound).HasDefaultValue(0);
            entity.Property(e => e.member_cnt_notes).HasDefaultValue(0);
            entity.Property(e => e.member_cnt_notice).HasDefaultValue(0);
            entity.Property(e => e.member_cnt_seizure).HasDefaultValue(0);
            entity.Property(e => e.member_end_dtm).HasColumnType("timestamp without time zone");
            entity.Property(e => e.member_leader_flag).HasDefaultValue(false);
            entity.Property(e => e.member_patrol_id).ValueGeneratedOnAdd();
            entity.Property(e => e.member_start_dtm).HasColumnType("timestamp without time zone");
            entity.Property(e => e.member_username).HasMaxLength(50);
            entity.Property(e => e.update_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.updated_by).HasDefaultValue(0);
        });

        modelBuilder.Entity<patrol_scheduler>(entity =>
        {
            entity.HasKey(e => e.scheduler_id).HasName("patrol_scheduler_pkey");

            entity.ToTable("patrol_scheduler", "patrol");

            entity.Property(e => e.active_flag).HasDefaultValue(true);
            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.update_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.scheduler_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.scheduler_location).HasMaxLength(255);
            entity.Property(e => e.scheduler_officer).HasMaxLength(50);
            entity.Property(e => e.updated_by).HasDefaultValue(0);
        });

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
                .HasComment("Timestamp indicating when the row was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User ID of the creator ");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the row has been logically deleted (soft deleted). 0 represents not deleted, and 1 represents deleted.");
            entity.Property(e => e.menu_id).HasComment("Identifier for the feature to which the core.permission applies.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the user record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User ID of the modifier");
            entity.Property(e => e.role_id).HasComment("Identifier for the role associated with the core.permission.");
        });

        modelBuilder.Entity<user_account>(entity =>
        {
            entity.HasKey(e => e.ua_user_id).HasName("user_accounts_pkey");

            entity.ToTable("user_accounts", "core", tb => tb.HasComment("The \"user_accounts\" table stores information about user accounts in the system. Each record represents a user account and includes details such as the user's full name, date of birth, identification information, address, nationality, marital status, and acceptance of terms and conditions."));

            entity.Property(e => e.ua_user_id)
                .ValueGeneratedNever()
                .HasComment("This column serves as a unique identifier for each user account and is a foreign key referencing the user associated with the account.");
            entity.Property(e => e.accept_terms1)
                .HasDefaultValue(true)
                .HasComment("Flag indicating whether the user accepted the first set of terms and conditions during registration.");
            entity.Property(e => e.accept_terms2)
                .HasDefaultValue(true)
                .HasComment("Flag indicating whether the user accepted the second set of terms and conditions during registration (if applicable).");
            entity.Property(e => e.accept_terms3)
                .HasDefaultValue(true)
                .HasComment("Flag indicating whether the user accepted the third set of terms and conditions during registration (if applicable).");
            entity.Property(e => e.addr_line1)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasComment("First line of the user's address.");
            entity.Property(e => e.addr_line2)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasComment("Second line of the user's address (optional).");
            entity.Property(e => e.country_id).HasComment("References the country of the user's address.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the user account was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User ID of the creator who created the account.");
            entity.Property(e => e.district_id).HasComment("References the district of the user's address.");
            entity.Property(e => e.dob).HasComment("Date of birth of the user. Could be extracted from the identification document (e.g., MyCard).");
            entity.Property(e => e.gen_id).HasComment("User gender (e.g., male, female).");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the row has been logically deleted (soft deleted). True indicates deleted, false indicates active.");
            entity.Property(e => e.is_married)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the user is married (true for married, false for single).");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the user account was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User ID of the modifier who last updated the account.");
            entity.Property(e => e.nat_id).HasComment("National Identification Number of the user.");
            entity.Property(e => e.postcode)
                .HasMaxLength(20)
                .HasDefaultValueSql("NULL::bpchar")
                .IsFixedLength()
                .HasComment("Postal code of the user's address.");
            entity.Property(e => e.race_id).HasComment("References the race/ethnicity of the user, as identified in the ref_races table.");
            entity.Property(e => e.state_id).HasComment("References the state of the user's address.");
            entity.Property(e => e.town_id).HasComment("References the town of the user's address.");
            entity.Property(e => e.ua_name)
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL::character varying")
                .HasComment("Full name of the user.");
            entity.Property(e => e.ua_photo_url)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasComment("Name of the user's account photo file location/path.");
            entity.Property(e => e.ua_signature_url)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasComment("Name of the user's account signature file location/path.");
        });

        modelBuilder.Entity<user_profile>(entity =>
        {
            entity.HasKey(e => e.profile_id).HasName("user_profile_pkey");

            entity.ToTable("user_profile", "users");

            entity.Property(e => e.created_by).HasDefaultValue(0);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.profile_accept_term1).HasColumnType("char");
            entity.Property(e => e.profile_accept_term2).HasColumnType("char");
            entity.Property(e => e.profile_accept_term3).HasColumnType("char");
            entity.Property(e => e.profile_email).HasMaxLength(150);
            entity.Property(e => e.profile_icno)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.profile_last_login).HasColumnType("timestamp without time zone");
            entity.Property(e => e.profile_name).HasMaxLength(50);
            entity.Property(e => e.profile_photo_filename).HasColumnType("character varying");
            entity.Property(e => e.profile_postcode).HasMaxLength(1);
            entity.Property(e => e.profile_signature_filename).HasColumnType("character varying");
            entity.Property(e => e.profile_status).HasMaxLength(30);
            entity.Property(e => e.profile_tel_no).HasMaxLength(150);
            entity.Property(e => e.profile_user_id).HasMaxLength(50);
            entity.Property(e => e.updated_by).HasDefaultValue(0);
            entity.Property(e => e.updated_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
