﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
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
}
