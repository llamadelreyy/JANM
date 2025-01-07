using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about unit under departments in PBT (e.g., Bahagian TRED dan Perniagaan dan Industri).
/// </summary>
public partial class ref_unit
{
    /// <summary>
    /// Unique identifier for each unit under division
    /// </summary>
    public int unit_id { get; set; }

    public int? div_id { get; set; }
    public string div_name { get; set; } = null!;

    /// <summary>
    /// Code of the unit (e.g., PL-TR).
    /// </summary>
    public string? unit_code { get; set; }

    /// <summary>
    /// Name of unit (e.g., Unit Kaunter).
    /// </summary>
    public string unit_name { get; set; } = null!;

    public string unit_code_name => $"{unit_code} - {unit_name}";

    /// <summary>
    /// Description about the unit (e.g., Roles, Job Description, etc.).
    /// </summary>
    public string? unit_desc { get; set; }

    /// <summary>
    /// User who created the record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp when the record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// User who last updated the record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when the record was last updated.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// Logical delete flag indicating if the record is active or deleted.
    /// </summary>
    public bool? is_deleted { get; set; }

    public int dept_id { get; set; }
    public string? dept_name { get; set; }
}
