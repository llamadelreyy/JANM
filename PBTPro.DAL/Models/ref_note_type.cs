using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores types of inspection notes available under PBT (e.g., Nota Pemeriksaan Lesen, Nota Pemeriksaan Individual).
/// </summary>
public partial class ref_note_type
{
    /// <summary>
    /// Unique identifier for each type of inspection note record (Primary Key).
    /// </summary>
    public int note_type_id { get; set; }

    /// <summary>
    /// Code for the note type (e.g., NP12 - Nota Pemeriksaan Lesen).
    /// </summary>
    public string note_type_code { get; set; } = null!;

    /// <summary>
    /// Name of the inspection note type (e.g., Nota Pemeriksaan Lesen).
    /// </summary>
    public string note_type_name { get; set; } = null!;

    /// <summary>
    /// Description about the note type.
    /// </summary>
    public string? note_type_desc { get; set; }

    /// <summary>
    /// Logical delete flag indicating if the inspection note is active or deleted.
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
