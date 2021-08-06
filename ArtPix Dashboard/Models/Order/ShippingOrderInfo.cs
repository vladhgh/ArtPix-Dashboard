using System;
using System.Collections.Generic;
using System.Globalization;
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

		private string _pickUpAt;

		[JsonProperty("pickup_at")]
		public string PickupAt
		{
			get
			{
				_pickUpAt = Utils.Utils.SelectDateText(_pickUpAt, false);
				return _pickUpAt;
			}
			set => _pickUpAt = value;
		}

		[JsonProperty("real_pickup_at")]
		public object RealPickupAt { get; set; }

		private string _deliveryAt;

		[JsonProperty("delivery_at")]
		public string DeliveryAt
	{
			get
			{
				_deliveryAt = String.IsNullOrEmpty(_deliveryAt) ? null : DateTime.Parse(_deliveryAt, CultureInfo.CurrentUICulture).ToString(CultureInfo.CurrentUICulture).Split(' ')[0];
				return _deliveryAt;
			}
			set => _deliveryAt = value;
		}

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
