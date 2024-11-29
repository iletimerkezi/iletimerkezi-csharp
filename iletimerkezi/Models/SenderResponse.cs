using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IletiMerkezi.Models
{
    public class SenderResponse : BaseResponse<SenderResponseData>
    {
        public List<string> Senders => Response?.Data?.Senders?.Sender ?? new List<string>();
    }

    public class SenderResponseData
    {
        [JsonPropertyName("senders")]
        public SenderList Senders { get; set; }
    }

    public class SenderList
    {
        [JsonPropertyName("sender")]
        public List<string> Sender { get; set; } = new List<string>();
    }
}