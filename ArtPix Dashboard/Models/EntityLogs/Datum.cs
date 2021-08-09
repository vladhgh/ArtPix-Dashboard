using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.EntityLogs
{
    public class Datum
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("entity_id")]
        public int EntityId { get; set; }

        [JsonProperty("entity_type")]
        public string EntityType { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        
        [JsonProperty("event_date")]
        public string EventDate { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
    }
}
