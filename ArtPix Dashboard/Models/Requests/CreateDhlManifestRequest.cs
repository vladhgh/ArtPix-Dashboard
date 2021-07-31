using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtPix_Dashboard.Models.Requests
{
	public class CreateDhlManifestRequest
	{
		public int domestic_containers_count { get; set; }

		public int international_containers_count { get; set; }
	}
}
