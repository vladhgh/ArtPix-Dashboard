using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtPix_Dashboard.Models.Requests
{
	public class ResolveProductionIssueRequest
	{
		public int machine_assign_error_id { get; set; }
		public int machine_assign_item_id { get; set; }
		public int id_products { get; set; }
		public string user { get; set; }
		public string issue_type { get; set; }
		public string status_error { get; set; }
		public string message { get; set; }
		public int machine_id { get; set; }

	}
}
