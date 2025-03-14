using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores the different statuses for each license (e.g., Aktif, Tidak Aktif, Batal).
/// </summary>
public partial class ref_license_status
{
    /// <summary>
    /// Unique identifier for each license status record (Primary Key).
    /// </summary>
    public int status_id { get; set; }

    /// <summary>
    /// Name of the license status (e.g., Aktif, Tidak Aktif, Batal).
    /// </summary>
    public string status_name { get; set; } = null!;

    /// <summary>
    /// Flag indicating if the status record is active or inactive.
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

    public int priority { get; set; }

    public string color { get; set; } = null!;

    public virtual ICollection<mst_license_premis_tax> mst_license_premis_taxes { get; set; } = new List<mst_license_premis_tax>();

    public virtual ICollection<mst_licensee> mst_licensees { get; set; } = new List<mst_licensee>();
}
