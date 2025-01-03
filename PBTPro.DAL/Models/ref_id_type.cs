using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// The ucIDTypes is reference table to store type of ID such as IC number, passpord etc.
/// </summary>
public partial class ref_id_type
{
    /// <summary>
    /// Type of ID
    /// </summary>
    public int id_type_id { get; set; }

    /// <summary>
    /// User&apos;s unique username used for authentication.
    /// </summary>
    public string id_type_name { get; set; } = null!;

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
