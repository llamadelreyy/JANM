using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information related to the Dewan Undangan Negeri (DUN) in Malaysia, which refers to the highest legislative body at the state level. Each DUN functions as a sub-division within a parliamentary constituency and is responsible for law-making and overseeing the implementation of state government policies.
/// </summary>
public partial class mst_dun
{
    /// <summary>
    /// Unique identifier for each DUN record (Primary Key).
    /// </summary>
    public int dun_id { get; set; }

    /// <summary>
    /// Parliament ID that this DUN falls under (FK to tn_parliament).
    /// </summary>
    public int parl_id { get; set; }

    /// <summary>
    /// Code for the DUN (e.g., N45).
    /// </summary>
    public string dun_code { get; set; } = null!;

    /// <summary>
    /// Name of the DUN (e.g., Dun Batu Tiga).
    /// </summary>
    public string dun_name { get; set; } = null!;

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

    public virtual ICollection<mst_ahlidun> mst_ahliduns { get; set; } = new List<mst_ahlidun>();

    public virtual ICollection<mst_zon> mst_zons { get; set; } = new List<mst_zon>();

    public virtual mst_parliament parl { get; set; } = null!;
}
