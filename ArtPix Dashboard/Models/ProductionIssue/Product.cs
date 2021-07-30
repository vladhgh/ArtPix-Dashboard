using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtPix_Dashboard.Utils.Helpers;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.ProductionIssue
{
	public class Product : PropertyChangedListener
	{
		[JsonProperty("url_original_img")]
		public string UrlOriginalImg { get; set; }

		private int _urlRenderImgSize;

		public int UrlRenderImgSize
		{
			get => IsImageExpanded ? _urlRenderImgSize : 175;
			set => SetProperty(ref _urlRenderImgSize, value);
		}

		private bool _isImageExpanded;

		public bool IsImageExpanded
		{
			get => _isImageExpanded;
			set => SetProperty(ref _isImageExpanded, value);
		}

		[JsonProperty("url_render_img")]
		public string UrlRenderImg { get; set; }

		[JsonProperty("url_shape_img")]
		public string UrlShapeImg { get; set; }

		[JsonProperty("filename_latest")]
		public string FilenameLatest { get; set; }
	}
}
