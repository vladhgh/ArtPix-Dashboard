using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Common
{
	public class Links
	{
		[JsonProperty("first")]
		public string First { get; set; }

		[JsonProperty("last")]
		public string Last { get; set; }

		[JsonProperty("prev")]
		public string Prev { get; set; }

		[JsonProperty("next")]
		public string Next { get; set; }
	}
}
