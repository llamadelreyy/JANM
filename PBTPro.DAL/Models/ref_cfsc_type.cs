using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store different types of confiscations, including their codes, names, and descriptions.
/// </summary>
public partial class ref_cfsc_type
{
    /// <summary>
    /// Unique identifier for each confiscation type.
    /// </summary>
    public int cfsc_type_id { get; set; }

    /// <summary>
    /// Code representing the confiscation type, must be unique.
    /// </summary>
    public string cfsc_type_code { get; set; } = null!;

    /// <summary>
    /// Name of the confiscation type.
    /// </summary>
    public string cfsc_type_name { get; set; } = null!;

    /// <summary>
    /// Description of the confiscation type.
    /// </summary>
    public string? cfsc_type_desc { get; set; }

    /// <summary>
    /// Flag indicating whether this record is deleted (soft delete).
    /// </summary>
    public bool? is_deleted { get; set; }

    /// <summary>
    /// ID of the user who created this record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// ID of the user who last modified this record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when this record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// Timestamp when this record was last modified.
    /// </summary>
    public DateTime? modified_at { get; set; }
}
