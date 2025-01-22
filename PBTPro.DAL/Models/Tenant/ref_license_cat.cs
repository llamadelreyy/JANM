using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores types of licenses.
/// </summary>
public partial class ref_license_cat
{
    /// <summary>
    /// Unique identifier for each license type.
    /// </summary>
    public int cat_id { get; set; }

    /// <summary>
    /// Code representing the license type.
    /// </summary>
    public string? cat_code { get; set; }

    /// <summary>
    /// Code representing the category of the license.
    /// </summary>
    public int? type_id { get; set; }

    /// <summary>
    /// Name of the license type.
    /// </summary>
    public string? cat_name { get; set; }

    /// <summary>
    /// ID of the user who created the record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp indicating when the record was created.
    /// </summary>
    public DateOnly? created_at { get; set; }

    /// <summary>
    /// ID of the user who last modified the record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp indicating when the record was last modified.
    /// </summary>
    public DateOnly? modified_at { get; set; }

    /// <summary>
    /// Indicates if the record is marked as deleted (soft delete).
    /// </summary>
    public bool? is_deleted { get; set; }

    public virtual ref_license_type? type { get; set; }
}
