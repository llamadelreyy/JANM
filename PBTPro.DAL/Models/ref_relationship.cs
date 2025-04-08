using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores relationships between the recipient to the premise owner e.g., Pekerja, Ahli Keluarga, Tiada kaitan
/// </summary>
public partial class ref_relationship
{
    public int relation_id { get; set; }

    public string? relation_name { get; set; }

    public DateTime? created_at { get; set; }

    public int? creator_id { get; set; }

    public DateTime? modified_at { get; set; }

    public int? modifier_id { get; set; }

    public bool? is_deleted { get; set; }

    public virtual ICollection<mst_pic_licensee> mst_pic_licensees { get; set; } = new List<mst_pic_licensee>();
}
