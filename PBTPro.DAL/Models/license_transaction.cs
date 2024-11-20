using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// TABLE TRANSAKSI LESEN
/// </summary>
public partial class license_transaction
{
    public long license_trans_id { get; set; }

    public long license_trans_info { get; set; }

    public string license_trans_account { get; set; } = null!;

    public string license_trans_code { get; set; } = null!;

    public string? license_trans_name { get; set; }

    public decimal? license_trans_amount { get; set; }

    public string? license_trans_status { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public string? created_by { get; set; }

    public string? updated_by { get; set; }

    public virtual license_information license_trans_infoNavigation { get; set; } = null!;
}
