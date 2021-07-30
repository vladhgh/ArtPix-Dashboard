using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Common
{
	public class Meta : PropertyChangedListener
	{
		[JsonProperty("current_page")] public int CurrentPage { get; set; }

		[JsonProperty("from")] public int From { get; set; }

		[JsonProperty("last_page")] public int LastPage { get; set; }

		[JsonProperty("path")] public string Path { get; set; }

		[JsonProperty("per_page")] public int PerPage { get; set; }

		[JsonProperty("to")] public int To { get; set; }


		private int _total;

		[JsonProperty("total")]
		public int Total
		{
			get => _total;
			set => SetProperty(ref _total, value);
		}
	}
}
