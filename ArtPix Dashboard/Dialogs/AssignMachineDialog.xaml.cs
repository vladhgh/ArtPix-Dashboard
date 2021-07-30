using System.Collections.Generic;
using ArtPix_Dashboard.Models.Workstation;

namespace ArtPix_Dashboard.Dialogs
{
    public partial class AssignMachineDialog : ModernWpf.Controls.ContentDialog
    {
	    public List<Machine> Machines { get; set; }
	    public AssignMachineDialog(List<Machine> machines)
	    {
		    InitializeComponent();
		    DataContext = this;
		    Machines = machines;
	    }
    }
}
