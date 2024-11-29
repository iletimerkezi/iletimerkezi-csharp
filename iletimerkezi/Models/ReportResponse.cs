using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IletiMerkezi.Models
{
    public class ReportResponse : BaseResponse<ReportResponseData>
    {
        public int Id => Response?.Data?.Order?.Id ?? 0;
        public int Status => Response?.Data?.Order?.Status ?? 113;
        public string StatusText => Response?.Data?.Order?.StatusText ?? "";
        public int Total => Response?.Data?.Order?.Total ?? 0;
        public int Delivered => Response?.Data?.Order?.Delivered ?? 0;
        public int Undelivered => Response?.Data?.Order?.Undelivered ?? 0;
        public int Waiting => Response?.Data?.Order?.Waiting ?? 0;
        public string SubmitAt => Response?.Data?.Order?.SubmitAt ?? "";
        public string SendAt => Response?.Data?.Order?.SendAt ?? "";
        public string Sender => Response?.Data?.Order?.Sender ?? "";
        public List<MessageReport> Messages => Response?.Data?.Order?.Messages ?? new List<MessageReport>();
    }

    public class ReportResponseData
    {
        [JsonPropertyName("order")]
        public OrderReport Order { get; set; }
    }

    public class OrderReport
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

        [JsonPropertyName("message")]
        public List<MessageReport> Messages { get; set; } = new List<MessageReport>();

        private static string TranslateOrderStatus(int status)
        {
            if (status == 113) return "SENDING";
            if (status == 114) return "COMPLETED";
            if (status == 115) return "CANCELED";
            
            return status.ToString();
        }
    }

    public class MessageReport
    {
        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        public string StatusText => TranslateMessageStatus(Status);

        private static string TranslateMessageStatus(int status)
        {
            if (status == 110) return "WAITING";
            if (status == 111) return "DELIVERED";
            if (status == 112) return "UNDELIVERED";
            
            return status.ToString();
        }
    }
}