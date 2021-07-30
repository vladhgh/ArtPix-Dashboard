using System.Collections.Generic;
using ArtPix_Dashboard.Models.Common;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.EntityLogs
{
	public class EntityLogsModel
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
