using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store confiscated items along with their types.
/// </summary>
public partial class trn_cfsc_item
{
    /// <summary>
    /// Unique identifier for each confiscated item.
    /// </summary>
    public int item_id { get; set; }

    public int? trn_cfsc_id { get; set; }

    /// <summary>
    /// Type of the confiscated item (Mudah Disita or Tidak Mudah Disita).
    /// </summary>
    public int? inv_id { get; set; }

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

    public string? description { get; set; }

    public int? cnt_item { get; set; }
}
