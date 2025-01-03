using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// core.gender reference Male &amp; Female
/// </summary>
public partial class ref_gender
{
    /// <summary>
    /// User&apos;s unique username used for authentication.
    /// </summary>
    public int gen_id { get; set; }

    /// <summary>
    /// Gendel Name
    /// </summary>
    public string? gen_name { get; set; }

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
