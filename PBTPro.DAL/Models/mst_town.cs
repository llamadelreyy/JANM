using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class mst_town
{
    public int town_id { get; set; }

    public string town_code { get; set; } = null!;

    public string town_name { get; set; } = null!;

    public string district_code { get; set; } = null!;

    public string district_name { get; set; } = null!;

    public string state_code { get; set; } = null!;

    public string state_name { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public int? creator_id { get; set; }

    public DateTime? modified_at { get; set; }

    public int? modifier_id { get; set; }

    public bool? is_deleted { get; set; }
}
