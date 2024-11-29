using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IletiMerkezi.Models
{
    public class SummaryResponse : BaseResponse<SummaryResponseData>
    {
        public int Count => Response?.Data?.Count ?? 0;
        public List<OrderSummary> Orders => Response?.Data?.Orders ?? new List<OrderSummary>();
    }

    public class SummaryResponseData
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("orders")]
        public List<OrderSummary> Orders { get; set; } = new List<OrderSummary>();
    }

    public class OrderSummary
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        public string StatusText => TranslateOrderStatus(Status);

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("delivered")]
        public int Delivered { get; set; }

        [JsonPropertyName("undelivered")]
        public int Undelivered { get; set; }
        
        [JsonPropertyName("waiting")]
        public int Waiting { get; set; }
        
        [JsonPropertyName("submitAt")]
        public string SubmitAt { get; set; }
        
        [JsonPropertyName("sendAt")]
        public string SendAt { get; set; }
        
        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        private static string TranslateOrderStatus(int status)
        {
            if (status == 113) return "SENDING";
            if (status == 114) return "COMPLETED";
            if (status == 115) return "CANCELED";
            
            return status.ToString();
        }
    }
}