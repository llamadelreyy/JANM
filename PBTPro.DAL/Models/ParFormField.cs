using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class ParFormField
{
    public int RecId { get; set; }

    public string FormType { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Label { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Option { get; set; }

    public string? SourceUrl { get; set; }

    public bool Required { get; set; }

    public bool ApiSeeded { get; set; }

    public int Orders { get; set; }

    public bool Isactive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDtm { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDtm { get; set; }
}
