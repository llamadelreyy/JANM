using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PBTPro.Shared.Models.CommonService
{
    public class AppSystemMessageModel
    {
        public string Feature { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Message { get; set; }
    }

    public class PublishNotiToUser
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; } = null!;
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; } = null!;
        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = null!;
        public string? RefUrl { get; set; }
        public string? CustomField1 { get; set; }
        public string? CustomField2 { get; set; }
        public string? CustomField3 { get; set; }
        public string? CustomField4 { get; set; }
        public string? CustomField5 { get; set; }
        public string? CustomField6 { get; set; }
        public string? CustomField7 { get; set; }
        public string? CustomField8 { get; set; }
        public string? CustomField9 { get; set; }
        public string? CustomField10 { get; set; }
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
