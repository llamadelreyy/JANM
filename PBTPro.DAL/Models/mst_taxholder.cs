using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store information about tax holders, including their business details and associated statuses.
/// </summary>
public partial class mst_taxholder
{
    /// <summary>
    /// Unique identifier for each tax holder.
    /// </summary>
    public int taxholder_id { get; set; }

    /// <summary>
    /// Tax account number associated with the tax holder.
    /// </summary>
    public string? tax_accno { get; set; }

    /// <summary>
    /// Identification number of the owner, linked to the owners table.
    /// </summary>
    public string? owner_icno { get; set; }

    /// <summary>
    /// Code representing the district where the business is located.
    /// </summary>
    public string? district_code { get; set; }

    /// <summary>
    /// Code representing the state where the business is located.
    /// </summary>
    public string? state_code { get; set; }

    /// <summary>
    /// Date when the tax obligation starts.
    /// </summary>
    public DateOnly? tax_start_date { get; set; }

    /// <summary>
    /// Date when the tax obligation ends.
    /// </summary>
    public DateOnly? tax_end_date { get; set; }

    /// <summary>
    /// Status of the tax holder, linked to a reference status table.
    /// </summary>
    public int? status_id { get; set; }

    /// <summary>
    /// Duration of the tax obligation, defaulting to 1 year.
    /// </summary>
    public TimeSpan? tax_duration { get; set; }

    /// <summary>
    /// Identifier for parliamentary representation related to taxation.
    /// </summary>
    public int? parliment_id { get; set; }

    /// <summary>
    /// Identifier for state assembly representation related to taxation.
    /// </summary>
    public int? dun_id { get; set; }

    /// <summary>
    /// Identifier for zoning related to taxation.
    /// </summary>
    public int? zon_id { get; set; }

    /// <summary>
    /// ID of the user who created this record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// ID of the user who last modified this record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when this record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// Timestamp when this record was last modified.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// Flag indicating whether this record is deleted (soft delete).
    /// </summary>
    public bool? is_deleted { get; set; }

    public int? town_id { get; set; }

    public string? codeid_premis { get; set; }

    public string? alamat { get; set; }

    public virtual mst_dun? dun { get; set; }

    public virtual mst_parliament? parliment { get; set; }

    public virtual ref_tax_status? status { get; set; }

    public virtual mst_zon? zon { get; set; }
}
