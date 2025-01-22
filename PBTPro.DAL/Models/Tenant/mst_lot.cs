using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class mst_lot
{
    public int gid { get; set; }

    public double? objectid { get; set; }

    public string? negeri { get; set; }

    public string? daerah { get; set; }

    public string? mukim { get; set; }

    public string? seksyen { get; set; }

    public string? lot { get; set; }

    public string? upi { get; set; }

    public decimal? s_area { get; set; }

    public decimal? m_area { get; set; }

    public decimal? g_area { get; set; }

    public string? unit { get; set; }

    public string? pa { get; set; }

    public string? refplan { get; set; }

    public string? apdate { get; set; }

    public string? cls { get; set; }

    public string? landusecod { get; set; }

    public string? landtitlec { get; set; }

    public string? entrymode { get; set; }

    public DateOnly? updated { get; set; }

    public string? guid { get; set; }

    public decimal? mi_prinx { get; set; }

    public decimal? shape_leng { get; set; }

    public decimal? shape_area { get; set; }

    public MultiPolygon? geom { get; set; }
}
