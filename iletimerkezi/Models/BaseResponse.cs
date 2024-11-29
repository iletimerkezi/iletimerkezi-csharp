using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;

namespace IletiMerkezi.Models
{
    public abstract class BaseResponse<T> where T : class
    {
        [JsonPropertyName("response")]
        public ResponseData<T> Response { get; set; }

        public bool Ok => Response?.Status?.Code == 200;
        
        public string Message => Response?.Status?.Message;
        
        public int StatusCode => Response?.Status?.Code ?? 0;
    }

    public class ResponseData<T> where T : class
    {
        [JsonPropertyName("status")]
        public Status Status { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> AdditionalData { get; set; }

        [JsonIgnore]
        public T Data => GetData();

        protected virtual T GetData()
        {
            if (AdditionalData == null) return null;
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var json = JsonSerializer.Serialize(AdditionalData);
            return JsonSerializer.Deserialize<T>(json, options);
        }
    }

    public class Status
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
} 