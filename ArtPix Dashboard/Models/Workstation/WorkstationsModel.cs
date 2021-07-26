using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public string MacAddress
		{
            get
			{
                if (Utils.Utils.MachineAddresses.FirstOrDefault(x => x.Value == Name).Key is string value)
				{
                    Debug.WriteLine("MAC FOUND: " + value);
                    return value;
				}
				else
				{
                    Debug.WriteLine("Unknown");
                    return "Unknown";
				}
            }
		}

        public string NetworkPath
		{
            get
			{
                switch (Name)
				{
                    case "1":
                        return "\\\\1-AB1-LL";
                    case "2":
                        return "\\\\2-AB1-LL";
                    case "3":
                        return "\\\\3-AB1-LL";
                    case "4":
                        return "\\\\4-AB1-LL";
                    case "5":
                        return "\\\\5-AB1-LL";
                    case "6":
                        return "\\\\6-AB1-LL";
                    case "7":
                        return "\\\\7-AB1-LL";
                    case "8":
                        return "\\\\8-AB1-LL";
                    case "9":
                        return "\\\\9-AB1-LL";
                    case "10":
                        return "\\\\10-AB1-LL";
                    case "11":
                        return "\\\\11-AB1-LL";
                    case "12":
                        return "\\\\12-AB1-LL";
                    case "13":
                        return "\\\\13-AB2-SL";
                    case "14":
                        return "\\\\14-AB2-SL";
                    case "15":
                        return "\\\\15-AB2-SL";
                    case "16":
                        return "\\\\16-AB3-SL";
                    case "17":
                        return "\\\\17-AB4-SL";
                    case "18":
                        return "\\\\18-AB2-SL";
                    case "19":
                        return "\\\\19-AB2-SL";
                    case "20":
                        return "\\\\20-AB1-LL";
                    case "21":
                        return "\\\\21-AB1-LL";
                    case "22":
                        return "\\\\22-AB1-LL";
                    case "23":
                        return "\\\\23-AB1-LL";
                    case "24":
                        return "\\\\24-AB1-LL";
                    case "25":
                        return "\\\\25-AB1-LL";
                    default:
                        return "none";
                }
			}
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
        public Visibility OfflineTextVisibility
        {
            get
            {
                foreach (var machine in Machines)
                {
                    if (machine.NetworkStatus == "Online")
                    {
                        return Visibility.Collapsed;
                    }
                }
                return JobsCount == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility OnlineTextVisibility
		{
            get
			{
                foreach (var machine in Machines)
				{
                    if (machine.NetworkStatus == "Online")
					{
                        return JobsCount == 0 ? Visibility.Visible : Visibility.Collapsed;
                    }
				}
                return Visibility.Collapsed;
            }
		}

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

	    private Double _panelSpacing = 45;

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
