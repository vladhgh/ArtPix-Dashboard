using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	[Serializable()]
	public class Comment
	{
		[JsonProperty("product_id")]
		public int ProductId { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("comment")]
		public string Message { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}
}
