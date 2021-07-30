using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.FindBestService
{
    public class Data
    {
        [JsonProperty("package")]
        public Package Package { get; set; }

        [JsonProperty("shipment")]
        public Shipment Shipment { get; set; }
    }
}
