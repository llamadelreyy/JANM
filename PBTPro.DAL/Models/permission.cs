using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// The core.permission table manages permissions for various features within the system. It associates roles with specific features and defines the level of access each role has for those features.
/// </summary>
public partial class permission
{
    /// <summary>
    /// Unique identifier for each core.permission record.
    /// </summary>
    public int permission_id { get; set; }

    /// <summary>
    /// Identifier for the role associated with the core.permission.
    /// </summary>
    public int role_id { get; set; }

    /// <summary>
    /// Identifier for the feature to which the core.permission applies.
    /// </summary>
    public int menu_id { get; set; }

    /// <summary>
    /// Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.
    /// </summary>
    public bool can_view { get; set; }

    /// <summary>
    /// Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.
    /// </summary>
    public bool can_add { get; set; }

    /// <summary>
    /// Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.
    /// </summary>
    public bool can_delete { get; set; }

    /// <summary>
    /// Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.
    /// </summary>
    public bool can_edit { get; set; }

    /// <summary>
    /// Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.
    /// </summary>
    public bool can_print { get; set; }

    /// <summary>
    /// Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.
    /// </summary>
    public bool can_download { get; set; }

    /// <summary>
    /// Flags indicating whether the role associated with the core.permission has specific access rights for the corresponding feature.
    /// </summary>
    public bool can_upload { get; set; }

    /// <summary>
    /// This column indicates whether a user with the specified role can execute or perform actions associated with a feature. It can be useful for scenarios where viewing, adding, editing, or deleting actions need to be restricted separately.
    /// </summary>
    public bool can_execute { get; set; }

    /// <summary>
    /// If your system includes approval processes, this column specifies whether users with the specified role have the authority to approve or authorize certain actions or transactions for themselves or as supervisors.
    /// </summary>
    public bool can_authorize { get; set; }

    /// <summary>
    /// For systems dealing with sensitive information, this column can control whether users with the specified role are allowed to view sensitive data.
    /// </summary>
    public bool can_view_sensitive { get; set; }

    /// <summary>
    /// If your system involves exporting data, this column can specify whether users with the specified role can export data from the system.
    /// </summary>
    public bool can_export_data { get; set; }

    /// <summary>
    /// Similarly, if data import functionality exists, this column can determine whether users with the specified role can import data into the system
    /// </summary>
    public bool can_import_data { get; set; }

    /// <summary>
    /// In systems where changes need approval before implementation, this column can specify whether users with the specified role can approve proposed changes.
    /// </summary>
    public bool can_approve_changes { get; set; }

    /// <summary>
    /// Timestamp indicating when the row was created.
    /// </summary>
    public DateOnly? created_at { get; set; }

    /// <summary>
    /// User ID of the creator 
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp indicating when the user record was last modified.
    /// </summary>
    public DateOnly? modified_at { get; set; }

    /// <summary>
    /// User ID of the modifier
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Flag indicating whether the row has been logically deleted (soft deleted). 0 represents not deleted, and 1 represents deleted.
    /// </summary>
    public bool is_deleted { get; set; }
}
