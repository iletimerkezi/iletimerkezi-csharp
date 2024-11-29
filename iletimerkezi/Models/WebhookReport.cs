using System.Text.Json.Serialization;

namespace IletiMerkezi.Models
{
    public class WebhookReport
    {
        [JsonPropertyName("report")]
        public ReportData Report { get; set; }

        public int Id => Report.Id;
        public int PacketId => Report.PacketId;
        public string Status => Report?.Status;
        public string To => Report?.To;
        public string Body => Report?.Body;

        public bool IsDelivered => Status == "delivered";
        public bool IsAccepted => Status == "accepted";
        public bool IsUndelivered => Status == "undelivered";
    }

    public class ReportData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("packet_id")]
        public int PacketId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }
    }
} 