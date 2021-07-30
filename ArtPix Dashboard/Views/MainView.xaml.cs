using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ArtPix_Dashboard.Properties;
using ArtPix_Dashboard.ViewModels;
using Microsoft.Toolkit.Uwp.Notifications;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using Newtonsoft.Json;
using ToastNotifications.Messages;
using ArtPix_Dashboard.Dialogs;
using ArtPix_Dashboard.Utils;
using ArtPix_Dashboard.Models.AppState;

namespace ArtPix_Dashboard.Views
{
	
	public partial class MainView
	{

		internal readonly MainViewModel ViewModel = new ();

		#region CONSTRUCTOR

		public MainView()
		{
			DataContext = ViewModel;
			InitializeComponent();
		}

		#endregion

		#region ON LOADED

		private void MainViewOnLoaded(object sender, RoutedEventArgs e)
		{
			ViewModel.Initialize();
			InitializeSettings();
			ShowChangesDialog();
		}
		
		#endregion

		#region SHOW CHANGES DIALOG - DONE

		private async void ShowChangesDialog()
		{
			if (ViewModel.AppState.CurrentSession.CurrentVersion == ViewModel.AppState.CurrentSession.PreviousVersion) return;
			var dialog = new ChangeLogsDialog();
			var result = await dialog.ShowAsync();
			ViewModel.AppState.CurrentSession.PreviousVersion = ViewModel.AppState.CurrentSession.CurrentVersion;
		}
		
		#endregion


		private void InitializeSettings()
		{
			Utils.Utils.EnableBlur(this);
			Window.Closing += Window_Closing;
			var x = JsonConvert.DeserializeObject<AppStateModel>(Settings.Default.AppState);
			if (x != null) ViewModel.AppState = x;
			SwitchStatusPaneGroupToType(ViewModel.AppState.CurrentSession.StatusGroup);
			SelectHeaderButton();
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
		}

		private void SelectHeaderButton()
		{
			switch (ViewModel.AppState.CombinedFilter.SelectedFilterGroup)
			{
				case "Search":
					{

						return;
					}

				case "Production Issues":
					{
						ProductionIssuesButton.IsChecked = true;
						ViewModel.AppState.NavigationStack.Add("Production Issues");
						return;
					}

				case "Engraving":
					{

						EngravingButton.IsChecked = true;
						ViewModel.AppState.NavigationStack.Add("Engraving");
						return;
					}

				case "Ready To Engrave":
					{
						ReadyToEngraveButton.IsChecked = true;
						ViewModel.AppState.NavigationStack.Add("Ready To Engrave");
						return;
					}


				case "Machine":
					{
				
						var x = ViewModel.AppState.CombinedFilter.machine;
						if (String.IsNullOrEmpty(x)) return;
						var machine = Int32.Parse(x);
						if (machine > 0 && machine < 5) ViewModel.WorkstationStats.Data[0].IsChecked = true;
						if (machine > 4 && machine < 9) ViewModel.WorkstationStats.Data[1].IsChecked = true;
						if (machine > 8 && machine < 13) ViewModel.WorkstationStats.Data[2].IsChecked = true;
						if (machine > 12 && machine < 15) ViewModel.WorkstationStats.Data[3].IsChecked = true;
						if (machine > 14 && machine < 17) ViewModel.WorkstationStats.Data[4].IsChecked = true;
						if (machine > 17 && machine < 22) ViewModel.WorkstationStats.Data[5].IsChecked = true;
						if (machine > 21 && machine < 26) ViewModel.WorkstationStats.Data[6].IsChecked = true;
						if (machine > 25 && machine < 28) ViewModel.WorkstationStats.Data[7].IsChecked = true;
						if (machine > 27 && machine < 30) ViewModel.WorkstationStats.Data[8].IsChecked = true;
						if (machine > 29 && machine < 33) ViewModel.WorkstationStats.Data[9].IsChecked = true;
						if (machine == 33) ViewModel.WorkstationStats.Data[10].IsChecked = true;
						return;
					}
				case "Ready To Ship":
					{
						ViewModel.AppState.NavigationStack.Add("Ready To Ship");
						ReadyToShipButton.IsChecked = true;
						return;
					}
				case "Awaiting Shipment":
					{
						ViewModel.AppState.NavigationStack.Add("Awaiting Shipment");
						AwaitingShipmentButton.IsChecked = true;
						return;
					}
				case "Awaiting Model":
				{
					ViewModel.AppState.NavigationStack.Add("Awaiting Model");
					AwaitingModelButton.IsChecked = true;
					return;
				}
				case "Engraved Today":
				{
					ViewModel.AppState.NavigationStack.Add("Engraved Today");
					EngravedTodayButton.IsChecked = true;
					return;
				}
				case "Ship By Today":
					{
						ViewModel.AppState.NavigationStack.Add("Ship By Today");
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
					ViewModel.AppState.CurrentSession.ShippingStatusGroupVisibility = Visibility.Visible;
					ViewModel.AppState.CurrentSession.EngravingStatusGroupVisibility = Visibility.Collapsed;
					ViewModel.AppState.CurrentSession.ActiveMachinesGroupVisibility = Visibility.Collapsed;
					ViewModel.AppState.CurrentSession.StatusGroup = "Shipping";
					return;
				}
				case "Engraving":
				{
					ViewModel.AppState.CurrentSession.ShippingStatusGroupVisibility = Visibility.Collapsed;
					ViewModel.AppState.CurrentSession.EngravingStatusGroupVisibility = Visibility.Visible;
					ViewModel.AppState.CurrentSession.ActiveMachinesGroupVisibility = Visibility.Collapsed;
					ViewModel.AppState.CurrentSession.StatusGroup = "Engraving";
					return;
				}
				case "Machines":
				{
					ViewModel.AppState.CurrentSession.ShippingStatusGroupVisibility = Visibility.Collapsed;
					ViewModel.AppState.CurrentSession.EngravingStatusGroupVisibility = Visibility.Collapsed;
					ViewModel.AppState.CurrentSession.ActiveMachinesGroupVisibility = Visibility.Visible;
					ViewModel.AppState.CurrentSession.StatusGroup = "Machines";
					return;
				}
			}
		}
		private void SwitchStatusPanel()
		{
			switch (ViewModel.AppState.CurrentSession.StatusGroup)
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
			Settings.Default.AppState = JsonConvert.SerializeObject(ViewModel.AppState, Formatting.Indented);
			Settings.Default.Save();
		}

