using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	public class ShippingOrderInfo
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("order_id")]
		public int OrderId { get; set; }

		[JsonProperty("shipping_package_id")]
		public int ShippingPackageId { get; set; }

		[JsonProperty("provider")]
		public object Provider { get; set; }

		[JsonProperty("real_provider")]
		public object RealProvider { get; set; }

		[JsonProperty("service")]
		public object Service { get; set; }

		[JsonProperty("real_service")]
		public object RealService { get; set; }

		[JsonProperty("cost")]
		public object Cost { get; set; }

		[JsonProperty("real_cost")]
		public object RealCost { get; set; }

		[JsonProperty("pickup_at")]
		public object PickupAt { get; set; }

		[JsonProperty("real_pickup_at")]
		public object RealPickupAt { get; set; }

		[JsonProperty("delivery_at")]
		public object DeliveryAt { get; set; }

		[JsonProperty("real_delivery_at")]
		public object RealDeliveryAt { get; set; }

		[JsonProperty("provider_estimate_delivery_at")]
		public object ProviderEstimateDeliveryAt { get; set; }

		[JsonProperty("tracking_number")]
		public object TrackingNumber { get; set; }

		[JsonProperty("label_created_at")]
		public object LabelCreatedAt { get; set; }

		[JsonProperty("shipping_package")]
		public ShippingPackage ShippingPackage { get; set; }
	}
}
