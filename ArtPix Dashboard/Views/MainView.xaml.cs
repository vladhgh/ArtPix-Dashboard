﻿using System;
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

		public ShippingDashboardView ShippingView = new();

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
		}
		
		#endregion


		#region INITIALIZE USER SETTINGS

		private void InitializeSettings()
		{
			Utils.Utils.EnableBlur(this);
			var x = JsonConvert.DeserializeObject<AppStateModel>(Settings.Default.AppState);
			var statusGroup = "Shipping";
			if (x != null)
			{
				statusGroup = x.CurrentSession.StatusGroup;
				x.CurrentSession = new();
				x.CombinedFilter.pageNumber = 1;
				ViewModel.AppState = x;
			}
			Window.Top = Settings.Default.Top;
			Window.Left = Settings.Default.Left;
			Window.Width = Settings.Default.Width;
			Window.Height = Settings.Default.Height;
			SwitchStatusPaneGroupToType(statusGroup);
			SelectHeaderButton();
			ContentFrame.Navigate(ShippingView, ViewModel.AppState);
		}

		#endregion


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

				case "Engraving In Progress":
					{

						EngravingButton.IsChecked = true;
						ViewModel.AppState.NavigationStack.Add("Engraving In Progress");
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
			Settings.Default.Top = Window.Top;
			Settings.Default.Left = Window.Left;
			Settings.Default.Width = Window.Width;
			Settings.Default.Height = Window.Height;
			Settings.Default.Save();
		}

		private void SwitchStatusPanelButtonOnClick(object sender, RoutedEventArgs e) => SwitchStatusPanel();

		private void ReadyToShipButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
			ShippingView.SendCombinedRequest(new CombinedFilterModel("Ready To Ship"));
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
						ViewModel.WorkstationStats.PanelSpacing = 47;
						workstation.MachinesGroupVisibility = Visibility.Collapsed;
						workstation.IsChecked = false;
						return;
					}

					if (workstation.MachinesGroupVisibility == Visibility.Visible)
					{
						ViewModel.WorkstationStats.PanelSpacing = 47;
						workstation.MachinesGroupVisibility = Visibility.Collapsed;
						workstation.IsChecked = false;
					}

					if (workstation.Id == Int32.Parse(button.Tag.ToString()))
					{
						workstation.MachinesGroupVisibility = Visibility.Visible;
						workstation.IsChecked = true;
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

			if (button.Tag.ToString().Contains(" "))
			{
				foreach (var workstation in ViewModel.WorkstationStats.Data)
				{
					workstation.IsChecked = false;
					workstation.MachinesGroupVisibility = Visibility.Collapsed;
				}
			}
		}

		private void AwaitingShipmentButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
			ShippingView.SendCombinedRequest(new CombinedFilterModel("Awaiting Shipment"));
		}

		private void ShipByTodayButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
			ShippingView.SendCombinedRequest(new CombinedFilterModel("Ship By Today"));
		}

		private void WorkstationButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
		}

		private void AwaitingModelButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
			ShippingView.SendCombinedRequest(new CombinedFilterModel("Awaiting Model"));
		}

		private void ReadyToEngraveButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
			ShippingView.SendCombinedRequest(new CombinedFilterModel("Ready To Engrave"));
		}

		private void EngravingButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
			ShippingView.SendCombinedRequest(new CombinedFilterModel("Engraving In Progress"));
		}

		private void ProductionIssuesButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
			ShippingView.SendCombinedRequest(new CombinedFilterModel("Production Issues"));
		}

		private void NavigateToMachine(object sender, RoutedEventArgs e)
		{
			var button = ((ToggleButton) sender);
			foreach (var workstation in ViewModel.WorkstationStats.Data)
			{
				if (workstation.IsChecked)
				{
					foreach (var machine in workstation.Machines)
					{
						machine.IsSelected = false;

						if (machine.IdMachines == (int)button.Tag)
						{
							ShippingView.SendCombinedRequest(new CombinedFilterModel("Machine", ((ToggleButton)sender).Tag.ToString()));
							machine.IsSelected = true;
						}
					}
				}
			}
			
		}

		

		private void EngravedTodayButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
			ShippingView.SendCombinedRequest(new CombinedFilterModel("Engraved Today"));
		}

		private void BackButtonOnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.AppState.NavigationStack.Remove(ViewModel.AppState.NavigationStack.Last());
			ViewModel.AppState.CombinedFilter = new CombinedFilterModel(ViewModel.AppState.NavigationStack.Last());
			SetActiveButton(null, ViewModel.AppState.NavigationStack.Last());
			ViewModel.AppState.CurrentSession.IsBackButtonActive = ViewModel.AppState.NavigationStack.Count > 1;
			ShippingView.SendCombinedRequest(new CombinedFilterModel(ViewModel.AppState.NavigationStack.Last()));
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new LoginDialog(ViewModel.AppState);
			var result = await dialog.ShowAsync();
			if (result != ContentDialogResult.Primary) return;
		}

		private void MachinesPowerButtonOnClick(object sender, RoutedEventArgs e)
		{
			var dialog = new MachinePowerDialog(ViewModel);
			dialog.ShowAsync();
		}
	}
}