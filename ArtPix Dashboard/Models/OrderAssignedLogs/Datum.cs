using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.OrderAssignedLogs
{
    public class Datum
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("shipping_module")]
        public int ShippingModule { get; set; }

        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        [JsonProperty("order_shipping_status")]
        public string OrderShippingStatus { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("order")]
        public Order Order { get; set; }

        public int ShippedOrdersCount { get; set; }
    }
}
