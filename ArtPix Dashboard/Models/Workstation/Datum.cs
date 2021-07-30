using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Workstation
{
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
}
