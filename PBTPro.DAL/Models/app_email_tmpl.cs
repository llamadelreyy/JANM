using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store email templates for various messages
/// </summary>
public partial class app_email_tmpl
{
    /// <summary>
    /// Unique identifier for each email template
    /// </summary>
    public int tmpl_id { get; set; }

    /// <summary>
    /// Code representing the type of email template
    /// </summary>
    public string tmpl_code { get; set; } = null!;

    /// <summary>
    /// Subject line of the email template
    /// </summary>
    public string tmpl_subject { get; set; } = null!;

    /// <summary>
    /// Content/body of the email template
    /// </summary>
    public string? tmpl_content { get; set; }

    /// <summary>
    /// ID of the user who created the template record
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp when the template record was created
    /// </summary>
    public DateTime created_at { get; set; }

    /// <summary>
    /// ID of the user who last modified the template record
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when the template record was last modified
    /// </summary>
    public DateTime modified_at { get; set; }

    /// <summary>
    /// Flag indicating whether the email template record is marked as deleted (true/false)
    /// </summary>
    public bool is_deleted { get; set; }
}
