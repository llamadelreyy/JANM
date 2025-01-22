using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about divisions under departments in PBT (e.g., Bahagian TRED dan Perniagaan dan Industri).
/// </summary>
public partial class ref_divisionss
{
    /// <summary>
    /// Unique identifier for each division record (Primary Key).
    /// </summary>
    public int div_id { get; set; }

    /// <summary>
    /// Code of the division (e.g., PL-TR).
    /// </summary>
    public string? div_code { get; set; }

    public int? dept_id { get; set; }

    public string? dept_code { get; set; }

    public string? dept_name { get; set; }

    /// <summary>
    /// Name of division (e.g., Bahagian TRED dan Perniagaan dan Industri).
    /// </summary>
    public string? div_name { get; set; }

    /// <summary>
    /// Description about the division (e.g., Roles, Job Description, etc.).
    /// </summary>
    public string? div_desc { get; set; }

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

    public virtual ref_departmentss? dept { get; set; }

    public virtual ICollection<ref_unitss> ref_unitsses { get; set; } = new List<ref_unitss>();
}
