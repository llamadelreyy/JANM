using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store information about compounds issued, including details about the owner, business, and associated documentation.
/// </summary>
public partial class trn_cmpd
{
    /// <summary>
    /// Unique identifier for each compound record.
    /// </summary>
    public int trn_cmpd_id { get; set; }

    /// <summary>
    /// Identification number of the owner.
    /// </summary>
    public string? owner_icno { get; set; }

    /// <summary>
    /// Reference number for the compound.
    /// </summary>
    public string? cmpd_ref_no { get; set; }

    /// <summary>
    /// Instructions regarding actions to be taken related to this compound.
    /// </summary>
    public string? instruction { get; set; }

    /// <summary>
    /// Location where the offense occurred.
    /// </summary>
    public string? offs_location { get; set; }

    /// <summary>
    /// Amount associated with the compound, stored as a numeric value.
    /// </summary>
    public decimal? amt_cmpd { get; set; }

    /// <summary>
    /// Identifier for delivery method used for this compound notice.
    /// </summary>
    public int? deliver_id { get; set; }

    /// <summary>
    /// Longitude of the location where the compound was issued.
    /// </summary>
    public decimal? cmpd_longitude { get; set; }

    /// <summary>
    /// Latitude of the location where the compound was issued.
    /// </summary>
    public decimal? cmpd_latitude { get; set; }

    /// <summary>
    /// Status of the compound, linked to a reference status table.
    /// </summary>
    public int? trnstatus_id { get; set; }

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

    public int? license_id { get; set; }

    public string? offense_code { get; set; }

    public string? uuk_code { get; set; }

    public string? act_code { get; set; }

    public string? section_code { get; set; }

    public int? schedule_id { get; set; }

    public string? tax_accno { get; set; }

    public bool? is_tax { get; set; }

    public string? doc_name { get; set; }

    public string? doc_pathurl { get; set; }

    public int? user_id { get; set; }
}
