using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ArtPix_Dashboard.ViewModels;
using System.Windows.Navigation;
using ArtPix_Dashboard.Models;
using ModernWpf.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ArtPix_Dashboard.API;
using ArtPix_Dashboard.Utils;
using ModernWpf.Controls.Primitives;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Web.UI.WebControls;
using ArtPix_Dashboard.Dialogs;
using ArtPix_Dashboard.Models.AppState;
using ToastNotifications.Messages;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ArtPix_Dashboard.Views
{


	//EMOJIS: ✅❎


	public partial class ShippingDashboardView
	{

		#region PROPERTIES

		internal readonly ShippingDashboardViewModel ViewModel = new ();

		private Expander _expander;

		private string _inputString;

		private bool _tabPressed;

		#endregion

		#region CONSTRUCTOR
		public ShippingDashboardView()
		{
			InitializeComponent();
			DataContext = ViewModel;
		}

		#endregion

		#region EVENT LISTENER INITIALIZERS
		
		private void SetEventListeners()
		{
			SortByComboBox.SelectionChanged += SortByComboBoxOnSelectionChanged;
			StoreComboBox.SelectionChanged += StoreComboBoxOnSelectionChanged;
			MachineComboBox.SelectionChanged += MachineComboBox_SelectionChanged;
			ToggleShipByToday.Click += ToggleShipByToday_Click;
			ToggleNoPackage.Click += ToggleNoPackageOnClick;
			ToggleInTotes.Click += ToggleInTotes_Click;
			ToggleNoCrystal.Click += ToggleNoCrystal_Click;
		}


		private void SetKeyPressEventListener()
		{
			ShippingDashboardPage.PreviewKeyDown += KeyPressEventListener;
			ShippingDashboardPage.KeyDown += ShippingDashboardPage_KeyDown; ;
			ShippingDashboardPage.PreviewKeyUp += ShippingDashboardPage_KeyDown;
			ShippingDashboardPage.KeyUp += ShippingDashboardPage_KeyDown;
			ShippingDashboardPage.Focus();
		}

		#endregion

		#region ON NAVIGATED TO

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.Initialize((AppStateModel)e.ExtraData, this);

			SetKeyPressEventListener();

			SendCombinedRequest(ViewModel.AppState.CombinedFilter);

			SetEventListeners();

		}

		#endregion

		#region UPDATE CONTROLS

		private void UpdateControls()
		{
			SortByComboBox.SelectedValue = ViewModel.AppState.CombinedFilter.sort_by;
			StoreComboBox.SelectedValue = ViewModel.AppState.CombinedFilter.store_name;
			MachineComboBox.SelectedValue = ViewModel.AppState.CombinedFilter.machine;
			ToggleShipByToday.IsChecked = ViewModel.AppState.CombinedFilter.shipByToday == "True";
			ToggleNoPackage.IsChecked = ViewModel.AppState.CombinedFilter.has_shipping_package == "0";
			ToggleInTotes.IsChecked = ViewModel.AppState.CombinedFilter.with_shipping_totes == "True";
			ToggleNoCrystal.IsChecked = ViewModel.AppState.CombinedFilter.with_crystals == "0";
			SearchTextBox.Text = ViewModel.AppState.CombinedFilter.name_order;
		}

		private void ToggleLoadingAnimation(int kind)
		{
			if (kind == 0)
			{
				Animation.FadeOut(ProgressRingImage);
				Animation.FadeIn(ShippingItemsListView);
				return;
			}

			if (kind == 1)
			{
				Animation.FadeOut(ShippingItemsListView);
				Animation.FadeIn(ProgressRingImage);
			}
			if (kind == 2)
			{
				Animation.FadeOut(ProgressRingImage);
				Animation.FadeIn(NoResultsText);
			}
		}

		private void UpdatePaginationButtons()
		{
			if (ViewModel.Pages.Count == 0) return;
			var selectedPage = ViewModel.Pages.First(p => p.IsSelected);
			ViewModel.PaginationForwardButtonVisibility = (ViewModel.Pages.Count > 1) && (ViewModel.Pages.Count > selectedPage.PageNumber);
			ViewModel.PaginationBackButtonVisibility = selectedPage.PageNumber > 1;
		}
		
		#endregion

		#region SEND COMBINED REQUEST

		public async void SendCombinedRequest(CombinedFilterModel orderFilterGroup)
		{

			ToggleLoadingAnimation(1);

			ViewModel.AppState.CombinedFilter = orderFilterGroup;
			ViewModel.AppState.CombinedFilter.withPages = true;

			UpdateControls();

			await ViewModel.GetOrdersList(ViewModel.AppState.CombinedFilter);

			if (ViewModel.Orders.Data == null) return;
			if (ViewModel.Orders.Data.Count <= 0)
			{
				ToggleLoadingAnimation(2);
				return;
			}
			if (ViewModel.Orders.Data.Count > 0 && NoResultsText.Opacity == 1)
			{
				Animation.FadeOut(NoResultsText);
			}
			if (ViewModel.Orders.Data.Count > 0)
			{
				var scrollViewer = Utils.Utils.GetScrollViewer(ShippingItemsListView) as ScrollViewer;
				scrollViewer.ScrollToVerticalOffset(0);
				ScrollAnimationBehavior.AnimateScroll(scrollViewer, 0);
				ScrollAnimationBehavior.intendedLocation = 0;
			}

			UpdatePaginationButtons();

			ToggleLoadingAnimation(0);

		}

		#endregion

		#region MACHINE POWER BUTTONS EVENT HANDLER - NOT DONE ❎ 

		//TODO: ADD CONTENT DIALOG POP UP

		private void PowerMachineButton(object sender, RoutedEventArgs e)
		{
			var kind = ((Button)sender).Tag.ToString();
			if (kind == "PowerOn")
			{
				var x = Utils.Utils.MachineAddresses.FirstOrDefault((y) => y.Value == ViewModel.AppState.CombinedFilter.machine);
				Utils.Utils.SendWakeOnLan(PhysicalAddress.Parse(x.Key));
				Utils.Utils.Notifier.ShowSuccess("Machine Power On Request Sent Succesfully!\nPlease Wait....");
			}
			if (kind == "PowerOff")
			{
				var networkPath = Utils.Utils.GetLocalMachineAddress(ViewModel.AppState.CombinedFilter.machine);
				Process.Start("shutdown", $"-s -f -t 00 -m {networkPath}");
				Utils.Utils.Notifier.ShowSuccess("Machine Power Off Request Sent Succesfully!\nPlease Wait....");
			}
		}

		#endregion

		#region SCAN EVENT LISTENER - NOT DONE - ❎

		//TODO: ADD TOTE SCAN ABILITY AND ASSURE PROPER FUNCTIONING

		public void KeyPressEventListener(object sender, KeyEventArgs e)
		{
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

					ViewModel.AppState.CurrentSession.EmployeeName = $"{firstNameToCapitalCase} {lastNameToCapitalCase}";
					
				} else
				{
					if (_inputString.Split('-').Length == 3)
					{
						SendCombinedRequest(new CombinedFilterModel("Search", "", "", _inputString.Split('-')[0]));
					}
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

		#region FILTER GROUP EVENT HANDLERS - DONE - ✅

		private void MachineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter.machine = ((ComboBoxItem)StoreComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest(ViewModel.AppState.CombinedFilter);
		}

		private void ToggleNoCrystal_Click(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleButton btn && btn.IsChecked != null)
			{
				ViewModel.AppState.CombinedFilter.with_crystals = (bool)btn.IsChecked ? "0" : "3";
				ViewModel.AppState.CombinedFilter.status_shipping = (bool)btn.IsChecked ? "" : "waiting";
			}
			SendCombinedRequest(ViewModel.AppState.CombinedFilter);
		}

		private void ButtonReloadOnClick(object sender, RoutedEventArgs e) => SendCombinedRequest(ViewModel.AppState.CombinedFilter);

		private void ToggleNoPackageOnClick(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
			{
				if (btn.IsChecked != null)
				{
					ViewModel.AppState.CombinedFilter.has_shipping_package = (bool)btn.IsChecked ? "0" : "";
				}
				SendCombinedRequest(ViewModel.AppState.CombinedFilter);
			}
		}

		private void StoreComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter.store_name = ((ComboBoxItem)StoreComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest(ViewModel.AppState.CombinedFilter);
		}

		private void ToggleInTotes_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					ViewModel.AppState.CombinedFilter.with_shipping_totes = ((bool)btn.IsChecked).ToString();
			SendCombinedRequest(ViewModel.AppState.CombinedFilter);
		}

		private void ToggleShipByToday_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					ViewModel.AppState.CombinedFilter.shipByToday = ((bool)btn.IsChecked).ToString();
			SendCombinedRequest(ViewModel.AppState.CombinedFilter);
		}

		private void SortByComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ViewModel.AppState.CombinedFilter.sort_by = ((ComboBoxItem)SortByComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest(ViewModel.AppState.CombinedFilter);
		}

		#endregion

		#region SEARCH BOX EVENT HANDLERS - DONE - ✅

		private void SearchTextBoxOnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) => SendCombinedRequest(new CombinedFilterModel("Search", "", SearchTextBox.Text));

		private void ButtonSearchOnClick(object sender, RoutedEventArgs e) => SendCombinedRequest(new CombinedFilterModel("Search", "", SearchTextBox.Text));

		private void ButtonClearSearchOnClick(object sender, RoutedEventArgs e) => SendCombinedRequest(new CombinedFilterModel("Awaiting Shipment"));

		#endregion

		#region EXPANDER EVENT HANDLERS - DONE - ✅

		private async void Expander_OnExpanded(object sender, RoutedEventArgs e)
		{
			if (ViewModel.AppState.CombinedFilter.SelectedFilterGroup == "Engraved Today") return;
			var scrollViewer = Utils.Utils.GetScrollViewer(ShippingItemsListView) as ScrollViewer;
			var thisOrder = ViewModel.Orders.Data.FirstOrDefault(i => i.NameOrder == ((Expander)sender).Tag.ToString());
			var expandSite = ((Expander)sender).Template.FindName("ExpandSite", ((Expander)sender)) as UIElement;
			expandSite.Visibility = Visibility.Visible;
			var sb1 = (Storyboard)((Expander)sender).FindResource("sbExpand");
			sb1.Begin();
			foreach (var order in ViewModel.Orders.Data)
			{
				order.IsExpanded = order.IdOrders == thisOrder.IdOrders;
			}
			if (ViewModel.Orders.Data.IndexOf(thisOrder) == 14)
			{
				ScrollAnimationBehavior.AnimateScroll(scrollViewer,
					scrollViewer.VerticalOffset + 600);
				ScrollAnimationBehavior.intendedLocation = scrollViewer.VerticalOffset + 600;
			} else
			{
				ScrollAnimationBehavior.AnimateScroll(scrollViewer,
					ShippingItemsListView.Items.IndexOf(thisOrder) * ((Expander)sender).ActualHeight);
				ScrollAnimationBehavior.intendedLocation = ShippingItemsListView.Items.IndexOf(thisOrder) * ((Expander)sender).ActualHeight;
			}

			if (thisOrder.Status == "Customer Service Issue")
			{
				foreach (var product in thisOrder.Products.Where(product => product.Status == "Customer Service Issue"))
				{
					var x = await ArtPixAPI.GetProductHistoryAsync(product.IdProducts);
					foreach (var comment in x.Data)
					{
						if (comment.Message.Contains("Added issues:"))
						{
							if (comment.Message.Contains("USABLE MANUAL ISSUE"))
							{
								product.Status = comment.Message.Remove(0, 32);
								continue;
							}
							if (comment.Message.Contains("USABLE AUTO ISSUE"))
							{
								product.Status = comment.Message.Remove(0, 30);
							}
						}
						
					}
				}
			}


			if (thisOrder.Status != "Engraving Issue") return;
			foreach (var product in thisOrder.Products.Where(product => product.Status == "Engraving Issue"))
			{
				var productionIssue =
					await ArtPixAPI.GetProductionIssueAsync(product.MachineAssignItemId.ToString());
				product.MachineAssignErrorId = productionIssue.Data[0].Id;
				product.Status = productionIssue.Data[0].ProductionIssueReason.Reason;
				product.Employee = productionIssue.Data[0].User;
			}

		}

		private void Expander_OnCollapsed(object sender, RoutedEventArgs e)
		{
			var expandSite = ((Expander)sender).Template.FindName("ExpandSite", ((Expander)sender)) as UIElement;
			expandSite.Visibility = System.Windows.Visibility.Visible;
			var sb1 = (Storyboard)((Expander)sender).FindResource("sbCollapse");
			sb1.Begin();
		}
		
		private void Expander_OnLoaded(object sender, RoutedEventArgs e)
		{
			if (ViewModel.AppState.CombinedFilter.SelectedFilterGroup != "Search") return;
			_expander = (Expander) sender;
			ViewModel.Orders.Data[0].IsExpanded = true;
			var expandSite = _expander.Template.FindName("ExpandSite", (_expander)) as UIElement;
			expandSite.Visibility = Visibility.Visible;
			var sb1 = (Storyboard)(_expander).FindResource("sbExpand");
			sb1.Begin();
			ViewModel.AppState.CombinedFilter.name_order = ViewModel.Orders.Data[0].NameOrder;
			SearchTextBox.Text = ViewModel.AppState.CombinedFilter.name_order;
		}

		#endregion

		#region PAGES NAVIGATION

		private async void PageButtonOnClick(object sender, RoutedEventArgs e)
		{
			ToggleLoadingAnimation(1);
			var btn = (TitleBarButton)sender;
			foreach (var page in ViewModel.Pages)
			{
				page.IsSelected = false;
			}
			ViewModel.Pages[(int)btn.Tag - 1].IsSelected = true;

			ViewModel.AppState.CombinedFilter.pageNumber = (int)btn.Tag;
			ViewModel.AppState.CombinedFilter.withPages = false;
			await ViewModel.GetOrdersList(ViewModel.AppState.CombinedFilter);
			if (ViewModel.Orders.Data.Count > 0)
			{
				var scrollViewer = Utils.Utils.GetScrollViewer(ShippingItemsListView) as ScrollViewer;
				scrollViewer.ScrollToVerticalOffset(0);
				ScrollAnimationBehavior.AnimateScroll(scrollViewer, 0);
				ScrollAnimationBehavior.intendedLocation = 0;
			}
			ViewModel.PaginationBackButtonVisibility = (int)btn.Tag > 1;
			ToggleLoadingAnimation(0);
		}

		private async void BackButton_OnClick(object sender, RoutedEventArgs e)
		{
			ToggleLoadingAnimation(1);
			var selectedPage = 0;
			foreach (var page in ViewModel.Pages)
			{
				if (page.IsSelected)
				{
					selectedPage = page.PageNumber;
				}
				page.IsSelected = false;
			}
			ViewModel.Pages[selectedPage - 2].IsSelected = true;
			ViewModel.AppState.CombinedFilter.pageNumber--;
			ViewModel.AppState.CombinedFilter.withPages = false;
			await ViewModel.GetOrdersList(ViewModel.AppState.CombinedFilter);

			if (ViewModel.Orders.Data.Count > 0)
			{
				ScrollViewer scrollViewer = Utils.Utils.GetScrollViewer(ShippingItemsListView) as ScrollViewer;
				Utils.ScrollAnimationBehavior.AnimateScroll(scrollViewer,
					0);
			}
			ViewModel.PaginationForwardButtonVisibility = ViewModel.Pages.Count > 1 ;
			ViewModel.PaginationBackButtonVisibility = selectedPage - 1 > 1 ;
			ToggleLoadingAnimation(0);
		}

		private async void ForwardButton_OnClick(object sender, RoutedEventArgs e)
		{
			ToggleLoadingAnimation(1);

			var selectedPage = 0;
			foreach (var page in ViewModel.Pages)
			{
				if (page.IsSelected)
				{
					selectedPage = page.PageNumber;
				}
				page.IsSelected = false;
			}
			ViewModel.Pages[selectedPage].IsSelected = true;
			ViewModel.AppState.CombinedFilter.pageNumber++;
			ViewModel.AppState.CombinedFilter.withPages = false;
			await ViewModel.GetOrdersList(ViewModel.AppState.CombinedFilter);
			if (ViewModel.Orders.Data.Count > 0)
			{
				ScrollViewer scrollViewer = Utils.Utils.GetScrollViewer(ShippingItemsListView) as ScrollViewer;
				Utils.ScrollAnimationBehavior.AnimateScroll(scrollViewer,
					0);
			}
			ViewModel.PaginationBackButtonVisibility = selectedPage > 1 ;
			ViewModel.PaginationForwardButtonVisibility = ViewModel.Pages.Count > 1;

			ToggleLoadingAnimation(0);

		}

		#endregion


		//TODO: MOVE THIS TO VIEW MODEL
		public async void ShowLoginDialog()
		{
			var dialog = new LoginDialog(ViewModel.AppState);
			var result = await dialog.ShowAsync();
		}

		private async void UpdateShippingAddressButtonOnClick(object sender, RoutedEventArgs e)
		{
			var orderId = ((Button) sender).Tag.ToString();
			var updatedOrder = await ArtPixAPI.UpdateShippingAddress(orderId);
			var order = ViewModel.Orders.Data.FirstOrDefault((x) => x.IdOrders.ToString() == orderId);
			order.customers = updatedOrder.Data[0].customers;
			order.ShippingAddress2Visibility = Visibility.Visible;
			Utils.Utils.Notifier.ShowSuccess("Address updated successfully!");
		}

		private void ShowOnMapButtonOnClick(object sender, RoutedEventArgs e)
		{
			var orderId = ((Button)sender).Tag.ToString();
			var order = ViewModel.Orders.Data.FirstOrDefault((x) => x.IdOrders.ToString() == orderId);
			var target =
				$"https://www.google.com/maps/place/{order.customers.ShippingAddress.address_1}+{order.customers.ShippingAddress.City}+{order.customers.ShippingAddress.State}+{order.customers.ShippingAddress.Postcode}";
			try
			{
				Process.Start(target);
			}
			catch (Win32Exception noBrowser)
			{
				if (noBrowser.ErrorCode == -2147467259)
					System.Windows.Forms.MessageBox.Show(noBrowser.Message);
			}
			catch (Exception other)
			{
				MessageBox.Show(other.Message);
			}
		}
	}
}
