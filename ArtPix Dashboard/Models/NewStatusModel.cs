using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtPix_Dashboard.Models
{
	public class NewStatusModel
	{
		public string new_status { get; set; }
		public int machine_assign_item_id { get; set; }
		public int order_id { get; set; }
		public string order_name { get; set; }
		public int product_id { get; set; }
	}
}
