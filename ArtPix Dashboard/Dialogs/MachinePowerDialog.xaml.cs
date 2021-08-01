using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;
using ModernWpf.Controls;
using ToastNotifications.Messages;

namespace ArtPix_Dashboard.Dialogs
{

    public partial class MachinePowerDialog
    {

        internal MainViewModel ViewModel;

        public MachinePowerDialog(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = this;
            ViewModel = viewModel;
        }

        private void PowerAllMachinesButtonClick(object sender, RoutedEventArgs e)
        {
	        var kind = ((Button)sender).Tag.ToString();
	        if (kind == "PowerOn")
	        {
		        foreach (var machineAddress in Utils.Utils.MachineAddresses)
		        {
			        Utils.Utils.SendWakeOnLan(PhysicalAddress.Parse(machineAddress.Key));
		        }
		        Utils.Utils.Notifier.ShowSuccess("Machine Power On Request Sent Succesfully!\nPlease Wait....");
	        }
	        if (kind == "PowerOff")
	        {
		        foreach (var workstation in ViewModel.WorkstationStats.Data)
		        {
			        foreach (var machine in workstation.Machines)
			        {
				        Process.Start("shutdown", $"-s -f -t 00 -m {machine.NetworkPath}");
			        }
		        }
		        Utils.Utils.Notifier.ShowSuccess("Machine Power Off Request Sent Succesfully!\nPlease Wait....");
	        }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
	        Hide();
        }
    }

}
