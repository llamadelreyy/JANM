using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores the status of patrols (e.g., Belum Mula, Dalam Rondaan, Selesai).
/// </summary>
public partial class ref_patrol_status
{
    /// <summary>
    /// Unique identifier for each patrol status.
    /// </summary>
    public int status_id { get; set; }

    /// <summary>
    /// Code representing the patrol status.
    /// </summary>
    public string? status_code { get; set; }

    /// <summary>
    /// Name of the patrol status.
    /// </summary>
    public string? status_name { get; set; }

    /// <summary>
    /// Description of the patrol status.
    /// </summary>
    public string? status_desc { get; set; }

    /// <summary>
    /// ID of the user who created the record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Record creation timestamp.
    /// </summary>
    public DateTime created_at { get; set; }

    /// <summary>
    /// ID of the user who last modified the record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Last modified timestamp.
    /// </summary>
    public DateTime modified_at { get; set; }

    /// <summary>
    /// Indicates if the record is marked as deleted (soft delete).
    /// </summary>
    public bool? is_deleted { get; set; }

    public virtual ICollection<mst_patrol_schedule> mst_patrol_schedules { get; set; } = new List<mst_patrol_schedule>();
}
