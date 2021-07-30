using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	[Serializable()]
	public class MachineAssignItem
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("machine_assign_id")]
		public int MachineAssignId { get; set; }

		public string machine_id { get; set; }

		[JsonProperty("product_id")]
		public int ProductId { get; set; }

		[JsonProperty("order_id")]
		public int OrderId { get; set; }

		[JsonProperty("order_name")]
		public string OrderName { get; set; }

		[JsonProperty("started_at")]
		public object StartedAt { get; set; }

		[JsonProperty("ended_at")]
		public object EndedAt { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		[JsonProperty("machine_slot")]
		public object MachineSlot { get; set; }

		[JsonProperty("is_qr_printed")]
		public bool IsQrPrinted { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}
}
