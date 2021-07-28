using System;
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
using ArtPix_Dashboard.Views.Dialogs;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ArtPix_Dashboard.API;
using ArtPix_Dashboard.Utils;
using ModernWpf.Controls.Primitives;
using System.Linq.Expressions;

namespace ArtPix_Dashboard.Views
{





	public partial class ShippingDashboardView
	{
		private readonly ShippingDashboardViewModel _vm = new ShippingDashboardViewModel();

		private string _inputString;

		private bool _tabPressed;

		public ShippingDashboardView()
		{
			InitializeComponent();
			DataContext = _vm;
		}

		private void SetEventListeners()
		{
			SortByComboBox.SelectionChanged += SortByComboBoxOnSelectionChanged;
			EngravingStatusComboBox.SelectionChanged += EngravingStatusComboBoxOnSelectionChanged;
			StoreComboBox.SelectionChanged += StoreComboBoxOnSelectionChanged;
			MachineComboBox.SelectionChanged += MachineComboBox_SelectionChanged;
			ToggleShipByToday.Click += ToggleShipByToday_Click;
			ToggleNoPackage.Click += ToggleNoPackageOnClick;
			ToggleInTotes.Click += ToggleInTotes_Click;
			ToggleNoCrystal.Click += ToggleNoCrystal_Click;
		}


		private void CheckForUserSettings()
		{
			if (_vm.AppState.OrderFilterGroup == null)
			{
				MessageBox.Show("Unable to load use settings. Application will exit now.");
				Environment.Exit(-1);
			}
		}

		private void SetKeyPressEventListener()
		{
			ShippingDashboardPage.PreviewKeyDown += KeyPressEventListener;
			ShippingDashboardPage.KeyDown += ShippingDashboardPage_KeyDown; ;
			ShippingDashboardPage.PreviewKeyUp += ShippingDashboardPage_KeyDown;
			ShippingDashboardPage.KeyUp += ShippingDashboardPage_KeyDown;
			ShippingDashboardPage.Focus();
		}

		private void ShippingDashboardPage_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_vm.Initialize((AppStateModel)e.ExtraData, this);

			SetKeyPressEventListener();

			CheckForUserSettings();

			SendCombinedRequest();

			SetEventListeners();

		}

		private void MachineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void UpdateControls()
		{
			SortByComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.sort_by;
			EngravingStatusComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.status_engraving;
			ShippingStatusComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.status_shipping;
			OrderStatusComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.status_order;
			StoreComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.store_name;
			MachineComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.machine;
			ToggleShipByToday.IsChecked = _vm.AppState.OrderFilterGroup.shipByToday == "True";
			ToggleNoPackage.IsChecked = _vm.AppState.OrderFilterGroup.has_shipping_package == "0";
			ToggleInTotes.IsChecked = _vm.AppState.OrderFilterGroup.with_shipping_totes == "True";
			ToggleNoCrystal.IsChecked = _vm.AppState.OrderFilterGroup.with_crystals == "0";
			SearchTextBox.Text = _vm.AppState.OrderFilterGroup.name_order;
		}

		

		private async void SendCombinedRequest(bool search = false)
		{

			
			Animation.FadeOut(ShippingItemsListView);
			Animation.FadeIn(ProgressRingImage);



			UpdateControls();

			await _vm.GetOrdersList(1, 15, true, _vm.AppState.OrderFilterGroup);

			if (_vm.Orders.Data == null) return;
			if (_vm.Orders.Data.Count <= 0)
			{
				Animation.FadeOut(ProgressRingImage);
				Animation.FadeOut(ActionsText);
				Animation.FadeIn(NoResultsText);
				return;
			}
			
			if (search)
			{
				while(_vm.Orders.Data[0].IsExpanded == false)
				{
					_vm.Orders.Data[0].IsExpanded = true;
				}
				_vm.AppState.OrderFilterGroup.name_order = _vm.Orders.Data[0].NameOrder;
				SearchTextBox.Text = _vm.AppState.OrderFilterGroup.name_order;
			}
			var selectedPage = _vm.Pages.First(p => p.IsSelected);
			_vm.PaginationForwardButtonVisibility = (_vm.Pages.Count > 1) && (_vm.Pages.Count > selectedPage.PageNumber);
			_vm.PaginationBackButtonVisibility = selectedPage.PageNumber > 1;

			Animation.FadeOut(ProgressRingImage);
			Animation.FadeIn(ShippingItemsListView);

		}


		public async void ShowLoginDialog()
		{
			var dialog = new LoginDialog(_vm.AppState);
			var result = await dialog.ShowAsync();
		}