		private void SwitchStatusPanelButtonOnClick(object sender, RoutedEventArgs e) => SwitchStatusPanel();

		private void ReadyToShipButtonOnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel("Ready To Ship");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
		}

		private void SetActiveButton(ToggleButton button, string elementName = "")
		{
			AwaitingShipmentButton.IsChecked = false;
			AwaitingModelButton.IsChecked = false;
			ShipByTodayButton.IsChecked = false;
			ReadyToShipButton.IsChecked = false;
			ReadyToEngraveButton.IsChecked = false;
			EngravingButton.IsChecked = false;
			EngravedTodayButton.IsChecked = false;
			ProductionIssuesButton.IsChecked = false;

			if (button == null)
			{
				button = (ToggleButton)this.FindName(elementName.Replace(" ", "") + "Button");
				if (button != null)
				{
					button.IsChecked = true;
					return;
				}
			}

			if (button.Tag != null && Int32.TryParse(button.Tag.ToString(), out int res))
			{
				foreach (var workstation in ViewModel.WorkstationStats.Data)
				{
					if (workstation.Id == Int32.Parse(button.Tag.ToString()) &&
					    workstation.MachinesGroupVisibility == Visibility.Visible)
					{
						ViewModel.WorkstationStats.PanelSpacing = 45;
						workstation.MachinesGroupVisibility = Visibility.Collapsed;
						workstation.IsChecked = false;
						Settings.Default.SelectedWorkstation = 0;
						return;
					}

					if (workstation.MachinesGroupVisibility == Visibility.Visible)
					{
						ViewModel.WorkstationStats.PanelSpacing = 45;
						workstation.MachinesGroupVisibility = Visibility.Collapsed;
						workstation.IsChecked = false;
						Settings.Default.SelectedWorkstation = 0;
					}

					if (workstation.Id == Int32.Parse(button.Tag.ToString()))
					{
						workstation.MachinesGroupVisibility = Visibility.Visible;
						workstation.IsChecked = true;
						Settings.Default.SelectedWorkstation = workstation.Id;
						if (workstation.Id == 10 || workstation.Id == 11) ViewModel.WorkstationStats.PanelSpacing = 35;
					}
				}
			}

			if (button != null)
			{
				button.IsChecked = true;
				ViewModel.AppState.NavigationStack.Add(button.Tag.ToString());
				ViewModel.AppState.CurrentSession.IsBackButtonActive = ViewModel.AppState.NavigationStack.Count > 0;
			}
		}

		private void AwaitingShipmentButtonOnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel("Awaiting Shipment");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
		}

		private void ShipByTodayButtonOnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel("Ship By Today");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
		}

		private void WorkstationButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
		}

		private void AwaitingModelButtonOnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel("Awaiting Model");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
		}

		private void ReadyToEngraveButtonOnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel("Ready To Engrave");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
		}

		private void EngravingButtonOnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel("Engraving");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
		}

		private void ProductionIssuesButtonOnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel("Production Issues");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
		}

		private void NavigateToMachine(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel("Machine", ((Button) sender).Tag.ToString());
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
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

		private void EngravedTodayButtonOnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel("Engraved Today");
			SetActiveButton((ToggleButton)sender);
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
		}

		private void BackButtonOnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.NavigationStack.Remove(ViewModel.AppState.NavigationStack.Last());
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel(ViewModel.AppState.NavigationStack.Last());
			SetActiveButton(null, ViewModel.AppState.NavigationStack.Last());
			ViewModel.AppState.CurrentSession.IsBackButtonActive = ViewModel.AppState.NavigationStack.Count > 1;
			ContentFrame.Navigate(typeof(ShippingDashboardView), ViewModel.AppState, new SuppressNavigationTransitionInfo());
		}
	}
}