using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class trn_witness
{
    public int witness_id { get; set; }

    public string? name { get; set; }

    public string? trn_type { get; set; }

    public int? trn_id { get; set; }

    public int? creator_id { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public int? user_id { get; set; }
}
