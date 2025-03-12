using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store confiscated items along with their types.
/// </summary>
public partial class ref_cfsc_inventory
{
    /// <summary>
    /// Unique identifier for each confiscated item.
    /// </summary>
    public int inv_id { get; set; }

    /// <summary>
    /// Name of the confiscated item.
    /// </summary>
    public string inv_name { get; set; } = null!;

    /// <summary>
    /// Type of the confiscated item (Mudah Disita or Tidak Mudah Disita).
    /// </summary>
    public int? item_type { get; set; }

    /// <summary>
    /// ID of the user who created this record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp when this record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// ID of the user who modified this record.
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

    public virtual ICollection<trn_cfsc_item> trn_cfsc_items { get; set; } = new List<trn_cfsc_item>();

    public virtual ICollection<trn_cfsc> trn_cfscs { get; set; } = new List<trn_cfsc>();
}
