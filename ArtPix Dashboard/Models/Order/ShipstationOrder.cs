using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	public class ShipstationOrder
	{
		[JsonProperty("id_orders")]
		public int IdOrders { get; set; }

		[JsonProperty("shipstation_id")]
		public int ShipstationId { get; set; }
	}
}
