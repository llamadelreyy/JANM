using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class config_system_param
{
    public int param_id { get; set; }

    public string param_app_layer { get; set; } = null!;

    public string param_group { get; set; } = null!;

    public string param_name { get; set; } = null!;

    public string param_value { get; set; } = null!;

    public bool active_flag { get; set; }

    public int? created_by { get; set; }

    public DateTime created_date { get; set; }

    public int? updated_by { get; set; }

    public DateTime? update_date { get; set; }
}
