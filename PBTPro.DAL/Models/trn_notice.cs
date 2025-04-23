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
    /// Identification number of the owner.
    /// </summary>
    public string? owner_icno { get; set; }

    /// <summary>
    /// Tax account number associated with the owner.
    /// </summary>
    public string? tax_accno { get; set; }

    /// <summary>
    /// Reference number for the notice.
    /// </summary>
    public string? notice_ref_no { get; set; }

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
    /// Longitude of the location where the notice was issued.
    /// </summary>
    public decimal? notice_longitude { get; set; }

    /// <summary>
    /// Latitude of the location where the notice was issued.
    /// </summary>
    public decimal? notice_latitude { get; set; }

    /// <summary>
    /// Status of the notice, linked to a reference status table.
    /// </summary>
    public int? trnstatus_id { get; set; }

    /// <summary>
    /// Duration PREMISE OWNER for how long this notice is valid or relevant.
    /// </summary>
    public int? duration_id { get; set; }

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

    public int? license_id { get; set; }

    public int? schedule_id { get; set; }

    public bool? is_tax { get; set; }

    public string? act_code { get; set; }

    public string? section_code { get; set; }

    public string? uuk_code { get; set; }

    public string? offense_code { get; set; }

    public string? doc_name { get; set; }

    public string? doc_pathurl { get; set; }

    public int? user_id { get; set; }

    public string? recipient_name { get; set; }

    public string? recipient_icno { get; set; }

    public string? recipient_telno { get; set; }

    public string? recipient_addr { get; set; }

    public string? recipient_sign { get; set; }

    public int? recipient_relation_id { get; set; }

    public virtual ref_deliver? deliver { get; set; }

    public virtual ref_notice_duration? duration { get; set; }

    public virtual mst_patrol_schedule? schedule { get; set; }

    public virtual ICollection<trn_notice_img> trn_notice_imgs { get; set; } = new List<trn_notice_img>();

    public virtual ref_trn_status? trnstatus { get; set; }
}
