using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Shipping
{
    public class Package
    {
        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("weight")]
        public double Weight { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

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

    public class Data
    {
        [JsonProperty("package")]
        public Package Package { get; set; }

        [JsonProperty("shipment")]
        public Shipment Shipment { get; set; }
    }

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
