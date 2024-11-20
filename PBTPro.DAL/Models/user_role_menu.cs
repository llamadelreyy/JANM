using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class user_role_menu
{
    public int rolemenu_id { get; set; }

    public int? rolemenu_role_id { get; set; }

    public int? rolemenu_menu_id { get; set; }

    public string permission_status { get; set; } = null!;

    public DateTime? created_date { get; set; }

    public int? created_by { get; set; }

    public DateTime? updated_date { get; set; }

    public int? updated_by { get; set; }
}
