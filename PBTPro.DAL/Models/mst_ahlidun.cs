using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about Ahli Majlis under DUN.
/// </summary>
public partial class mst_ahlidun
{
    /// <summary>
    /// Unique identifier for each Ahli Majlis record (Primary Key).
    /// </summary>
    public int ahlidun_id { get; set; }

    /// <summary>
    /// DUN ID that this member represents (FK to tn_dun).
    /// </summary>
    public int dun_id { get; set; }

    /// <summary>
    /// Name of Ahli Majlis (e.g., En Abd Razak).
    /// </summary>
    public string ahlidun_name { get; set; } = null!;

    /// <summary>
    /// Logical delete flag indicating if the record is active or deleted.
    /// </summary>
    public bool? is_deleted { get; set; }

    /// <summary>
    /// User who created the record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// User who last updated the record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when the record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// Timestamp when the record was last updated.
    /// </summary>
    public DateTime? modified_at { get; set; }

    public virtual mst_dun dun { get; set; } = null!;
}
