using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class faq_info
{
    public int faq_id { get; set; }

    public string faq_category { get; set; } = null!;

    public string faq_question { get; set; } = null!;

    public string faq_answer { get; set; } = null!;

    public string faq_status { get; set; } = null!;

    public DateTime? created_date { get; set; }

    public int? created_by { get; set; }

    public DateTime? updated_date { get; set; }

    public int? updated_by { get; set; }
    //public string IconCssClass { get; set; }
    //public string BadgeCssClass { get; set; }
    //public List<faq_info> Items { get; set; }
}
