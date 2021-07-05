using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Logs
{
    public class Data
    {
        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("points")]
        public string Points { get; set; }

        [JsonProperty("machine")]
        public string Machine { get; set; }

        [JsonProperty("machines")]
        public List<string> Machines { get; set; }

        [JsonProperty("file_name")]
        public object FileName { get; set; }

        [JsonProperty("order_ids")]
        public List<int> OrderIds { get; set; }

        [JsonProperty("laser_time")]
        public string LaserTime { get; set; }

        [JsonProperty("order_names")]
        public List<string> OrderNames { get; set; }

        [JsonProperty("product_ids")]
        public List<int> ProductIds { get; set; }

        [JsonProperty("count_in_product")]
        public int CountInProduct { get; set; }

        [JsonProperty("number_in_product")]
        public int NumberInProduct { get; set; }

        [JsonProperty("machine_assign_item_ids")]
        public List<int> MachineAssignItemIds { get; set; }
    }

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

    public class Links
    {
        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }

        [JsonProperty("prev")]
        public object Prev { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }
    }

    public class Meta
    {
        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("last_page")]
        public int LastPage { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("per_page")]
        public string PerPage { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

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
