using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class user_menu
{
    public int menu_id { get; set; }

    public int? menu_role_id { get; set; }

    public int? menu_menu_id { get; set; }

    public string menu_name { get; set; } = null!;

    public string? menu_path { get; set; }

    public int? menu_has_submenu { get; set; }

    public string menu_status { get; set; } = null!;

    public DateTime? created_date { get; set; }

    public int? created_by { get; set; }

    public DateTime? updated_date { get; set; }

    public int? updated_by { get; set; }
}
