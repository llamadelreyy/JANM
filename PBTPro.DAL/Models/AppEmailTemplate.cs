using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class AppEmailTemplate
{
    public int RecId { get; set; }

    public string Code { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string? Content { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDtm { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDtm { get; set; }

    public bool Isactive { get; set; }
}
