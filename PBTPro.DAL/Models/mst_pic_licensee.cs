using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores about person in charge on the premise, based on license
/// </summary>
public partial class mst_pic_licensee
{
    public int pic_id { get; set; }

    public string? pic_name { get; set; }

    public string? pic_icno { get; set; }

    public string? pic_addr { get; set; }

    public string? pic_telno { get; set; }

    public int? relation_id { get; set; }

    public string? codeid_premis { get; set; }

    public string? license_accno { get; set; }

    public int? licensee_id { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public virtual mst_premis? codeid_premisNavigation { get; set; }

    public virtual mst_licensee? licensee { get; set; }

    public virtual ref_relationship? relation { get; set; }
}
