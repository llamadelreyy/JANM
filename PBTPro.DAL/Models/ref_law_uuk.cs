using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models;

public partial class ref_law_uuk
{
    public int uuk_id { get; set; }

    public string? act_code { get; set; }

    public string? section_code { get; set; }

    [Required(ErrorMessage = "Ruangan Kod diperlukan.")]
    public string uuk_code { get; set; } = null!;

    [Required(ErrorMessage = "Ruangan Nama diperlukan.")]
    public string uuk_name { get; set; } = null!;

    public string? uuk_description { get; set; }

    public DateTime? created_at { get; set; }

    public int? creator_id { get; set; }

    public DateTime? modified_at { get; set; }

    public int? modifier_id { get; set; }

    public bool is_deleted { get; set; }
}
