using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// Stores images associated with compound records
/// </summary>
public partial class trn_cmpd_img
{
    /// <summary>
    /// Unique identifier for the compound image record
    /// </summary>
    public int cmpd_img_id { get; set; }

    /// <summary>
    /// Foreign key to the trn_compounds table
    /// </summary>
    public int? trn_cmpd_id { get; set; }

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

    public int? creator_id { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }
}
