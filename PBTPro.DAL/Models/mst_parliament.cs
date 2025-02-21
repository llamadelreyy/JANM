using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about parliaments, including their codes and names.
/// </summary>
public partial class mst_parliament
{
    /// <summary>
    /// Unique identifier for each parliament record (Primary Key).
    /// </summary>
    public int parl_id { get; set; }

    /// <summary>
    /// Code assigned to each parliament (e.g., P110).
    /// </summary>
    public string parl_code { get; set; } = null!;

    /// <summary>
    /// Name of the parliament (e.g., Parliament Klang).
    /// </summary>
    public string parl_name { get; set; } = null!;

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

    public virtual ICollection<mst_ahliparl> mst_ahliparls { get; set; } = new List<mst_ahliparl>();

    public virtual ICollection<mst_dun> mst_duns { get; set; } = new List<mst_dun>();

    public virtual ICollection<mst_licensee> mst_licensees { get; set; } = new List<mst_licensee>();

    public virtual ICollection<mst_taxholder> mst_taxholders { get; set; } = new List<mst_taxholder>();
}
