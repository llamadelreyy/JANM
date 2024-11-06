using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.Shared.Models
{
    public partial class HubungiKamiProp
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "hubkamiID")]
        public int hubkamiID { get; set; }

        [Required, MaxLength(15)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "tiketID")]
        public string? tiketID { get; set; }

        [Required, MaxLength(255)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "namaPenghantar")]
        public string? namaPenghantar { get; set; }

        [Required, MaxLength(255)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "emailPenghantar")]
        public string? emailPenghantar { get; set; }

        [Required, MaxLength(500)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "catatan")]
        public string? catatan { get; set; }

        [Required, MaxLength(255)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "namaPenerima")]
        public string? namaPenerima { get; set; }

        public DateTime rekCipta { get; set; }
        public int rekCiptaUserID { get; set; }
        public DateTime rekUbah { get; set; }
        public int rekUbahUserID { get; set; }

    }
}
