using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores reference information about transaction statuses (e.g., Kompaun, Notis).
/// </summary>
public partial class ref_trn_status
{
    /// <summary>
    /// Unique identifier for each transaction status record (Primary Key).
    /// </summary>
    public int status_id { get; set; }

    /// <summary>
    /// Name of the transaction status (e.g., Baru, Dalam Tindakan, Tutup).
    /// </summary>
    public string status_name { get; set; } = null!;

    /// <summary>
    /// User ID of the individual who created this record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// User ID of the individual who last updated this record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp indicating when this record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// Timestamp indicating when this record was last updated.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// Flag indicating if the status record is active (False) or deleted (True).
    /// </summary>
    public bool? is_deleted { get; set; }
}
