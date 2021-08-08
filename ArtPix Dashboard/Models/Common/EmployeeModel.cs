using ArtPix_Dashboard.Models.Workstation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtPix_Dashboard.Utils.Helpers;

namespace ArtPix_Dashboard.Models.Common
{
	public class EmployeeModel : PropertyChangedListener
	{
		public string Name { get; set; }

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
				_averagePerformance = String.Format("{0:N}", EngravedCrystalCount / TotalEngravingTimeInHours) + " per hour";
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
	}

}
