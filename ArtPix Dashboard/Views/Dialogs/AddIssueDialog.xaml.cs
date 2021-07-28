

using System.Collections.Generic;
using System.Threading.Tasks;
using ArtPix_Dashboard.API;
using ArtPix_Dashboard.Models.Machine;
using ArtPix_Dashboard.Models.Types;
using ArtPix_Dashboard.ViewModels;
using Datum = ArtPix_Dashboard.Models.Product.Datum;

namespace ArtPix_Dashboard.Views.Dialogs
{
    public partial class AddIssueDialog : ModernWpf.Controls.ContentDialog
    {

	    private AddIssueDialogViewModel _vm = new AddIssueDialogViewModel();

	    public AddIssueDialog()
	    {
		    InitializeComponent();
		    DataContext = _vm;
		    GetIssueReasons();

	    }

	    public async void GetIssueReasons()
	    {
		    _vm.IssueReasons = await ArtPixAPI.GetIssueReasonsAsync();

	    }
    }

    public class AddIssueDialogViewModel : PropertyChangedListener
    {

	    private IssueReasonsModel _issueReasons = new IssueReasonsModel();
	    public IssueReasonsModel IssueReasons
		{
		    get => _issueReasons;
		    set => SetProperty(ref _issueReasons, value);
	    }
	}
}
