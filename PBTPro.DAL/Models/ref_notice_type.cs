using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores types of notices available under PBT (e.g., Notis Atas Premis, Notis Lesen).
/// </summary>
public partial class ref_notice_type
{
    /// <summary>
    /// Unique identifier for each type of notice record (Primary Key).
    /// </summary>
    public int notice_type_id { get; set; }

    /// <summary>
    /// Code for the notice type (e.g., N12 - Notis Lesen).
    /// </summary>
    public string notice_type_code { get; set; } = null!;

    /// <summary>
    /// Name of the notice type (e.g., Notis Lesen).
    /// </summary>
    public string notice_type_name { get; set; } = null!;

    /// <summary>
    /// Description about the notice type.
    /// </summary>
    public string? notice_type_desc { get; set; }

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

    //public virtual ICollection<trn_notice> trn_notices { get; set; } = new List<trn_notice>();
}
