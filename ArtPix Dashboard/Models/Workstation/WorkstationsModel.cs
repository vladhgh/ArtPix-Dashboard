using System;
using System.Collections.Generic;
using System.Windows;
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

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        private int _jobsCount;

        public int JobsCount
        {
	        get => _jobsCount;
	        set => SetProperty(ref _jobsCount, value);
        }
        public string JobsCountColor => JobsCount < 3 ? "DarkRed" : JobsCount < 5 ? "DarkOrange" : "DarkGreen";

        public Visibility OfflineTextVisibility => NetworkStatus == "Offline" && JobsCount == 0 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility OnlineTextVisibility => NetworkStatus == "Online" && JobsCount == 0 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility JobsCountVisibility => JobsCount > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public class Datum : PropertyChangedListener
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        private int _jobsCount;

        public int JobsCount
        {
	        get => _jobsCount;
	        set => SetProperty(ref _jobsCount, value);
        }
        public string JobsCountColor => JobsCount < 3 ? "DarkRed" : JobsCount < 5 ? "DarkOrange" : "DarkGreen";


        private List<Machine> _machines = new List<Machine>();

        [JsonProperty("machines")]
        public List<Machine> Machines
        {
	        get => _machines;
	        set => SetProperty(ref _machines, value);
        }


        private Visibility _machinesGroupVisibility = Visibility.Collapsed;

        public Visibility MachinesGroupVisibility
        {
	        get => _machinesGroupVisibility;
	        set => SetProperty(ref _machinesGroupVisibility, value);
        }

        public Visibility JobsCountVisibility => JobsCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility OfflineTextVisibility => JobsCount == 0 ? Visibility.Visible : Visibility.Collapsed;

        private bool _isChecked;

        public bool IsChecked
        {
	        get => _isChecked;
	        set => SetProperty(ref _isChecked, value);
        }
    }

    public class Links
    {
        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }

        [JsonProperty("prev")]
        public object Prev { get; set; }

        [JsonProperty("next")]
        public object Next { get; set; }
    }

    public class Meta
    {
        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("last_page")]
        public int LastPage { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class WorkstationsModel : PropertyChangedListener
    {
	    private List<Datum> _data;

	    [JsonProperty("data")]
	    public List<Datum> Data
	    {
		    get => _data;
		    set => SetProperty(ref _data, value);
	    }

	    private Double _panelSpacing = 51;

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
