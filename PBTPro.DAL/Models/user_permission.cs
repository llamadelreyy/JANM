using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class user_permission
{
    public int permission_id { get; set; }

    public int? permission_menu_id { get; set; }

    public string permission_status { get; set; } = null!;

    public DateTime? created_date { get; set; }

    public int? created_by { get; set; }

    public DateTime? updated_date { get; set; }

    public int? updated_by { get; set; }
}
