using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about owners.
/// </summary>
public partial class mst_owner
{
    /// <summary>
    /// Unique identifier for each owner.
    /// </summary>
    public int owner_id { get; set; }

    /// <summary>
    /// IC number (must be unique).
    /// </summary>
    public string owner_icno { get; set; } = null!;

    /// <summary>
    /// Owner&apos;s name.
    /// </summary>
    public string? owner_name { get; set; }

    /// <summary>
    /// Type of ID (No Kad Pengenalan/Pasport).
    /// </summary>
    public int? id_type_id { get; set; }

    /// <summary>
    /// Email address.
    /// </summary>
    public string? owner_email { get; set; }

    /// <summary>
    /// Address of the owner.
    /// </summary>
    public string? owner_addr { get; set; }

    /// <summary>
    /// Town where the owner resides.
    /// </summary>
    public string? town_code { get; set; }

    /// <summary>
    /// District where the owner resides.
    /// </summary>
    public string? district_code { get; set; }

    /// <summary>
    /// State where the owner resides.
    /// </summary>
    public string? state_code { get; set; }

    /// <summary>
    /// Phone number of the owner.
    /// </summary>
    public string? owner_telno { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public virtual ICollection<mst_licensee> mst_licensees { get; set; } = new List<mst_licensee>();

    public virtual ICollection<mst_taxholder> mst_taxholders { get; set; } = new List<mst_taxholder>();
}
