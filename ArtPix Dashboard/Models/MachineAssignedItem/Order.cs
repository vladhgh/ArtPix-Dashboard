using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.MachineAssignedItem
{
	[Serializable()]
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


		private string _estimateProcessingMaxDate;

		[JsonProperty("estimate_processing_max_date")]
		public string EstimateProcessingMaxDate
		{
			get
			{
				_estimateProcessingMaxDate = DateTime.Parse(_estimateProcessingMaxDate, CultureInfo.CurrentUICulture).AddHours(-5).ToString(CultureInfo.CurrentUICulture).Split(' ')[0];
				return _estimateProcessingMaxDate;
			}
			set => _estimateProcessingMaxDate = value;
		}

		[JsonProperty("estimate_delivery_min_date")]
		public string EstimateDeliveryMinDate { get; set; }

		private string _estimatedDeliveryMaxDate;

		[JsonProperty("estimate_delivery_max_date")]
		public string EstimateDeliveryMaxDate
		{
			get => DateTime.Parse(_estimatedDeliveryMaxDate, CultureInfo.CurrentUICulture).AddHours(-5).ToString(CultureInfo.CurrentUICulture).Split(' ')[0];
			set => _estimatedDeliveryMaxDate = value;
		}



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
}
