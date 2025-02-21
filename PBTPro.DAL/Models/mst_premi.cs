using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class mst_premi
{
    public int gid { get; set; }

    public string? no_akaun { get; set; }

    public string? lesen { get; set; }

    public string? gambar1 { get; set; }

    public string? gambar2 { get; set; }

    public Point? geom { get; set; }

    public string? negeri { get; set; }

    public string? daerah { get; set; }

    public string? mukim { get; set; }

    public string? seksyen { get; set; }

    public string? lot { get; set; }

    public DateOnly? tempoh_sah_cukai { get; set; }

    public DateOnly? tempoh_sah_lesen { get; set; }

    public virtual ICollection<mst_licensee> mst_licensees { get; set; } = new List<mst_licensee>();

    public virtual ICollection<mst_taxholder> mst_taxholders { get; set; } = new List<mst_taxholder>();
}
