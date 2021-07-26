using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using ArtPix_Dashboard.ViewModels;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using ArtPix_Dashboard.API;
using ArtPix_Dashboard.Models.Logs;
using RestSharp;
using DataFormat = System.Windows.DataFormat;

namespace ArtPix_Dashboard.Models.Order
{

	public class ShipstationOrder
	{
		[JsonProperty("id_orders")]
		public int IdOrders { get; set; }

		[JsonProperty("shipstation_id")]
		public int ShipstationId { get; set; }
	}

	[Serializable()]
	public class Comment
	{
		[JsonProperty("product_id")]
		public int ProductId { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("comment")]
		public string Message { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	[Serializable()]
	public class CrystalType
	{
		[JsonProperty("id_crystals")]
		public int IdCrystals { get; set; }

		[JsonProperty("index")]
		public string Index { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("form")]
		public string Form { get; set; }

		[JsonProperty("size")]
		public string Size { get; set; }

		[JsonProperty("price")]
		public double Price { get; set; }

		[JsonProperty("sku")]
		public string Sku { get; set; }

		public int has_3d { get; set; }

		public string Is3d => has_3d == 1 ? "3D" : "2D";

		[JsonProperty("weight_gram")]
		public double WeightGram { get; set; }

		[JsonProperty("size_mm_x")]
		public int SizeMmX { get; set; }

		[JsonProperty("size_mm_y")]
		public int SizeMmY { get; set; }

		[JsonProperty("size_mm_z")]
		public int SizeMmZ { get; set; }
	}

	[Serializable()]
	public class MachineAssignItem
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("machine_assign_id")]
		public int MachineAssignId { get; set; }

		public string machine_id { get; set; }

		[JsonProperty("product_id")]
		public int ProductId { get; set; }

		[JsonProperty("order_id")]
		public int OrderId { get; set; }

		[JsonProperty("order_name")]
		public string OrderName { get; set; }

		[JsonProperty("started_at")]
		public object StartedAt { get; set; }

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

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	[Serializable()]
	public class Product : PropertyChangedListener
	{
		public string MachineId { get; set; }
		public int MachineAssignItemId { get; set; }

		public double ProductImageSize
		{
			get 
			{
				var crystalType = CrystalType.Sku.ToCharArray()[1];
				if ((crystalType == 'R' || crystalType == 'L' || crystalType == 'G') || (crystalType == 'F' && CrystalType.Sku.ToCharArray()[0] == '5'))
				{
					return 100;
				}
				if (CrystalType.Sku.ToCharArray()[0] == '7')
				{
					return 100;
				}
				if (CrystalType.Sku == "1PBS" || CrystalType.Sku == "1PBM")
				{
					return 100;
				}
				return 175;
			}
		}

		public Visibility MachineButtonVisibility =>
			string.IsNullOrEmpty(MachineId) ? Visibility.Collapsed : Visibility.Visible;


		private Visibility _manualCompleteButtonVisibility;
		
		public Visibility ManualCompleteButtonVisibility
		{
			get
			{

				_manualCompleteButtonVisibility = Status == "Engraving Done" ? Visibility.Collapsed : Visibility.Visible;
				return _manualCompleteButtonVisibility;
			}
			set => SetProperty(ref _manualCompleteButtonVisibility, value);
		}
		public Visibility CrystalIssueButtonVisibility => _status == "engrave_processing" || _status == "engrave_ready" || _status == "engrave_done" || _status == "engrave_redo" ? Visibility.Visible : Visibility.Collapsed;
		public Visibility AssignMachineButtonVisibility => _status == "ready_to_engrave" || _status == "engrave_redo" ? Visibility.Visible : Visibility.Collapsed; 
		public Visibility UnAssignMachineButtonVisibility => _status == "engrave_processing" ? Visibility.Visible : Visibility.Collapsed;
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
		public double Price { get; set; }

		[JsonProperty("convert_to_3d")]
		public int ConvertTo3d { get; set; }


