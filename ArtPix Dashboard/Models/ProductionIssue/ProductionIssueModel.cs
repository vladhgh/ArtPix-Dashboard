using System;
using System.Collections.Generic;
using System.Globalization;
using ArtPix_Dashboard.Models.Common;
using ArtPix_Dashboard.Utils.Helpers;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.ProductionIssue
{
	[Serializable]
    public class ProductionIssueModel
    {

        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }

    }
}
