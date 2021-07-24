using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using ArtPix_Dashboard.API;
using ArtPix_Dashboard.ViewModels;

namespace ArtPix_Dashboard.Models.Product
{

    public class Product : PropertyChangedListener
    {
        [JsonProperty("url_original_img")]
        public string UrlOriginalImg { get; set; }



        private int _urlRenderImgSize;

        public int UrlRenderImgSize
        {
	        get => IsImageExpanded ? _urlRenderImgSize : 175;
            set => SetProperty(ref _urlRenderImgSize, value);
        }

        private bool _isImageExpanded;

        public bool IsImageExpanded
        {
	        get => _isImageExpanded;
	        set => SetProperty(ref _isImageExpanded, value);
        }

        [JsonProperty("url_render_img")]
        public string UrlRenderImg { get; set; }

        

        [JsonProperty("url_shape_img")]
        public string UrlShapeImg { get; set; }

        [JsonProperty("filename_latest")]
        public string FilenameLatest { get; set; }
    }

    public class Order
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("estimate_delivery_max_date")]
        public string EstimateDeliveryMaxDate { get; set; }

        private string _shipping_type;
        [JsonProperty("shipping_type")]
        public string ShippingType
        {
            get
            {
                switch (_shipping_type)
                {
                    case "free_shipping": return "Free Shipping";
                    case "standard": return "Standard";
                    case "economy": return "Economy";
                    case "express": return "Express";
                    case "high_priority_express": return "High Priority Express";
                    case "local_pickup": return "Pick-Up";
                    case "amazon": return "Amazon";
                    case "amazon_standard": return "Amazon Standard";
                    case "amazon_expedited": return "Amazon Expedited";
                    default: return "Not Found";
                }
            }
            set => _shipping_type = value;
        }
        public string ShippingTypeColor
        {
            get
            {
                switch (_shipping_type)
                {
                    case "free_shipping": return "Gray";
                    case "standard": return "Green";
                    case "economy": return "Green";
                    case "express": return "Red";
                    case "high_priority_express": return "Red";
                    case "local_pickup": return "Brown";
                    case "amazon": return "Orange";
                    case "amazon_standard": return "Green";
                    case "amazon_expedited": return "Orange";
                    default: return "White";
                }
            }
        }
    }

    public class Datum
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("machine_assign_id")]
        public int MachineAssignId { get; set; }

        [JsonProperty("machine_assign_item_id")]
        public int MachineAssignItemId { get; set; }

        [JsonProperty("machine_id")]
        public int MachineId { get; set; }

        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        
        [JsonProperty("error_text")]
        public string ErrorText { get; set; }


	    [JsonProperty("user")]
        public string User { get; set; }

        private string _source;
        [JsonProperty("source")]
        public string Source
		{
            get
			{
				switch (_source)
				{
                    case "machine":
                        _source = "on Machine " + this.MachineId.ToString();
                        return _source;
                    case "packing_station":
                        _source = "at Shipping Station";
                        return _source;
                    default: return _source;
                }
			}
            set => _source = value;
		}

        [JsonProperty("previous_status")]
        public string PreviousStatus { get; set; }


        public string CreatedAtAge
        {
	        get
	        {
		        var today = DateTime.Now;
		        var lastUpdated = DateTime.Parse(CreatedAt, CultureInfo.CurrentUICulture);
		        var diff = today - lastUpdated;
		        if (diff.Days > 1)
		        {
			        return $"{diff.Days} days";
		        }

		        if (diff.Days == 1)
		        {
			        return $"{diff.Days} days {diff.Hours} hours";
		        }

		        if (diff.Days < 1 && diff.Hours > 1)
		        {
			        return $"{diff.Hours} hours";
		        }

		        if (diff.Hours == 1)
		        {
			        return $"{diff.Hours} hour";
		        }

		        return diff.Hours < 1 ? $"{diff.Minutes} minutes" : "nan";
	        }
        }

        public string CreatedAtAgeColor
        {
	        get
	        {
		        var today = DateTime.Now;
		        var lastUpdated = DateTime.Parse(CreatedAt, CultureInfo.CurrentUICulture);
		        var diff = today - lastUpdated;

		        return diff.Days >= 1 ? "DarkRed" : diff.Hours > 4 ? "DarkOrange" : "Gray";
	        }
        }



        private string _createdAt;

        [JsonProperty("created_at")]
        public string CreatedAt
        {
	        get
	        {
		        var createdAt = DateTime.Parse(_createdAt, CultureInfo.CurrentUICulture);
		        return createdAt.AddHours(-5).ToString(CultureInfo.CurrentUICulture);
	        }
	        set => _createdAt = value;
        }

        [JsonProperty("filename_issue")]
        public string FilenameIssue { get; set; }

        [JsonProperty("product")]
        public Product Product { get; set; }

        [JsonProperty("order")]
        public Order Order { get; set; }
        public class IssueReason
        {
	        [JsonProperty("id")]
	        public int Id { get; set; }

	        [JsonProperty("stage")]
	        public string Stage { get; set; }

	        [JsonProperty("type")]
	        public string Type { get; set; }

	        [JsonProperty("reason")]
	        public string Reason { get; set; }
        }
        [JsonProperty("production_issue_reason")]
        public IssueReason ProductionIssueReason { get; set; }
    }

    public class Links
    {
        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }

        [JsonProperty("prev")]
        public object Prev { get; set; }

        [JsonProperty("next")]
        public object Next { get; set; }
    }

    public class Meta
    {
        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("last_page")]
        public int LastPage { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    [Serializable]
    public class ProductionIssueModel
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty("first_page_url")]
        public string FirstPageUrl { get; set; }

        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("last_page")]
        public int LastPage { get; set; }

        [JsonProperty("last_page_url")]
        public string LastPageUrl { get; set; }

        [JsonProperty("next_page_url")]
        public object NextPageUrl { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [JsonProperty("prev_page_url")]
        public object PrevPageUrl { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

    }


}
