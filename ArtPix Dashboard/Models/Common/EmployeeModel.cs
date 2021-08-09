using ArtPix_Dashboard.Models.Workstation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtPix_Dashboard.Utils.Helpers;
using System.Windows;

namespace ArtPix_Dashboard.Models.Common
{
	public class EmployeeModel : PropertyChangedListener
	{
		public string Name { get; set; }

		public string DisplayedName => $"{Name} ({EngravedCrystalCount})";

		public string EngravingPointsDisplayText => "Points: " + EngravingPoints.ToString();

		public string PerformanceDisplayText => "Performance: " + EngravingPoints.ToString();

		public int EngravedCrystalCount { get; set; }

		public double TotalEngravingTime { get ; set; }

		private List<Machine> _assignedMachines = new List<Machine>();

		public List<Machine> AssignedMachines
		{
			get { return _assignedMachines; }
			set { _assignedMachines = value; }
		}

		private string _averagePerformance;

		public string AveragePerformance
		{
			get
			{
				_averagePerformance = "Performance: " + String.Format("{0:N}", EngravedCrystalCount / TotalEngravingTimeInHours) + " per hour";
				return _averagePerformance;
			}
			set
			{
				SetProperty(ref _averagePerformance, value);
			}
		} 

		public double TotalEngravingTimeInHours
		{
			get
			{
				var total = 0.0;
				foreach (var machine in AssignedMachines)
				{
					total += machine.TotalEngravingTimeInHours;
				}

				return total / AssignedMachines.Select(x => x.IdMachines).Distinct().Count();
			}
		}

		private double _points = 0;

		public double EngravingPoints
		{
			get => _points;
			set => SetProperty(ref _points, value);
		}

		private Visibility _engravingPointsVisibility = Visibility.Visible;

		public Visibility EngravingPointsVisibility
		{
			get => Name == "All" ? Visibility.Collapsed : Visibility.Visible;
			set => SetProperty(ref _engravingPointsVisibility, value);
		}
	}

}
