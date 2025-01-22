using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store information about notices issued, including details about the owner, business, and associated documentation.
/// </summary>
public partial class trn_notice
{
    /// <summary>
    /// Unique identifier for each notice.
    /// </summary>
    public int trn_notice_id { get; set; }

    /// <summary>
    /// Identifier for the type of notice issued.
    /// </summary>
    public int? notice_type_id { get; set; }

    /// <summary>
    /// Identification number of the owner.
    /// </summary>
    public string? owner_icno { get; set; }

    /// <summary>
    /// Telephone number of the owner.
    /// </summary>
    public string? owner_telno { get; set; }

    /// <summary>
    /// Tax account number associated with the owner.
    /// </summary>
    public string? tax_accno { get; set; }

    /// <summary>
    /// License account number associated with the owner.
    /// </summary>
    public string? license_accno { get; set; }

    /// <summary>
    /// Name of the business associated with the notice.
    /// </summary>
    public string? business_name { get; set; }

    /// <summary>
    /// Address of the business associated with the notice.
    /// </summary>
    public string? business_addr { get; set; }

    /// <summary>
    /// Code representing the type of notice.
    /// </summary>
    public string? type_code { get; set; }

    /// <summary>
    /// Code representing the category of notice.
    /// </summary>
    public string? cat_code { get; set; }

    /// <summary>
    /// Reference number for the notice.
    /// </summary>
    public string? notice_ref_no { get; set; }

    /// <summary>
    /// Identifier for the act related to the notice.
    /// </summary>
    public int? act_type_id { get; set; }

    /// <summary>
    /// Identifier for the specific section of the act related to the notice.
    /// </summary>
    public int? section_act_id { get; set; }

    /// <summary>
    /// Instructions regarding actions to be taken related to this notice.
    /// </summary>
    public string? instruction { get; set; }

    /// <summary>
    /// Location where the offense occurred.
    /// </summary>
    public string? offs_location { get; set; }

    /// <summary>
    /// Identifier for delivery method used for this notice.
    /// </summary>
    public int? deliver_id { get; set; }

    /// <summary>
    /// Path to proof image 1 associated with this notice.
    /// </summary>
    public string? proof_img1 { get; set; }

    /// <summary>
    /// Path to proof image 2 associated with this notice.
    /// </summary>
    public string? proof_img2 { get; set; }

    /// <summary>
    /// Path to proof image 3 associated with this notice.
    /// </summary>
    public string? proof_img3 { get; set; }

    /// <summary>
    /// Path to proof image 4 associated with this notice.
    /// </summary>
    public string? proof_img4 { get; set; }

    /// <summary>
    /// Path to proof image 5 associated with this notice.
    /// </summary>
    public string? proof_img5 { get; set; }

    /// <summary>
    /// Longitude of the location where the notice was issued.
    /// </summary>
    public decimal? ntc_longitude { get; set; }

    /// <summary>
    /// Latitude of the location where the notice was issued.
    /// </summary>
    public decimal? ntc_latitude { get; set; }

    /// <summary>
    /// Status of the notice, linked to a reference status table.
    /// </summary>
    public int? trnstatus_id { get; set; }

    /// <summary>
    /// Duration PREMISE OWNER for how long this notice is valid or relevant.
    /// </summary>
    public int? duration_id { get; set; }

    /// <summary>
    /// Identification number OFFICER related to this specific notice entry.
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

    public bool? is_deleted { get; set; }

    public virtual ref_ntc_duration? duration { get; set; }

    public virtual ref_notice_type? notice_type { get; set; }
}
