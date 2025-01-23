using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store information about compounds issued, including details about the owner, business, and associated documentation.
/// </summary>
public partial class trn_compound
{
    /// <summary>
    /// Unique identifier for each compound record.
    /// </summary>
    public int trn_cmpd_id { get; set; }

    /// <summary>
    /// Identifier for the type of compound issued.
    /// </summary>
    public int? cmpd_type_id { get; set; }

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
    /// Name of the business associated with the compound.
    /// </summary>
    public string? business_name { get; set; }

    /// <summary>
    /// Address of the business associated with the compound.
    /// </summary>
    public string? business_addr { get; set; }

    /// <summary>
    /// Code representing the type of compound.
    /// </summary>
    public string? type_code { get; set; }

    /// <summary>
    /// Code representing the category of compound.
    /// </summary>
    public string? cat_code { get; set; }

    /// <summary>
    /// Reference number for the compound.
    /// </summary>
    public string? cmpd_ref_no { get; set; }

    /// <summary>
    /// Identifier for the act related to the compound.
    /// </summary>
    public int? act_type_id { get; set; }

    /// <summary>
    /// Identifier for the specific section of the act related to the compound.
    /// </summary>
    public int? section_act_id { get; set; }

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
    /// Path to proof image 1 associated with this compound.
    /// </summary>
    public string? proof_img1 { get; set; }

    /// <summary>
    /// Path to proof image 2 associated with this compound.
    /// </summary>
    public string? proof_img2 { get; set; }

    /// <summary>
    /// Path to proof image 3 associated with this compound.
    /// </summary>
    public string? proof_img3 { get; set; }

    /// <summary>
    /// Path to proof image 4 associated with this compound.
    /// </summary>
    public string? proof_img4 { get; set; }

    /// <summary>
    /// Path to proof image 5 associated with this compound.
    /// </summary>
    public string? proof_img5 { get; set; }

    /// <summary>
    /// Longitude of the location where the compound was issued.
    /// </summary>
    public decimal? ntc_longitude { get; set; }

    /// <summary>
    /// Latitude of the location where the compound was issued.
    /// </summary>
    public decimal? ntc_latitude { get; set; }

    /// <summary>
    /// Status of the compound, linked to a reference status table.
    /// </summary>
    public int? trnstatus_id { get; set; }

    /// <summary>
    /// id officer that issued the ticket
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

    public virtual ref_cmpd_type? cmpd_type { get; set; }
}
