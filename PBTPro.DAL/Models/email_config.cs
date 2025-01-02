using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models
{
    public class email_config
    {

        public int email_id { get; set; }
        [Required]
        public string smtp_email { get; set; }
        [Required]
        public string smtp_host { get; set; }
        [Required]
        [RegularExpression("([0-9]+)")]
        public string smtp_port { get; set; }
        [Required]
        public string smtp_user { get; set; }
        [Required]
        public string smtp_password { get; set; }
        public string smtp_sender { get; set; }
        public bool smtp_protocol { get; set; }
        public bool smtp_default { get; set; }
        public DateTime? rek_cipta { get; set; }
        public int? rek_cipta_user_id { get; set; }
        public DateTime? rek_ubah { get; set; }
        public int? rek_ubah_user_id { get; set; }
        public string rek_status { get; set; }
    }
}
