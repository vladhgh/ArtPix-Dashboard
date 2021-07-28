

using System.Collections.Generic;
using ArtPix_Dashboard.Models.Machine;
using Datum = ArtPix_Dashboard.Models.Product.Datum;

namespace ArtPix_Dashboard.Views.Dialogs
{
    public partial class AssignJobsDialog : ModernWpf.Controls.ContentDialog
    {
	    public AssignJobsDialog()
	    {
		    InitializeComponent();
		    DataContext = this;
	    }
    }
}
