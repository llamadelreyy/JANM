using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class patrol_info
{
    public int patrol_id { get; set; }

    public int patrol_cnt_notice { get; set; }

    public int patrol_cnt_compound { get; set; }

    public int patrol_cnt_notes { get; set; }

    public int patrol_cnt_seizure { get; set; }

    public string patrol_status { get; set; } = null!;

    public DateTime? patrol_start_dtm {   get; set; }

    public Point? patrol_start_location { get; set; }

    public DateTime? patrol_end_dtm { get; set; }

    public Point? patrol_end_location { get; set; }

    public bool active_flag { get; set; }

    public int? created_by { get; set; }

    public DateTime created_date { get; set; }

    public int? updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public string patrol_officer_name { get; set; } = null!;

    public string patrol_location { get; set; } = null!;

    public int? patrol_dept_id { get; set; }

    public virtual department_info department_id_info { get; set; } = null!;

}
