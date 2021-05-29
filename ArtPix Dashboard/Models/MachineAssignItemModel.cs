using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace ArtPix_Dashboard.Models.Machine
{
    public class Product
    {
        [JsonProperty("id_products")]
        public int IdProducts { get; set; }

        [JsonProperty("id_orders")]
        public int IdOrders { get; set; }

        [JsonProperty("id_store_products")]
        public string IdStoreProducts { get; set; }

        [JsonProperty("id_variation_store")]
        public string IdVariationStore { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("convert_to_3d")]
        public int ConvertTo3d { get; set; }

        [JsonProperty("url_original_img")]
        public string UrlOriginalImg { get; set; }

        [JsonProperty("url_render_img")]
        public string UrlRenderImg { get; set; }

        [JsonProperty("url_shape_img")]
        public string UrlShapeImg { get; set; }

        [JsonProperty("url_original_render")]
        public string UrlOriginalRender { get; set; }

        [JsonProperty("url_outside_render")]
        public string UrlOutsideRender { get; set; }

        [JsonProperty("customer_font")]
        public string CustomerFont { get; set; }

        [JsonProperty("customer_engraving")]
        public string CustomerEngraving { get; set; }

        [JsonProperty("crystal_position")]
        public string CrystalPosition { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("editor_id")]
        public string EditorId { get; set; }

        [JsonProperty("use_optimize")]
        public bool UseOptimize { get; set; }

        [JsonProperty("url_optimize_img")]
        public string UrlOptimizeImg { get; set; }

        [JsonProperty("number_in_order")]
        public int NumberInOrder { get; set; }

        [JsonProperty("id_store_line_items")]
        public object IdStoreLineItems { get; set; }

        [JsonProperty("tax")]
        public double Tax { get; set; }

        [JsonProperty("packing_service_product_ids")]
        public List<int> PackingServiceProductIds { get; set; }

        [JsonProperty("is_virtual")]
        public bool IsVirtual { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        public string UpdatedAtAge
        {
            get
            {
                var today = DateTime.Now;
                var lastUpdated = DateTime.Parse(UpdatedAt, CultureInfo.CurrentUICulture);
                var diff = today - lastUpdated;
                if (diff.Days > 1)
                {
                    return $"{diff.Days} days";
                }

                if (diff.Days == 1)
                {
                    return $"{diff.Days} days {diff.Hours} hours";
                }

                if (diff.Days < 1 && diff.Hours > 1)
                {
                    return $"{diff.Hours} hours";
                }

                if (diff.Hours == 1)
                {
                    return $"{diff.Hours} hour";
                }

                return diff.Hours < 1 ? $"{diff.Minutes} minutes" : "nan";
            }
        }

        public string UpdatedAtAgeColor
        {
            get
            {
                var today = DateTime.Now;
                var lastUpdated = DateTime.Parse(UpdatedAt, CultureInfo.CurrentUICulture);
                var diff = today - lastUpdated;
                if (diff.Days > 1)
                {
                    return "DarkRed";
                }

                if (diff.Days == 1)
                {
                    return "DarkOrange";
                }

                if (diff.Days < 1)
                {
                    return "Gray";
                }

                if (diff.Hours == 1)
                {
                    return "Gray";
                }

                return diff.Hours < 1 ? "Gray" : "nan";
            }
        }

        private string _updatedAt;

        [JsonProperty("updated_at")]
        public string UpdatedAt
        {
            get
            {
                var lastUpdated = DateTime.Parse(_updatedAt, CultureInfo.CurrentUICulture);
                return lastUpdated.AddHours(-5).ToString(CultureInfo.CurrentUICulture);
            }
            set => _updatedAt = value;
        }
    }

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
        public string StatusOrderColor
        {
	        get
	        {
		        switch (StatusOrder)
		        {
			        case "processing": return "SteelBlue";
			        case "completed": return "Green";
                    default: return "Gray";

		        }
	        }
        }

        [JsonProperty("status_engraving")]
        public string StatusEngraving { get; set; }
        public string StatusEngravingColor
        {
	        get
	        {
		        switch (StatusEngraving)
		        {
			        case "wait_models": return "DarkOrange";
			        case "done": return "Green";
			        case "error": return "Red";
			        case "processing": return "SteelBlue";
			        default: return "Gray";

		        }
	        }
        }

        [JsonProperty("status_shipping")]
        public string StatusShipping { get; set; }

        public string StatusShippingColor
        {
	        get
	        {
		        switch (StatusShipping)
		        {
			        case "waiting": return "DarkOrange";
			        case "processing": return "SteelBlue";
                    case "done": return "Green";
                    default: return "Gray";

		        }
	        }
        }

        [JsonProperty("discount_total")]
        public int? DiscountTotal { get; set; }

        [JsonProperty("discount_tax")]
        public int? DiscountTax { get; set; }

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

        private string _shippingType;
        [JsonProperty("shipping_type")]
        public string ShippingType
        {
	        get
	        {
		        switch (_shippingType)
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
	        set => _shippingType = value;
        }
        public string ShippingTypeColor
        {
	        get
	        {
		        switch (_shippingType)
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

        [JsonProperty("is_redo")]
        public bool IsRedo { get; set; }
    }

    public class Datum
    {

	    #region VISIBILITY

	    public Visibility AssignMachineButtonVisibility => Status == "ready_to_engrave" || Status == "engrave_redo"
		    ? Visibility.Visible
		    : Visibility.Collapsed;
	    public Visibility IsActionNeeded
	    {
		    get
		    {
			    var today = DateTime.Now.Date.AddHours(-12);
			    var lastUpdated = DateTime.Parse(UpdatedAt, CultureInfo.InvariantCulture);
			    return lastUpdated < today ? Visibility.Visible : Visibility.Hidden;

		    }
	    }
        #endregion


        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("machine_assign_id")]
        public int MachineAssignId { get; set; }

        [JsonProperty("machine_id")]
        public string MachineId { get; set; }

        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        [JsonProperty("order_name")]
        public string OrderName { get; set; }

        [JsonProperty("started_at")]
        public string StartedAt { get; set; }

        [JsonProperty("ended_at")]
        public object EndedAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("machine_slot")]
        public object MachineSlot { get; set; }

        [JsonProperty("is_qr_printed")]
        public bool IsQrPrinted { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        public string UpdatedAtAge
        {
            get
            {
                var today = DateTime.Now;
                var lastUpdated = DateTime.Parse(UpdatedAt, CultureInfo.CurrentUICulture);
                var diff = today - lastUpdated;
                if (diff.Days > 1)
                {
                    return $"{diff.Days} days";
                }

                if (diff.Days == 1)
                {
                    return $"{diff.Days} days {diff.Hours} hours";
                }

                if (diff.Days < 1 && diff.Hours > 1)
                {
                    return $"{diff.Hours} hours";
                }

                if (diff.Hours == 1)
                {
                    return $"{diff.Hours} hour";
                }

                return diff.Hours < 1 ? $"{diff.Minutes} minutes" : "nan";
            }
        }

        public string UpdatedAtAgeColor
        {
            get
            {
                var today = DateTime.Now;
                var lastUpdated = DateTime.Parse(UpdatedAt, CultureInfo.CurrentUICulture);
                var diff = today - lastUpdated;
                if (diff.Days > 1)
                {
                    return "DarkRed";
                }

                if (diff.Days == 1)
                {
                    return "DarkOrange";
                }

                if (diff.Days < 1)
                {
                    return "Gray";
                }

                if (diff.Hours == 1)
                {
                    return "Gray";
                }

                return diff.Hours < 1 ? "Gray" : "nan";
            }
        }

        private string _updatedAt;

        [JsonProperty("updated_at")]
        public string UpdatedAt
        {
            get
            {
                var lastUpdated = DateTime.Parse(_updatedAt, CultureInfo.CurrentUICulture);
                return lastUpdated.AddHours(-5).ToString(CultureInfo.CurrentUICulture);
            }
            set => _updatedAt = value;
        }

        [JsonProperty("product")]
        public Product Product { get; set; }

        [JsonProperty("order")]
        public Order Order { get; set; }

        [JsonProperty("shipping_tote")]
        public object ShippingTote { get; set; }

        [JsonProperty("place")]
        public object Place { get; set; }

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
        public int PerPage { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class MachineAssignItemModel
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

}