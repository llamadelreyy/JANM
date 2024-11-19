using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models
{
    public class EmailProp
    {

        public int emailID { get; set; }
        [Required]
        public string smtpEmail { get; set; }
        [Required]
        public string smtpHost { get; set; }
        [Required]
        [RegularExpression("([0-9]+)")]
        public string smtpPort { get; set; }
        [Required]
        public string smtpUser { get; set; }
        [Required]
        public string smtpPassword { get; set; }
        public string smtpSender { get; set; }
        public bool smtpProtocol { get; set; }
        public bool smtpDefault { get; set; }
        public DateTime? rekCipta { get; set; }
        public int? rekCiptaUserID { get; set; }
        public DateTime? rekUbah { get; set; }
        public int? rekUbahUserID { get; set; }
        public string rekStatus { get; set; }
    }
}
