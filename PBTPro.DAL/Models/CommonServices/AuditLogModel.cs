using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models.CommonServices
{
    public enum AuditType
    {
        [Description("Error")]
        Error = 1,
        [Description("Information")]
        Information = 2
    }
    public enum AuditTypeLookup
    {
        [Display(Name = "Ralat")]
        Ralat = 1,
        [Display(Name = "Informasi")]
        Informasi = 2
    }
}
