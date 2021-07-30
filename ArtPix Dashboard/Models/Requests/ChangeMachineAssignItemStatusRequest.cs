namespace ArtPix_Dashboard.Models.Requests
{
	public class ChangeMachineAssignItemStatusRequest
	{

		public string new_status { get; set; }

		public int machine_assign_item_id { get; set; }

		public int order_id { get; set; }

		public string order_name { get; set; }

		public int product_id { get; set; }

		public string machine { get; set; }

		public string user { get; set; }

	}
}
