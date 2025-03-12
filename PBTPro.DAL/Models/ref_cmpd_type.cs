using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores types of compounds available under PBT (e.g., Kompaun Atas Premis, Kompaun Lesen).
/// </summary>
public partial class ref_cmpd_type
{
    /// <summary>
    /// Unique identifier for each type of compound record (Primary Key).
    /// </summary>
    public int cmpd_type_id { get; set; }

    /// <summary>
    /// Code for the compound type (e.g., K12 - Kompaun Lesen).
    /// </summary>
    public string cmpd_type_code { get; set; } = null!;

    /// <summary>
    /// Name of the compound type (e.g., Kompaun Lesen).
    /// </summary>
    public string cmpd_type_name { get; set; } = null!;

    /// <summary>
    /// Description about the compound type.
    /// </summary>
    public string? cmpd_type_desc { get; set; }

    /// <summary>
    /// Logical delete flag indicating if the record is active or deleted.
    /// </summary>
    public bool? is_deleted { get; set; }

    /// <summary>
    /// User who created the record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// User who last updated the record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when the record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// Timestamp when the record was last updated.
    /// </summary>
    public DateTime? modified_at { get; set; }
}
