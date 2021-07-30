using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.FindBestService
{
	public class FindBestServiceModel
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("message")]
        public object Message { get; set; }
    }
}
