using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtPix_Dashboard.Models.Requests
{
	public class AddProductionIssueRequest
	{
		public int machine_assign_item_id { get; set; }
		public string user { get; set; }
		public string error { get; set; }
		public string source { get; set; }
		public string issue_title { get; set; }
		public int production_issue_reason_id { get; set; }

	}
}
