using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores categories under the tax types available under PBT (e.g., Cukai Taksiran consists of Kediaman, Industri, Pertanian).
/// </summary>
public partial class ref_tax_cat
{
    /// <summary>
    /// Unique identifier for each category of tax record (Primary Key).
    /// </summary>
    public int cat_id { get; set; }

    /// <summary>
    /// Type of tax that this category belongs to (FK to tax_type).
    /// </summary>
    public int? type_id { get; set; }

    /// <summary>
    /// Code for the tax category (e.g., A1).
    /// </summary>
    public string? cat_code { get; set; }

    /// <summary>
    /// Name of the tax category (e.g., Kediaman).
    /// </summary>
    public string? cat_name { get; set; }

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

    public virtual ref_tax_type? type { get; set; }
}
