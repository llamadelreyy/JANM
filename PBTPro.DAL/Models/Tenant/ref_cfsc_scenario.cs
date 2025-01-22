using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store different scenarios related to confiscation cases.
/// </summary>
public partial class ref_cfsc_scenario
{
    /// <summary>
    /// Unique identifier for each confiscation scenario.
    /// </summary>
    public int scen_id { get; set; }

    public string? scen_name { get; set; }

    /// <summary>
    /// Description of the confiscation scenario.
    /// </summary>
    public string? scen_desc { get; set; }

    /// <summary>
    /// ID of the user who created this record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp when this record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// ID of the user who last modified this record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when this record was last modified.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// Flag indicating whether this record is deleted (soft delete).
    /// </summary>
    public bool? is_deleted { get; set; }

    public virtual ICollection<trn_confiscation> trn_confiscations { get; set; } = new List<trn_confiscation>();
}
