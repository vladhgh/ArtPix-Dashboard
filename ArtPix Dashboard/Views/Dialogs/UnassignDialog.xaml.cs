using System.Collections.Generic;
using ArtPix_Dashboard.Models.Machine;

namespace ArtPix_Dashboard.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for CancelIssueDialog.xaml
    /// </summary>
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