		[JsonProperty("url_original_img")]
		public string UrlOriginalImg { get; set; }

		private string _urlRenderImg;


		public BitmapImage ProductImage
		{
			get
			{
				var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
				var productImagePath = Path.Combine(outPutDirectory, $"..\\..\\Assets\\{UrlRenderImg}");
				var productImageLocalPath = new Uri(productImagePath).LocalPath;

				if (String.IsNullOrEmpty(_urlRenderImg))
				{
					var bmp = new BitmapImage();
					bmp.BeginInit();
					bmp.CacheOption = BitmapCacheOption.OnLoad;
					bmp.DecodePixelWidth = 100;
					bmp.UriSource = new Uri(productImageLocalPath, UriKind.RelativeOrAbsolute);
					bmp.EndInit();
					return bmp;
				}
				else
				{
					var bmp = new BitmapImage();
					bmp.BeginInit();
					bmp.CacheOption = BitmapCacheOption.OnLoad;
					bmp.DecodePixelWidth = 175;
					bmp.UriSource = new Uri(_urlRenderImg, UriKind.RelativeOrAbsolute);
					bmp.EndInit();
					return bmp;
				}
			}
		}

		[JsonProperty("url_render_img")]
		public string UrlRenderImg {
			get
			{
				switch (CrystalType.Sku)
				{
					case "6GFS3P3": return "greeting_card_flowers_orange_ribbon.png";
					case "6GSS2R3": return "greeting_card_loving_heart.png";
					case "6GSS1P3": return "greeting_card_i_love_you.png";
					case "6GSS5R3": return "greeting_card_cupid_couple.png";
				}

				switch (Name)
				{
					case "Rotating Base Small":
						return "rotating_base_small.png";
					case "Rotating Base Large":
						return "rotating_base_large.png";
					case "Small Wood Base with Light":
						return "light_base_small.png";
					case "Small Wood Light Base":
						return "light_base_small.png";
					case "Light Base Small":
						return "light_base_small.png";
					case "Medium Wood Base with Light":
						return "light_base_medium.png";
					case "Medium Wood Light Base":
						return "light_base_medium.png";
					case "Light Base Medium":
						return "light_base_medium.png";
					case "Large Wood Base with Light":
						return "light_base_large.png";
					case "Large Wood Light Base":
						return "light_base_large.png";
					case "Light Base Large":
						return "light_base_large.png"; 
					case "XL Wood Light Base":
						return "light_base_xl.png";
					case "Light Base X-Large":
						return "light_base_xl.png";
					case "Small Rotating Light Base":
						return "rotating_base_small.png";
					case "Large Rotating Light Base":
						return "rotating_base_large.png";
					case "3D Clover Flower Set With Blue Ribbon Card":
						return "https://artpix3d.com/wp-content/uploads/2020/12/Greeting-card-2-2.jpg";
					case "3D Clover Flower Set With Orange Ribbon Card":
						return "https://artpix3d.com/wp-content/uploads/2020/12/Greeting-card-3-2.jpg";
					case "Cleaning Kit":
						return "cleaning_kit.png";
					case "Sunshine Wrapping Paper":
						return "sunshine_wrapping_paper.png";
					case "Balloon Wrapping Paper":
						return "baloon_wrapping_paper.png";
					case "Red Star Wrapping Paper":
						return "red_star_wrapping_paper.png";
					case "Golden Star Wrapping Paper":
						return "golden_star_wrapping_paper.png";
					case "Blue Polka Dot Wrapping Paper":
						return "blue_polka_dot_wrapping_paper.png";
					case "3D Sunflower Set Card":
						return "sunflower_greeting_card.png";
					case "Medium Plastic Light Base":
						return "plastic_light_base_medium.png";
					case "Small Plastic Light Base":
						return "plastic_light_base_medium.png";
					default:
						return _urlRenderImg ?? "placeholder.png";
				}
			}
			set => _urlRenderImg = value;
		}

