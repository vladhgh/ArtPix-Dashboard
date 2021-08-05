using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using ArtPix_Dashboard.Utils.Helpers;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.ProductionIssue
{
    public class Datum
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("machine_assign_id")]
        public int MachineAssignId { get; set; }

        [JsonProperty("machine_assign_item_id")]
        public int MachineAssignItemId { get; set; }

        

        private Visibility _failedTextEngravingPanelVisibility = Visibility.Collapsed;

        public Visibility FailedTextEngravingPanelVisibility
        {
	        get
	        {
		        _failedTextEngravingPanelVisibility = ProductionIssueReason.Reason == "Text Validation Failed"
			        ? Visibility.Visible
			        : Visibility.Collapsed;

		        return _failedTextEngravingPanelVisibility;

	        }
	        set => _failedTextEngravingPanelVisibility = value;
        }

        public string OriginalCustomerEngraving
        {
	        get
	        {
		        if (ProductionIssueReason.Reason == "Text Validation Failed")
		        {
			        var originalText = "";
			        if (ErrorText.Split('|').Length > 2)
			        {
				        originalText = Regex.Replace(ErrorText.Split('|')[2], "<.*?>", String.Empty);
			        }
			        else
			        {
				        originalText = Regex.Replace(ErrorText.Split('|')[1], "<.*?>", String.Empty);
			        }
					return originalText.Replace("&amp;", "&").Replace("&quot;", "\"").Remove(0, 1);
		        }

		        return null;

	        }
        }

        public string FailedCustomerEngraving
        {
	        get
	        {
		        if (ProductionIssueReason.Reason == "Text Validation Failed")
		        {
			        var failedText = "";
			        if (ErrorText.Split('|').Length > 2)
			        {
				        failedText = Regex.Replace(ErrorText.Split('|')[1], "<.*?>", String.Empty);
			        }
			        else
			        {
				        failedText = Regex.Replace(ErrorText.Split('|')[0], "<.*?>", String.Empty);
                    }
			        return failedText.Replace("&amp;", "&").Replace("&quot;", "\"");
		        }

		        return null;

	        }
        }

        

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

                return diff.Days >= 1 ? "DarkRed" : diff.Hours > 4 ? "#bf6900" : "#494949";
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
}
