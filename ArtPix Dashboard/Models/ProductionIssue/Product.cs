using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ArtPix_Dashboard.Utils.Helpers;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.ProductionIssue
{
	public class Product : PropertyChangedListener
	{
		[JsonProperty("url_original_img")]
		public string UrlOriginalImg { get; set; }

		public BitmapImage ProductImage
		{
			get
			{
				var bmp = new BitmapImage();
				bmp.BeginInit();
				bmp.CacheOption = BitmapCacheOption.None;
				bmp.DecodePixelWidth = 150;
				bmp.UriSource = new Uri(UrlRenderImg, UriKind.RelativeOrAbsolute);
				bmp.EndInit();
				return bmp;
			}
		}

		[JsonProperty("url_render_img")]
		public string UrlRenderImg { get; set; }

		[JsonProperty("url_shape_img")]
		public string UrlShapeImg { get; set; }

		[JsonProperty("filename_latest")]
		public string FilenameLatest { get; set; }
	}
}
