using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store history of emails that have been sent
/// </summary>
public partial class his_email_history
{
    /// <summary>
    /// Unique identifier for each email history record
    /// </summary>
    public int hist_id { get; set; }

    /// <summary>
    /// Email address of the recipient
    /// </summary>
    public string hist_recipient { get; set; } = null!;

    /// <summary>
    /// Subject line of the sent email
    /// </summary>
    public string hist_subject { get; set; } = null!;

    /// <summary>
    /// Content/body of the sent email
    /// </summary>
    public string? hist_content { get; set; }

    /// <summary>
    /// Status of the email (e.g., New, Sent, Failed)
    /// </summary>
    public string hist_status { get; set; } = null!;

    /// <summary>
    /// Additional remarks related to the email sending process
    /// </summary>
    public string? hist_remark { get; set; }

    /// <summary>
    /// Timestamp when the email was sent
    /// </summary>
    public DateTime? date_sent { get; set; }

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
