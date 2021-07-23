

using System.Collections.Generic;
using ArtPix_Dashboard.Models.Machine;
using Datum = ArtPix_Dashboard.Models.Product.Datum;

namespace ArtPix_Dashboard.Views.Dialogs
{
    public partial class ChangeAddressDialog : ModernWpf.Controls.ContentDialog
    {
	    public ChangeAddressDialog()
	    {
		    InitializeComponent();
		    DataContext = this;
	    }
    }
}
