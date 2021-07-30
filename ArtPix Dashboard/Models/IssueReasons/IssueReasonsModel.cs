using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.IssueReasons
{
	public class IssueReasonsModel
	{
		[JsonProperty("data")]
		public List<Datum> Data { get; set; }
	}
}
