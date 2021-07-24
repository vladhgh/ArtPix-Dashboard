using System.Collections.Generic;
using ArtPix_Dashboard.Models.Machine;

namespace ArtPix_Dashboard.Views.Dialogs
{
    public partial class ReEngraveDialog : ModernWpf.Controls.ContentDialog
    {
	    public List<Machine> Machines { get; set; }
	    public ReEngraveDialog(List<Machine> machines)
	    {
		    InitializeComponent();
		    DataContext = this;
		    this.Machines = machines;
			
	    }
    }
}
