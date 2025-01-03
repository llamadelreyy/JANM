using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Nationality of the user.
/// </summary>
public partial class ref_nationality
{
    /// <summary>
    /// A n identifier for each nationality, which serves as the primary key of the table.
    /// </summary>
    public int nat_id { get; set; }

    /// <summary>
    /// The name of the nationality
    /// </summary>
    public string nat_name { get; set; } = null!;

    /// <summary>
    /// Timestamp indicating when the row was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// User ID of the creator 
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp indicating when the user record was last modified.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// User ID of the modifier
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Flag indicating whether the row has been logically deleted (soft deleted). 0 represents not deleted, and 1 represents deleted.
    /// </summary>
    public bool? is_deleted { get; set; }
}
