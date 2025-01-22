using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store confiscation transactions, including details about the owner and items confiscated.
/// </summary>
public partial class trn_confiscation
{
    /// <summary>
    /// Unique identifier for each confiscation transaction.
    /// </summary>
    public int trn_cfsc_id { get; set; }

    /// <summary>
    /// Identifier for the type of confiscation, referencing the ref_cfsc_types table.
    /// </summary>
    public int? cfsc_type_id { get; set; }

    /// <summary>
    /// Identification number of the owner associated with the confiscated items.
    /// </summary>
    public string? owner_icno { get; set; }

    /// <summary>
    /// Name of the owner associated with the confiscated items.
    /// </summary>
    public string? owner_name { get; set; }

    /// <summary>
    /// Tax account number associated with the business being inspected.
    /// </summary>
    public string? tax_accno { get; set; }

    /// <summary>
    /// License account number associated with the business being inspected.
    /// </summary>
    public string? license_accno { get; set; }

    /// <summary>
    /// Name of the business being inspected.
    /// </summary>
    public string? business_name { get; set; }

    /// <summary>
    /// Address of the business being inspected.
    /// </summary>
    public string? business_addr { get; set; }

    /// <summary>
    /// Reference number for tracking this specific confiscation.
    /// </summary>
    public string? cfsc_ref_no { get; set; }

    /// <summary>
    /// Identifier for the legal act related to this confiscation.
    /// </summary>
    public int? act_type_id { get; set; }

    /// <summary>
    /// Identifier for the specific section of law relevant to this confiscation.
    /// </summary>
    public int? section_act_id { get; set; }

    /// <summary>
    /// Instructions regarding actions to be taken related to this confiscation.
    /// </summary>
    public string? instruction { get; set; }

    /// <summary>
    /// Location where any offenses occurred during the confiscation.
    /// </summary>
    public string? offs_location { get; set; }

    /// <summary>
    /// scenario that happend durng confiscation (e.g., Pemilik Tidak Dijumpai, linked to a reference ref_cfsc_scenarios table.
    /// </summary>
    public int? scen_id { get; set; }

    /// <summary>
    /// Longitude of the location where the confiscation occurred.
    /// </summary>
    public decimal? ntc_longitude { get; set; }

    /// <summary>
    /// Latitude of the location where the confiscation occurred.
    /// </summary>
    public decimal? ntc_latitude { get; set; }

    /// <summary>
    /// Status of the confiscation transaction, linked to a reference status table.
    /// </summary>
    public int? trnstatus_id { get; set; }

    /// <summary>
    /// Identification number of officer who issued confiscation entry.
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

    public virtual ref_cfsc_type? cfsc_type { get; set; }

    public virtual ref_cfsc_scenario? scen { get; set; }
}
