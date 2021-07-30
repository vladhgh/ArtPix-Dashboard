using Newtonsoft.Json;
using System.Collections.Generic;
using ArtPix_Dashboard.Models.Common;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;


namespace ArtPix_Dashboard.Models.Order
{
	public class OrderModel : PropertyChangedListener
	{

		[JsonProperty("data")]
		public List<Datum> Data { get; set; }

		[JsonProperty("links")]
		public Links Links { get; set; }

		private Meta _meta;

		[JsonProperty("meta")]
		public Meta Meta
		{
			get => _meta;
			set => SetProperty(ref _meta, value);
		}
	}
}
