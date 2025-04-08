using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class mst_area
{
    public int gid { get; set; }

    public int? id { get; set; }

    public string? area { get; set; }

    public MultiPolygon? geom { get; set; }
}
