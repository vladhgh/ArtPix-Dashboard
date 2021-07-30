using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.EntityLogs
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
}