		public Visibility OUploadButtonVisibility =>
			String.IsNullOrEmpty(UrlOriginalOriginal) ? Visibility.Collapsed : Visibility.Visible;
		public Visibility ORenderButtonVisibility =>
			String.IsNullOrEmpty(UrlOriginalRender) ? Visibility.Collapsed : Visibility.Visible;

		[JsonProperty("url_shape_img")]
		public string UrlShapeImg { get; set; }

		[JsonProperty("url_original_render")]
		public string UrlOriginalRender { get; set; }

		[JsonProperty("url_original_original")]
		public string UrlOriginalOriginal { get; set; }

		[JsonProperty("url_outside_render")]
		public object UrlOutsideRender { get; set; }

		[JsonProperty("customer_font")]
		public string CustomerFont { get; set; }

		private string _customerEngraving;

		[JsonProperty("customer_engraving")]
		public string CustomerEngraving
		{
			get => string.IsNullOrEmpty(_customerEngraving) ? null : _customerEngraving.Replace("&amp;", "&");
			set => SetProperty(ref _customerEngraving, value);
		}
		public Visibility CustomerEngravingVisibility => string.IsNullOrEmpty(CustomerEngraving) ? Visibility.Collapsed : Visibility.Visible;

		[JsonProperty("crystal_position")]
		public string CrystalPosition { get; set; }

		private string _employee;
		public string Employee
		{
			get => _employee;
			set => SetProperty(ref _employee, value);
		}

		private int _machineAssignErrorId;
		public int MachineAssignErrorId
		{
			get => _machineAssignErrorId;
			set => SetProperty(ref _machineAssignErrorId, value);
		}
		


		private string _status;

		[JsonProperty("status")]
		public string Status
		{
			get
			{
				return Utils.Utils.SelectStatusText(_status);
			}
			set => SetProperty(ref _status, value);
		}

		public Visibility ProductStatusVisibility => CrystalType.Type == "Crystal" || CrystalType.Type == "Necklace" || CrystalType.Type == "Keychain" || CrystalType.Type == "Fingerprint" || CrystalType.Type == "Wine Stopper" ? Visibility.Visible : Visibility.Collapsed;

		private string _statusColor;

		public string StatusColor
		{
			get
			{
				return Utils.Utils.SelectStatusColor(_status);
			}
			set => SetProperty(ref _statusColor, value);
		}

		[JsonProperty("editor_id")]
		public string EditorId { get; set; }

		[JsonProperty("use_optimize")]
		public bool UseOptimize { get; set; }

		[JsonProperty("url_optimize_img")]
		public string UrlOptimizeImg { get; set; }

		[JsonProperty("number_in_order")]
		public int NumberInOrder { get; set; }

		[JsonProperty("id_store_line_items")]
		public long? IdStoreLineItems { get; set; }

