using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// TABLE BAGI MENYIMPAN ALAMAT LESEN SEDIA ADA DAN BARU
/// </summary>
public partial class license_address_swap
{
    public long swap_license_id { get; set; }

    public long swap_id_info { get; set; }

    public string swap_license_account { get; set; } = null!;

    public string? swap_current_addr1 { get; set; }

    public string? swap_current_addr2 { get; set; }

    public string? swap_current_addr3 { get; set; }

    public string? swap_current_area { get; set; }

    public decimal? swap_current_pcode { get; set; }

    public string? swap_current_state { get; set; }

    public string? swap_new_addr1 { get; set; }

    public string? swap_new_addr2 { get; set; }

    public string? swap_new_addr3 { get; set; }

    public string? swap_new_area { get; set; }

    public decimal? swap_new_pcode { get; set; }

    public string? swap_new_state { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual license_information swap_id_infoNavigation { get; set; } = null!;
}
