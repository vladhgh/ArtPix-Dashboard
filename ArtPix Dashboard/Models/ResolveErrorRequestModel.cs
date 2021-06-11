using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models
{
    public class ResolveErrorRequestModel
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

    public class AssignProcessingModel
    {
	    public int product_id { get; set; }

	    public int order_id { get; set; }
        public int copy_id { get; set; }

        public string order_name { get; set; }

	    public string machine { get; set; }
    }
    public class FindBestServiceRequestModel
	{
        public string order_id { get; set; }
	}

    public class OrderCombineFilterModel
    {
	    public string name_order { get; set; }
        public string status_engraving { get; set; }
        public string status_shipping { get; set; }
	    public string status_order { get; set; }
        public string with_crystals { get; set; }
	    public string has_shipping_package { get; set; }
	    public string with_shipping_totes { get; set; }
	    public string with_production_issue { get; set; }
	    public string sort_by { get; set; }
	    public string shipByToday { get; set; }
	    public string store_name { get; set; }

    }
}
