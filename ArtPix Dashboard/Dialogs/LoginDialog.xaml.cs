using ArtPix_Dashboard.Models.AppState;
using ArtPix_Dashboard.Utils.Helpers;


namespace ArtPix_Dashboard.Dialogs
{
    public partial class LoginDialog : ModernWpf.Controls.ContentDialog
    {
		private LoginDialogViewModel _vm = new LoginDialogViewModel();

	    public LoginDialog(AppStateModel appState)
	    {
		    InitializeComponent();
		    DataContext = _vm;
			_vm.AppState = appState;
	    }
    }

	public class LoginDialogViewModel : PropertyChangedListener
	{
		private AppStateModel _appState = new AppStateModel();
		public AppStateModel AppState
		{
			get => _appState;
			set => SetProperty(ref _appState, value);
		}
	}
}
