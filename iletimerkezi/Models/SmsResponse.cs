using System.Text.Json.Serialization;

namespace IletiMerkezi.Models
{
    public class SmsResponse : BaseResponse<SmsResponseData>
    {
        public int OrderId => Response?.Data?.Order?.Id ?? 0;
    }

    public class SmsResponseData
    {
        [JsonPropertyName("order")]
        public Order Order { get; set; }
    }

    public class Order
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
} 