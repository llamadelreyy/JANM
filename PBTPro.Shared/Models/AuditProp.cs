using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.Shared.Models
{
    public partial class AuditProp
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "auditID")]
        public int auditID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "perananID")]
        public int perananID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "jenisAudit")]
        public int jenisAudit { get; set; }       

        [Required, MaxLength(255)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "namaModule")]
        public string? namaModule { get; set; }

        [Required, MaxLength(255)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "method")]
        public string? method { get; set; }

        [Required, MaxLength(500)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "catatan")]
        public string? catatan { get; set; }

        public DateTime rekCipta { get; set; }
        public int rekCiptaUserID { get; set; }
    }
}
