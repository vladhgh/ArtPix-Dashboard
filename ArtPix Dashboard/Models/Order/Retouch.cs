using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Order
{
	[Serializable()]
	public class Retouch
	{
		[JsonProperty("id_retouchers")]
		public int IdRetouchers { get; set; }

		[JsonProperty("link_to_3d_model")]
		public string LinkTo3dModel { get; set; }

		[JsonProperty("id_user_retoucher")]
		public object IdUserRetoucher { get; set; }

		[JsonProperty("filename")]
		public string Filename { get; set; }

		[JsonProperty("file_url")]
		public string FileUrl { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}
}
