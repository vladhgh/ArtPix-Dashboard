using System;
using System.Windows;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Workstation
{
    public class Machine : PropertyChangedListener
    {
        [JsonProperty("id_machines")]
        public int IdMachines { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id_for_api")]
        public string IdForApi { get; set; }

        [JsonProperty("machine_type")]
        public string MachineType { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        private string _networkStatus = "Offline";

        public string NetworkStatus
        {
            get => _networkStatus;
            set => SetProperty(ref _networkStatus, value);
        }

        public string LogInTime { get; set; }

        public string LogOutTime { get; set; }

        public double TotalEngravingTimeInHours {
            get 
            {
                if (!String.IsNullOrEmpty(LogInTime) && !String.IsNullOrEmpty(LogOutTime))
				{
                    return (DateTime.Parse(LogOutTime) - DateTime.Parse(LogInTime)).TotalHours;
				}
                return 0;
            }
        }


        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
	        get => _isSelected;
	        set => SetProperty(ref _isSelected, value);
        }

        private int _jobsCount;

        public int JobsCount
        {
            get => _jobsCount;
            set => SetProperty(ref _jobsCount, value);
        }

        public string NetworkPath => Utils.Utils.GetLocalMachineAddress(Name);

        public string JobsCountColor => JobsCount < 3 ? "DarkRed" : JobsCount < 5 ? "#bf6900" : "DarkGreen";

        public Visibility OfflineTextVisibility => NetworkStatus == "Offline" && JobsCount == 0 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility OnlineTextVisibility => NetworkStatus == "Online" && JobsCount == 0 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility JobsCountVisibility => JobsCount > 0 ? Visibility.Visible : Visibility.Collapsed;
    }
}
