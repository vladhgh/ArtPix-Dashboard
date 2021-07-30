using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Imaging;
using ArtPix_Dashboard.Models.Common;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.MachineAssignedItem
{
	
	public class MachineAssignItemModel
	{

		[JsonProperty("data")]
		public List<Datum> Data { get; set; }

		[JsonProperty("links")]
		public Links Links { get; set; }

		[JsonProperty("meta")]
		public Meta Meta { get; set; }
	}

}