using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Stores information about different roles in the system, such as user roles and permissions.
/// </summary>
public partial class role
{
    /// <summary>
    /// Unique identifier for each role.
    /// </summary>
    public int role_id { get; set; }

    /// <summary>
    /// Meaningful name for the role (e.g., Admin, User, etc.).
    /// </summary>
    public string role_name { get; set; } = null!;

    /// <summary>
    /// Description providing additional information about the role and its responsibilities.
    /// </summary>
    public string? role_desc { get; set; }

    /// <summary>
    /// Flag indicating if this role is the default role for users in the system.
    /// </summary>
    public bool? is_default_role { get; set; }

    /// <summary>
    /// Flag indicating if this role is associated with tenant modules or multi-tenancy functionality.
    /// </summary>
    public bool? is_tenant { get; set; }

    /// <summary>
    /// Timestamp indicating when the role was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// ID of the user who created this role record.
    /// </summary>
    public int creator_id { get; set; }

    /// <summary>
    /// Timestamp indicating when the role record was last modified.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// ID of the user who last modified this role record.
    /// </summary>
    public int modifier_id { get; set; }

    /// <summary>
    /// Flag indicating whether the role record has been logically deleted (soft deleted). 
    ///         false means not deleted, true means deleted.
    /// </summary>
    public bool? is_deleted { get; set; }
}
