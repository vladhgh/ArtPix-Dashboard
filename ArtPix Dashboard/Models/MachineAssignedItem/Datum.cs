using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.Utils.Helpers;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.MachineAssignedItem
{
	[Serializable()]
	public class Datum : PropertyChangedListener
	{

		#region VISIBILITY

		public Visibility AssignMachineButtonVisibility => Status == "ready_to_engrave" || Status == "engrave_redo"
			? Visibility.Visible
			: Visibility.Collapsed;
		public Visibility CrystalIssueButtonVisibility => Status == "engrave_processing" ? Visibility.Visible : Visibility.Collapsed;
		public Visibility UnassignButtonVisibility => string.IsNullOrEmpty(MachineId) ? Visibility.Collapsed : Visibility.Visible;
		public Visibility MachineButtonVisibility =>
			string.IsNullOrEmpty(MachineId) ? Visibility.Collapsed : Visibility.Visible;
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


		public Visibility IsLateShipment => Visibility.Collapsed;
		public Visibility IssueResolved => Visibility.Collapsed;

		public Visibility EngravedAgeVisibility => Visibility.Visible;
		public Visibility UpdatedAtAgeVisibility => Status == "Shipped" || Status == "Engraving Done" ? Visibility.Collapsed : Visibility.Visible;

		public Visibility TrackingNumberTextBlockVisibility => Visibility.Collapsed;

		public BitmapImage OrderImage
		{
			get
			{
				var bmp = new BitmapImage();
				bmp.BeginInit();
				bmp.CacheOption = BitmapCacheOption.OnLoad;
				bmp.DecodePixelWidth = 100;
				bmp.UriSource = new Uri(Product.UrlRenderImg, UriKind.RelativeOrAbsolute);
				bmp.EndInit();
				return bmp;
			}

		}

		public string NameOrder => OrderName;

		public Visibility ProductStatusVisibility => Visibility.Collapsed;

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

		private string _status;

		[JsonProperty("status")]
		public string Status
		{
			get => Utils.Utils.SelectStatusText(_status);
			set => SetProperty(ref _status, value);
		}

		private bool _isExpanded;

		public bool IsExpanded
		{
			get => _isExpanded;
			set => SetProperty(ref _isExpanded, value);
		}

		public string ShippingType
		{
			get
			{
				switch (Order.ShippingType)
				{
					case "free_shipping": return "Free Shipping";
					case "standard": return "Standard Shipping";
					case "economy": return "Economy Shipping";
					case "express": return "Express Shipping";
					case "high_priority_express": return "High Priority Express Shipping";
					case "local_pickup": return "Fashion Outlets Pickup";
					case "amazon": return "Amazon Free Shipping";
					case "amazon_standard": return "Amazon Standard Shipping";
					case "amazon_expedited": return "Amazon Expedited Shipping";
					case "amazon_second_day": return "Amazon Second Day Shipping";
					case "amazon_next_day": return "Amazon Next Day Shipping";
					case "dhl_parcel_direct": return "DHL Parcel Direct Shipping";
					default: return Order.ShippingType;
				}
			}
		}
		public string ShippingTypeColor
		{
			get
			{
				switch (Order.ShippingType)
				{
					case "free_shipping": return "DarkGray";
					case "standard": return "DarkGreen";
					case "economy": return "DarkGreen";
					case "express": return "#D50101";
					case "high_priority_express": return "#D50101";
					case "local_pickup": return "DarkGray";
					case "amazon": return "Orange";
					case "amazon_standard": return "DarkGreen";
					case "amazon_expedited": return "Orange";
					case "amazon_second_day": return "#D50101";
					case "amazon_next_day": return "#D50101";
					case "dhl_parcel_direct": return "DarkGreen";
					default: return "DarkGray";
				}
			}
		}

		public string EstimateProcessingMaxDate => Order.EstimateProcessingMaxDate;

		public string EstimateDeliveryMaxDate => Order.EstimateDeliveryMaxDate;

		public string StatusOrderColor => Utils.Utils.SelectStatusColor(Status);

		public bool IsExpanderEnabled => false;

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
					return "#bf6900";
				}

				if (diff.Days < 1)
				{
					return "#494949";
				}

				if (diff.Hours == 1)
				{
					return "#494949";
				}

				return diff.Hours < 1 ? "#494949" : "nan";
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
}
