using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.ProductionIssue
{
    public class Order
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("estimate_delivery_max_date")]
        public string EstimateDeliveryMaxDate { get; set; }

        private string _shipping_type;
        [JsonProperty("shipping_type")]
        public string ShippingType
        {
            get
            {
                switch (_shipping_type)
                {
                    case "free_shipping": return "Free Shipping";
                    case "standard": return "Standard";
                    case "economy": return "Economy";
                    case "express": return "Express";
                    case "high_priority_express": return "High Priority Express";
                    case "local_pickup": return "Pick-Up";
                    case "amazon": return "Amazon";
                    case "amazon_standard": return "Amazon Standard";
                    case "amazon_expedited": return "Amazon Expedited";
                    default: return "Not Found";
                }
            }
            set => _shipping_type = value;
        }
        public string ShippingTypeColor
        {
            get
            {
                switch (_shipping_type)
                {
                    case "free_shipping": return "Gray";
                    case "standard": return "Green";
                    case "economy": return "Green";
                    case "express": return "Red";
                    case "high_priority_express": return "Red";
                    case "local_pickup": return "Brown";
                    case "amazon": return "Orange";
                    case "amazon_standard": return "Green";
                    case "amazon_expedited": return "Orange";
                    default: return "White";
                }
            }
        }
    }
}
