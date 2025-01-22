using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores types of tax (cukai) that are available under PBT (e.g., Cukai Taksiran).
/// </summary>
public partial class ref_tax_type
{
    /// <summary>
    /// Unique identifier for each type of tax record (Primary Key).
    /// </summary>
    public int type_id { get; set; }

    public string? type_code { get; set; }

    /// <summary>
    /// Name of tax type (e.g., Cukai Taksiran).
    /// </summary>
    public string? type_name { get; set; }

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

    public virtual ICollection<ref_tax_cat> ref_tax_cats { get; set; } = new List<ref_tax_cat>();
}
