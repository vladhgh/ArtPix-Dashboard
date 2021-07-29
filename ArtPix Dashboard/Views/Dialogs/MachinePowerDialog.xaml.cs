

using ArtPix_Dashboard.ViewModels;

namespace ArtPix_Dashboard.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for CancelIssueDialog.xaml
    /// </summary>
    public partial class MachinePowerDialog
    {
        private MachinePowerViewModel _vm = new();

        public MachinePowerDialog(string kind)
        {
            InitializeComponent();
            DataContext = _vm;
            _vm.PrimaryButtonText = kind == "PowerOn" ? "Power On" : "Power Off";
            _vm.MainText = kind == "PowerOn" ? "Are you sure you want to power all machines on?" : "Are you sure you want to power all machines off?";
        }
    }

    public class MachinePowerViewModel : PropertyChangedListener
	{
        private string _primaryButtonText;

        public string PrimaryButtonText
        {
            get => _primaryButtonText;

            set => SetProperty(ref _primaryButtonText, value);
        }
        
        private string _mainText;

		public string MainText
		{
            get => _mainText;

            set => SetProperty(ref _mainText, value);
		}


    }
}
