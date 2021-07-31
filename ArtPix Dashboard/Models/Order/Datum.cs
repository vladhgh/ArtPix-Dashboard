using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	public class Datum : PropertyChangedListener
	{
		#region VISIBILITY

		public Visibility IsLateShipment => DateTime.Parse(EstimateProcessingMaxDate, CultureInfo.CurrentUICulture).AddHours(19) < DateTime.Now ? Visibility.Visible : Visibility.Collapsed;
		public Visibility IssueResolved => HasIssueResolved ? Visibility.Visible : Visibility.Collapsed;

		private Visibility _expanderVisibility = Visibility.Visible;


		public Visibility ExpanderVisibility
		{
			get => _expanderVisibility;
			set => SetProperty(ref _expanderVisibility, value);
		}

		public Visibility ShippingPanelVisibility => ShippingMethodName == "Local Pickup" ? Visibility.Collapsed : Visibility.Visible;

		public Visibility TrackingNumberTextBlockVisibility => ShippingOrderInfo.TrackingNumber == null ? Visibility.Collapsed : Visibility.Visible;

		private Visibility _shippingInformationPanelVisibility;

		public Visibility ShippingInformationPanelVisibility
		{
			get
			{
				if (ShippingOrderInfo.Service != null)
				{
					_shippingInformationPanelVisibility = Visibility.Visible;
					return _shippingInformationPanelVisibility;
				}
				else
				{
					_shippingInformationPanelVisibility = Visibility.Collapsed;
					return _shippingInformationPanelVisibility;
				}
			}
			set => SetProperty(ref _shippingInformationPanelVisibility, value);

		}

		public Visibility FindBestServiceButtonVisibility
		{
			get
			{
				if (ShippingOrderInfo != null)
				{
					return ShippingOrderInfo.Provider == null ? Visibility.Visible : Visibility.Collapsed;
				}
				return Visibility.Hidden;
			}

		}

		public Visibility ShippingAddressVisibility => customers == null ? Visibility.Collapsed : Visibility.Visible;

		public Visibility CustomerNoteVisibility => string.IsNullOrEmpty(CustomerNote) ? Visibility.Collapsed : Visibility.Visible;


		private Visibility _shippingAddress2Visibility;

		public Visibility ShippingAddress2Visibility
		{
			get
			{
				if (customers == null)
				{
					_shippingAddress2Visibility = Visibility.Collapsed;
					return _shippingAddress2Visibility;
				}

				if (customers.ShippingAddress == null)
				{
					_shippingAddress2Visibility = Visibility.Collapsed;
					return _shippingAddress2Visibility;
				}
				_shippingAddress2Visibility = string.IsNullOrEmpty(customers.ShippingAddress.address_2) ? Visibility.Collapsed : Visibility.Visible;
				return _shippingAddress2Visibility;
			}
			set => SetProperty(ref _shippingAddress2Visibility, value);
		}



		#endregion


		private bool _isExpanded;

		public bool IsExpanded
		{
			get => _isExpanded;
			set => SetProperty(ref _isExpanded, value);
		}

		[JsonProperty("id_orders")]
		public int IdOrders { get; set; }

		private string _isShippingServiceFound;

		public string IsShippingServiceFound
		{
			get
			{
				if (ShippingOrderInfo.Service != null)
				{
					_isShippingServiceFound = "Shipping Service Found";
					return _isShippingServiceFound;
				}
				else
				{
					_isShippingServiceFound = "Shipping Service Not Found";
					return _isShippingServiceFound;
				}
			}
			set => SetProperty(ref _isShippingServiceFound, value);

		}

		private string _isShippingServiceFoundColor;

		public string IsShippingServiceFoundColor
		{
			get
			{
				if (ShippingOrderInfo.Service != null)
				{
					_isShippingServiceFoundColor = "DarkGreen";
					return _isShippingServiceFoundColor;
				}
				else
				{
					_isShippingServiceFoundColor = "DarkRed";
					return _isShippingServiceFoundColor;
				}
			}
			set => SetProperty(ref _isShippingServiceFoundColor, value);

		}

		private bool _isShippingInformationLoading;

		public bool IsShippingInformationLoading
		{
			get => _isShippingInformationLoading;
			set => SetProperty(ref _isShippingInformationLoading, value);
		}

		[JsonProperty("name_order")]
		public string NameOrder { get; set; }

		[JsonProperty("store_order")]
		public int StoreOrder { get; set; }

		private string _status;

		[JsonProperty("status")]
		public string Status
		{
			get
			{
				if (HasIssueOpened)
				{
					return "Customer Service Issue";
				}


				if (_status == "engraving" || _status == "3d_conversion" || _status == "retouching" || _status == "waiting_to_confirm")
				{
					var newStatus = "";
					foreach (var product in Products)
					{
						if (product.CrystalType.Type == "Crystal" || product.CrystalType.Type == "Necklace" ||
							product.CrystalType.Type == "Keychain" || product.CrystalType.Type == "Fingerprint" || product.CrystalType.Type == "Wine Stopper")
						{

							if (product.Status == "3D Model Pending" || product.Status == "3D Model In Progress")
							{
								return product.Status;
							}
							if (product.Status == "Engraving Issue")
							{
								return product.Status;
							}
							if (product.Status == "Retouch Issue")
							{
								return product.Status;
							}
							if (product.Status == "Ready To Engrave")
							{
								return product.Status;
							}
							if (product.Status == "Engraving In Progress")
							{
								return product.Status;
							}
							newStatus = newStatus == product
								.Status
								? newStatus
								: product.Status;
						}
					}

					return newStatus;
				}
				switch (_status)
				{
					case "shipped": return "Shipped";
					case "ready_to_ship": return "Ready To Ship";
					case "3d_conversion": return "3D Conversion";
					case "retouching": return "Retouching";
					case "waiting_to_confirm": return "Awaiting Confirmation";
					default: return _status;
				}
			}
			set => SetProperty(ref _status, value);
		}

		[JsonProperty("status_order")]
		public string StatusOrder { get; set; }

		private string _statusOrderColor;

		public string StatusOrderColor
		{
			get
			{
				if (HasIssueOpened)
				{
					return "DarkRed";
				}

				switch (Status)
				{
					case "Awaiting Model": return "#bf6900";
					case "Ready To Engrave": return "#494949";
					case "Engraving Issue": return "DarkRed";
					case "Retouch Issue": return "DarkRed";
					case "Retouch Pending": return "#bf6900";
					case "Ready To Ship": return "DarkGreen";
					case "Retouch In Progress": return "SteelBlue";
					case "Engraving In Progress": return "SteelBlue";
					case "3D Model Pending": return "#bf6900";
					case "3D Model In Progress": return "SteelBlue";
				}

				switch (_status)
				{
					case "shipped": return "DarkGreen";
					case "ready_to_ship": return "DarkGreen";
					case "waiting_to_confirm": return "#bf6900";
					default: return "#494949";
				}
			}
			set => SetProperty(ref _statusOrderColor, value);
		}

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
		public double? ShippingTotal { get; set; }

		[JsonProperty("shipping_tax")]
		public double? ShippingTax { get; set; }

		[JsonProperty("cart_tax")]
		public double? CartTax { get; set; }

		[JsonProperty("total_order")]
		public double? TotalOrder { get; set; }

		[JsonProperty("total_tax")]
		public double? TotalTax { get; set; }

		[JsonProperty("total_crystal")]
		public int TotalCrystal { get; set; }

		public Visibility EngravedAgeVisibility => Visibility.Collapsed;

		public Visibility ProductStatusVisibility => Visibility.Visible;

		public BitmapImage OrderImage
		{
			get
			{
				var item = Products.Find(product => Utils.Utils.IsCrystal(product));
				var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				var path = "pack://application:,,,/Assets/Images/multiple_item_order_preview.png";
				var logoImage = new Uri("pack://application:,,,/Assets/Images/multiple_item_order_preview.png", UriKind.RelativeOrAbsolute);

				if (item == null)
				{
					var bmp = new BitmapImage();
					bmp.BeginInit();
					bmp.CacheOption = BitmapCacheOption.None;
					bmp.DecodePixelWidth = 100;
					bmp.UriSource = logoImage;
					bmp.EndInit();
					return bmp;
				}
				else
				{
					var img = TotalCrystal == 1 && !string.IsNullOrEmpty(item.UrlRenderImg) ? item.UrlRenderImg : path;
					var bmp = new BitmapImage();
					bmp.BeginInit();
					bmp.CacheOption = BitmapCacheOption.None;
					bmp.DecodePixelWidth = 100;
					bmp.UriSource = new Uri(img, UriKind.RelativeOrAbsolute);
					bmp.EndInit();
					return bmp;
				}

			}

		}

		[JsonProperty("total_products")]
		public int TotalProducts { get; set; }

		[JsonProperty("order_created_at")]
		public string OrderCreatedAt { get; set; }

		[JsonProperty("customer_note")]
		public string CustomerNote { get; set; }

		[JsonProperty("id_customers")]
		public int? IdCustomers { get; set; }

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
				_estimateProcessingMaxDate = String.IsNullOrEmpty(_estimateProcessingMaxDate) ? DateTime.Parse(ShipBy, CultureInfo.CurrentUICulture).AddHours(-5).ToString(CultureInfo.CurrentUICulture).Split(' ')[0] : DateTime.Parse(_estimateProcessingMaxDate, CultureInfo.CurrentUICulture).AddHours(-5).ToString(CultureInfo.CurrentUICulture).Split(' ')[0];
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
			get => String.IsNullOrEmpty(_estimatedDeliveryMaxDate) ? DateTime.Parse(ShipBy, CultureInfo.CurrentUICulture).AddDays(3).ToString(CultureInfo.CurrentUICulture).Split(' ')[0] : DateTime.Parse(_estimatedDeliveryMaxDate, CultureInfo.CurrentUICulture).AddHours(-5).ToString(CultureInfo.CurrentUICulture).Split(' ')[0];
			set => _estimatedDeliveryMaxDate = value;
		}

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		private string _updatedAtAge;

		public string UpdatedAtAge
		{
			get
			{
				var today = DateTime.Now;
				var lastUpdated = DateTime.Parse(UpdatedAt, CultureInfo.CurrentUICulture);
				var diff = today - lastUpdated;
				if (diff.Days > 1)
				{
					_updatedAtAge = $"{diff.Days} days ago";
					return _updatedAtAge;
				}

				if (diff.Days == 1)
				{
					_updatedAtAge = $"{diff.Days} days {diff.Hours} hours ago";
					return _updatedAtAge;
				}

				if (diff.Days < 1 && diff.Hours > 1)
				{
					_updatedAtAge = $"{diff.Hours} hours ago";
					return _updatedAtAge;
				}

				if (diff.Hours == 1)
				{
					_updatedAtAge = $"{diff.Hours} hour ago";
					return _updatedAtAge;
				}

				if (diff.Minutes == 1)
				{
					_updatedAtAge = $"1 minute ago";
					return _updatedAtAge;
				}
				if (diff.Minutes == 0)
				{
					_updatedAtAge = $"just now";
					return _updatedAtAge;
				}
				_updatedAtAge = diff.Hours < 1 ? $"{diff.Minutes} minutes ago" : "nan";
				return _updatedAtAge;
			}
			set => SetProperty(ref _updatedAtAge, value);
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
				foreach (var product in Products.Where(product => product.Comments.Count > 0))
				{
					var date = DateTime.Parse(product.Comments[product.Comments.Count - 1].UpdatedAt,
						CultureInfo.CurrentUICulture);
					if (date > lastUpdated)
					{
						lastUpdated = date;
					}
				}
				return lastUpdated.AddHours(-5).ToString(CultureInfo.CurrentUICulture);
			}
			set => SetProperty(ref _updatedAt, value);
		}


		

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
					case "dhl_parcel_standard": return "DHL Parcel Standard";
					default: return _shippingType;
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
					case "dhl_parcel_standard": return "DarkGreen";
					default: return "DarkGray";
				}
			}
		}

		public bool IsExpanderEnabled => true;

		[JsonProperty("is_redo")]
		public bool IsRedo { get; set; }

		[JsonProperty("has_issue_opened")]
		public bool HasIssueOpened { get; set; }

		[JsonProperty("has_issue_resolved")]
		public bool HasIssueResolved { get; set; }

		[JsonProperty("has_rework")]
		public bool HasRework { get; set; }

		[JsonProperty("ship_by")]
		public string ShipBy { get; set; }

		[JsonProperty("ship_by_min")]
		public string ShipByMin { get; set; }

		[JsonProperty("ship_by_max")]
		public string ShipByMax { get; set; }

		[JsonProperty("delivery_by")]
		public string DeliveryBy { get; set; }

		[JsonProperty("shipstation_order")]
		public ShipstationOrder ShipstationOrder { get; set; }

		private List<Product> _products;
		[JsonProperty("products")]
		public List<Product> Products
		{
			get => _products;
			set
			{
				var newProductList = new List<Product>(value);
				TotalProducts = 0;
				foreach (Product product in value.ToList())
				{

					if (product.MachineAssignItems.Count >= 1)
					{
						var i = 0;
						foreach (var item in product.MachineAssignItems)
						{
							if (i > 0)
							{
								var newProduct = Utils.Utils.DeepCopy(product);
								newProduct.MachineAssignItemId = item.Id;
								newProduct.MachineId = item.machine_id;
								if (item.Status == "wait_model")
								{
									newProductList.Add(newProduct);
									TotalProducts++;
									i++;
									continue;
								}
								newProduct.Status = Utils.Utils.SelectStatusText(item.Status);
								newProductList.Add(newProduct);
							}
							else
							{
								product.MachineAssignItemId = item.Id;
								product.MachineId = item.machine_id;
								if (item.Status == "wait_model")
								{
									TotalProducts++;
									i++;
									continue;
								}
								product.Status = Utils.Utils.SelectStatusText(item.Status);

							}
							TotalProducts++;
							i++;
						}

					}
					if (product.CrystalType.Type.Contains("Base") || product.CrystalType.Type == "Full Kit" || product.CrystalType.Type.Contains("Card") || product.CrystalType.Type.Contains("Paper"))
					{
						TotalProducts++;
						for (int i = 0; i < product.Quantity - 1; i++)
						{
							var newProduct = Utils.Utils.DeepCopy(product);
							newProductList.Add(newProduct);
							TotalProducts++;
						}
					}

					if (HasIssueOpened)
					{
						if (product.CrystalType.Type.Contains("Base") || product.CrystalType.Type == "Full Kit" || product.CrystalType.Type.Contains("Card") || product.CrystalType.Type.Contains("Paper")) continue;
						
						product.Status = "Customer Service Issue";
					}

				}
				_products = newProductList;

			}
		}

		private Customers _customers;

		public Customers customers
		{
			get => _customers;
			set => SetProperty(ref _customers, value);
		}

		private ShippingOrderInfo _shippingOrderInfo = new ShippingOrderInfo();
		[JsonProperty("shipping_order_info")]
		public ShippingOrderInfo ShippingOrderInfo
		{
			get => _shippingOrderInfo;
			set => SetProperty(ref _shippingOrderInfo, value);
		}

		[JsonProperty("payment")]
		public Payment Payment { get; set; }

		[JsonProperty("shipping_totes")]
		public List<ShippingTote> ShippingTotes { get; set; }

		[JsonProperty("deadline")]
		public Deadline Deadline { get; set; }

	}
}
