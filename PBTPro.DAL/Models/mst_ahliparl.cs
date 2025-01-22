using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about ahli parliament.
/// </summary>
public partial class mst_ahliparl
{
    /// <summary>
    /// Unique identifier for each ahli parliament record (Primary Key).
    /// </summary>
    public int ahliparl_id { get; set; }

    /// <summary>
    /// ID for the parliament under this ahli (FK to tn_parliament).
    /// </summary>
    public int parl_id { get; set; }

    /// <summary>
    /// Name of ahli parliament (e.g., Tn Tuan Ganabatirau a/l Veraman).
    /// </summary>
    public string ahliparl_name { get; set; } = null!;

    /// <summary>
    /// Party name of ahli parliament (e.g., Bebas).
    /// </summary>
    public string? ahliparl_party { get; set; }

    /// <summary>
    /// Start date of the term in office.
    /// </summary>
    public DateTime? term_start { get; set; }

    /// <summary>
    /// End date of the term in office.
    /// </summary>
    public DateTime? term_end { get; set; }

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

    public virtual mst_parliament parl { get; set; } = null!;
}
