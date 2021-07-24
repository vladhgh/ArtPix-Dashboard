using System;
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

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_vm.AppState = (AppStateModel)e.ExtraData;
			_vm.Initialize();

			ShippingDashboardPage.PreviewKeyDown += KeyPressEventListener;
			ShippingDashboardPage.Focus();

			if (_vm.AppState.OrderFilterGroup == null) return;

			SendCombinedRequest();

			SortByComboBox.SelectionChanged += SortByComboBoxOnSelectionChanged;
			EngravingStatusComboBox.SelectionChanged += EngravingStatusComboBoxOnSelectionChanged;
			StoreComboBox.SelectionChanged += StoreComboBoxOnSelectionChanged;
			ToggleShipByToday.Click += ToggleShipByToday_Click;
			ToggleNoPackage.Click += ToggleNoPackageOnClick;
			ToggleInTotes.Click += ToggleInTotes_Click;
			ToggleNoCrystal.Click += ToggleNoCrystal_Click;


		}

		private async void SendCombinedRequest(bool search = false)
		{
			SortByComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.sort_by;
			EngravingStatusComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.status_engraving;
			ShippingStatusComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.status_shipping;
			OrderStatusComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.status_order;
			StoreComboBox.SelectedValue = _vm.AppState.OrderFilterGroup.store_name;
			ToggleShipByToday.IsChecked = _vm.AppState.OrderFilterGroup.shipByToday == "True";
			ToggleNoPackage.IsChecked = _vm.AppState.OrderFilterGroup.has_shipping_package == "0";
			ToggleInTotes.IsChecked = _vm.AppState.OrderFilterGroup.with_shipping_totes == "True";
			ToggleNoCrystal.IsChecked = _vm.AppState.OrderFilterGroup.with_crystals == "0";
			SearchTextBox.Text = _vm.AppState.OrderFilterGroup.name_order;

			await _vm.GetOrdersList(1, 15, true, _vm.AppState.OrderFilterGroup);

			if (_vm.Orders.Data == null) return;
			if (_vm.Orders.Data.Count <= 0) return;
			ShippingItemsListView.ScrollIntoView(_vm.Orders.Data[0]);
			if (search)
			{
				while(_vm.Orders.Data[0].IsExpanded == false)
				{
					_vm.Orders.Data[0].IsExpanded = true;
				}
				_vm.AppState.OrderFilterGroup.name_order = _vm.Orders.Data[0].NameOrder;
				SearchTextBox.Text = _vm.AppState.OrderFilterGroup.name_order;
			}

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

				if (e.Key == Key.OemMinus)
				{
					_inputString += "-";
					e.Handled = true;
				}
			}

			if (e.Key == Key.Enter)
			{
				_tabPressed = false;
				_vm.AppState.OrderFilterGroup.name_order = "";
				_vm.AppState.OrderFilterGroup.status_engraving = "";
				_vm.AppState.OrderFilterGroup.status_order = "";
				_vm.AppState.OrderFilterGroup.status_shipping = "";
				_vm.AppState.OrderFilterGroup.shipByToday = "";
				_vm.AppState.OrderFilterGroup.order_id = _inputString.Split('-')[0];
				_vm.AppState.OrderFilterGroup.SelectedFilterGroup = "Search:";
				SendCombinedRequest(true);
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

		private void OrderStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox)
			{
				_vm.AppState.OrderFilterGroup.status_order = ((ComboBoxItem)OrderStatusComboBox.SelectedItem).Tag.ToString();
				SendCombinedRequest();
			}
		}

		private void ShippingStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox)
			{
				_vm.AppState.OrderFilterGroup.status_shipping = ((ComboBoxItem)ShippingStatusComboBox.SelectedItem).Tag.ToString();
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
			if (_vm.Orders.Data.Count > 0) ShippingItemsListView.ScrollIntoView(_vm.Orders.Data[0]);
		}
		private void SearchTextBoxOnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			_vm.AppState.OrderFilterGroup.SelectedFilterGroup = "Search";
			_vm.AppState.OrderFilterGroup.status_engraving = "";
			_vm.AppState.OrderFilterGroup.status_order = "";
			_vm.AppState.OrderFilterGroup.status_shipping = "";
			_vm.AppState.OrderFilterGroup.shipByToday = "";
			_vm.AppState.OrderFilterGroup.name_order = SearchTextBox.Text ?? "" ;
			SendCombinedRequest(true);
		}

		private void ButtonSearchOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.SelectedFilterGroup = "Search";
			_vm.AppState.OrderFilterGroup.status_engraving = "";
			_vm.AppState.OrderFilterGroup.status_order = "";
			_vm.AppState.OrderFilterGroup.status_shipping = "";
			_vm.AppState.OrderFilterGroup.shipByToday = "";
			_vm.AppState.OrderFilterGroup.name_order = SearchTextBox.Text ?? "" ;
			SendCombinedRequest(true);
		}

		private void ButtonClearSearchOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.status_engraving = "";
			_vm.AppState.OrderFilterGroup.status_shipping = "waiting";
			_vm.AppState.OrderFilterGroup.shipByToday = "";
			_vm.AppState.OrderFilterGroup.name_order = "";
			_vm.AppState.OrderFilterGroup.order_id = "";
			_vm.AppState.OrderFilterGroup.SelectedFilterGroup = "Awaiting Shipment";
			_vm.AppState.OrderFilterGroup.status_order = "processing";
			SendCombinedRequest();
		}

		private async void Expander_OnExpanded(object sender, RoutedEventArgs e)
		{
			ScrollViewer scrollViewer = GetScrollViewer(ShippingItemsListView) as ScrollViewer;
			var thisOrder = _vm.Orders.Data.Find(i => i.NameOrder == ((Expander)sender).Tag.ToString());
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
				ScrollAnimateBehavior.AttachedBehaviors.ScrollAnimationBehavior.AnimateScroll(scrollViewer,
					scrollViewer.VerticalOffset + 400);
			} else
			{
				ScrollAnimateBehavior.AttachedBehaviors.ScrollAnimationBehavior.AnimateScroll(scrollViewer,
					ShippingItemsListView.Items.IndexOf(thisOrder) * ((Expander)sender).ActualHeight);
			}

			if (thisOrder.Status == "Engraving Issue")
			{
				foreach (var product in thisOrder.Products)
				{
					product.Status = product.Status == "Engraving Issue"
						? await ArtPixAPI.GetProductionIssueReasonFromEntityLogsAsync(product.IdProducts.ToString())
						: product.Status;
				}
			}

		}

		private void Expander_OnCollapsed(object sender, RoutedEventArgs e)
		{
			var expandSite = ((Expander)sender).Template.FindName("ExpandSite", ((Expander)sender)) as UIElement;
			expandSite.Visibility = System.Windows.Visibility.Visible;
			var sb1 = (Storyboard)((Expander)sender).FindResource("sbCollapse");
			sb1.Begin();
		}

		#endregion



		public static DependencyObject GetScrollViewer(DependencyObject o)
		{
			// Return the DependencyObject if it is a ScrollViewer
			if (o is ScrollViewer)
			{ return o; }

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
			{
				var child = VisualTreeHelper.GetChild(o, i);

				var result = GetScrollViewer(child);
				if (result == null)
				{
					continue;
				}
				else
				{
					return result;
				}
			}
			return null;
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
	}
}
