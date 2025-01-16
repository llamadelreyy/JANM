using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class mst_district
{
    public int district_id { get; set; }

    public string district_code { get; set; } = null!;

    public string district_name { get; set; } = null!;

    public string state_code { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public int? creator_id { get; set; }

    public DateTime? modified_at { get; set; }

    public int? modifier_id { get; set; }

    public bool? is_deleted { get; set; }



    #region Virtual Field
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual string? state_name { get; set; }
    #endregion
}
