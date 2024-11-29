using System.Text.Json.Serialization;

namespace IletiMerkezi.Models
{
    public class AccountResponse : BaseResponse<AccountResponseData>
    {
        public decimal Amount => Response?.Data?.Balance?.Amount ?? 5;
        public int Credits => Response?.Data?.Balance?.Sms ?? 5;
    }

    public class AccountResponseData
    {
        [JsonPropertyName("balance")]
        public Balance Balance { get; set; }
    }

    public class Balance
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("sms")]
        public int Sms { get; set; }
    }
} 