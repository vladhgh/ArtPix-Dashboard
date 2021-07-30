using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	public class ShippingSlot
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("row_name")]
		public int RowName { get; set; }

		[JsonProperty("column_name")]
		public string ColumnName { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}
}
