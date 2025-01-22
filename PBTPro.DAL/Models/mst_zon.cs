using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information related to zone information under DUN. Example: Zon Teluk Gong di Klang.
/// </summary>
public partial class mst_zon
{
    /// <summary>
    /// Unique identifier for each zone record (Primary Key).
    /// </summary>
    public int zon_id { get; set; }

    /// <summary>
    /// Code for the zone (e.g., Zon N42B).
    /// </summary>
    public string zon_code { get; set; } = null!;

    /// <summary>
    /// List of locations under the zone (e.g., Taman Kapar, Taman Aman, etc.).
    /// </summary>
    public List<string>? zon_list { get; set; }

    /// <summary>
    /// DUN ID that this zone is associated with (FK to tn_dun).
    /// </summary>
    public int dun_id { get; set; }

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

    public virtual ICollection<mst_ahlimajli> mst_ahlimajlis { get; set; } = new List<mst_ahlimajli>();
}
