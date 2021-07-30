using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	public class Payment
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("order_id")]
		public int OrderId { get; set; }

		[JsonProperty("payment_method_title")]
		public string PaymentMethodTitle { get; set; }

		[JsonProperty("last_card_digits")]
		public string LastCardDigits { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}
}
