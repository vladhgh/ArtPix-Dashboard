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
	    public int pageNumber { get; set; }
	    public bool withPages { get; set; }
        public string withCrystal { get; set; }
        public string perPage { get; set; }
	    public string hasShippingPackage { get; set; }
	    public string withShippingTotes { get; set; }
	    public string withProductionIssue { get; set; }
	    public string sortBy { get; set; }
	    public string shipByToday { get; set; }
	    public string storeName { get; set; }
	    public string shippingStatus { get; set; }
	    public string orderStatus { get; set; }
	    public string statusEngraving { get; set; }
    }
}
