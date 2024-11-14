using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;

namespace PBTPro.DAL.Models;

//public partial class TbAuditlog
//{
//    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Auditid")]
//    public int Auditid { get; set; }
//    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Perananid")]
//    public int? Perananid { get; set; } = 0;
//    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Jenisaudit")]
//    public int? Jenisaudit { get; set; }
//    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Namamodule")]
//    public string? Namamodule { get; set; } = null!;
//    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Username")]
//    public string? Username { get; set; } = null!;
//    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Method")]
//    public string? Method { get; set; } = null!;
//    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Catatan")]
//    public string? Catatan { get; set; }
//    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Rekciptauserid")]
//    public int? Rekciptauserid { get; set; }
//    [JsonProperty(PropertyName = "Rekcipta")]
//    public DateTime Rekcipta { get; set; } = DateTime.Today;
//}

//public enum AuditType
//{
//    [Description("Error")]
//    Error = 1,
//    [Description("Information")]
//    Information = 2
//}
//public enum AuditTypeLookup
//{
//    [Display(Name = "Ralat")]
//    Ralat = 1,
//    [Display(Name = "Informasi")]
//    Informasi = 2
//}
