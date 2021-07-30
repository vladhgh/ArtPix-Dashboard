using System.Collections.Generic;
using ArtPix_Dashboard.Models.Workstation;

namespace ArtPix_Dashboard.Dialogs
{
   
    public partial class UnassignDialog
    {
	    public List<Machine> Machines { get; set; }

        public UnassignDialog(List<Machine> machines)
        {
            InitializeComponent();
            DataContext = this;
            Machines = machines;
        }
    }
}
