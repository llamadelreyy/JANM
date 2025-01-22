using PBTPro.DAL.Tenant.Models;
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
    /// Category code of the license (if applicable).
    /// </summary>
    public int? type_id { get; set; }

    /// <summary>
    /// Name of the business operated by the license holder.
    /// </summary>
    public string? business_name { get; set; }

    /// <summary>
    /// Physical address of the business operated by the license holder.
    /// </summary>
    public string? business_addr { get; set; }

    /// <summary>
    /// Code representing the town where the business is located.
    /// </summary>
    public string? town_code { get; set; }

    /// <summary>
    /// Code representing the district where the business is located.
    /// </summary>
    public string? district_code { get; set; }

    /// <summary>
    /// Code representing the state where the business is located.
    /// </summary>
    public string? state_code { get; set; }

    /// <summary>
    /// Date when the license was initially registered.
    /// </summary>
    public DateTime? reg_date { get; set; }

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
    public TimeSpan? license_duration { get; set; }

    /// <summary>
    /// Code representing the type of license issued.
    /// </summary>
    public int? cat_id { get; set; }

    /// <summary>
    /// Code representing specific operations covered by this license.
    /// </summary>
    public int? ops_id { get; set; }

    /// <summary>
    /// Reference to parliament jurisdiction for this license.
    /// </summary>
    public int? parl_id { get; set; }

    /// <summary>
    /// Reference to DUN jurisdiction for this license.
    /// </summary>
    public int? dun_id { get; set; }

    /// <summary>
    /// Reference to zoning jurisdiction for this license.
    /// </summary>
    public int? zon_id { get; set; }

    /// <summary>
    /// User ID of the individual who created this record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// User ID of the individual who last updated this record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp indicating when this record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// Timestamp indicating when this record was last updated.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// Flag indicating whether this record is marked as deleted (soft delete).
    /// </summary>
    public bool? is_deleted { get; set; }

    public virtual mst_owner? owner_icnoNavigation { get; set; }

    public virtual ref_license_status? status { get; set; }
}
