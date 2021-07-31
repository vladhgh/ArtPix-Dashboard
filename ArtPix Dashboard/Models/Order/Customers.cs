using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	public class Customers
	{
		[JsonProperty("id_customers")]
		public int IdCustomers { get; set; }


		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("phone")]
		public string Phone { get; set; }

		[JsonProperty("billing_address")]
		public BillingAddress BillingAddress { get; set; }

		[JsonProperty("shipping_address")]
		public ShippingAddress ShippingAddress { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}
}
