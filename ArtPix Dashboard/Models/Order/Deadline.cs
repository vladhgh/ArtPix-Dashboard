using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
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
}
