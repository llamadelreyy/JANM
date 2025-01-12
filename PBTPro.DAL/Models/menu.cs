using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about the menus available in the system. It can be hierarchical, where each core.menu item may have a parent core.menu.
/// </summary>
public partial class menu
{
    /// <summary>
    /// Identifier for each core.menu item.
    /// </summary>
    public int menu_id { get; set; }

    /// <summary>
    /// Identifier for the module associated with the core.menu item.
    /// </summary>
    public int? module_id { get; set; }

    /// <summary>
    /// Identifier linking the core.menu item to its parent core.menu item (if applicable).
    /// </summary>
    public int parent_id { get; set; }

    /// <summary>
    /// Name of the core.menu item.
    /// </summary>
    [Required(ErrorMessage = "Ruangan Nama diperlukan.")]
    public string menu_name { get; set; } = null!;

    /// <summary>
    /// Sequence number indicating the order of the core.menu item.
    /// </summary>
    public int menu_sequence { get; set; }

    /// <summary>
    /// Path to the icon associated with the core.menu item.
    /// </summary>
    public string? icon_path { get; set; }

    /// <summary>
    /// Indicates whether the core.menu is associated with tenant modules.
    /// </summary>
    public bool? is_tenant { get; set; }

    /// <summary>
    /// Timestamp indicating when the row was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// User ID of the creator.
    /// </summary>
    public int creator_id { get; set; }

    /// <summary>
    /// Timestamp indicating when the user record was last modified.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// User ID of the modifier.
    /// </summary>
    public int modifier_id { get; set; }

    /// <summary>
    /// Flag indicating whether the row has been logically deleted (soft deleted). 0 represents not deleted, and 1 represents deleted.
    /// </summary>
    public bool is_deleted { get; set; }

    /// <summary>
    /// Path/Route to the UI with the core.menu item.
    /// </summary>
    public string? menu_path { get; set; }
}
