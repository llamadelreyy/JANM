using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class MstArea
{
    public int Gid { get; set; }

    public double? Objectid { get; set; }

    public string? Fcd { get; set; }

    public string? Fnm { get; set; }

    public string? Nam { get; set; }

    public string? KodNegeri { get; set; }

    public string? KodDaerah { get; set; }

    public string? Ark { get; set; }

    public string? Acc { get; set; }

    public string? Bds { get; set; }

    public string? Keluasan { get; set; }

    public string? Kemaskini { get; set; }

    public decimal? ShapeLeng { get; set; }

    public decimal? ShapeArea { get; set; }

    public MultiPolygon? Geom { get; set; }
}
