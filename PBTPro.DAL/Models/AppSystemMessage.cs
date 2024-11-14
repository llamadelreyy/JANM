using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class AppSystemMessage
{
    public int RecId { get; set; }

    public string Code { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Feature { get; set; } = null!;

    public string? Message { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDtm { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDtm { get; set; }

    public bool? Isactive { get; set; }
}
