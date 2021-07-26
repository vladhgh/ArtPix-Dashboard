using System.Collections.Generic;
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.Machine;
using ArtPix_Dashboard.ViewModels;
using Datum = ArtPix_Dashboard.Models.Product.Datum;

namespace ArtPix_Dashboard.Views.Dialogs
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
