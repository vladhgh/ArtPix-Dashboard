using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.ProductHistory
{
	public class User
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("roles")]
		public List<string> Roles { get; set; }
	}
}
