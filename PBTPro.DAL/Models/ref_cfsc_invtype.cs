using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store types of confiscated items, such as easy to confiscate and hard to confiscate.
/// </summary>
public partial class ref_cfsc_invtype
{
    /// <summary>
    /// Unique identifier for each type of confiscated item.
    /// </summary>
    public int inv_type_id { get; set; }

    /// <summary>
    /// Description of the type of confiscated item.
    /// </summary>
    public string inv_type_desc { get; set; } = null!;

    /// <summary>
    /// ID of the user who created this record (default is 0).
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp when this record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// ID of the user who last modified this record (default is 0).
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when this record was last modified.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// Flag indicating whether this record is deleted (soft delete).
    /// </summary>
    public bool? is_deleted { get; set; }

    public virtual ICollection<trn_cfsc> trn_cfscs { get; set; } = new List<trn_cfsc>();
}
