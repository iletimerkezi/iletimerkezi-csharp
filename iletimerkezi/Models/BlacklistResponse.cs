using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace IletiMerkezi.Models
{
    public class BlacklistResponse : BaseResponse<BlacklistResponseData>
    {
        public int Count => Response?.Data?.Blacklist?.Count ?? 0;
        public List<string> Numbers => Response?.Data?.Blacklist?.Numbers ?? new List<string>();
    }

    public class BlacklistResponseData
    {
        [JsonPropertyName("blacklist")]
        public Blacklist Blacklist { get; set; }
    }

    public class Blacklist
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("number")]
        public List<string> Numbers { get; set; }
    }
}