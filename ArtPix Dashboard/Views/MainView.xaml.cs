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
			SwitchStatusPanel();
			_vm.AppState.OrderFilterGroup = JsonConvert.DeserializeObject<OrderCombineFilterModel>(Settings.Default.OrderFilterGroup);
			SwitchStatusPaneGroupToType("Shipping");
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState,
				new SuppressNavigationTransitionInfo());
		}

		private void MainNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			if (ContentFrame.CanGoBack) ContentFrame.GoBack();
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
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void AwaitingShipmentButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Awaiting Shipment");
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void ShipByTodayButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Ship By Today");
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void WorkstationButtonOnClick(object sender, RoutedEventArgs e)
		{
			var tag = (int) ((ToggleButton) sender).Tag;
			foreach (var workstation in _vm.Workstations.Data)
			{
				if (workstation.Id == tag && workstation.MachinesGroupVisibility == Visibility.Visible)
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

				if (workstation.Id == tag)
				{
					workstation.MachinesGroupVisibility = Visibility.Visible;
					workstation.IsChecked = true;
					Settings.Default.SelectedWorkstation = workstation.Id;
					if (workstation.Id == 10 || workstation.Id == 11) _vm.Workstations.PanelSpacing = 35;
				}
			}
		}

		private void ReadyToEngraveButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Ready To Engrave");
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void EngravingButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Engraving");
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void ProductionIssuesButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Production Issues");
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void NavigateToMachine(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Machine", ((Button) sender).Tag.ToString());
			ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new SuppressNavigationTransitionInfo());
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			SendWakeOnLan(PhysicalAddress.Parse(((MenuItem) sender).Tag.ToString()));
			Utils.Utils.Notifier.ShowSuccess("Machine Turned On Succesfully!\nPlease Wait....");
		}

		public void SendWakeOnLan(PhysicalAddress target)
		{
			var header = Enumerable.Repeat(byte.MaxValue, 6);
			var data = Enumerable.Repeat(target.GetAddressBytes(), 16).SelectMany(mac => mac);

			var magicPacket = header.Concat(data).ToArray();

			var client = new UdpClient();

			client.Send(magicPacket, magicPacket.Length, new IPEndPoint(IPAddress.Broadcast, 9));
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			var tag = ((MenuItem) sender).Tag.ToString();
			Process.Start("shutdown", $"-s -f -t 00 -m {tag}");
			Utils.Utils.Notifier.ShowSuccess(
				$"Machine{tag.Split('-')[0].Replace('\\', ' ')} Turned Off Succesfully!\nPlease Wait....");
		}

		private async void MenuItem_Click_2(object sender, RoutedEventArgs e)
		{
			var tag = ((MenuItem) sender).Tag;
			if (tag != null) await ArtPixAPI.RemoveCurrentJobsFromMachineAsync((int) tag);
		}
	}
}