		[JsonProperty("tax")]
		public double? Tax { get; set; }

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
				foreach (var comment in Comments)
				{
					var date = DateTime.Parse(comment.UpdatedAt,
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

		[JsonProperty("crystal_type")]
		public CrystalType CrystalType { get; set; }

		[JsonProperty("machine_assign_items")]
		public List<MachineAssignItem> MachineAssignItems { get; set; }

		[JsonProperty("services")]
		public List<object> Services { get; set; }

		[JsonProperty("comments")]
		public List<Comment> Comments { get; set; }

		[JsonProperty("retouch")]
		public Retouch Retouch { get; set; }

		public Visibility VitroMarkButtonVisibility => Retouch == null ? Visibility.Collapsed : Visibility.Visible;

		public Visibility ProductionIssueButtonsVisibility => _status == "engrave_issue" ? Visibility.Visible : Visibility.Collapsed;
		
		public Visibility ReEngraveButtonVisibility =>
			_status == "engrave_done" ? Visibility.Visible : Visibility.Collapsed;
	}
	[Serializable()]
	public class Retouch
	{
		[JsonProperty("id_retouchers")]
		public int IdRetouchers { get; set; }

		[JsonProperty("link_to_3d_model")]
		public string LinkTo3dModel { get; set; }

		[JsonProperty("id_user_retoucher")]
		public object IdUserRetoucher { get; set; }

		[JsonProperty("filename")]
		public string Filename { get; set; }

		[JsonProperty("file_url")]
		public string FileUrl { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class BillingAddress
	{
		[JsonProperty("id_address")]
		public int IdAddress { get; set; }

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("last_name")]
		public string LastName { get; set; }

		[JsonProperty("company")]
		public string Company { get; set; }

		[JsonProperty("address_1")]
		public string Address { get; set; }

		[JsonProperty("address_2")]
		public string Address2 { get; set; }

		[JsonProperty("city")]
		public string City { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("postcode")]
		public string Postcode { get; set; }

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("is_verified")]
		public object IsVerified { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class ShippingAddress
	{
		[JsonProperty("id_address")]
		public int IdAddress { get; set; }

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("last_name")]
		public string LastName { get; set; }

		[JsonProperty("company")]
		public string Company { get; set; }

		public string address_1 { get; set; }

		public string address_2 { get; set; }

		[JsonProperty("city")]
		public string City { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("postcode")]
		public string Postcode { get; set; }

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("is_verified")]
		public object IsVerified { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class Customers
	{
		[JsonProperty("id_customers")]
		public int IdCustomers { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("phone")]
		public string Phone { get; set; }

		[JsonProperty("billing_address")]
		public BillingAddress BillingAddress { get; set; }

		[JsonProperty("shipping_address")]
		public ShippingAddress ShippingAddress { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class Box
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }


		[JsonProperty("weight")]
		public double Weight { get; set; }

		[JsonProperty("length_inch")]
		public int LengthInch { get; set; }

		[JsonProperty("width_inch")]
		public int WidthInch { get; set; }

		[JsonProperty("height_inch")]
		public int HeightInch { get; set; }

		[JsonProperty("weight_pound")]
		public double WeightPound { get; set; }

		[JsonProperty("cost")]
		public double Cost { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class WrappingPaper
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("weight")]
		public double Weight { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class ShippingPackage
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("box_id")]
		public int? BoxId { get; set; }

		[JsonProperty("wrapping_paper_id")]
		public int? WrappingPaperId { get; set; }

		[JsonProperty("wrapping_paper_length")]
		public int? WrappingPaperLength { get; set; }

		[JsonProperty("wrapping_paper_count")]
		public int? WrappingPaperCount { get; set; }

		[JsonProperty("fedex_box_alias")]
		public string FedexBoxAlias { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		public string VersionStatusColor => VersionStatus == "Package Not Found" ? "DarkRed" : "DarkGreen";
		public Visibility ShippingPackageInformationVisibility => VersionStatus == "Package Not Found" ? Visibility.Collapsed : Visibility.Visible;

		private string _versionStatus;

		[JsonProperty("version_status")]
		public string VersionStatus
		{
			get => _versionStatus == "not_approved" || _versionStatus == "waiting" ? "Package Not Found" : "Package Approved";
			set => _versionStatus = value;
		}

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }

		[JsonProperty("box")]
		public Box Box { get; set; }

		[JsonProperty("wrapping_paper")]
		public WrappingPaper WrappingPaper { get; set; }
	}


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

	public class Payment
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("order_id")]
		public int OrderId { get; set; }

		[JsonProperty("payment_method_title")]
		public string PaymentMethodTitle { get; set; }

		[JsonProperty("last_card_digits")]
		public string LastCardDigits { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class Deadline
	{
		[JsonProperty("ship_by")]
		public string ShipBy { get; set; }

		[JsonProperty("model_3d")]
		public string Model3d { get; set; }

		[JsonProperty("retoucher")]
		public string Retoucher { get; set; }

		[JsonProperty("engraving")]
		public string Engraving { get; set; }

		[JsonProperty("ship_by_chicago")]
		public string ShipByChicago { get; set; }

		[JsonProperty("model_3d_chicago")]
		public string Model3dChicago { get; set; }

		[JsonProperty("retoucher_chicago")]
		public string RetoucherChicago { get; set; }

		[JsonProperty("engraving_chicago")]
		public string EngravingChicago { get; set; }
	}

	public class Datum : PropertyChangedListener
	{

		private Visibility _expanderVisibility = Visibility.Visible;

		public Visibility ExpanderVisibility
		{
			get => _expanderVisibility;
			set => SetProperty(ref _expanderVisibility, value);
		}


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
				} else
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

		public Visibility ShippingPanelVisibility =>
			ShippingMethodName == "Local Pickup" ? Visibility.Collapsed : Visibility.Visible;
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

		public Visibility ShippingAddress2Visibility
		{
			get
			{
				if (customers == null)
				{
					return Visibility.Collapsed;
				}
				return string.IsNullOrEmpty(customers.ShippingAddress.address_2) ? Visibility.Collapsed : Visibility.Visible;

			}
		}



		[JsonProperty("name_order")]
		public string NameOrder { get; set; }


		private double _productsGridOpacity = 1;
		public double ProductsGridOpacity
		{
			get => _productsGridOpacity;
			set => SetProperty(ref _productsGridOpacity, value);
		}

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
				

				if (_status == "engraving" || _status == "3d_conversion" || _status == "retouching")
				{
					var newStatus = "";
					foreach (var product in Products)
					{
						if (product.CrystalType.Type == "Crystal" || product.CrystalType.Type == "Necklace" ||
						    product.CrystalType.Type == "Keychain" || product.CrystalType.Type == "Fingerprint")
						{
							if (product.Status == "3D Model Pending" || product.Status == "3D Model In Progress")
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
					case "shipped" : return "Shipped";
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
					case "Ready To Engrave": return "#494949";
					case "Engraving Issue": return "DarkRed";
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

		public BitmapImage OrderImage
		{
			get
			{
				var item = this.Products.Find(x => x.CrystalType.Type == "Crystal" || x.CrystalType.Type == "Keychain" || x.CrystalType.Type == "Necklace" || x.CrystalType.Type == "Wine Stopper" || x.CrystalType.Type.Contains("Fingerprint") );
				var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
				var logoImage = Path.Combine(outPutDirectory, "..\\..\\Assets\\multiple_item_order_preview.png");
				var relLogo = new Uri(logoImage).LocalPath;

				if (item == null)
				{
					var bmp = new BitmapImage();
					bmp.BeginInit();
					bmp.CacheOption = BitmapCacheOption.OnLoad;
					bmp.DecodePixelWidth = 100;
					bmp.UriSource = new Uri(logoImage, UriKind.RelativeOrAbsolute);
					bmp.EndInit();
					return bmp;
				}
				else
				{
					var img = TotalCrystal == 1 && !string.IsNullOrEmpty(item.UrlRenderImg) ? item.UrlRenderImg : relLogo;
					//if (TotalProducts == 1 && !string.IsNullOrEmpty(item.UrlRenderImg))
					//{
					//	var imagePath = Path.Combine(outPutDirectory, $"..\\..\\Assets\\{item.UrlRenderImg}");
					//	var path = new Uri(imagePath).LocalPath;
					//}
					var bmp = new BitmapImage();
					bmp.BeginInit();
					bmp.CacheOption = BitmapCacheOption.OnLoad;
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
			get => DateTime.Parse(_estimateProcessingMaxDate, CultureInfo.CurrentUICulture).AddHours(-5).ToString(CultureInfo.CurrentUICulture).Split(' ')[0];
			set => _estimateProcessingMaxDate = value;
		}

		[JsonProperty("estimate_delivery_min_date")]
		public string EstimateDeliveryMinDate { get; set; }

		private string _estimatedDeliveryMaxDate;

		[JsonProperty("estimate_delivery_max_date")]
		public string EstimateDeliveryMaxDate
		{
			get => DateTime.Parse(_estimatedDeliveryMaxDate, CultureInfo.CurrentUICulture).AddHours(-5)
				.ToString(CultureInfo.CurrentUICulture).Split(' ')[0];
			set => _estimatedDeliveryMaxDate = value;
		}

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
			set => _updatedAt = value;
		}

		public Visibility IsActionNeeded
		{
			get
			{
				var today = DateTime.Now.AddHours(-12);
				var lastUpdated = DateTime.Parse(UpdatedAt, CultureInfo.InvariantCulture);
				return lastUpdated < today ? Visibility.Visible : Visibility.Hidden;
				
			}
		}

		public Visibility IsLateShipment => DateTime.Parse(EstimateProcessingMaxDate, CultureInfo.CurrentUICulture) < DateTime.Now ? Visibility.Visible : Visibility.Collapsed;

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
					case "express": return "DarkRed";
					case "high_priority_express": return "DarkRed";
					case "local_pickup": return "DarkGray";
					case "amazon": return "Orange";
					case "amazon_standard": return "DarkGreen";
					case "amazon_expedited": return "Orange";
					case "amazon_second_day": return "DarkRed";
					case "amazon_next_day": return "DarkRed";
					case "dhl_parcel_direct": return "DarkGreen";
					default: return "DarkGray";
				}
			}
		}

		[JsonProperty("is_redo")]
		public bool IsRedo { get; set; }

		public Visibility RedoVisibility => IsRedo ? Visibility.Visible : Visibility.Collapsed;

		[JsonProperty("has_issue_opened")]
		public bool HasIssueOpened { get; set; }

		public Visibility IssueOpened => HasIssueOpened ? Visibility.Visible : Visibility.Collapsed;

		[JsonProperty("has_issue_resolved")]
		public bool HasIssueResolved { get; set; }
		public Visibility IssueResolved => HasIssueResolved ? Visibility.Visible : Visibility.Collapsed;

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
								newProductList.Add(newProduct);
							}
							else
							{
								product.MachineAssignItemId = item.Id;
								product.MachineId = item.machine_id;

							}
							TotalProducts++;
							i++;
						}
					
					}
					if(product.CrystalType.Type.Contains("Base") || product.CrystalType.Type == "Full Kit" || product.CrystalType.Type.Contains("Card") || product.CrystalType.Type.Contains("Paper"))
					{
						TotalProducts++;
						for (int i = 0; i < product.Quantity - 1; i++)
						{
							var newProduct = Utils.Utils.DeepCopy(product);
							newProductList.Add(newProduct);
							TotalProducts++;
						}
					}
					
					
				}
				_products = newProductList;

			}
		}
		public Customers customers { get; set; }

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


		private bool _isOrderLoading;
		public bool IsOrderLoading
		{
			get => _isOrderLoading;
			set => SetProperty(ref _isOrderLoading, value);
		}
	}

	public class ShippingSlot
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("row_name")]
		public int RowName { get; set; }

		[JsonProperty("column_name")]
		public string ColumnName { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class ShippingTote
	{
		public Visibility SlotButtonVisibility => ShippingSlot == null ? Visibility.Collapsed : Visibility.Visible;
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }

		[JsonProperty("shipping_slot")]
		public ShippingSlot ShippingSlot { get; set; }
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
		public int PerPage { get; set; }

		[JsonProperty("to")]
		public int To { get; set; }

		[JsonProperty("total")]
		public int Total { get; set; }
	}

	public class OrderModel
	{


		[JsonProperty("data")]
		public List<Datum> Data { get; set; }

		[JsonProperty("links")]
		public Links Links { get; set; }

		[JsonProperty("meta")]
		public Meta Meta { get; set; }
	}




}
