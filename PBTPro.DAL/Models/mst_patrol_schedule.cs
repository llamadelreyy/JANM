using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class mst_patrol_schedule
{
    public int schedule_id { get; set; }

    public string? idno { get; set; }

    public DateTime start_time { get; set; }

    public DateTime end_time { get; set; }

    public int? status_id { get; set; }

    public bool is_scheduled { get; set; }

    public string? loc_name { get; set; }

    public int? type_id { get; set; }

    public int? dept_id { get; set; }

    public Point? start_location { get; set; }

    public Point? end_location { get; set; }

    public int cnt_notice { get; set; }

    public int cnt_cmpd { get; set; }

    public int cnt_notes { get; set; }

    public int cnt_seizure { get; set; }

    public int? creator_id { get; set; }

    public DateTime created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public string? district_code { get; set; }

    public string? town_code { get; set; }

    public int? user_id { get; set; }

    //public virtual ref_departmentss? dept { get; set; }
    public virtual ref_department? dept { get; set; }

    public virtual ref_patrol_status? status { get; set; }

    public virtual ICollection<trn_patrol_officer> trn_patrol_officers { get; set; } = new List<trn_patrol_officer>();

    public virtual ref_patrol_type? type { get; set; }
}
