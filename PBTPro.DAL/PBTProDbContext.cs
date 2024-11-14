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

    public virtual DbSet<CompoundAct> CompoundActs { get; set; }

    public virtual DbSet<CompoundInfo> CompoundInfos { get; set; }

    public virtual DbSet<CompoundLocation> CompoundLocations { get; set; }

    public virtual DbSet<CompoundMedium> CompoundMedia { get; set; }

    public virtual DbSet<CompoundOfficer> CompoundOfficers { get; set; }

    public virtual DbSet<LicenseAddressSwap> LicenseAddressSwaps { get; set; }

    public virtual DbSet<LicenseHistory> LicenseHistories { get; set; }

    public virtual DbSet<LicenseHolder> LicenseHolders { get; set; }

    public virtual DbSet<LicenseInformation> LicenseInformations { get; set; }

    public virtual DbSet<LicenseMedium> LicenseMedia { get; set; }

    public virtual DbSet<LicenseTax> LicenseTaxes { get; set; }

    public virtual DbSet<LicenseTransaction> LicenseTransactions { get; set; }

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

        modelBuilder.Entity<CompoundAct>(entity =>
        {
            entity.HasKey(e => e.ActId).HasName("compound_act_pkey");

            entity.ToTable("compound_act", "compound");

            entity.Property(e => e.ActId).HasColumnName("act_id");
            entity.Property(e => e.ActAmount1)
                .HasPrecision(15, 2)
                .HasColumnName("act_amount1");
            entity.Property(e => e.ActAmount2)
                .HasPrecision(15, 2)
                .HasColumnName("act_amount2");
            entity.Property(e => e.ActAmount3)
                .HasPrecision(15, 2)
                .HasColumnName("act_amount3");
            entity.Property(e => e.ActCode)
                .HasMaxLength(30)
                .HasColumnName("act_code");
            entity.Property(e => e.ActCourtAmount)
                .HasPrecision(15, 2)
                .HasColumnName("act_court_amount");
            entity.Property(e => e.ActDeptCode)
                .HasMaxLength(30)
                .HasColumnName("act_dept_code");
            entity.Property(e => e.ActDeptName)
                .HasMaxLength(250)
                .HasColumnName("act_dept_name");
            entity.Property(e => e.ActFnoticeAmount)
                .HasPrecision(15, 2)
                .HasColumnName("act_fnotice_amount");
            entity.Property(e => e.ActFnoticePeriod)
                .HasPrecision(5)
                .HasColumnName("act_fnotice_period");
            entity.Property(e => e.ActName)
                .HasMaxLength(500)
                .HasColumnName("act_name");
            entity.Property(e => e.ActNoticeAmount)
                .HasPrecision(15, 2)
                .HasColumnName("act_notice_amount");
            entity.Property(e => e.ActNoticePeriod)
                .HasPrecision(5)
                .HasColumnName("act_notice_period");
            entity.Property(e => e.ActOffenceCode)
                .HasMaxLength(30)
                .HasColumnName("act_offence_code");
            entity.Property(e => e.ActOffenceName)
                .HasMaxLength(500)
                .HasColumnName("act_offence_name");
            entity.Property(e => e.ActPbtCode)
                .HasPrecision(5)
                .HasColumnName("act_pbt_code");
            entity.Property(e => e.ActPeriod1)
                .HasPrecision(5)
                .HasColumnName("act_period1");
            entity.Property(e => e.ActPeriod2)
                .HasPrecision(5)
                .HasColumnName("act_period2");
            entity.Property(e => e.ActPeriod3)
                .HasPrecision(5)
                .HasColumnName("act_period3");
            entity.Property(e => e.ActTransactionCode)
                .HasMaxLength(30)
                .HasColumnName("act_transaction_code");
            entity.Property(e => e.ActTransactionName)
                .HasMaxLength(500)
                .HasColumnName("act_transaction_name");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<CompoundInfo>(entity =>
        {
            entity.HasKey(e => e.CompoundId).HasName("compound_info_pkey");

            entity.ToTable("compound_info", "compound");

            entity.Property(e => e.CompoundId).HasColumnName("compound_id");
            entity.Property(e => e.CompoundActCode)
                .HasMaxLength(30)
                .HasColumnName("compound_act_code");
            entity.Property(e => e.CompoundAmount)
                .HasPrecision(15, 2)
                .HasColumnName("compound_amount");
            entity.Property(e => e.CompoundCourtDate).HasColumnName("compound_court_date");
            entity.Property(e => e.CompoundDate).HasColumnName("compound_date");
            entity.Property(e => e.CompoundDesc)
                .HasMaxLength(500)
                .HasColumnName("compound_desc");
            entity.Property(e => e.CompoundLicenseNo)
                .HasMaxLength(50)
                .HasColumnName("compound_license_no");
            entity.Property(e => e.CompoundNo)
                .HasMaxLength(30)
                .HasColumnName("compound_no");
            entity.Property(e => e.CompoundOffenceCode)
                .HasMaxLength(30)
                .HasColumnName("compound_offence_code");
            entity.Property(e => e.CompoundOffenderAddr1)
                .HasMaxLength(100)
                .HasColumnName("compound_offender_addr1");
            entity.Property(e => e.CompoundOffenderAddr2)
                .HasMaxLength(150)
                .HasColumnName("compound_offender_addr2");
            entity.Property(e => e.CompoundOffenderAddr3)
                .HasMaxLength(150)
                .HasColumnName("compound_offender_addr3");
            entity.Property(e => e.CompoundOffenderArea)
                .HasMaxLength(50)
                .HasColumnName("compound_offender_area");
            entity.Property(e => e.CompoundOffenderId)
                .HasMaxLength(30)
                .HasColumnName("compound_offender_id");
            entity.Property(e => e.CompoundOffenderName)
                .HasMaxLength(250)
                .HasColumnName("compound_offender_name");
            entity.Property(e => e.CompoundOffenderPcode)
                .HasPrecision(5)
                .HasColumnName("compound_offender_pcode");
            entity.Property(e => e.CompoundOffenderState)
                .HasMaxLength(30)
                .HasColumnName("compound_offender_state");
            entity.Property(e => e.CompoundOfficerCode)
                .HasMaxLength(30)
                .HasColumnName("compound_officer_code");
            entity.Property(e => e.CompoundPayAmount)
                .HasPrecision(15, 2)
                .HasColumnName("compound_pay_amount");
            entity.Property(e => e.CompoundPayDate).HasColumnName("compound_pay_date");
            entity.Property(e => e.CompoundPayStatus)
                .HasMaxLength(30)
                .HasColumnName("compound_pay_status");
            entity.Property(e => e.CompoundPbtCode)
                .HasPrecision(5)
                .HasColumnName("compound_pbt_code");
            entity.Property(e => e.CompoundRoadTax)
                .HasMaxLength(30)
                .HasColumnName("compound_road_tax");
            entity.Property(e => e.CompoundStatus)
                .HasMaxLength(30)
                .HasColumnName("compound_status");
            entity.Property(e => e.CompoundTransCode)
                .HasMaxLength(30)
                .HasColumnName("compound_trans_code");
            entity.Property(e => e.CompoundVehicleBrand)
                .HasMaxLength(50)
                .HasColumnName("compound_vehicle_brand");
            entity.Property(e => e.CompoundVehicleModel)
                .HasMaxLength(50)
                .HasColumnName("compound_vehicle_model");
            entity.Property(e => e.CompoundVehiclePlate)
                .HasMaxLength(10)
                .HasColumnName("compound_vehicle_plate");
            entity.Property(e => e.CompoundVehicleType)
                .HasMaxLength(30)
                .HasColumnName("compound_vehicle_type");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<CompoundLocation>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("compound_location_pkey");

            entity.ToTable("compound_location", "compound");

            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.LocationCompId).HasColumnName("location_comp_id");
            entity.Property(e => e.LocationCompNo)
                .HasMaxLength(30)
                .HasColumnName("location_comp_no");
            entity.Property(e => e.LocationLatitude)
                .HasMaxLength(100)
                .HasColumnName("location_latitude");
            entity.Property(e => e.LocationLongitude)
                .HasMaxLength(100)
                .HasColumnName("location_longitude");
            entity.Property(e => e.LocationPbtCode)
                .HasPrecision(5)
                .HasColumnName("location_pbt_code");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<CompoundMedium>(entity =>
        {
            entity.HasKey(e => e.MediaCompId).HasName("compound_media_pkey");

            entity.ToTable("compound_media", "compound");

            entity.Property(e => e.MediaCompId).HasColumnName("media_comp_id");
            entity.Property(e => e.MediaCompIdno).HasColumnName("media_comp_idno");
            entity.Property(e => e.MediaCompNo)
                .HasMaxLength(30)
                .HasColumnName("media_comp_no");
            entity.Property(e => e.MediaPbtCode)
                .HasPrecision(5)
                .HasColumnName("media_pbt_code");
            entity.Property(e => e.MediaUrlLink)
                .HasMaxLength(500)
                .HasColumnName("media_url_link");
        });

        modelBuilder.Entity<CompoundOfficer>(entity =>
        {
            entity.HasKey(e => e.OfficerId).HasName("compound_officer_pkey");

            entity.ToTable("compound_officer", "compound");

            entity.Property(e => e.OfficerId).HasColumnName("officer_id");
            entity.Property(e => e.OfficerDept)
                .HasMaxLength(30)
                .HasColumnName("officer_dept");
            entity.Property(e => e.OfficerGrade)
                .HasMaxLength(30)
                .HasColumnName("officer_grade");
            entity.Property(e => e.OfficerGradeDesc)
                .HasMaxLength(150)
                .HasColumnName("officer_grade_desc");
            entity.Property(e => e.OfficerName)
                .HasMaxLength(250)
                .HasColumnName("officer_name");
            entity.Property(e => e.OfficerPbtCode)
                .HasPrecision(5)
                .HasColumnName("officer_pbt_code");
            entity.Property(e => e.OfficerSerial)
                .HasMaxLength(30)
                .HasColumnName("officer_serial");
        });

        modelBuilder.Entity<LicenseAddressSwap>(entity =>
        {
            entity.HasKey(e => e.SwapLicenseId).HasName("license_address_swap_pkey");

            entity.ToTable("license_address_swap", "license", tb => tb.HasComment("TABLE BAGI MENYIMPAN ALAMAT LESEN SEDIA ADA DAN BARU"));

            entity.Property(e => e.SwapLicenseId).HasColumnName("swap_license_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.SwapCurrentAddr1)
                .HasMaxLength(100)
                .HasColumnName("swap_current_addr1");
            entity.Property(e => e.SwapCurrentAddr2)
                .HasMaxLength(150)
                .HasColumnName("swap_current_addr2");
            entity.Property(e => e.SwapCurrentAddr3)
                .HasMaxLength(150)
                .HasColumnName("swap_current_addr3");
            entity.Property(e => e.SwapCurrentArea)
                .HasMaxLength(50)
                .HasColumnName("swap_current_area");
            entity.Property(e => e.SwapCurrentPcode)
                .HasPrecision(5)
                .HasColumnName("swap_current_pcode");
            entity.Property(e => e.SwapCurrentState)
                .HasMaxLength(30)
                .HasColumnName("swap_current_state");
            entity.Property(e => e.SwapIdInfo).HasColumnName("swap_id_info");
            entity.Property(e => e.SwapLicenseAccount)
                .HasMaxLength(20)
                .HasColumnName("swap_license_account");
            entity.Property(e => e.SwapNewAddr1)
                .HasMaxLength(100)
                .HasColumnName("swap_new_addr1");
            entity.Property(e => e.SwapNewAddr2)
                .HasMaxLength(150)
                .HasColumnName("swap_new_addr2");
            entity.Property(e => e.SwapNewAddr3)
                .HasMaxLength(150)
                .HasColumnName("swap_new_addr3");
            entity.Property(e => e.SwapNewArea)
                .HasMaxLength(50)
                .HasColumnName("swap_new_area");
            entity.Property(e => e.SwapNewPcode)
                .HasPrecision(5)
                .HasColumnName("swap_new_pcode");
            entity.Property(e => e.SwapNewState)
                .HasMaxLength(30)
                .HasColumnName("swap_new_state");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.SwapIdInfoNavigation).WithMany(p => p.LicenseAddressSwaps)
                .HasForeignKey(d => d.SwapIdInfo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("swap_id_info");
        });

        modelBuilder.Entity<LicenseHistory>(entity =>
        {
            entity.HasKey(e => e.LicenseHistId).HasName("license_history_pkey");

            entity.ToTable("license_history", "license", tb => tb.HasComment("TABLE SEJARAH LESEN"));

            entity.Property(e => e.LicenseHistId).HasColumnName("license_hist_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.LastUpdated)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_updated");
            entity.Property(e => e.LicenseHistAccount)
                .HasMaxLength(20)
                .HasColumnName("license_hist_account");
            entity.Property(e => e.LicenseHistAddr1)
                .HasMaxLength(100)
                .HasColumnName("license_hist_addr1");
            entity.Property(e => e.LicenseHistAddr2)
                .HasMaxLength(150)
                .HasColumnName("license_hist_addr2");
            entity.Property(e => e.LicenseHistAddr3)
                .HasMaxLength(150)
                .HasColumnName("license_hist_addr3");
            entity.Property(e => e.LicenseHistArea)
                .HasMaxLength(50)
                .HasColumnName("license_hist_area");
            entity.Property(e => e.LicenseHistEndd).HasColumnName("license_hist_endd");
            entity.Property(e => e.LicenseHistHolder)
                .HasMaxLength(20)
                .HasColumnName("license_hist_holder");
            entity.Property(e => e.LicenseHistPcode)
                .HasPrecision(5)
                .HasColumnName("license_hist_pcode");
            entity.Property(e => e.LicenseHistStartd).HasColumnName("license_hist_startd");
            entity.Property(e => e.LicenseHistState)
                .HasMaxLength(30)
                .HasColumnName("license_hist_state");
        });

        modelBuilder.Entity<LicenseHolder>(entity =>
        {
            entity.HasKey(e => e.LicenseHolderId).HasName("license_holder_pkey");

            entity.ToTable("license_holder", "license", tb => tb.HasComment("TABLE MAKLUMAT PELESEN"));

            entity.Property(e => e.LicenseHolderId).HasColumnName("license_holder_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.LicenseHolderAccount)
                .HasMaxLength(20)
                .HasColumnName("license_holder_account");
            entity.Property(e => e.LicenseHolderAddr1)
                .HasMaxLength(100)
                .HasColumnName("license_holder_addr1");
            entity.Property(e => e.LicenseHolderAddr2)
                .HasMaxLength(150)
                .HasColumnName("license_holder_addr2");
            entity.Property(e => e.LicenseHolderAddr3)
                .HasMaxLength(150)
                .HasColumnName("license_holder_addr3");
            entity.Property(e => e.LicenseHolderArea)
                .HasMaxLength(50)
                .HasColumnName("license_holder_area");
            entity.Property(e => e.LicenseHolderCustid)
                .HasMaxLength(50)
                .HasColumnName("license_holder_custid");
            entity.Property(e => e.LicenseHolderEmail)
                .HasMaxLength(100)
                .HasColumnName("license_holder_email");
            entity.Property(e => e.LicenseHolderInfo).HasColumnName("license_holder_info");
            entity.Property(e => e.LicenseHolderName)
                .HasMaxLength(250)
                .HasColumnName("license_holder_name");
            entity.Property(e => e.LicenseHolderPcode)
                .HasPrecision(5)
                .HasColumnName("license_holder_pcode");
            entity.Property(e => e.LicenseHolderPhone)
                .HasPrecision(20)
                .HasColumnName("license_holder_phone");
            entity.Property(e => e.LicenseHolderState)
                .HasMaxLength(30)
                .IsFixedLength()
                .HasColumnName("license_holder_state");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(10)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.LicenseHolderInfoNavigation).WithMany(p => p.LicenseHolders)
                .HasForeignKey(d => d.LicenseHolderInfo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("license_holder_info");
        });

        modelBuilder.Entity<LicenseInformation>(entity =>
        {
            entity.HasKey(e => e.LicenseId).HasName("license_information_pkey");

            entity.ToTable("license_information", "license", tb => tb.HasComment("TABLE BAGI MAKLUMAT LESEN"));

            entity.HasIndex(e => e.LicenseId, "index_license_id")
                .IsUnique()
                .HasAnnotation("Npgsql:StorageParameter:deduplicate_items", "false");

            entity.Property(e => e.LicenseId).HasColumnName("license_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.LicenseAccountNumber)
                .HasMaxLength(50)
                .HasColumnName("license_account_number");
            entity.Property(e => e.LicenseAmount)
                .HasPrecision(15, 2)
                .HasColumnName("license_amount");
            entity.Property(e => e.LicenseAmountBalance)
                .HasPrecision(15, 2)
                .HasColumnName("license_amount_balance");
            entity.Property(e => e.LicenseApplyDate).HasColumnName("license_apply_date");
            entity.Property(e => e.LicenseApprovedDate).HasColumnName("license_approved_date");
            entity.Property(e => e.LicenseBusinessAddr1)
                .HasMaxLength(100)
                .HasColumnName("license_business_addr1");
            entity.Property(e => e.LicenseBusinessAddr2)
                .HasMaxLength(150)
                .HasColumnName("license_business_addr2");
            entity.Property(e => e.LicenseBusinessAddr3)
                .HasMaxLength(150)
                .HasColumnName("license_business_addr3");
            entity.Property(e => e.LicenseBusinessArea)
                .HasMaxLength(50)
                .HasColumnName("license_business_area");
            entity.Property(e => e.LicenseBusinessName)
                .HasMaxLength(250)
                .HasColumnName("license_business_name");
            entity.Property(e => e.LicenseBusinessPcode)
                .HasPrecision(5)
                .HasColumnName("license_business_pcode");
            entity.Property(e => e.LicenseBusinessState)
                .HasMaxLength(30)
                .HasColumnName("license_business_state");
            entity.Property(e => e.LicenseEndDate).HasColumnName("license_end_date");
            entity.Property(e => e.LicenseLatitude)
                .HasMaxLength(250)
                .HasColumnName("license_latitude");
            entity.Property(e => e.LicenseLongitud)
                .HasMaxLength(250)
                .HasColumnName("license_longitud");
            entity.Property(e => e.LicensePaymentStatus)
                .HasMaxLength(30)
                .HasColumnName("license_payment_status");
            entity.Property(e => e.LicensePbtOrigin)
                .HasPrecision(5)
                .HasColumnName("license_pbt_origin");
            entity.Property(e => e.LicensePeriod).HasColumnName("license_period");
            entity.Property(e => e.LicensePeriodStatus)
                .HasMaxLength(30)
                .HasColumnName("license_period_status");
            entity.Property(e => e.LicenseRiskStatus)
                .HasMaxLength(30)
                .HasColumnName("license_risk_status");
            entity.Property(e => e.LicenseStartDate).HasColumnName("license_start_date");
            entity.Property(e => e.LicenseStatus)
                .HasMaxLength(20)
                .HasColumnName("license_status");
            entity.Property(e => e.LicenseType)
                .HasMaxLength(50)
                .HasColumnName("license_type");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(10)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<LicenseMedium>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("license_media_pkey");

            entity.ToTable("license_media", "license");

            entity.Property(e => e.MediaId).HasColumnName("media_id");
            entity.Property(e => e.MediaIdInfo).HasColumnName("media_id_info");
            entity.Property(e => e.MediaLicenseAccount)
                .HasMaxLength(50)
                .HasColumnName("media_license_account");
            entity.Property(e => e.MediaUrlLink)
                .HasMaxLength(500)
                .HasColumnName("media_url_link");

            entity.HasOne(d => d.MediaIdInfoNavigation).WithMany(p => p.LicenseMedia)
                .HasForeignKey(d => d.MediaIdInfo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("media_id_info");
        });

        modelBuilder.Entity<LicenseTax>(entity =>
        {
            entity.HasKey(e => e.TaxId).HasName("license_tax_pkey");

            entity.ToTable("license_tax", "license", tb => tb.HasComment("TABLE MAKLUMAT CUKAI TAKSIRAN"));

            entity.Property(e => e.TaxId).HasColumnName("tax_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.TaxLicenseAccount)
                .HasMaxLength(20)
                .HasColumnName("tax_license_account");
            entity.Property(e => e.TaxLicenseInfo).HasColumnName("tax_license_info");
            entity.Property(e => e.TaxMainAccount)
                .HasMaxLength(20)
                .HasColumnName("tax_main_account");
            entity.Property(e => e.TaxPropertyAddress1)
                .HasMaxLength(100)
                .HasColumnName("tax_property_address1");
            entity.Property(e => e.TaxPropertyAddress2)
                .HasMaxLength(250)
                .HasColumnName("tax_property_address2");
            entity.Property(e => e.TaxPropertyAmount)
                .HasPrecision(15, 2)
                .HasColumnName("tax_property_amount");
            entity.Property(e => e.TaxPropertyArea)
                .HasMaxLength(50)
                .HasColumnName("tax_property_area");
            entity.Property(e => e.TaxPropertyPcode)
                .HasPrecision(5)
                .HasColumnName("tax_property_pcode");
            entity.Property(e => e.TaxPropertyState)
                .HasMaxLength(50)
                .HasColumnName("tax_property_state");
            entity.Property(e => e.TaxPropertyStatus)
                .HasMaxLength(1)
                .HasColumnName("tax_property_status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(10)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.TaxLicenseInfoNavigation).WithMany(p => p.LicenseTaxes)
                .HasForeignKey(d => d.TaxLicenseInfo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tax_license_info");
        });

        modelBuilder.Entity<LicenseTransaction>(entity =>
        {
            entity.HasKey(e => e.LicenseTransId).HasName("license_transaction_pkey");

            entity.ToTable("license_transaction", "license", tb => tb.HasComment("TABLE TRANSAKSI LESEN"));

            entity.Property(e => e.LicenseTransId).HasColumnName("license_trans_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.LicenseTransAccount)
                .HasMaxLength(20)
                .HasColumnName("license_trans_account");
            entity.Property(e => e.LicenseTransAmount)
                .HasPrecision(15, 2)
                .HasColumnName("license_trans_amount");
            entity.Property(e => e.LicenseTransCode)
                .HasMaxLength(50)
                .HasColumnName("license_trans_code");
            entity.Property(e => e.LicenseTransInfo).HasColumnName("license_trans_info");
            entity.Property(e => e.LicenseTransName)
                .HasMaxLength(250)
                .HasColumnName("license_trans_name");
            entity.Property(e => e.LicenseTransStatus)
                .HasMaxLength(1)
                .HasColumnName("license_trans_status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(10)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.LicenseTransInfoNavigation).WithMany(p => p.LicenseTransactions)
                .HasForeignKey(d => d.LicenseTransInfo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("license_trans_info");
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
