using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store return messages from API related to changes
/// </summary>
public partial class app_system_msg
{
    /// <summary>
    /// Unique identifier for each message
    /// </summary>
    public int message_id { get; set; }

    /// <summary>
    /// Code representing the type of message
    /// </summary>
    public string message_code { get; set; } = null!;

    /// <summary>
    /// Type of message (e.g., info, warning, error)
    /// </summary>
    public string message_type { get; set; } = null!;

    /// <summary>
    /// Feature or component associated with the message
    /// </summary>
    public string message_feature { get; set; } = null!;

    /// <summary>
    /// The actual content of the message
    /// </summary>
    public string? message_body { get; set; }

    /// <summary>
    /// ID of the user who created the record
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp when the record was created
    /// </summary>
    public DateTime created_at { get; set; }

    /// <summary>
    /// ID of the user who last modified the record
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when the record was last modified
    /// </summary>
    public DateTime modified_date { get; set; }

    /// <summary>
    /// Flag indicating whether the record is marked as deleted (true/false)
    /// </summary>
    public bool? is_deleted { get; set; }
}
