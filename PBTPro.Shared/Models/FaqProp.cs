using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.Shared.Models
{    
    public partial class FaqProp
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "faqID")]
        public int faqID { get; set; }

        [Required, MaxLength(255)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "kategoriFaq")]
        public string? kategoriFaq { get; set; }

        [Required, MaxLength(500)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "soalanFaq")]
        public string? soalanFaq { get; set; }

        [Required, MaxLength(500)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "jawapanFaq")]
        public string? jawapanFaq { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "rekStatus")]
        public string? rekStatus { get; set; }
        public DateTime? rekCipta { get; set; }
        public int rekCiptaUserID { get; set; }
        public DateTime? rekUbah { get; set; }
        public int rekUbahUserID { get; set; }
    }

}
