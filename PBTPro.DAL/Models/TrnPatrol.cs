using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class TrnPatrol
{
    public int RecId { get; set; }

    public int CntNotice { get; set; }

    public int CntCompound { get; set; }

    public int CntNotes { get; set; }

    public int CntSeizure { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? StartDtm { get; set; }

    public Point? StartLocation { get; set; }

    public DateTime? StopDtm { get; set; }

    public Point? StopLocation { get; set; }

    public bool Isactive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDtm { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDtm { get; set; }
}
