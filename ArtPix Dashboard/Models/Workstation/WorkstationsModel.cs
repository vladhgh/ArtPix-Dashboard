using System;
using System.Collections.Generic;
using ArtPix_Dashboard.Models.Common;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Workstation
{
	public class WorkstationsModel : PropertyChangedListener
    {
	    private List<Datum> _data;

	    [JsonProperty("data")]
	    public List<Datum> Data
	    {
		    get => _data;
		    set => SetProperty(ref _data, value);
	    }

	    private Double _panelSpacing = 46.5;

        public Double PanelSpacing
        {
	        get => _panelSpacing;
	        set => SetProperty(ref _panelSpacing, value);
        }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }


}
