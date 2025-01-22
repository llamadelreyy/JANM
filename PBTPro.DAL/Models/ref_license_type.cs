using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores categories of licenses.
/// </summary>
public partial class ref_license_type
{
    /// <summary>
    /// Unique identifier for each license category.
    /// </summary>
    public int type_id { get; set; }

    /// <summary>
    /// Code representing the license category.
    /// </summary>
    public string? type_code { get; set; }

    /// <summary>
    /// Name of the license category.
    /// </summary>
    public string? type_name { get; set; }

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

    public virtual ICollection<ref_license_cat> ref_license_cats { get; set; } = new List<ref_license_cat>();

    public virtual ICollection<ref_license_op> ref_license_ops { get; set; } = new List<ref_license_op>();
}
