using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class config_building
{
    public long building_id { get; set; }

    public string? building_code { get; set; }

    public string? building_name { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }
}
