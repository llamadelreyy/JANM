using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Stores images associated with confiscation records
/// </summary>
public partial class trn_cfsc_img
{
    /// <summary>
    /// Unique identifier for the confiscation image record
    /// </summary>
    public int cfsc_img_id { get; set; }

    /// <summary>
    /// Foreign key to the trn_confiscation table
    /// </summary>
    public int? trn_cfsc_id { get; set; }

    /// <summary>
    /// Original filename of the image
    /// </summary>
    public string? filename { get; set; }

    /// <summary>
    /// URL or file path where the image is stored
    /// </summary>
    public string pathurl { get; set; } = null!;

    /// <summary>
    /// Optional description of the image
    /// </summary>
    public string? desc { get; set; }

    /// <summary>
    /// ID of the user who created this record.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// ID of the user who last modified this record.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when this record was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// Timestamp when this record was last modified.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// Flag indicating whether this record is deleted (soft delete).
    /// </summary>
    public bool? is_deleted { get; set; }
}
