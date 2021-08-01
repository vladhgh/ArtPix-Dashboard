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

		private List<Datum> _data = new ();
		[JsonProperty("data")]
		public List<Datum> Data
		{
			get
			{
				var newData = Utils.Utils.DeepCopy(_data);
				foreach (var item in newData.ToArray())
				{
					if (item.OrderName == "434076" || item.OrderName == "412759" || item.OrderName == "310130" || item.OrderName == "309045" || item.OrderName == "292717" || item.OrderName == "286636")
					{
						newData.Remove(item);
						Meta.Total--;
					}

					if (item.Order.StatusOrder != "processing" && item.Order.StatusEngraving == "processing")
					{
						newData.Remove(item);
						Meta.Total--;
					}
				}
				return newData;
			}
			set => _data = value;
		}

		[JsonProperty("links")]
		public Links Links { get; set; }

		[JsonProperty("meta")]
		public Meta Meta { get; set; }
	}

}