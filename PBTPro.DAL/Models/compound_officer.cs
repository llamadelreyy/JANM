using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class compound_officer
{
    public long officer_id { get; set; }

    public string officer_serial { get; set; } = null!;

    public string? officer_name { get; set; }

    public string? officer_dept { get; set; }

    public string? officer_grade { get; set; }

    public string? officer_grade_desc { get; set; }

    public decimal? officer_pbt_code { get; set; }
}
