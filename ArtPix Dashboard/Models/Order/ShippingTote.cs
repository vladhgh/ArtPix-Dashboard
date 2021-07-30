using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	public class ShippingTote
	{
		public Visibility SlotButtonVisibility => ShippingSlot == null ? Visibility.Collapsed : Visibility.Visible;

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }

		[JsonProperty("shipping_slot")]
		public ShippingSlot ShippingSlot { get; set; }
	}
}
