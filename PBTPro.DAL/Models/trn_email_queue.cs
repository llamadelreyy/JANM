using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to manage a queue for sending emails
/// </summary>
public partial class trn_email_queue
{
    /// <summary>
    /// Unique identifier for each email queue record
    /// </summary>
    public int queue_id { get; set; }

    /// <summary>
    /// Email address of the recipient in the queue
    /// </summary>
    public string queue_recipient { get; set; } = null!;

    /// <summary>
    /// Subject line of the email in the queue
    /// </summary>
    public string queue_subject { get; set; } = null!;

    /// <summary>
    /// Content/body of the email in the queue
    /// </summary>
    public string? queue_content { get; set; }

    /// <summary>
    /// Current status of the email in the queue (e.g., New, Sent, Failed)
    /// </summary>
    public string queue_status { get; set; } = null!;

    /// <summary>
    /// Additional remarks related to this email queue entry
    /// </summary>
    public string? queue_remark { get; set; }

    /// <summary>
    /// Timestamp when the email was sent or scheduled to be sent
    /// </summary>
    public DateTime date_sent { get; set; }

    /// <summary>
    /// Count of how many times sending this email has been retried
    /// </summary>
    public int cnt_retry { get; set; }

    /// <summary>
    /// ID of the user who created this record
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp when this record was created
    /// </summary>
    public DateTime created_at { get; set; }

    /// <summary>
    /// ID of the user who last modified this record
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when this record was last modified
    /// </summary>
    public DateTime modified_at { get; set; }

    /// <summary>
    /// Flag indicating whether this record is marked as deleted (true/false)
    /// </summary>
    public bool is_deleted { get; set; }
}
