using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	public class ShippingPackage
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("box_id")]
		public int? BoxId { get; set; }

		[JsonProperty("wrapping_paper_id")]
		public int? WrappingPaperId { get; set; }

		[JsonProperty("wrapping_paper_length")]
		public int? WrappingPaperLength { get; set; }

		[JsonProperty("wrapping_paper_count")]
		public int? WrappingPaperCount { get; set; }

		[JsonProperty("fedex_box_alias")]
		public string FedexBoxAlias { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		public string VersionStatusColor => VersionStatus == "Package Not Found" ? "DarkRed" : "DarkGreen";
		public Visibility ShippingPackageInformationVisibility => VersionStatus == "Package Not Found" ? Visibility.Collapsed : Visibility.Visible;

		private string _versionStatus;

		[JsonProperty("version_status")]
		public string VersionStatus
		{
			get => _versionStatus == "not_approved" || _versionStatus == "waiting" ? "Package Not Found" : "Package Approved";
			set => _versionStatus = value;
		}

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }

		[JsonProperty("box")]
		public Box Box { get; set; }

		[JsonProperty("wrapping_paper")]
		public WrappingPaper WrappingPaper { get; set; }
	}
}
