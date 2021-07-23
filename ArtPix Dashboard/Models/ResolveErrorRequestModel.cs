using ArtPix_Dashboard.ViewModels;
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
        public int machine_assign_item_id { get; set; }

        public string order_name { get; set; }

	    public string machine { get; set; }

        public string user { get; set; }
    }
    public class FindBestServiceRequestModel
	{
        public string order_id { get; set; }
	}

    public class OrderCombineFilterModel : PropertyChangedListener
    {

		private string _order_id;

		public string order_id
		{
			get => _order_id;
			set => SetProperty(ref _order_id, value);
		}
		private string _name_order;

	    public string name_order
	    {
		    get => _name_order;
		    set => SetProperty(ref _name_order, value);
	    }

	    private string _status_engraving;
	    public string status_engraving
	    {
		    get => _status_engraving;
		    set => SetProperty(ref _status_engraving, value);
	    }

        private string _status_shipping;
        public string status_shipping
        {
	        get => _status_shipping;
	        set => SetProperty(ref _status_shipping, value);
        }

        private string _status_order;
        public string status_order
        {
	        get => _status_order;
	        set => SetProperty(ref _status_order, value);
        }

	    private string _with_crystals;
	    public string with_crystals
	    {
		    get => _with_crystals;
		    set => SetProperty(ref _with_crystals, value);
	    }

        private string _has_shipping_package;
        public string has_shipping_package
        {
	        get => _has_shipping_package;
	        set => SetProperty(ref _has_shipping_package, value);
        }

	    private string _with_shipping_totes;
	    public string with_shipping_totes
	    {
		    get => _with_shipping_totes;
		    set => SetProperty(ref _with_shipping_totes, value);
	    }

	    private string _with_production_issue;
	    public string with_production_issue
	    {
		    get => _with_production_issue;
		    set => SetProperty(ref _with_production_issue, value);
	    }

	    private string _sort_by;
	    public string sort_by
	    {
		    get => _sort_by;
		    set => SetProperty(ref _sort_by, value);
	    }

	    private string _shipByToday;
	    public string shipByToday
	    {
		    get => _shipByToday;
		    set => SetProperty(ref _shipByToday, value);
	    }

	    private string _store_name;
	    public string store_name
	    {
		    get => _store_name;
		    set => SetProperty(ref _store_name, value);
	    }

    }
}
