using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class TbAuditlog
{
    public int Auditid { get; set; }

    public int Perananid { get; set; }

    public string Jenisaudit { get; set; } = null!;

    public string Namamodule { get; set; } = null!;

    public string Method { get; set; } = null!;

    public string? Catatan { get; set; }

    public int? Rekciptauserid { get; set; }

    public DateTime? Rekcipta { get; set; }
}
