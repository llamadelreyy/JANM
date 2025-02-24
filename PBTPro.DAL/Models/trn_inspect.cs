using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store inspection records, including details about the inspection type, owner information, and related documentation.
/// </summary>
public partial class trn_inspect
{
    /// <summary>
    /// Unique identifier for each inspection record.
    /// </summary>
    public int trn_inspect_id { get; set; }

    /// <summary>
    /// Identification number of the owner associated with the inspection.
    /// </summary>
    public string? owner_icno { get; set; }

    /// <summary>
    /// Reference number for tracking this specific inspection.
    /// </summary>
    public string? inspect_ref_no { get; set; }

    /// <summary>
    /// Additional notes or comments regarding the inspection.
    /// </summary>
    public string? notes { get; set; }

    /// <summary>
    /// Location where any offenses occurred during the inspection.
    /// </summary>
    public string? offs_location { get; set; }

    /// <summary>
    /// Identifier for the department responsible for conducting this inspection.
    /// </summary>
    public int? dept_id { get; set; }

    /// <summary>
    /// Longitude of the location where the inspection occurred.
    /// </summary>
    public decimal? inspect_longitude { get; set; }

    /// <summary>
    /// Latitude of the location where the inspection occurred.
    /// </summary>
    public decimal? inspect_latitude { get; set; }

    /// <summary>
    /// Status of the inspection record, linked to a reference status table.
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

    public int gid { get; set; }

    public int? schedule_id { get; set; }

    public string? tax_accno { get; set; }

    public bool? is_tax { get; set; }

    public string? doc_name { get; set; }

    public string? doc_pathurl { get; set; }

    public int? user_id { get; set; }
}
