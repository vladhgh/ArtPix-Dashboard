using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.MachineAssignedItem
{
	[Serializable()]
	public class Product
	{
		public Visibility ProductStatusVisibility => Visibility.Collapsed;

		public Visibility EngravedAgeVisibility => Visibility.Visible;


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
}
