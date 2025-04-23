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

    public virtual DbSet<app_email_tmpl> app_email_tmpls { get; set; }

    public virtual DbSet<app_form_field> app_form_fields { get; set; }

    public virtual DbSet<app_system_msg> app_system_msgs { get; set; }

    public virtual DbSet<app_system_param> app_system_params { get; set; }

    public virtual DbSet<his_email_history> his_email_histories { get; set; }

    public virtual DbSet<menu> menus { get; set; }

    public virtual DbSet<mst_country> mst_countries { get; set; }

    public virtual DbSet<mst_district> mst_districts { get; set; }

    public virtual DbSet<mst_state> mst_states { get; set; }

    public virtual DbSet<mst_town> mst_towns { get; set; }

    public virtual DbSet<permission> permissions { get; set; }

    //public virtual DbSet<ref_department> ref_departments { get; set; }

    //public virtual DbSet<ref_division> ref_divisions { get; set; }

    public virtual DbSet<ref_law_act> ref_law_acts { get; set; }

    public virtual DbSet<ref_law_offense> ref_law_offenses { get; set; }

    public virtual DbSet<ref_law_section> ref_law_sections { get; set; }

    public virtual DbSet<ref_law_uuk> ref_law_uuks { get; set; }

    //public virtual DbSet<ref_unit> ref_units { get; set; }

    public virtual DbSet<ref_id_type> ref_id_types { get; set; }

    public virtual DbSet<ref_race> ref_races { get; set; }

    public virtual DbSet<ref_nationality> ref_nationalities { get; set; }

    public virtual DbSet<ref_gender> ref_genders { get; set; }

    public virtual DbSet<contact_us> contact_us { get; set; }

    public virtual DbSet<tenant> tenants { get; set; }

    public virtual DbSet<trn_email_queue> trn_email_queues { get; set; }

    public virtual DbSet<ref_faq> ref_faqs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<auditlog_archive_info>(entity =>
        {
            entity.HasKey(e => e.archive_id).HasName("auditlog_archive_info_pkey");

            entity.ToTable("auditlog_archive_info", "core");

            entity.Property(e => e.archive_id).HasDefaultValueSql("nextval('core.archive_info_archive_id_seq'::regclass)");
            entity.Property(e => e.archive_method).HasMaxLength(255);
            entity.Property(e => e.archive_module_name)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.archive_username).HasMaxLength(25);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.is_archived).HasDefaultValue(true);
            entity.Property(e => e.role_id).HasDefaultValue(0);
        });

        modelBuilder.Entity<auditlog_info>(entity =>
        {
            entity.HasKey(e => e.log_id).HasName("auditlog_pkey");

            entity.ToTable("auditlog_info", "core");

            entity.Property(e => e.log_id).HasDefaultValueSql("nextval('core.auditlog_info_audit_id_seq'::regclass)");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.is_archived).HasDefaultValue(false);
            entity.Property(e => e.log_method).HasMaxLength(255);
            entity.Property(e => e.module_name)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.role_id).HasDefaultValue(0);
            entity.Property(e => e.username).HasMaxLength(25);
        });
     
        modelBuilder.Entity<app_email_tmpl>(entity =>
        {
            entity.HasKey(e => e.tmpl_id).HasName("app_email_tmpl_pkey");

            entity.ToTable("app_email_tmpl", "core", tb => tb.HasComment("Table to store email templates for various messages"));

            entity.Property(e => e.tmpl_id).HasComment("Unique identifier for each email template");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the template record was created")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created the template record");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the email template record is marked as deleted (true/false)");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the template record was last modified")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified the template record");
            entity.Property(e => e.tmpl_code)
                .HasMaxLength(50)
                .HasComment("Code representing the type of email template");
            entity.Property(e => e.tmpl_content).HasComment("Content/body of the email template");
            entity.Property(e => e.tmpl_subject)
                .HasMaxLength(255)
                .HasComment("Subject line of the email template");
        });

        modelBuilder.Entity<app_form_field>(entity =>
        {
            entity.HasKey(e => e.field_id).HasName("app_form_field_pkey");

            entity.ToTable("app_form_field", "core", tb => tb.HasComment("Table to store form templates (e.g., sitaan)"));

            entity.Property(e => e.field_id).HasComment("Unique identifier for each form field");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created the record");
            entity.Property(e => e.field_api_seeded)
                .HasDefaultValue(false)
                .HasComment("Indicates if this field is populated by an API (true/false)");
            entity.Property(e => e.field_form_type)
                .HasMaxLength(50)
                .HasComment("Type of the form that this field belongs to");
            entity.Property(e => e.field_label)
                .HasMaxLength(50)
                .HasComment("Label displayed for the form field");
            entity.Property(e => e.field_name)
                .HasMaxLength(50)
                .HasComment("Name of the form field");
            entity.Property(e => e.field_option).HasComment("Options available for this field, if applicable");
            entity.Property(e => e.field_orders)
                .HasDefaultValue(0)
                .HasComment("Order in which this field appears in the form");
            entity.Property(e => e.field_required)
                .HasDefaultValue(false)
                .HasComment("Indicates whether this field is mandatory (true/false)");
            entity.Property(e => e.field_source_url)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasComment("URL for fetching data for this field, if applicable");
            entity.Property(e => e.field_type)
                .HasMaxLength(10)
                .HasComment("Data type of the form field (e.g., text, number)");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the record is marked as deleted (true/false)");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last modified")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified the record");
        });

        modelBuilder.Entity<app_system_msg>(entity =>
        {
            entity.HasKey(e => e.message_id).HasName("app_system_msg_pkey");

            entity.ToTable("app_system_msg", "core", tb => tb.HasComment("Table to store return messages from API related to changes"));

            entity.Property(e => e.message_id).HasComment("Unique identifier for each message");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created the record");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the record is marked as deleted (true/false)");
            entity.Property(e => e.message_body).HasComment("The actual content of the message");
            entity.Property(e => e.message_code)
                .HasMaxLength(255)
                .HasComment("Code representing the type of message");
            entity.Property(e => e.message_feature)
                .HasMaxLength(50)
                .HasComment("Feature or component associated with the message");
            entity.Property(e => e.message_type)
                .HasMaxLength(1)
                .HasComment("Type of message (e.g., info, warning, error)");
            entity.Property(e => e.modified_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last modified")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified the record");
        });

        modelBuilder.Entity<app_system_param>(entity =>
        {
            entity.HasKey(e => e.param_id).HasName("app_system_param_pkey");

            entity.ToTable("app_system_param", "core", tb => tb.HasComment("Table to store configuration parameters for applications in the core layer"));

            entity.Property(e => e.param_id).HasComment("Unique identifier for each configuration parameter");
            entity.Property(e => e.app_layer)
                .HasMaxLength(10)
                .HasComment("Layer of the application (e.g., frontend, backend)");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was created")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created the record");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the record is marked as deleted (true/false)");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when the record was last modified")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified the record");
            entity.Property(e => e.param_group)
                .HasMaxLength(50)
                .HasComment("Group categorizing the parameter (e.g., database, API)");
            entity.Property(e => e.param_name)
                .HasMaxLength(50)
                .HasComment("Name of the configuration parameter");
            entity.Property(e => e.param_value)
                .HasMaxLength(1000)
                .HasComment("Value assigned to the configuration parameter");
        });

        modelBuilder.Entity<his_email_history>(entity =>
        {
            entity.HasKey(e => e.hist_id).HasName("his_email_history_pkey");

            entity.ToTable("his_email_history", "core", tb => tb.HasComment("Table to store history of emails that have been sent"));

            entity.Property(e => e.hist_id).HasComment("Unique identifier for each email history record");
            entity.Property(e => e.cnt_retry)
                .HasDefaultValue(0)
                .HasComment("Count of how many times sending this email has been retried");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record");
            entity.Property(e => e.date_sent)
                .HasComment("Timestamp when the email was sent")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.hist_content).HasComment("Content/body of the sent email");
            entity.Property(e => e.hist_recipient)
                .HasMaxLength(255)
                .HasComment("Email address of the recipient");
            entity.Property(e => e.hist_remark).HasComment("Additional remarks related to the email sending process");
            entity.Property(e => e.hist_status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'New'::character varying")
                .HasComment("Status of the email (e.g., New, Sent, Failed)");
            entity.Property(e => e.hist_subject)
                .HasMaxLength(255)
                .HasComment("Subject line of the sent email");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is marked as deleted (true/false)");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record");
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

        modelBuilder.Entity<mst_country>(entity =>
        {
            entity.HasKey(e => e.country_id).HasName("mst_countries_pkey");

            entity.ToTable("mst_countries", "core");

            entity.HasIndex(e => e.country_code, "mst_countries_country_code_key").IsUnique();

            entity.Property(e => e.country_code).HasMaxLength(10);
            entity.Property(e => e.country_name).HasMaxLength(50);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<mst_district>(entity =>
        {
            entity.HasKey(e => e.district_id).HasName("mst_districts_pkey");

            entity.ToTable("mst_districts", "core");

            entity.HasIndex(e => new { e.district_code, e.state_code }, "unique_district_code_state_code").IsUnique();

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.district_code).HasMaxLength(10);
            entity.Property(e => e.district_name).HasMaxLength(50);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.state_code).HasMaxLength(10);
        });

        modelBuilder.Entity<mst_state>(entity =>
        {
            entity.HasKey(e => e.state_id).HasName("mst_states_pkey");

            entity.ToTable("mst_states", "core");

            entity.HasIndex(e => new { e.state_code, e.country_code }, "unique_state_code_country_code").IsUnique();

            entity.Property(e => e.country_code).HasMaxLength(10);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.state_code).HasMaxLength(10);
            entity.Property(e => e.state_name).HasMaxLength(50);
        });

        modelBuilder.Entity<mst_town>(entity =>
        {
            entity.HasKey(e => e.town_id).HasName("mst_towns_pkey");

            entity.ToTable("mst_towns", "core");

            entity.HasIndex(e => new { e.town_code, e.district_code }, "unique_town_code_district_code").IsUnique();

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.district_code).HasMaxLength(10);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.town_code).HasMaxLength(10);
            entity.Property(e => e.town_name).HasMaxLength(50);
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

        modelBuilder.Entity<ref_law_act>(entity =>
        {
            entity.HasKey(e => e.act_id).HasName("ref_law_acts_pkey");

            entity.ToTable("ref_law_acts", "core");

            entity.Property(e => e.act_code).HasMaxLength(20);
            entity.Property(e => e.act_name).HasMaxLength(40);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<ref_law_offense>(entity =>
        {
            entity.HasKey(e => e.offense_id).HasName("ref_law_offenses_pkey");

            entity.ToTable("ref_law_offenses", "core");

            entity.Property(e => e.act_code).HasMaxLength(20);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.offense_code).HasMaxLength(20);
            entity.Property(e => e.offense_name).HasMaxLength(40);
            entity.Property(e => e.section_code).HasMaxLength(20);
            entity.Property(e => e.uuk_code).HasMaxLength(20);
        });

        modelBuilder.Entity<ref_law_section>(entity =>
        {
            entity.HasKey(e => e.section_id).HasName("ref_law_sections_pkey");

            entity.ToTable("ref_law_sections", "core");

            entity.Property(e => e.act_code).HasMaxLength(20);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.section_code).HasMaxLength(20);
            entity.Property(e => e.section_name).HasMaxLength(40);
        });

        modelBuilder.Entity<ref_law_uuk>(entity =>
        {
            entity.HasKey(e => e.uuk_id).HasName("ref_law_uuks_pkey");

            entity.ToTable("ref_law_uuks", "core");

            entity.Property(e => e.act_code).HasMaxLength(20);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.section_code).HasMaxLength(20);
            entity.Property(e => e.uuk_code).HasMaxLength(20);
            entity.Property(e => e.uuk_name).HasMaxLength(40);
        });

        modelBuilder.Entity<ref_id_type>(entity =>
        {
            entity.HasKey(e => e.id_type_id).HasName("ref_id_types_pkey");

            entity.ToTable("ref_id_types", "core", tb => tb.HasComment("The ucIDTypes is reference table to store type of ID such as IC number, passpord etc."));

            entity.Property(e => e.id_type_id).HasComment("Type of ID");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the row was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User ID of the creator ");
            entity.Property(e => e.id_type_name)
                .HasMaxLength(50)
                .HasComment("User's unique username used for authentication.");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the row has been logically deleted (soft deleted). 0 represents not deleted, and 1 represents deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the user record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User ID of the modifier");
        });

        modelBuilder.Entity<ref_race>(entity =>
        {
            entity.HasKey(e => e.race_id).HasName("pk_races");

            entity.ToTable("ref_races", "core", tb => tb.HasComment("List of races (biological)"));

            entity.Property(e => e.race_id)
                .HasDefaultValueSql("nextval('core.races_race_id_seq'::regclass)")
                .HasComment("Primary key serves as the table's unique identifier.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the row was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User ID of the creator ");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the row has been logically deleted (soft deleted). 0 represents not deleted, and 1 represents deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the user record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User ID of the modifier");
            entity.Property(e => e.race_name)
                .HasMaxLength(50)
                .HasComment("Name");
        });

        modelBuilder.Entity<ref_nationality>(entity =>
        {
            entity.HasKey(e => e.nat_id).HasName("pk_nationalities");

            entity.ToTable("ref_nationalities", "core", tb => tb.HasComment("Nationality of the user."));

            entity.Property(e => e.nat_id)
                .HasDefaultValueSql("nextval('core.nationalities_nat_id_seq'::regclass)")
                .HasComment("A n identifier for each nationality, which serves as the primary key of the table.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the row was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User ID of the creator ");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the row has been logically deleted (soft deleted). 0 represents not deleted, and 1 represents deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the user record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User ID of the modifier");
            entity.Property(e => e.nat_name)
                .HasMaxLength(50)
                .HasComment("The name of the nationality");
        });

        modelBuilder.Entity<ref_gender>(entity =>
        {
            entity.HasKey(e => e.gen_id).HasName("pk_gender");

            entity.ToTable("ref_gender", "core", tb => tb.HasComment("core.gender reference Male & Female"));

            entity.Property(e => e.gen_id)
                .HasDefaultValueSql("nextval('core.gender_gen_id_seq'::regclass)")
                .HasComment("User's unique username used for authentication.");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the row was created.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasComment("User ID of the creator ");
            entity.Property(e => e.gen_name)
                .HasMaxLength(50)
                .HasComment("Gendel Name");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether the row has been logically deleted (soft deleted). 0 represents not deleted, and 1 represents deleted.");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when the user record was last modified.")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasComment("User ID of the modifier");
        });

        modelBuilder.Entity<contact_us>(entity =>
        {
            entity.HasKey(e => e.contact_id).HasName("contact_us_pkey");

            entity.ToTable("contact_us", "core");

            entity.Property(e => e.contact_id)
                .HasDefaultValueSql("nextval('core.contact_us_contact_id_seq'::regclass)");
            entity.Property(e => e.contact_email).HasColumnType("character varying");
            entity.Property(e => e.contact_inq_no).HasColumnType("character varying");
            entity.Property(e => e.contact_message).HasColumnType("character varying");
            entity.Property(e => e.contact_name).HasColumnType("character varying");
            entity.Property(e => e.contact_subject).HasColumnType("character varying");
            entity.Property(e => e.contact_telno).HasColumnType("character varying");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.contact_status).HasColumnType("character varying");
            entity.Property(e => e.response_message).HasColumnType("character varying");
            entity.Property(e => e.creator_id).HasComment("User ID of the creator ");
            entity.Property(e => e.modifier_id).HasComment("User ID of the modifier");
        });

        modelBuilder.Entity<tenant>(entity =>
        {
            entity.HasKey(e => e.tenant_id).HasName("tenants_pkey");

            entity.ToTable("tenants", "core");

            entity.Property(e => e.addr_line1).HasMaxLength(50);
            entity.Property(e => e.addr_line2).HasMaxLength(50);
            entity.Property(e => e.confirm_website_link).HasMaxLength(50);
            entity.Property(e => e.connection_string).HasMaxLength(50);
            entity.Property(e => e.contact_name).HasMaxLength(50);
            entity.Property(e => e.country_code).HasMaxLength(10);
            entity.Property(e => e.created_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.district_code).HasMaxLength(10);
            entity.Property(e => e.modified_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.phone_number).HasMaxLength(20);
            entity.Property(e => e.postcode).HasMaxLength(20);
            entity.Property(e => e.recipe_name).HasMaxLength(50);
            entity.Property(e => e.schema_name).HasMaxLength(50);
            entity.Property(e => e.site_name).HasMaxLength(50);
            entity.Property(e => e.state_code).HasMaxLength(10);
            entity.Property(e => e.table_prefix).HasMaxLength(50);
            entity.Property(e => e.tn_doc_filename).HasMaxLength(255);
            entity.Property(e => e.tn_doc_url).HasMaxLength(255);
            entity.Property(e => e.tn_email).HasMaxLength(50);
            entity.Property(e => e.tn_handle).HasMaxLength(50);
            entity.Property(e => e.tn_name).HasMaxLength(50);
            entity.Property(e => e.tn_photo_filename).HasMaxLength(255);
            entity.Property(e => e.tn_photo_url).HasMaxLength(255);
            entity.Property(e => e.town_code).HasMaxLength(10);
            entity.Property(e => e.url_prefix).HasMaxLength(50);
            entity.Property(e => e.website_link).HasMaxLength(50);
        });

        modelBuilder.Entity<trn_email_queue>(entity =>
        {
            entity.HasKey(e => e.queue_id).HasName("trn_email_queue_pkey");

            entity.ToTable("trn_email_queue", "core", tb => tb.HasComment("Table to manage a queue for sending emails"));

            entity.Property(e => e.queue_id).HasComment("Unique identifier for each email queue record");
            entity.Property(e => e.cnt_retry)
                .HasDefaultValue(0)
                .HasComment("Count of how many times sending this email has been retried");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was created")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who created this record");
            entity.Property(e => e.date_sent)
                .HasComment("Timestamp when the email was sent or scheduled to be sent")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_deleted)
                .HasDefaultValue(false)
                .HasComment("Flag indicating whether this record is marked as deleted (true/false)");
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp when this record was last modified")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id)
                .HasDefaultValue(0)
                .HasComment("ID of the user who last modified this record");
            entity.Property(e => e.queue_content).HasComment("Content/body of the email in the queue");
            entity.Property(e => e.queue_recipient)
                .HasMaxLength(255)
                .HasComment("Email address of the recipient in the queue");
            entity.Property(e => e.queue_remark).HasComment("Additional remarks related to this email queue entry");
            entity.Property(e => e.queue_status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'New'::character varying")
                .HasComment("Current status of the email in the queue (e.g., New, Sent, Failed)");
            entity.Property(e => e.queue_subject)
                .HasMaxLength(255)
                .HasComment("Subject line of the email in the queue");
        });

        modelBuilder.Entity<ref_faq>(entity =>
        {
            entity.HasKey(e => e.faq_id).HasName("faq_info_pkey");

            entity.ToTable("ref_faq", "core");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.creator_id).HasDefaultValue(0);
            entity.Property(e => e.faq_category).HasMaxLength(50);
            entity.Property(e => e.faq_status).HasMaxLength(30);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.modified_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.modifier_id).HasDefaultValue(0);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
