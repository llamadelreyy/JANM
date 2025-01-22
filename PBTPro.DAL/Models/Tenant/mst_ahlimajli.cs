using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about Ahli Majlis under Zon.
/// </summary>
public partial class mst_ahlimajli
{
    /// <summary>
    /// Unique identifier for each Ahli Majlis record (Primary Key).
    /// </summary>
    public int ahlimj_id { get; set; }

    /// <summary>
    /// ZON ID that this member represents (FK to pbt_zon).
    /// </summary>
    public int zon_id { get; set; }

    /// <summary>
    /// Name of Ahli Majlis (e.g., En Abd Razak).
    /// </summary>
    public string ahlimj_name { get; set; } = null!;

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

    public virtual mst_zon zon { get; set; } = null!;
}
