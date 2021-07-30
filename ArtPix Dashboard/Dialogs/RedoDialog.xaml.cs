using System.Collections.Generic;
using ArtPix_Dashboard.Models.Workstation;

namespace ArtPix_Dashboard.Dialogs
{
    public partial class RedoDialog : ModernWpf.Controls.ContentDialog
    {
	    public List<Machine> Machines { get; set; }
	    public RedoDialog(List<Machine> machines)
	    {
		    InitializeComponent();
		    DataContext = this;
		    this.Machines = machines;
	    }
    }
}
