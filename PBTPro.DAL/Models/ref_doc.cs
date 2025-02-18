using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class ref_doc
{
    public int doc_id { get; set; }

    public string? filename { get; set; }

    public string? pathurl { get; set; }

    public string? doc_cat { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }
}
