using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class mst_area
{
    public int gid { get; set; }

    public double? objectid { get; set; }

    public string? fcd { get; set; }

    public string? fnm { get; set; }

    public string? nam { get; set; }

    public string? kod_negeri { get; set; }

    public string? kod_daerah { get; set; }

    public string? ark { get; set; }

    public string? acc { get; set; }

    public string? bds { get; set; }

    public string? keluasan { get; set; }

    public string? kemaskini { get; set; }

    public decimal? shape_leng { get; set; }

    public decimal? shape_area { get; set; }

    public MultiPolygon? geom { get; set; }
}
