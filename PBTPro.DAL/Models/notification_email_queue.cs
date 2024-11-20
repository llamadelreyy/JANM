using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class notification_email_queue
{
    public int queue_id { get; set; }

    public string queue_recipient { get; set; } = null!;

    public string queue_subject { get; set; } = null!;

    public string? queue_content { get; set; }

    public string queue_status { get; set; } = null!;

    public string? queue_remark { get; set; }

    public DateTime queue_date_sent { get; set; }

    public int queue_cnt_retry { get; set; }

    public bool active_flag { get; set; }

    public int? created_by { get; set; }

    public DateTime created_date { get; set; }

    public int? updated_by { get; set; }

    public DateTime? update_date { get; set; }
}
