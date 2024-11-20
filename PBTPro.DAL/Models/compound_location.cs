using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class compound_location
{
    public long location_id { get; set; }

    public long location_comp_id { get; set; }

    public string? location_comp_no { get; set; }

    public string? location_longitude { get; set; }

    public string? location_latitude { get; set; }

    public decimal? location_pbt_code { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual compound_info location_comp { get; set; } = null!;
}
