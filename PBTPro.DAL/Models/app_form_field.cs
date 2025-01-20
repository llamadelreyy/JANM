using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store form templates (e.g., sitaan)
/// </summary>
public partial class app_form_field
{
    /// <summary>
    /// Unique identifier for each form field
    /// </summary>
    public int field_id { get; set; }

    /// <summary>
    /// Type of the form that this field belongs to
    /// </summary>
    public string field_form_type { get; set; } = null!;

    /// <summary>
    /// Name of the form field
    /// </summary>
    public string field_name { get; set; } = null!;

    /// <summary>
    /// Label displayed for the form field
    /// </summary>
    public string field_label { get; set; } = null!;

    /// <summary>
    /// Data type of the form field (e.g., text, number)
    /// </summary>
    public string field_type { get; set; } = null!;

    /// <summary>
    /// Options available for this field, if applicable
    /// </summary>
    public string? field_option { get; set; }

    /// <summary>
    /// URL for fetching data for this field, if applicable
    /// </summary>
    public string? field_source_url { get; set; }

    /// <summary>
    /// Indicates whether this field is mandatory (true/false)
    /// </summary>
    public bool field_required { get; set; }

    /// <summary>
    /// Indicates if this field is populated by an API (true/false)
    /// </summary>
    public bool field_api_seeded { get; set; }

    /// <summary>
    /// Order in which this field appears in the form
    /// </summary>
    public int field_orders { get; set; }

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
    public DateTime modified_at { get; set; }

    /// <summary>
    /// Flag indicating whether the record is marked as deleted (true/false)
    /// </summary>
    public bool is_deleted { get; set; }
}
