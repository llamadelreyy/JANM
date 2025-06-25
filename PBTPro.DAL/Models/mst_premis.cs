using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class mst_premis
{
    public int id { get; set; }

    public Point? geom { get; set; }

    public string? gkeseluruh { get; set; }

    public double? _latitude { get; set; }

    public double? _longitude { get; set; }

    public string codeid_premis { get; set; } = null!;

    public int? creator_id { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public string? lot { get; set; }

    public string? category { get; set; }

    public virtual ICollection<mst_license_premis_tax> mst_license_premis_taxes { get; set; } = new List<mst_license_premis_tax>();

    public virtual ICollection<mst_licensee> mst_licensees { get; set; } = new List<mst_licensee>();

    public virtual ICollection<mst_pic_licensee> mst_pic_licensees { get; set; } = new List<mst_pic_licensee>();

    public virtual ICollection<trn_premis_visit> trn_premis_visits { get; set; } = new List<trn_premis_visit>();
}
