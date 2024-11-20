using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class notification_email_history
{
    public int history_id { get; set; }

    public string history_recipient { get; set; } = null!;

    public string history_subject { get; set; } = null!;

    public string? history_content { get; set; }

    public string history_status { get; set; } = null!;

    public string? history_remark { get; set; }

    public DateTime history_date_sent { get; set; }

    public int history_cnt_retry { get; set; }

    public bool active_flag { get; set; }

    public int? created_by { get; set; }

    public DateTime created_date { get; set; }

    public int? updated_by { get; set; }

    public DateTime? update_date { get; set; }
}
