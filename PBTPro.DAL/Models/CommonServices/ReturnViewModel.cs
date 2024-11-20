using System.Text.Json.Serialization;

namespace PBTPro.DAL.Models.CommonServices
{
    public class ReturnViewModel
    {
        [JsonPropertyName("returnCode")]
        public int ReturnCode { get; set; }
        [JsonPropertyName("returnMessage")]
        public string ReturnMessage { get; set; }
        [JsonPropertyName("returnParam")]
        public List<string>? ReturnParameter { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("dateTime")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("data")]
        public object? Data { get; set; }
        [JsonPropertyName("pageInfo")]
        public PaginationInfo? PaginationInfo { get; set; }
        [JsonPropertyName("server")]
        public string Server { get; set; } = Environment.MachineName;
        public ReturnViewModel()
        {
            ReturnCode = 200;
            ReturnMessage = "";
            ReturnParameter = new List<string>();
            Status = "OK";
            DateTime = DateTime.Now;
            Data = "";
            Server = Environment.MachineName;
            PaginationInfo = new PaginationInfo();
        }
        public ReturnViewModel(int returnCode, string returnMessage, string status, DateTime dateTime, object result, List<string>? returnParameter = null)
        {
            ReturnCode = returnCode;
            ReturnMessage = returnMessage;
            Status = status;
            DateTime = dateTime;
            Data = result;
            ReturnParameter = returnParameter;
        }
    }
}
