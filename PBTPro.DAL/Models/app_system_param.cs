using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Table to store configuration parameters for applications in the core layer
/// </summary>
public partial class app_system_param
{
    /// <summary>
    /// Unique identifier for each configuration parameter
    /// </summary>
    public int param_id { get; set; }

    /// <summary>
    /// Layer of the application (e.g., frontend, backend)
    /// </summary>
    public string app_layer { get; set; } = null!;

    /// <summary>
    /// Group categorizing the parameter (e.g., database, API)
    /// </summary>
    public string param_group { get; set; } = null!;

    /// <summary>
    /// Name of the configuration parameter
    /// </summary>
    public string param_name { get; set; } = null!;

    /// <summary>
    /// Value assigned to the configuration parameter
    /// </summary>
    public string param_value { get; set; } = null!;

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
