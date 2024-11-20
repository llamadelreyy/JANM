using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class compound_act
{
    public long act_id { get; set; }

    public string act_code { get; set; } = null!;

    public string? act_name { get; set; }

    public string act_offence_code { get; set; } = null!;

    public string? act_offence_name { get; set; }

    public string? act_transaction_code { get; set; }

    public string? act_transaction_name { get; set; }

    public string? act_dept_code { get; set; }

    public string? act_dept_name { get; set; }

    public decimal? act_amount1 { get; set; }

    public decimal? act_period1 { get; set; }

    public decimal? act_amount2 { get; set; }

    public decimal? act_period2 { get; set; }

    public decimal? act_amount3 { get; set; }

    public decimal? act_period3 { get; set; }

    public decimal? act_notice_amount { get; set; }

    public decimal? act_notice_period { get; set; }

    public decimal? act_fnotice_amount { get; set; }

    public decimal? act_fnotice_period { get; set; }

    public decimal? act_court_amount { get; set; }

    public decimal? act_pbt_code { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }
}
