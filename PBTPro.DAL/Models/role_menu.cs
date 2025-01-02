using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class role_menu
{
    public int _id { get; set; }

    public int role_id { get; set; }

    public string? role_name { get; set; }

    public int menu_id { get; set; }

    public int header_id { get; set; }

    public string? menu_name { get; set; }

    public string? submenu_name { get; set; }

    public int parent_id { get; set; }

    public string? menu_path { get; set; }

    public string? icon_url { get; set; }

    public int sort_order { get; set; }

    public bool bln_create { get; set; }

    public bool bln_read { get; set; }

    public bool bln_update { get; set; }

    public bool bln_delete { get; set; }

    public bool bln_print { get; set; }

    public DateTime? created_date { get; set; }

    public int? created_by { get; set; }

    public DateTime? updated_date { get; set; }

    public int? updated_by { get; set; }
}
