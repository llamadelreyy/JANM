using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// List of races (biological)
/// </summary>
public partial class ref_race
{
    /// <summary>
    /// Primary key serves as the table&apos;s unique identifier.
    /// </summary>
    public int race_id { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string race_name { get; set; } = null!;

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
