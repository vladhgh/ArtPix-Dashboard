using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Machine
{
    public class Machine
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id_for_api")]
        public string IdForApi { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public int JobsCount { get; set; }

        public string JobsCountColor => JobsCount < 3 ? "DarkRed" : JobsCount < 5 ? "DarkOrange" : "DarkGreen";
    }

    public class MachineModel
    {
        [JsonProperty("data")]
        public List<Machine> Data { get; set; }
    }
}
