using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Shipping
{
    public class Order
    {
        [JsonProperty("id_orders")]
        public int IdOrders { get; set; }

        [JsonProperty("name_order")]
        public string NameOrder { get; set; }

        [JsonProperty("store_order")]
        public int StoreOrder { get; set; }

        [JsonProperty("status_order")]
        public string StatusOrder { get; set; }

        [JsonProperty("status_engraving")]
        public string StatusEngraving { get; set; }

        [JsonProperty("status_shipping")]
        public string StatusShipping { get; set; }

        [JsonProperty("discount_total")]
        public double? DiscountTotal { get; set; }

        [JsonProperty("discount_tax")]
        public double? DiscountTax { get; set; }

        [JsonProperty("shipping_id")]
        public int? ShippingId { get; set; }

        [JsonProperty("shipping_total")]
        public double ShippingTotal { get; set; }

        [JsonProperty("shipping_tax")]
        public double? ShippingTax { get; set; }

        [JsonProperty("cart_tax")]
        public double? CartTax { get; set; }

        [JsonProperty("total_order")]
        public double TotalOrder { get; set; }

        [JsonProperty("total_tax")]
        public double TotalTax { get; set; }

        [JsonProperty("total_crystal")]
        public int TotalCrystal { get; set; }

        [JsonProperty("total_products")]
        public int TotalProducts { get; set; }

        [JsonProperty("order_created_at")]
        public string OrderCreatedAt { get; set; }

        [JsonProperty("customer_note")]
        public string CustomerNote { get; set; }

        [JsonProperty("id_customers")]
        public int IdCustomers { get; set; }

        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("payment_transaction_id")]
        public string PaymentTransactionId { get; set; }

        [JsonProperty("payment_date_gmt")]
        public string PaymentDateGmt { get; set; }

        [JsonProperty("shipping_method_id")]
        public string ShippingMethodId { get; set; }

        [JsonProperty("shipping_method_name")]
        public string ShippingMethodName { get; set; }

        [JsonProperty("ship_by")]
        public string ShipBy { get; set; }

        [JsonProperty("ship_by_min")]
        public string ShipByMin { get; set; }

        [JsonProperty("ship_by_max")]
        public string ShipByMax { get; set; }

        [JsonProperty("delivery_by")]
        public string DeliveryBy { get; set; }

        [JsonProperty("estimate_processing_max_date")]
        public string EstimateProcessingMaxDate { get; set; }

        [JsonProperty("estimate_delivery_min_date")]
        public string EstimateDeliveryMinDate { get; set; }

        [JsonProperty("estimate_delivery_max_date")]
        public string EstimateDeliveryMaxDate { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }

        [JsonProperty("shipping_type")]
        public string ShippingType { get; set; }

        [JsonProperty("is_redo")]
        public bool IsRedo { get; set; }
    }

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

    public class Links
    {
        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }

        [JsonProperty("prev")]
        public string Prev { get; set; }

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

    public class OrderAssignModel
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }


}
