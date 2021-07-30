using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.IssueReasons
{
    public class Datum
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("stage")]
        public string Stage { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
