using System.Text.Json.Serialization;

namespace IletiMerkezi.Models
{
    public class SmsResponse : BaseResponse<SmsResponseData>
    {
        public int OrderId => int.TryParse(Response?.Data?.Order?.Id, out var id) ? id : 0;
    }

    public class SmsResponseData
    {
        [JsonPropertyName("order")]
        public Order Order { get; set; }
    }

    public class Order
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
} 