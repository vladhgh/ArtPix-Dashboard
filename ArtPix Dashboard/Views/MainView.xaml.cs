using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using ArtPix_Dashboard.API;
using ArtPix_Dashboard.Utils;
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Properties;
using ArtPix_Dashboard.ViewModels;
using ArtPix_Dashboard.Views.Dialogs;
using Microsoft.Toolkit.Uwp.Notifications;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using Newtonsoft.Json;
using ToastNotifications.Messages;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ArtPix_Dashboard.Views
{
	


	public partial class MainView
	{
		private readonly MainViewModel _vm = new MainViewModel();
		


		public MainView()
		{
			DataContext = _vm;
			InitializeComponent();
		}

		

		private void MainViewOnLoaded(object sender, RoutedEventArgs e)
		{
			_vm.Initialize();
			InitializeSettings();
			ShowChangesDialog();
		}

		private async void ShowChangesDialog()
		{
			if (_vm.AppState.CurrentVersion == _vm.AppState.PreviousVersion) return;
			var dialog = new ChangeLogsDialog();
			var result = await dialog.ShowAsync();
			_vm.AppState.PreviousVersion = _vm.AppState.CurrentVersion;
		}

		private void InitializeSettings()
		{
			Utils.Utils.EnableBlur(this);
			Window.Closing += Window_Closing;
			_vm.AppState.EmployeeName = "Supervisor";
			_vm.AppState.Top = Settings.Default.Top;
			_vm.AppState.Left = Settings.Default.Left;
			_vm.AppState.Height = Settings.Default.Height;
			_vm.AppState.Width = Settings.Default.Width;
			_vm.AppState.WindowState = Settings.Default.Maximized ? WindowState.Maximized : WindowState.Normal;
			_vm.AppState.StatusGroup = Settings.Default.StatusGroup ?? "Engraving";
			_vm.AppState.CurrentVersion = Settings.Default.CurrentVersion ?? "DEV";
			_vm.AppState.PreviousVersion = Settings.Default.PreviousVersion ?? "DEV";
			_vm.AppState.OrderFilterGroup = JsonConvert.DeserializeObject<OrderCombineFilterModel>(Settings.Default.OrderFilterGroup);
			SwitchStatusPaneGroupToType(_vm.AppState.StatusGroup);
			SelectHeaderButton();
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState,
				new SuppressNavigationTransitionInfo());
		}

		private void MainNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			if (ContentFrame.CanGoBack) ContentFrame.GoBack();
		}

		private void SelectHeaderButton()
		{
			switch (_vm.AppState.OrderFilterGroup.SelectedFilterGroup)
			{
				case "Search":
					{

						return;
					}

				case "Production Issues":
					{
						ProductionIssuesButton.IsChecked = true;
						return;
					}

				case "Engraving":
					{

						EngravingButton.IsChecked = true;
						return;
					}

				case "Ready To Engrave":
					{
						ReadyToEngraveButton.IsChecked = true;
						return;
					}


				case "Machine":
					{
				
						var x = _vm.AppState.OrderFilterGroup.machine;
						if (String.IsNullOrEmpty(x)) return;
						var machine = Int32.Parse(x);
						if (machine > 0 && machine < 5) _vm.Workstations.Data[0].IsChecked = true;
						if (machine > 4 && machine < 9) _vm.Workstations.Data[1].IsChecked = true;
						if (machine > 8 && machine < 13) _vm.Workstations.Data[2].IsChecked = true;
						if (machine > 12 && machine < 15) _vm.Workstations.Data[3].IsChecked = true;
						if (machine > 14 && machine < 17) _vm.Workstations.Data[4].IsChecked = true;
						if (machine > 17 && machine < 22) _vm.Workstations.Data[5].IsChecked = true;
						if (machine > 21 && machine < 26) _vm.Workstations.Data[6].IsChecked = true;
						if (machine > 25 && machine < 28) _vm.Workstations.Data[7].IsChecked = true;
						if (machine > 27 && machine < 30) _vm.Workstations.Data[8].IsChecked = true;
						if (machine > 29 && machine < 33) _vm.Workstations.Data[9].IsChecked = true;
						if (machine == 33) _vm.Workstations.Data[10].IsChecked = true;
						return;
					}
				case "Ready To Ship":
					{

						ReadyToShipButton.IsChecked = true;
						return;
					}
				case "Awaiting Shipment":
					{
						AwaitingShipmentButton.IsChecked = true;
						return;
					}
				case "Ship By Today":
					{

						ShipByTodayButton.IsChecked = true;
						return;
					}

			}
		}

		private void SwitchStatusPaneGroupToType(string type)
		{
			switch (type)
			{
				case "Shipping":
				{
					_vm.ShippingStatusGroupVisibility = Visibility.Visible;
					_vm.EngravingStatusGroupVisibility = Visibility.Collapsed;
					_vm.ActiveMachinesGroupVisibility = Visibility.Collapsed;
					_vm.AppState.StatusGroup = "Shipping";
					return;
				}
				case "Engraving":
				{
					_vm.ShippingStatusGroupVisibility = Visibility.Collapsed;
					_vm.EngravingStatusGroupVisibility = Visibility.Visible;
					_vm.ActiveMachinesGroupVisibility = Visibility.Collapsed;
					_vm.AppState.StatusGroup = "Engraving";
					return;
				}
				case "Machines":
				{
					_vm.ShippingStatusGroupVisibility = Visibility.Collapsed;
					_vm.EngravingStatusGroupVisibility = Visibility.Collapsed;
					_vm.ActiveMachinesGroupVisibility = Visibility.Visible;
					_vm.AppState.StatusGroup = "Machines";
					return;
				}
			}
		}
		private void SwitchStatusPanel()
		{
			switch (_vm.AppState.StatusGroup)
			{
				case "Engraving":
					SwitchStatusPaneGroupToType("Machines");
					return;
				case "Machines":
					SwitchStatusPaneGroupToType("Shipping");
					return;
				case "Shipping":
					SwitchStatusPaneGroupToType("Engraving");
					return;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			ToastNotificationManagerCompat.History.Clear();
			Settings.Default.Top = _vm.AppState.Top;
			Settings.Default.Left = _vm.AppState.Left;
			Settings.Default.Height = _vm.AppState.Height;
			Settings.Default.Width = _vm.AppState.Width;
			Settings.Default.Maximized = _vm.AppState.WindowState == WindowState.Maximized;
			Settings.Default.OrderFilterGroup = JsonConvert.SerializeObject(_vm.AppState.OrderFilterGroup, Formatting.Indented);
			Settings.Default.StatusGroup = _vm.AppState.StatusGroup;
			Settings.Default.PreviousVersion = _vm.AppState.PreviousVersion;
			Settings.Default.Save();
		}

		private void SwitchStatusPanelButtonOnClick(object sender, RoutedEventArgs e) => SwitchStatusPanel();

		private void ReadyToShipButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Ready To Ship");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void SetActiveButton(ToggleButton button)
		{
			AwaitingShipmentButton.IsChecked = false;
			ShipByTodayButton.IsChecked = false;
			ReadyToShipButton.IsChecked = false;
			ReadyToEngraveButton.IsChecked = false;
			EngravingButton.IsChecked = false;
			ProductionIssuesButton.IsChecked = false;
			if (button.Tag != null)
			{
				foreach (var workstation in _vm.Workstations.Data)
			{
				if (workstation.Id == Int32.Parse(button.Tag.ToString()) && workstation.MachinesGroupVisibility == Visibility.Visible)
				{
					_vm.Workstations.PanelSpacing = 45;
					workstation.MachinesGroupVisibility = Visibility.Collapsed;
					workstation.IsChecked = false;
					Settings.Default.SelectedWorkstation = 0;
					return;
				}

				if (workstation.MachinesGroupVisibility == Visibility.Visible)
				{
					_vm.Workstations.PanelSpacing = 45;
					workstation.MachinesGroupVisibility = Visibility.Collapsed;
					workstation.IsChecked = false;
					Settings.Default.SelectedWorkstation = 0;
				}
				if (workstation.Id == Int32.Parse(button.Tag.ToString()))
				{
					workstation.MachinesGroupVisibility = Visibility.Visible;
					workstation.IsChecked = true;
					Settings.Default.SelectedWorkstation = workstation.Id;
					if (workstation.Id == 10 || workstation.Id == 11) _vm.Workstations.PanelSpacing = 35;
				}
			}
			}
			button.IsChecked = true;
		}

		private void AwaitingShipmentButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Awaiting Shipment");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void ShipByTodayButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Ship By Today");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void WorkstationButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
		}

		private void ReadyToEngraveButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Ready To Engrave");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void EngravingButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Engraving");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void ProductionIssuesButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Production Issues");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void NavigateToMachine(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Machine", ((Button) sender).Tag.ToString());
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}


		

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			var tag = ((MenuItem) sender).Tag.ToString();
			Process.Start("shutdown", $"-s -f -t 00 -m {tag}");
			Utils.Utils.Notifier.ShowSuccess(
				$"Machine{tag.Split('-')[0].Replace('\\', ' ')} Turned Off Succesfully!\nPlease Wait....");
		}

		private async void PowerAllMachinesButtonClick(object sender, RoutedEventArgs e)
		{
			var kind = ((Button)sender).Tag.ToString();
			var dialog = new MachinePowerDialog(kind);
			var result = await dialog.ShowAsync();
			if (result != ContentDialogResult.Primary) return;
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
				foreach (var workstation in _vm.Workstations.Data)
				{
					foreach (var machine in workstation.Machines)
					{
						Process.Start("shutdown", $"-s -f -t 00 -m {machine.NetworkPath}");
					}
				}
				Utils.Utils.Notifier.ShowSuccess("Machine Power Off Request Sent Succesfully!\nPlease Wait....");
			}
		}
	}
}