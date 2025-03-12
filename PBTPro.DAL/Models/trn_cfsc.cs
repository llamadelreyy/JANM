using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store confiscation transactions, including details about the owner and items confiscated.
/// </summary>
public partial class trn_cfsc
{
    /// <summary>
    /// Unique identifier for each confiscation transaction.
    /// </summary>
    public int trn_cfsc_id { get; set; }

    /// <summary>
    /// Identification number of the owner associated with the confiscated items.
    /// </summary>
    public string? owner_icno { get; set; }

    /// <summary>
    /// Tax account number associated with the business being inspected.
    /// </summary>
    public string? tax_accno { get; set; }

    /// <summary>
    /// Reference number for tracking this specific confiscation.
    /// </summary>
    public string? cfsc_ref_no { get; set; }

    /// <summary>
    /// Instructions regarding actions to be taken related to this confiscation.
    /// </summary>
    public string? instruction { get; set; }

    /// <summary>
    /// Location where any offenses occurred during the confiscation.
    /// </summary>
    public string? offs_location { get; set; }

    /// <summary>
    /// Scenario that happened during confiscation (e.g., Pemilik Tidak Dijumpai, linked to a reference ref_cfsc_scenarios table.
    /// </summary>
    public int? scen_id { get; set; }

    /// <summary>
    /// Longitude of the location where the confiscation occurred.
    /// </summary>
    public decimal? cfsc_longitude { get; set; }

    /// <summary>
    /// Latitude of the location where the confiscation occurred.
    /// </summary>
    public decimal? cfsc_latitude { get; set; }

    /// <summary>
    /// Status of the confiscation transaction, linked to a reference status table.
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

    public int? inv_id { get; set; }

    public int? inv_type_id { get; set; }

    public string? offense_code { get; set; }

    public string? uuk_code { get; set; }

    public string? act_code { get; set; }

    public string? section_code { get; set; }

    public int? schedule_id { get; set; }

    public bool? is_tax { get; set; }

    public string? doc_name { get; set; }

    public string? doc_pathurl { get; set; }

    public int? user_id { get; set; }

    public virtual ref_cfsc_inventory? inv { get; set; }

    public virtual ref_cfsc_invtype? inv_type { get; set; }

    public virtual ref_cfsc_scenario? scen { get; set; }

    public virtual mst_patrol_schedule? schedule { get; set; }

    public virtual ICollection<trn_cfsc_img> trn_cfsc_imgs { get; set; } = new List<trn_cfsc_img>();

    public virtual ICollection<trn_cfsc_item> trn_cfsc_items { get; set; } = new List<trn_cfsc_item>();

    public virtual ref_trn_status? trnstatus { get; set; }
}
