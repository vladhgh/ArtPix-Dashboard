
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.Product;
using ArtPix_Dashboard.ViewModels;

namespace ArtPix_Dashboard.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for CancelIssueDialog.xaml
    /// </summary>
    public partial class CancelIssueDialog
    {
        public Datum Item { get; set; }
        public CancelIssueDialog(Datum item)
        {
            InitializeComponent();
            DataContext = this;
            this.Item = item;
        }
    }
}
