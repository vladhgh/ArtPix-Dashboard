using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	public class BillingAddress
	{
		[JsonProperty("id_address")]
		public int IdAddress { get; set; }

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("last_name")]
		public string LastName { get; set; }

		[JsonProperty("company")]
		public string Company { get; set; }

		[JsonProperty("address_1")]
		public string Address { get; set; }

		[JsonProperty("address_2")]
		public string Address2 { get; set; }

		[JsonProperty("city")]
		public string City { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("postcode")]
		public string Postcode { get; set; }

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("is_verified")]
		public object IsVerified { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}
}
