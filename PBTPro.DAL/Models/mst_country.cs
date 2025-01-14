using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class mst_country
{
    public int country_id { get; set; }

    public string country_code { get; set; } = null!;

    public string country_name { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public int? creator_id { get; set; }

    public DateTime? modified_at { get; set; }

    public int? modifier_id { get; set; }

    public bool? is_deleted { get; set; }
}
