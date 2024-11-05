using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PBTPro.Shared.Models;

public partial class Action
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Actionid")]
    public int Actionid { get; set; }
    [Required, MaxLength(255)]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Actionname")]
    public string? Actionname { get; set; }
    [Required, MaxLength(255)]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Actiondescription")]
    public string? Actiondescription { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Actionenabled")]
    public bool? Actionenabled { get; set; }
}
