using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class TrnPatrolDet
{
    public int RecId { get; set; }

    public int PatrolId { get; set; }

    public string Username { get; set; } = null!;

    public int CntNotice { get; set; }

    public int CntCompound { get; set; }

    public int CntNotes { get; set; }

    public int CntSeizure { get; set; }

    public bool Isleader { get; set; }

    public bool Isactive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDtm { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDtm { get; set; }
}
