using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.FindBestService
{
    public class Shipment
    {
        [JsonProperty("carrier")]
        public string Carrier { get; set; }

        [JsonProperty("service")]
        public string Service { get; set; }

        [JsonProperty("cost")]
        public double Cost { get; set; }

        [JsonProperty("pickup_at")]
        public string PickupAt { get; set; }

        [JsonProperty("delivery_at")]
        public string DeliveryAt { get; set; }

        [JsonProperty("saturday_delivery")]
        public bool SaturdayDelivery { get; set; }
    }
}
