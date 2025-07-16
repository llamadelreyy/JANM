using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class shp_ahli_majlis
{
    public int id { get; set; }

    public MultiPolygon? geom { get; set; }

    public string? AHLIMAJLIS { get; set; }

    public string? AHLIMAJL0 { get; set; }

    public string? NO_TELEFON { get; set; }

    public string? PENGGAL { get; set; }

    public string? DUN { get; set; }

    public string? EKAR { get; set; }
}