		#region EVENT HANDLERS

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
				Debug.WriteLine(_inputString);
				if (_inputString.Split('-')[0] == "LOGIN")
				{
					_vm.AppState.EmployeeName = _inputString.Split('-')[1];
					ShowLoginDialog();
				} else
				{
					_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Search", "", "",  _inputString.Split('-')[0]);
					SendCombinedRequest(true);
				}
				_inputString = "";
				e.Handled = true;
			}
		}


		private void ToggleNoCrystal_Click(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
			{
				if (btn.IsChecked != null)
				{
					_vm.AppState.OrderFilterGroup.with_crystals = (bool)btn.IsChecked ? "0" : "3";
					_vm.AppState.OrderFilterGroup.status_shipping = (bool)btn.IsChecked ? "" : "waiting";
				}
			}
			SendCombinedRequest();
		}

		private void ButtonReloadOnClick(object sender, RoutedEventArgs e)
		{
			SendCombinedRequest();
		}

		private void ToggleNoPackageOnClick(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
			{
				if (btn.IsChecked != null)
				{
					_vm.AppState.OrderFilterGroup.has_shipping_package = (bool)btn.IsChecked ? "0" : "";
				}
				SendCombinedRequest();
			}
		}

		private void EngravingStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox)
			{
				_vm.AppState.OrderFilterGroup.status_engraving =
					((ComboBoxItem)EngravingStatusComboBox.SelectedItem).Tag.ToString();
				SendCombinedRequest();
			}
		}

		private void StoreComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.store_name = ((ComboBoxItem)StoreComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest();
		}

		private void ToggleInTotes_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					_vm.AppState.OrderFilterGroup.with_shipping_totes = ((bool)btn.IsChecked).ToString();
			SendCombinedRequest();
		}

		private void ToggleShipByToday_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					_vm.AppState.OrderFilterGroup.shipByToday = ((bool)btn.IsChecked).ToString();
			SendCombinedRequest();
		}

		private void SortByComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.sort_by = ((ComboBoxItem)SortByComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			var btn = (TitleBarButton)sender;
			foreach (var page in _vm.Pages)
			{
				page.IsSelected = false;
			}
			_vm.Pages[(int)btn.Tag - 1].IsSelected = true;

			if (_vm.Orders.Data.Count > 0)
			{
				var scrollViewer = Utils.Utils.GetScrollViewer(ShippingItemsListView) as ScrollViewer;
				scrollViewer.ScrollToVerticalOffset(0);
				ScrollAnimationBehavior.AnimateScroll(scrollViewer, 0);
				ScrollAnimationBehavior.intendedLocation = 0;
			}

			_vm.PaginationBackButtonVisibility = (int) btn.Tag > 1 ;
		}
		private void SearchTextBoxOnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Search", "", SearchTextBox.Text);
			SendCombinedRequest(true);
		}

		private void ButtonSearchOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Search", "", SearchTextBox.Text);
			SendCombinedRequest(true);
		}

		private void ButtonClearSearchOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel("Awaiting Shipment");
			SendCombinedRequest();
		}

		private async void Expander_OnExpanded(object sender, RoutedEventArgs e)
		{
			var scrollViewer = Utils.Utils.GetScrollViewer(ShippingItemsListView) as ScrollViewer;
			var thisOrder = _vm.Orders.Data.FirstOrDefault(i => i.NameOrder == ((Expander)sender).Tag.ToString());
			var expandSite = ((Expander)sender).Template.FindName("ExpandSite", ((Expander)sender)) as UIElement;
			expandSite.Visibility = System.Windows.Visibility.Visible;
			var sb1 = (Storyboard)((Expander)sender).FindResource("sbExpand");
			sb1.Begin();
			foreach (var order in _vm.Orders.Data)
			{
				order.IsExpanded = order.IdOrders == thisOrder.IdOrders;
			}
			if (_vm.Orders.Data.IndexOf(thisOrder) == 14)
			{
				Utils.ScrollAnimationBehavior.AnimateScroll(scrollViewer,
					scrollViewer.VerticalOffset + 400);
			} else
			{
				Utils.ScrollAnimationBehavior.AnimateScroll(scrollViewer,
					ShippingItemsListView.Items.IndexOf(thisOrder) * ((Expander)sender).ActualHeight);
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

		private async void EditAddressButtonClick(object sender, RoutedEventArgs e)
		{
			//var product = (Product)param;
			//var order = Orders.Data.SingleOrDefault(p => p.IdOrders == product.IdOrders);
			//var machines = await ArtPixAPI.GetMachines(product.IdProducts);
			var dialog = new ChangeAddressDialog();
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				
			}
		}

		private async void BackButton_OnClick(object sender, RoutedEventArgs e)
		{
			var selectedPage = 0;
			foreach (var page in _vm.Pages)
			{
				if (page.IsSelected)
				{
					selectedPage = page.PageNumber;
				}
				page.IsSelected = false;
			}
			_vm.Pages[selectedPage - 2].IsSelected = true;
			await _vm.GetOrdersList(selectedPage - 1, 15, false, _vm.AppState.OrderFilterGroup);

			if (_vm.Orders.Data.Count > 0)
			{
				ScrollViewer scrollViewer = Utils.Utils.GetScrollViewer(ShippingItemsListView) as ScrollViewer;
				Utils.ScrollAnimationBehavior.AnimateScroll(scrollViewer,
					0);
			}
			_vm.PaginationForwardButtonVisibility = _vm.Pages.Count > 1 ;
			_vm.PaginationBackButtonVisibility = selectedPage - 1 > 1 ;
		}

		private async void ForwardButton_OnClick(object sender, RoutedEventArgs e)
		{
			var selectedPage = 0;
			foreach (var page in _vm.Pages)
			{
				if (page.IsSelected)
				{
					selectedPage = page.PageNumber;
				}
				page.IsSelected = false;
			}
			_vm.Pages[selectedPage].IsSelected = true;
			await _vm.GetOrdersList(selectedPage + 1, 15, false, _vm.AppState.OrderFilterGroup);
			if (_vm.Orders.Data.Count > 0)
			{
				ScrollViewer scrollViewer = Utils.Utils.GetScrollViewer(ShippingItemsListView) as ScrollViewer;
				Utils.ScrollAnimationBehavior.AnimateScroll(scrollViewer,
					0);
			}
			_vm.PaginationBackButtonVisibility = selectedPage > 1 ;
			_vm.PaginationForwardButtonVisibility = _vm.Pages.Count > 1;

		}

		#endregion



		

	}
}
