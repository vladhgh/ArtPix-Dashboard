

using System.Collections.Generic;
using ArtPix_Dashboard.Models.Machine;
using Datum = ArtPix_Dashboard.Models.Product.Datum;

namespace ArtPix_Dashboard.Views.Dialogs
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
