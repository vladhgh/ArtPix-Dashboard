using ArtPix_Dashboard.Utils.Helpers;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.IssueReasons
{
    public class Datum : PropertyChangedListener
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("stage")]
        public string Stage { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        private bool _isChecked;

        public bool IsChecked
        {
	        get => _isChecked;
	        set => SetProperty(ref _isChecked, value);
        }

        public string IssueReason => $"{Reason} ({Count})";

        public int Count { get; set; }
    }
}
