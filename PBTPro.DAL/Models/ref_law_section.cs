using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models;

public partial class ref_law_section
{
    public int section_id { get; set; }

    [Required(ErrorMessage = "Ruangan Akta diperlukan.")]
    public string act_code { get; set; } = null!;

    [Required(ErrorMessage = "Ruangan Kod diperlukan.")]
    public string section_code { get; set; } = null!;

    [Required(ErrorMessage = "Ruangan Nama diperlukan.")]
    public string section_name { get; set; } = null!;

    public string? section_description { get; set; }

    public DateTime? created_at { get; set; }

    public int? creator_id { get; set; }

    public DateTime? modified_at { get; set; }

    public int? modifier_id { get; set; }

    public bool is_deleted { get; set; }
}
