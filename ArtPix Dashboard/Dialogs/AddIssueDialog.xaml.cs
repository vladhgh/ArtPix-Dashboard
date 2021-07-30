using ArtPix_Dashboard.API;
using ArtPix_Dashboard.Models.IssueReasons;
using ArtPix_Dashboard.Utils.Helpers;


namespace ArtPix_Dashboard.Dialogs
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
