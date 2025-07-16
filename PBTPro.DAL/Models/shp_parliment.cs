using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class shp_parliment
{
    public int id { get; set; }

    public MultiPolygon? geom { get; set; }

    public string? Name { get; set; }

    public string? end { get; set; }

    public string? altitudeMo { get; set; }

    public long? tessellate { get; set; }

    public long? extrude { get; set; }

    public long? visibility { get; set; }

    public long? drawOrder { get; set; }

    public string? icon { get; set; }

    public string? snippet { get; set; }
}
