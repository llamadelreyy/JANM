using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about departments under PBT (e.g., Jabatan Pelesenan).
/// </summary>
public partial class ref_department
{
    /// <summary>
    /// Unique identifier for each department record (Primary Key).
    /// </summary>
    public int dept_id { get; set; }

    /// <summary>
    /// Code of the department (e.g., PL).
    /// </summary>
    public string? dept_code { get; set; }

    /// <summary>
    /// Name of the department (e.g., Jabatan Pelesenan).
    /// </summary>
    public string dept_name { get; set; } = null!;

    /// <summary>
    /// Description about the department (e.g., Roles, Job Description, etc.).
    /// </summary>
    public string? dept_desc { get; set; }

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

    public string? dept_email { get; set; }
    public virtual ICollection<mst_patrol_schedule> mst_patrol_schedules { get; set; } = new List<mst_patrol_schedule>();

    public virtual ICollection<ref_division> ref_divisions { get; set; } = new List<ref_division>();

    public virtual ICollection<ref_unit> ref_units { get; set; } = new List<ref_unit>();

    public virtual ICollection<trn_inspect> trn_inspects { get; set; } = new List<trn_inspect>();
}
