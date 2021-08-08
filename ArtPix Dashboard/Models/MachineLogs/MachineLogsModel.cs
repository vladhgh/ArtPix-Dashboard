using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtPix_Dashboard.Models.MachineLogs
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Raw
    {
        [JsonProperty("SKU")]
        public string SKU { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("id_orders")]
        public string IdOrders { get; set; }

        [JsonProperty("machine_id")]
        public string MachineId { get; set; }

        [JsonProperty("copy_number")]
        public string CopyNumber { get; set; }

        [JsonProperty("id_products")]
        public string IdProducts { get; set; }

        [JsonProperty("Laser_Time")]
        public string LaserTime { get; set; }

        [JsonProperty("points")]
        public string Points { get; set; }
    }

    public class Datum
    {
        [JsonProperty("id_machine_logs")]
        public int IdMachineLogs { get; set; }

        [JsonProperty("machine_id")]
        public int MachineId { get; set; }

        [JsonProperty("id_products")]
        public int? IdProducts { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }

        [JsonProperty("message")]
        public object Message { get; set; }

        [JsonProperty("copy_number")]
        public int CopyNumber { get; set; }

        [JsonProperty("raw")]
        public Raw Raw { get; set; }

        [JsonProperty("machine_user")]
        public string MachineUser { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("type_logs")]
        public string TypeLogs { get; set; }

        [JsonProperty("type_logs_level")]
        public string TypeLogsLevel { get; set; }
    }

    public class Links
    {
        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("last")]
        public object Last { get; set; }

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

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("per_page")]
        public string PerPage { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }
    }

    public class MachineLogsModel
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }


}
