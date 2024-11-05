using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class MstLot
{
    public long Id { get; set; }

    public MultiPolygon? Geom { get; set; }

    public decimal? Objectid { get; set; }

    public string? Negeri { get; set; }

    public string? Daerah { get; set; }

    public string? Mukim { get; set; }

    public string? Seksyen { get; set; }

    public string? Lot { get; set; }

    public string? Upi { get; set; }

    public decimal? SArea { get; set; }

    public decimal? MArea { get; set; }

    public decimal? GArea { get; set; }

    public string? Unit { get; set; }

    public string? Pa { get; set; }

    public string? Refplan { get; set; }

    public string? Apdate { get; set; }

    public string? Cls { get; set; }

    public string? Landusecod { get; set; }

    public string? Landtitlec { get; set; }

    public string? Entrymode { get; set; }

    public DateOnly? Updated { get; set; }

    public string? Guid { get; set; }

    public decimal? MiPrinx { get; set; }

    public decimal? ShapeLeng { get; set; }

    public decimal? ShapeArea { get; set; }
}
