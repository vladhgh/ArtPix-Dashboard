using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	[Serializable()]
	public class Product : PropertyChangedListener
	{

		#region VISIBILITY

		public Visibility VitroMarkButtonVisibility => Retouch == null ? Visibility.Collapsed : Visibility.Visible;

		public Visibility ProductStatusVisibility => CrystalType.Type == "Crystal" || CrystalType.Type == "Necklace" || CrystalType.Type == "Keychain" || CrystalType.Type == "Fingerprint" || CrystalType.Type == "Wine Stopper" ? Visibility.Visible : Visibility.Collapsed;

		private Visibility _productionIssueButtonsVisibility;

		public Visibility ProductionIssueButtonsVisibility
		{
			get
			{
				_productionIssueButtonsVisibility = Status == "Engraving Issue" ? Visibility.Visible : Visibility.Collapsed;
				return _productionIssueButtonsVisibility;
			}
			set => SetProperty(ref _productionIssueButtonsVisibility, value);
		}

		private Visibility _reEngraveButtonVisibility;

		public Visibility ReEngraveButtonVisibility
		{
			get
			{
				_reEngraveButtonVisibility = Status == "Engraving Done" || Status == "Completed Manually"
					? Visibility.Visible
					: Visibility.Collapsed;
				return _reEngraveButtonVisibility;
			}
			set => SetProperty(ref _reEngraveButtonVisibility, value);
		}

		private Visibility _machineButtonVisibility;

		public Visibility MachineButtonVisibility
		{
			get
			{
				_machineButtonVisibility = string.IsNullOrEmpty(MachineId) ? Visibility.Collapsed : Visibility.Visible;
				return _machineButtonVisibility;
			}
			set => SetProperty(ref _machineButtonVisibility, value);
		}

		private Visibility _manualCompleteButtonVisibility;

		public Visibility ManualCompleteButtonVisibility
		{
			get
			{
				_manualCompleteButtonVisibility = Status == "Engraving Done" || Status == "Completed Manually"
					? Visibility.Collapsed
					: Visibility.Visible;
				return _manualCompleteButtonVisibility;
			}
			set => SetProperty(ref _manualCompleteButtonVisibility, value);
		}

		private Visibility _crystalIssueButtonVisibility;

		public Visibility CrystalIssueButtonVisibility
		{
			get
			{
				_crystalIssueButtonVisibility = Status == "Engraving In Progress" || Status == "Engraving Done" || Status == "Completed Manually"
					? Visibility.Visible
					: Visibility.Collapsed;
				return _crystalIssueButtonVisibility;
			}
			set => SetProperty(ref _crystalIssueButtonVisibility, value);
		}

		private Visibility _assignMachineButtonVisibility;

		public Visibility AssignMachineButtonVisibility
		{
			get
			{
				_assignMachineButtonVisibility = Status == "Ready To Engrave" ? Visibility.Visible : Visibility.Collapsed;
				return _assignMachineButtonVisibility;
			}
			set => SetProperty(ref _assignMachineButtonVisibility, value);
		}

		private Visibility _unAssignMachineButtonVisibility;

		public Visibility UnAssignMachineButtonVisibility
		{
			get
			{
				_unAssignMachineButtonVisibility = MachineId == null ? Visibility.Collapsed :
					Status == "Engraving In Progress" || Status == "Ready To Engrave" ? Visibility.Visible :
					Visibility.Collapsed;
				return _unAssignMachineButtonVisibility;
			}
			set => SetProperty(ref _unAssignMachineButtonVisibility, value);
		}

		public Visibility CustomerEngravingVisibility => string.IsNullOrEmpty(CustomerEngraving) ? Visibility.Collapsed : Visibility.Visible;

		public Visibility OUploadButtonVisibility => string.IsNullOrEmpty(UrlOriginalOriginal) ? Visibility.Collapsed : Visibility.Visible;

		public Visibility OptimizedButtonVisibility => string.IsNullOrEmpty(UrlOptimizeImg) ? Visibility.Collapsed : Visibility.Visible;

		public Visibility ORenderButtonVisibility => string.IsNullOrEmpty(UrlOriginalRender) ? Visibility.Collapsed : Visibility.Visible;

		#endregion

		private string _machineId;

		public string MachineId
		{
			get => _machineId;
			set => SetProperty(ref _machineId, value);
		}

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
				var logoImage = new Uri($"pack://application:,,,/Assets/Images/{UrlRenderImg}", UriKind.RelativeOrAbsolute);

				if (String.IsNullOrEmpty(_urlRenderImg))
				{
					var bmp = new BitmapImage();
					bmp.BeginInit();
					bmp.CacheOption = BitmapCacheOption.OnLoad;
					bmp.DecodePixelWidth = 100;
					bmp.UriSource = logoImage;
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

		public Visibility EngravedAgeVisibility => Visibility.Collapsed;

		[JsonProperty("url_render_img")]
		public string UrlRenderImg
		{
			get
			{
				switch (CrystalType.Sku)
				{
					case "6GFS3P3": return "GreetingCards/greeting_card_flowers_orange_ribbon.png";
					case "6GSS2R3": return "GreetingCards/greeting_card_loving_heart.png";
					case "6GSS1P3": return "GreetingCards/greeting_card_i_love_you.png";
					case "6GSS5R3": return "GreetingCards/greeting_card_cupid_couple.png";
					case "6GSS4R3": return "GreetingCards/greeting_card_sweatheart_couple.png"; 
					case "1PBS": return "LightBases/plastic_light_base_small.png";
					case "1PBM": return "LightBases/plastic_light_base_medium.png";
					case "1LBS": return "LightBases/light_base_small.png";
					case "1LBM": return "LightBases/light_base_medium.png";
					case "1LBL": return "LightBases/light_base_large.png";
					case "1LBX": return "LightBases/light_base_xl.png";
					case "1RBS": return "LightBases/rotating_base_small.png";
					case "1RBL": return "LightBases/rotating_base_large.png";
				}

				switch (Name)
				{
					case "Cleaning Kit":
						return "cleaning_kit.png";
					case "Sunshine Wrapping Paper":
						return "WrappingPaper/sunshine_wrapping_paper.png";
					case "Balloon Wrapping Paper":
						return "WrappingPaper/baloon_wrapping_paper.png";
					case "Red Star Wrapping Paper":
						return "WrappingPaper/red_star_wrapping_paper.png";
					case "Golden Star Wrapping Paper":
						return "WrappingPaper/golden_star_wrapping_paper.png";
					case "Blue Polka Dot Wrapping Paper":
						return "WrappingPaper/blue_polka_dot_wrapping_paper.png";
					default:
						return _urlRenderImg ?? "multiple_item_order_preview.png";
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

		private string _customerEngraving;

		[JsonProperty("customer_engraving")]
		public string CustomerEngraving
		{
			get => string.IsNullOrEmpty(_customerEngraving) ? null : _customerEngraving.Replace("&amp;", "&").Replace("&quot;", "\"");
			set => SetProperty(ref _customerEngraving, value);
		}
		
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

		private string _statusColor;

		public string StatusColor
		{
			get => Utils.Utils.SelectStatusColor(_status);
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
					return $"{diff.Days} days ago";
				}

				if (diff.Days == 1)
				{
					return $"{diff.Days} days {diff.Hours} hours ago";
				}

				if (diff.Days < 1 && diff.Hours > 1)
				{
					return $"{diff.Hours} hours ago";
				}

				if (diff.Hours == 1)
				{
					return $"{diff.Hours} hour ago";
				}

				if (diff.Minutes == 0)
				{
					return "just now";
				}

				if (diff.Minutes == 1)
				{
					return "1 minute ago";
				}

				return diff.Hours < 1 ? $"{diff.Minutes} minutes ago" : "nan";
			}
			set => SetProperty(ref _updatedAtAge, value);
		}

		private string _updatedAtAgeColor;

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
			set => SetProperty(ref _updatedAtAgeColor, value);
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

		
	}
}
