using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class ref_faq
{
    public int faq_id { get; set; }

    public string faq_category { get; set; } = null!;

    public string faq_question { get; set; } = null!;

    public string faq_answer { get; set; } = null!;

    public string faq_status { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public int? creator_id { get; set; }

    public DateTime? modified_at { get; set; }

    public int? modifier_id { get; set; }

    public bool? is_deleted { get; set; }
}
