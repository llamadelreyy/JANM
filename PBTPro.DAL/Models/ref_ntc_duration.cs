using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores duration values for notices (e.g., Serta Merta, 3 hari, 5 hari).
/// </summary>
public partial class ref_ntc_duration
{
    /// <summary>
    /// Unique identifier for each notice duration record (Primary Key).
    /// </summary>
    public int duration_id { get; set; }

    /// <summary>
    /// Value of the notice duration (e.g., Serta Merta, 3 hari, 5 hari).
    /// </summary>
    public string duration_value { get; set; } = null!;

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

    public virtual ICollection<trn_notice> trn_notices { get; set; } = new List<trn_notice>();
}
