using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// TABLE SEJARAH LESEN
/// </summary>
public partial class license_history
{
    public long license_hist_id { get; set; }
    public long hist_id_info { get; set; }
    public string license_hist_account { get; set; } = null!;

    public string? license_hist_holder { get; set; }

    public DateOnly? license_hist_startd { get; set; }

    public DateOnly? license_hist_endd { get; set; }

    public string? license_hist_addr1 { get; set; }

    public string? license_hist_addr2 { get; set; }

    public string? license_hist_addr3 { get; set; }

    public string? license_hist_area { get; set; }

    public decimal? license_hist_pcode { get; set; }

    public string? license_hist_state { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }
    public virtual license_information hist_id_license { get; set; } = null!;

}
