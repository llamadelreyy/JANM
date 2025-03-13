using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class mst_license_premis_tax
{
    public int license_premis_tax_id { get; set; }

    public string? license_accno { get; set; }

    public string? codeid_premis { get; set; }

    public string? floor_building { get; set; }

    public string? tax_accno { get; set; }

    public int? status_lesen_id { get; set; }

    public int? status_tax_id { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public virtual mst_premis? codeid_premisNavigation { get; set; }

    public virtual ref_license_status? status_lesen { get; set; }

    public virtual ref_tax_status? status_tax { get; set; }
}
