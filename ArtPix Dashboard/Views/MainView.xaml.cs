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
using System.Windows.Input;
using System.Reactive.Linq;

namespace ArtPix_Dashboard.Views
{
	
	public partial class MainView
	{

		internal readonly MainViewModel MainViewModel = new ();

		public ShippingDashboardView ShippingView = new();

		private string _inputString;

		private bool _tabPressed;

		#region CONSTRUCTOR

		public MainView()
		{
			DataContext = MainViewModel;
			InitializeComponent();
		}

		#endregion

		#region ON LOADED

		private async void MainViewOnLoaded(object sender, RoutedEventArgs e)
		{
			ToggleLoadingAnimation(1);

			InitializeSettings();
			
			await MainViewModel.Initialize();

			ToggleLoadingAnimation(0);
		}

		#endregion

		#region TOGGLE MAIN LOADING ANIMATION

		private void ToggleLoadingAnimation(int kind)
		{
			if (kind == 0)
			{
				if (String.IsNullOrEmpty(MainViewModel.AppState.EmployeeName))
				{
					Animation.FadeIn(WelcomeText);
					Animation.FadeIn(WelcomeSecondaryText);
					Animation.FadeIn(AwaitingScanText);
					SetKeyPressEventListener();
					return;
				}
				MainViewModel.AppState.CurrentSession.MainNavigationViewVisibility = Visibility.Visible;
				Animation.FadeIn(MainNavigationView);
				Animation.FadeOut(MainLoader);
				if (WelcomeText.Opacity == 1)
				{
					Animation.FadeOut(WelcomeText);
					Animation.FadeOut(WelcomeSecondaryText);
					Animation.FadeOut(AwaitingScanText);
					UnloadKeyPressEventListener();
				}
				ShippingView.ShippingDashboardPage.Focus();
				MainViewModel.SetGetEntityLogsTimer();
				SetSessionTimeOutTimer();
			}

			if (kind == 1)
			{
				Animation.FadeOut(MainNavigationView);
				Animation.FadeIn(MainLoader);
			}
		}

		#endregion

		private void SetSessionTimeOutTimer()
		{
			var updateSessionTimeOutTimer = Observable.Interval(TimeSpan.FromSeconds(15));
			updateSessionTimeOutTimer.Subscribe(tick => CheckSessionTimeOut());
		}

