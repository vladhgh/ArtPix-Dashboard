using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.DhlManifest
{
    public class Manifest
    {
        [JsonProperty("is_international")]
        public bool IsInternational { get; set; }

        [JsonProperty("copies_count")]
        public int CopiesCount { get; set; }

        [JsonProperty("file_data")]
        public string FileData { get; set; }
    }

    public class DhlManifestModel
    {
        [JsonProperty("manifests")]
        public List<Manifest> Manifests { get; set; }
    }
}
