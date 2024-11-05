using System;
using System.Collections.Generic;

namespace PBTPro.Shared.Models;

public partial class Offense
{
    public int Offenseid { get; set; }

    public string? Offensename { get; set; }

    public string? Offensedescription { get; set; }

    public bool? Offenseenabled { get; set; }
}