		private void CheckSessionTimeOut()
		{
			MainViewModel.AppState.SessionTimeOut -= 15;
			if (MainViewModel.AppState.SessionTimeOut <= 0 && MainViewModel.AppState.EmployeeName != "")
			{
				MainViewModel.AppState.SessionTimeOut = 0;
				MainViewModel.AppState.EmployeeName = "";
				Application.Current.Dispatcher.Invoke(
				() =>
				{
					Animation.FadeOut(MainNavigationView);
					Animation.FadeIn(MainLoader);
					Animation.FadeIn(WelcomeText);
					Animation.FadeIn(WelcomeSecondaryText);
					Animation.FadeIn(AwaitingScanText);
					MainViewModel.AppState.CurrentSession.MainNavigationViewVisibility = Visibility.Hidden;
					SetKeyPressEventListener();
				});
				
			}
		}

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
				MainViewModel.AppState = x;
			}
			Window.Top = Settings.Default.Top;
			Window.Left = Settings.Default.Left;
			Window.Width = Settings.Default.Width;
			Window.Height = Settings.Default.Height;
			SwitchStatusPaneGroupToType(statusGroup);
			SelectHeaderButton();
			ContentFrame.Navigate(ShippingView, MainViewModel.AppState);
		}

		#endregion

		#region SWITCH STATUS PANEL GROUP TO TYPE

		private void SwitchStatusPaneGroupToType(string type)
		{
			switch (type)
			{
				case "Shipping":
					{
						MainViewModel.AppState.CurrentSession.ShippingStatusGroupVisibility = Visibility.Visible;
						MainViewModel.AppState.CurrentSession.EngravingStatusGroupVisibility = Visibility.Collapsed;
						MainViewModel.AppState.CurrentSession.ActiveMachinesGroupVisibility = Visibility.Collapsed;
						MainViewModel.AppState.CurrentSession.StatusGroup = "Shipping";
						return;
					}
				case "Engraving":
					{
						MainViewModel.AppState.CurrentSession.ShippingStatusGroupVisibility = Visibility.Collapsed;
						MainViewModel.AppState.CurrentSession.EngravingStatusGroupVisibility = Visibility.Visible;
						MainViewModel.AppState.CurrentSession.ActiveMachinesGroupVisibility = Visibility.Collapsed;
						MainViewModel.AppState.CurrentSession.StatusGroup = "Engraving";
						return;
					}
				case "Machines":
					{
						MainViewModel.AppState.CurrentSession.ShippingStatusGroupVisibility = Visibility.Collapsed;
						MainViewModel.AppState.CurrentSession.EngravingStatusGroupVisibility = Visibility.Collapsed;
						MainViewModel.AppState.CurrentSession.ActiveMachinesGroupVisibility = Visibility.Visible;
						MainViewModel.AppState.CurrentSession.StatusGroup = "Machines";
						return;
					}
			}
		}

		#endregion

		#region SELECT HEADER BUTTON

		private void SelectHeaderButton()
		{
			switch (MainViewModel.AppState.CombinedFilter.SelectedFilterGroup)
			{
				case "Search":
					{

						return;
					}

				case "Production Issues":
					{
						ProductionIssuesButton.IsChecked = true;
						MainViewModel.AppState.NavigationStack.Add("Production Issues");
						return;
					}

				case "Engraving In Progress":
					{

						EngravingButton.IsChecked = true;
						MainViewModel.AppState.NavigationStack.Add("Engraving In Progress");
						return;
					}

				case "Ready To Engrave":
					{
						ReadyToEngraveButton.IsChecked = true;
						MainViewModel.AppState.NavigationStack.Add("Ready To Engrave");
						return;
					}


				case "Machine":
					{
				
						var x = MainViewModel.AppState.CombinedFilter.machine;
						if (String.IsNullOrEmpty(x)) return;
						var machine = Int32.Parse(x);
						if (machine > 0 && machine < 5) MainViewModel.WorkstationStats.Data[0].IsChecked = true;
						if (machine > 4 && machine < 9) MainViewModel.WorkstationStats.Data[1].IsChecked = true;
						if (machine > 8 && machine < 13) MainViewModel.WorkstationStats.Data[2].IsChecked = true;
						if (machine > 12 && machine < 15) MainViewModel.WorkstationStats.Data[3].IsChecked = true;
						if (machine > 14 && machine < 17) MainViewModel.WorkstationStats.Data[4].IsChecked = true;
						if (machine > 17 && machine < 22) MainViewModel.WorkstationStats.Data[5].IsChecked = true;
						if (machine > 21 && machine < 26) MainViewModel.WorkstationStats.Data[6].IsChecked = true;
						if (machine > 25 && machine < 28) MainViewModel.WorkstationStats.Data[7].IsChecked = true;
						if (machine > 27 && machine < 30) MainViewModel.WorkstationStats.Data[8].IsChecked = true;
						if (machine > 29 && machine < 33) MainViewModel.WorkstationStats.Data[9].IsChecked = true;
						if (machine == 33) MainViewModel.WorkstationStats.Data[10].IsChecked = true;
						return;
					}
				case "Ready To Ship":
					{
						MainViewModel.AppState.NavigationStack.Add("Ready To Ship");
						ReadyToShipButton.IsChecked = true;
						return;
					}
				case "Awaiting Shipment":
					{
						MainViewModel.AppState.NavigationStack.Add("Awaiting Shipment");
						AwaitingShipmentButton.IsChecked = true;
						return;
					}
				case "Awaiting Model":
				{
					MainViewModel.AppState.NavigationStack.Add("Awaiting Model");
					AwaitingModelButton.IsChecked = true;
					return;
				}
				case "Engraved Today":
				{
					MainViewModel.AppState.NavigationStack.Add("Engraved Today");
					EngravedTodayButton.IsChecked = true;
					return;
				}
				case "Ship By Today":
					{
						MainViewModel.AppState.NavigationStack.Add("Ship By Today");
						ShipByTodayButton.IsChecked = true;
						return;
					}

			}
		}

		#endregion

		#region SWITCH STATUS PANEL TO NEXT GROUP

		private void SwitchStatusPanel()
		{
			switch (MainViewModel.AppState.CurrentSession.StatusGroup)
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

		#endregion

		#region WINDOW CLOSING EVENT HANDLER

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			ToastNotificationManagerCompat.History.Clear();
			Settings.Default.AppState = JsonConvert.SerializeObject(MainViewModel.AppState, Formatting.Indented);
			Settings.Default.EmployeeName = MainViewModel.AppState.EmployeeName;
			Settings.Default.SessionTimeOut = MainViewModel.AppState.SessionTimeOut;
			Settings.Default.Top = Window.Top;
			Settings.Default.Left = Window.Left;
			Settings.Default.Width = Window.Width;
			Settings.Default.Height = Window.Height;
			Settings.Default.Save();
		}

		#endregion

		#region HEADER BUTTON CLICK EVENT HANDLERS (LIST VIEW FILTERS)

		private void ReadyToShipButtonOnClick(object sender, RoutedEventArgs e)
		{
			SetActiveButton((ToggleButton)sender);
			ShippingView.SendCombinedRequest(new CombinedFilterModel("Ready To Ship"));
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

		private void WorkstationButtonOnClick(object sender, RoutedEventArgs e) => SetActiveButton((ToggleButton)sender);

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
			var button = ((ToggleButton)sender);
			foreach (var workstation in MainViewModel.WorkstationStats.Data)
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

		#endregion

		#region SET ACTIVE HEADER BUTTON

		public void SetActiveButton(ToggleButton button, string elementName = "")
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
				if (elementName == "None")
				{
					foreach (var workstation in MainViewModel.WorkstationStats.Data)
					{
						workstation.IsChecked = false;
						workstation.MachinesGroupVisibility = Visibility.Collapsed;
					}
				}
				button = (ToggleButton)this.FindName(elementName.Replace(" ", "") + "Button");
				if (button != null)
				{
					button.IsChecked = true;
					return;
				}
			}

			if (button.Tag != null && Int32.TryParse(button.Tag.ToString(), out int res))
			{
				foreach (var workstation in MainViewModel.WorkstationStats.Data)
				{
					if (workstation.Id == Int32.Parse(button.Tag.ToString()) &&
						workstation.MachinesGroupVisibility == Visibility.Visible)
					{
						MainViewModel.WorkstationStats.PanelSpacing = 47;
						workstation.MachinesGroupVisibility = Visibility.Collapsed;
						workstation.IsChecked = false;
						return;
					}

					if (workstation.MachinesGroupVisibility == Visibility.Visible)
					{
						MainViewModel.WorkstationStats.PanelSpacing = 47;
						workstation.MachinesGroupVisibility = Visibility.Collapsed;
						workstation.IsChecked = false;
					}

					if (workstation.Id == Int32.Parse(button.Tag.ToString()))
					{
						workstation.MachinesGroupVisibility = Visibility.Visible;
						workstation.IsChecked = true;
						if (workstation.Id == 10 || workstation.Id == 11) MainViewModel.WorkstationStats.PanelSpacing = 35;
					}
				}
			}

			if (button != null)
			{
				button.IsChecked = true;
				MainViewModel.AppState.NavigationStack.Add(button.Tag.ToString());
				MainViewModel.AppState.CurrentSession.IsBackButtonActive = MainViewModel.AppState.NavigationStack.Count > 0;
			}

			if (button.Tag.ToString().Contains(" "))
			{
				foreach (var workstation in MainViewModel.WorkstationStats.Data)
				{
					workstation.IsChecked = false;
					workstation.MachinesGroupVisibility = Visibility.Collapsed;
				}
			}
		}

		#endregion

		#region SCAN EVENT LISTENER

		//TODO: ADD TOTE SCAN ABILITY AND ASSURE PROPER FUNCTIONING

		private void SetKeyPressEventListener()
		{
			Window.PreviewKeyDown += KeyPressEventListener;
			Window.KeyDown += ShippingDashboardPage_KeyDown; ;
			Window.PreviewKeyUp += ShippingDashboardPage_KeyDown;
			Window.KeyUp += ShippingDashboardPage_KeyDown;
			Window.Focus();
		}

		private void UnloadKeyPressEventListener()
		{
			Window.PreviewKeyDown -= KeyPressEventListener;
			Window.KeyDown -= ShippingDashboardPage_KeyDown; ;
			Window.PreviewKeyUp -= ShippingDashboardPage_KeyDown;
			Window.KeyUp -= ShippingDashboardPage_KeyDown;
			ShippingView.ShippingDashboardPage.Focus();
		}

		public void KeyPressEventListener(object sender, KeyEventArgs e)
		{
			Debug.WriteLine("FIRED");
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

					MainViewModel.AppState.EmployeeName = $"{firstNameToCapitalCase} {lastNameToCapitalCase}";
					LoginButtonText.Text = $"{firstNameToCapitalCase} {lastNameToCapitalCase}";
					MainViewModel.AppState.SessionTimeOut = 24 * 60 * 60;
					ToggleLoadingAnimation(0);

				}
				_inputString = "";
				e.Handled = true;
			}
		}

		private void ShippingDashboardPage_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
		}

		#endregion

		#region NAVIGATION VIEW MENU BUTTON CLICK EVENT HANDLERS

		private void SwitchStatusPanelButtonOnClick(object sender, RoutedEventArgs e) => SwitchStatusPanel();

		private void BackButtonOnClick(object sender, RoutedEventArgs e)
		{
			MainViewModel.AppState.NavigationStack.Remove(MainViewModel.AppState.NavigationStack.Last());
			MainViewModel.AppState.CombinedFilter = new CombinedFilterModel(MainViewModel.AppState.NavigationStack.Last());
			SetActiveButton(null, MainViewModel.AppState.NavigationStack.Last());
			MainViewModel.AppState.CurrentSession.IsBackButtonActive = MainViewModel.AppState.NavigationStack.Count > 1;
			ShippingView.SendCombinedRequest(new CombinedFilterModel(MainViewModel.AppState.NavigationStack.Last()));
		}

		private async void LogOutButtonOnClick(object sender, RoutedEventArgs e)
		{
			MainViewModel.AppState.SessionTimeOut = 0;
			MainViewModel.AppState.EmployeeName = "";
			Animation.FadeOut(MainNavigationView);
			Animation.FadeIn(MainLoader);
			Animation.FadeIn(WelcomeText);
			Animation.FadeIn(WelcomeSecondaryText);
			Animation.FadeIn(AwaitingScanText);
			MainViewModel.AppState.CurrentSession.MainNavigationViewVisibility = Visibility.Hidden;
			SetKeyPressEventListener();
		}

		private void MachinesPowerButtonOnClick(object sender, RoutedEventArgs e)
		{
			var dialog = new MachinePowerDialog(MainViewModel);
			dialog.ShowAsync();
		}

		#endregion

		private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
		{
			LoginButtonText.Text = "Log Out";
		}

		private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
		{
			LoginButtonText.Text = MainViewModel.AppState.EmployeeName;
		}
	}
}