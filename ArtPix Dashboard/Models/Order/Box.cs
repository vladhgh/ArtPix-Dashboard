using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
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
}
