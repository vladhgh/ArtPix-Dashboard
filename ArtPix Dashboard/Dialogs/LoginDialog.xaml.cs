using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using ArtPix_Dashboard.Models.AppState;
using ArtPix_Dashboard.Utils.Helpers;
using RestSharp.Extensions;

namespace ArtPix_Dashboard.Dialogs
{
    public partial class LoginDialog : ModernWpf.Controls.ContentDialog
    {
		private LoginDialogViewModel _vm = new LoginDialogViewModel();

		private string _inputString;

		private bool _tabPressed;
		public LoginDialog(AppStateModel appState)
	    {
		    InitializeComponent();
		    DataContext = _vm;
			_vm.AppState = appState;
			SetKeyPressEventListener();

		}

		private void SetKeyPressEventListener()
		{
			PreviewKeyDown += KeyPressEventListener;
			KeyDown += LoginDialogPage_KeyDown;
			PreviewKeyUp += LoginDialogPage_KeyDown;
			KeyUp += LoginDialogPage_KeyDown;
			Focus();
		}

		private void ContentDialog_Closing(ModernWpf.Controls.ContentDialog sender, ModernWpf.Controls.ContentDialogClosingEventArgs args)
		{
			args.Cancel = false;
		}

		public void KeyPressEventListener(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
			}
			if (e.Key == Key.Tab && !_tabPressed)
			{
				_tabPressed = true;
				e.Handled = true;
			}

			if (e.Key != Key.Tab && _tabPressed)
			{
				if ((e.Key >= Key.D0) && (e.Key <= Key.D9))
				{
					_inputString += e.Key.ToString().ToCharArray()[1];
					e.Handled = true;
				}
				if (e.Key == Key.Space)
				{
					_inputString += " ";
					e.Handled = true;
				}
				if ((e.Key >= Key.A) && (e.Key <= Key.Z))
				{
					_inputString += e.Key.ToString();
					e.Handled = true;
				}
				if (e.Key == Key.OemMinus)
				{
					_inputString += "-";
					e.Handled = true;
				}
			}

			if (e.Key == Key.Enter && _tabPressed)
			{
				_tabPressed = false;
				if (_inputString.Split('-')[0] == "LOGIN")
				{

					var fullName = _inputString.Split('-')[1];
					var firstName = fullName.Split(' ')[0];
					var firstNameToCapitalCase = char.ToUpper(firstName.First()) + firstName.Substring(1).ToLower();
					var lastName = fullName.Split(' ')[1];
					var lastNameToCapitalCase = char.ToUpper(lastName.First()) + lastName.Substring(1).ToLower();

					_vm.AppState.CurrentSession.EmployeeName = $"{firstNameToCapitalCase} {lastNameToCapitalCase}";
					Hide();
				}
				_inputString = "";
				e.Handled = true;
			}
		}

		private void LoginDialogPage_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
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
