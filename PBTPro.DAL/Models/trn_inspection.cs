using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store inspection records, including details about the inspection type, owner information, and related documentation.
/// </summary>
public partial class trn_inspection
{
    /// <summary>
    /// Unique identifier for each inspection record.
    /// </summary>
    public int trn_inspect_id { get; set; }

    /// <summary>
    /// Identifier for the type of inspection conducted.
    /// </summary>
    public int? note_type_id { get; set; }

    /// <summary>
    /// Identification number of the owner associated with the inspection.
    /// </summary>
    public string? owner_icno { get; set; }

    /// <summary>
    /// Telephone number of the owner associated with the inspection.
    /// </summary>
    public string? owner_telno { get; set; }

    /// <summary>
    /// Name of the business being inspected.
    /// </summary>
    public string? business_name { get; set; }

    /// <summary>
    /// Address of the business being inspected.
    /// </summary>
    public string? business_addr { get; set; }

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
    /// Path to proof image 1 associated with this inspection.
    /// </summary>
    public string? proof_img1 { get; set; }

    /// <summary>
    /// Path to proof image 2 associated with this inspection.
    /// </summary>
    public string? proof_img2 { get; set; }

    /// <summary>
    /// Path to proof image 3 associated with this inspection.
    /// </summary>
    public string? proof_img3 { get; set; }

    /// <summary>
    /// Path to proof image 4 associated with this inspection.
    /// </summary>
    public string? proof_img4 { get; set; }

    /// <summary>
    /// Path to proof image 5 associated with this inspection.
    /// </summary>
    public string? proof_img5 { get; set; }

    /// <summary>
    /// Longitude of the location where the inspection occurred.
    /// </summary>
    public decimal? ntc_longitude { get; set; }

    /// <summary>
    /// Latitude of the location where the inspection occurred.
    /// </summary>
    public decimal? ntc_latitude { get; set; }

    /// <summary>
    /// Status of the inspection record, linked to a reference status table.
    /// </summary>
    public int? trnstatus_id { get; set; }

    /// <summary>
    /// Identification number of officer who issuing the ticket.
    /// </summary>
    public string? idno { get; set; }

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

    //public virtual ref_departmentss? dept { get; set; }
    public virtual ref_department? dept { get; set; }

    public virtual ref_note_type? note_type { get; set; }
}
