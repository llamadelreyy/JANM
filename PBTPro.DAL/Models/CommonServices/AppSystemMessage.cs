using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models.CommonServices
{
    public class config_system_message_view
    {
        public string Feature { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Message { get; set; }
    }

    public enum MessageTypeEnum
    {
        [Description("Notification-Success")] //0
        Success,
        [Description("Notification-Warning")] //1
        Warning,
        [Description("Notification-Error")] //2
        Error,
        [Description("Notification-Browser Alert")] //2
        Alert
    }
}
