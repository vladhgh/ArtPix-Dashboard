using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.ProductHistory
{
	public class ProductHistoryModel
	{
		[JsonProperty("data")]
		public List<Datum> Data { get; set; }
	}
}
