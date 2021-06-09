using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using ArtPix_Dashboard.ViewModels;

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

		public Visibility MachineButtonVisibility =>
			string.IsNullOrEmpty(MachineId) ? Visibility.Collapsed : Visibility.Visible;
		public Visibility ManualCompleteButtonVisibility => Status == "engrave_done" ? Visibility.Collapsed : Visibility.Visible;
		public Visibility CrystalIssueButtonVisibility => Status == "engrave_processing" ? Visibility.Visible : Visibility.Collapsed;
		public Visibility AssignMachineButtonVisibility => Status == "ready_to_engrave" || Status == "engrave_redo" ? Visibility.Visible : Visibility.Collapsed; 
		public Visibility UnAssignMachineButtonVisibility => Status == "engrave_processing" ? Visibility.Visible : Visibility.Collapsed;
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


		private bool _isImageExpanded;

		public bool IsImageExpanded
		{
			get => _isImageExpanded;
			set => SetProperty(ref _isImageExpanded, value);
		}


		[JsonProperty("url_original_img")]
		public string UrlOriginalImg { get; set; }

		private string _urlRenderImg;


		private int _urlRenderImgSize;

		public int UrlRenderImgSize
		{
			get =>
				IsImageExpanded ? _urlRenderImgSize : CrystalType.Type == "Crystal" || CrystalType.Type == "Necklace" ||
													  CrystalType.Type == "Keychain" || CrystalType.Type == "Fingerprint"
					? 175
					: 100;
			set => SetProperty(ref _urlRenderImgSize, value);
		}

		[JsonProperty("url_render_img")]
		public string UrlRenderImg {
			get
			{
				switch (Name)
				{
					case "Rotating Base Small":
						return "/Assets/rotating_base_small.png";
					case "Rotating Base Large":
						return "/Assets/rotating_base_large.png";
					case "Small Wood Base with Light":
						return "/Assets/light_base_small.png";
					case "Small Wood Light Base":
						return "/Assets/light_base_small.png";
					case "Light Base Small":
						return "/Assets/light_base_small.png";
					case "Medium Wood Base with Light":
						return "/Assets/light_base_medium.png";
					case "Medium Wood Light Base":
						return "/Assets/light_base_medium.png";
					case "Light Base Medium":
						return "/Assets/light_base_medium.png";
					case "Large Wood Base with Light":
						return "/Assets/light_base_large.png";
					case "Large Wood Light Base":
						return "/Assets/light_base_large.png";
					case "Light Base Large":
						return "/Assets/light_base_large.png";
					case "XL Wood Light Base":
						return "/Assets/light_base_xl.png";
					case "Small Rotating Light Base":
						return "/Assets/rotating_base_small.png";
					case "Large Rotating Light Base":
						return "/Assets/rotating_base_large.png";
					case "3D Clover Flower Set With Blue Ribbon Card":
						return "https://artpix3d.com/wp-content/uploads/2020/12/Greeting-card-2-2.jpg";
					case "3D Clover Flower Set With Orange Ribbon Card":
						return "https://artpix3d.com/wp-content/uploads/2020/12/Greeting-card-3-2.jpg";
					case "Cleaning Kit":
						return "/Assets/cleaning_kit.png";
					case "Sunshine Wrapping Paper":
						return "/Assets/sunshine_wrapping_paper.png";
					case "Balloon Wrapping Paper":
						return "/Assets/baloon_wrapping_paper.png";
					case "Red Star Wrapping Paper":
						return "/Assets/red_star_wrapping_paper.png";
					case "Golden Star Wrapping Paper":
						return "/Assets/golden_star_wrapping_paper.png";
					case "Blue Polka Dot Wrapping Paper":
						return "/Assets/blue_polka_dot_wrapping_paper.png";
					case "3D Sunflower Set Card":
						return "/Assets/sunflower_greeting_card.png"; 
					default:
						return _urlRenderImg ?? "/Assets/placeholder.png";
				}
			}
			set => _urlRenderImg = value;
		}

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

		[JsonProperty("customer_engraving")]
		public string CustomerEngraving { get; set; }
		public Visibility CustomerEngravingVisibility => string.IsNullOrEmpty(CustomerEngraving) ? Visibility.Collapsed : Visibility.Visible;

		[JsonProperty("crystal_position")]
		public string CrystalPosition { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		public Visibility ProductStatusVisibility => CrystalType.Type == "Crystal" || CrystalType.Type == "Necklace" || CrystalType.Type == "Keychain" || CrystalType.Type == "Fingerprint" || CrystalType.Type == "Wine Stopper" ? Visibility.Visible : Visibility.Collapsed;

		public string StatusColor
		{
			get
			{
				switch (Status)
				{
					case "photoshop": return "SteelBlue";
					case "issue": return "DarkRed";
					case "3d_model_in_progress": return "SteelBlue";
					case "3d_model_pending": return "DarkOrange";
					case "retoucher_in_progress": return "SteelBlue";
					case "retoucher_pending": return "DarkOrange";
					case "waiting_to_confirm": return "DarkOrange";
					case "wait_model": return "DarkOrange";
					case "engrave_issue": return "DarkRed";
					case "engrave_processing": return "SteelBlue";
					case "engrave_done": return "DarkGreen";
					case "shipping_label_printed": return "Green";
					default: return "Gray";
				}
			}
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
				/*foreach (var product in Products.Where(product => product.Comments.Count > 0))
				{
					var date = DateTime.Parse(product.Comments[product.Comments.Count - 1].UpdatedAt,
						CultureInfo.CurrentUICulture);
					if (date > lastUpdated)
					{
						lastUpdated = date;
					}
				}*/
				return lastUpdated.AddHours(-5).ToString(CultureInfo.CurrentUICulture);
			}
			set => _updatedAt = value;
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
					case "done": return "Green";
					default: return "Gray";

				}
			}
		}

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

		public string OrderImage
		{
			get
			{
				var item = this.Products.Find(x => x.CrystalType.Type == "Crystal" || x.CrystalType.Type == "Keychain");
			
				if (item != null)
				{
					Debug.WriteLine(item.UrlRenderImg);
					return TotalCrystal == 1 && !string.IsNullOrEmpty(item.UrlRenderImg) ? item.UrlRenderImg : "/Assets/multiple_item_order_preview.png";
				}
				return "/Assets/multiple_item_order_preview.png";
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
			get => DateTime.Parse(_estimateProcessingMaxDate, CultureInfo.CurrentUICulture).AddHours(-5).ToString(CultureInfo.CurrentUICulture);
			set => _estimateProcessingMaxDate = value;
		}

		[JsonProperty("estimate_delivery_min_date")]
		public string EstimateDeliveryMinDate { get; set; }

		private string _estimatedDeliveryMaxDate;

		[JsonProperty("estimate_delivery_max_date")]
		public string EstimateDeliveryMaxDate
		{
			get => DateTime.Parse(_estimatedDeliveryMaxDate, CultureInfo.CurrentUICulture).AddHours(-5)
				.ToString(CultureInfo.CurrentUICulture);
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

		public bool IsLateShipment => DateTime.Parse(EstimateProcessingMaxDate, CultureInfo.CurrentUICulture) < DateTime.Now;

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
					case "amazon_second_day": return "Amazon Second Day";
					case "amazon_next_day": return "Amazon Next Day";
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
					case "amazon_second_day": return "Red";
					case "amazon_next_day": return "Red";
					default: return "White";
				}
			}
		}

		[JsonProperty("is_redo")]
		public bool IsRedo { get; set; }

		[JsonProperty("has_issue_opened")]
		public bool HasIssueOpened { get; set; }

		public Visibility IssueOpened
		{
			get
			{
				return HasIssueOpened ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		[JsonProperty("has_issue_resolved")]
		public bool HasIssueResolved { get; set; }
		public Visibility IssueResolved
		{
			get
			{
				return HasIssueResolved ? Visibility.Visible : Visibility.Collapsed;
			}
		}

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
					if(product.CrystalType.Type == "Light Base" || product.CrystalType.Type == "Rotating Base" || product.CrystalType.Type == "Full Kit" || product.CrystalType.Type == "Greeting Card" || product.CrystalType.Type == "Greeting Cards")
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
