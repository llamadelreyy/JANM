using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores operations related to licenses.
/// </summary>
public partial class ref_license_op
{
    /// <summary>
    /// Unique identifier for each license operation.
    /// </summary>
    public int ops_id { get; set; }

    /// <summary>
    /// Code representing the license operation.
    /// </summary>
    public string? ops_code { get; set; }

    /// <summary>
    /// Code representing the category of the license.
    /// </summary>
    public int? type_id { get; set; }

    /// <summary>
    /// Name of the license operation.
    /// </summary>
    public string? ops_name { get; set; }

    /// <summary>
    /// ID of the user who created the record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp indicating when the record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// ID of the user who last modified the record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp indicating when the record was last modified.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// Indicates if the record is marked as deleted (soft delete).
    /// </summary>
    public bool? is_deleted { get; set; }

    public virtual ICollection<mst_licensee> mst_licensees { get; set; } = new List<mst_licensee>();

    public virtual ref_license_type? type { get; set; }
}
