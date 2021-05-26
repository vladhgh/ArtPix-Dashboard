using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ArtPix_Dashboard.Models.Types
{
	public class IssueReasonsModel
	{
		[JsonProperty("data")]
		public List<Datum> Data { get; set; }
	}
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
