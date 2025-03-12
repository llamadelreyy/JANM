using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about individuals or entities that hold licenses, including details such as the license holder identity, the business they operate, and relevant contact information.
/// </summary>
public partial class mst_licensee
{
    /// <summary>
    /// Unique identifier for each license holder (Primary Key).
    /// </summary>
    public int licensee_id { get; set; }

    /// <summary>
    /// Unique account number assigned to the license holder.
    /// </summary>
    public string? license_accno { get; set; }

    /// <summary>
    /// Identification card number of the owner (foreign key).
    /// </summary>
    public string? owner_icno { get; set; }

    /// <summary>
    /// Name of the business operated by the license holder.
    /// </summary>
    public string? business_name { get; set; }

    /// <summary>
    /// Physical address of the business operated by the license holder.
    /// </summary>
    public string? business_addr { get; set; }

    /// <summary>
    /// Code representing the district where the business is located.
    /// </summary>
    public string? district_code { get; set; }

    /// <summary>
    /// Code representing the state where the business is located.
    /// </summary>
    public string? state_code { get; set; }

    /// <summary>
    /// Start date of the current licensing period.
    /// </summary>
    public DateOnly? start_date { get; set; }

    /// <summary>
    /// End date of the current licensing period.
    /// </summary>
    public DateOnly? end_date { get; set; }

    /// <summary>
    /// Current status of the license (FK to status reference).
    /// </summary>
    public int? status_id { get; set; }

    /// <summary>
    /// Duration of the license validity in years or intervals.
    /// </summary>
    public string? license_duration { get; set; }

    /// <summary>
    /// Code representing the type of license issued.
    /// </summary>
    public int? cat_id { get; set; }

    /// <summary>
    /// Timestamp indicating when this record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    public int? town_id { get; set; }

    public string? codeid_premis { get; set; }

    public string? ssm_no { get; set; }

    public string? license_type { get; set; }

    public double? total_amount { get; set; }

    public string? doc_support { get; set; }

    public int? total_signboard { get; set; }

    public int? signboard_size { get; set; }

    public string? g_activity_1 { get; set; }

    public string? g_activity_2 { get; set; }

    public string? g_activity_3 { get; set; }

    public string? g_signbboard_1 { get; set; }

    public string? g_signbboard_2 { get; set; }

    public string? g_signbboard_3 { get; set; }

    public string? lot { get; set; }

    public int? mukim_id { get; set; }

    public int? creator_id { get; set; }

    public DateTime? modified_at { get; set; }

    public int? modifier_id { get; set; }

    public bool? is_deleted { get; set; }

    public virtual ref_license_cat? cat { get; set; }

    public virtual mst_premi? codeid_premisNavigation { get; set; }

    public virtual ref_license_status? status { get; set; }
}
