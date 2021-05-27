using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ArtPix_Dashboard.ViewModels;
using System.Windows.Navigation;
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.Order;
using ModernWpf.Controls;

namespace ArtPix_Dashboard.Views
{

	public partial class ShippingDashboardView
	{
		private readonly ShippingDashboardViewModel _vm = new ShippingDashboardViewModel();
		private AppStateModel _appState = new AppStateModel();
		public ShippingDashboardView()
		{
			InitializeComponent();
			DataContext = _vm;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_appState = (AppStateModel)e.ExtraData;
			if (_appState.OrderFilterGroup == null) return;
			SortByComboBox.SelectedValue = _appState.OrderFilterGroup.sortBy ?? "estimate_processing_max_date";
			EngravingStatusComboBox.SelectedValue = _appState.OrderFilterGroup.statusEngraving ?? "";
			ShippingStatusComboBox.SelectedValue = _appState.OrderFilterGroup.shippingStatus ?? "waiting";
			OrderStatusComboBox.SelectedValue = _appState.OrderFilterGroup.orderStatus ?? "processing";
			StoreComboBox.SelectedValue = _appState.OrderFilterGroup.storeName ?? "";
			ToggleShipByToday.IsChecked = _appState.OrderFilterGroup.shipByToday == null || _appState.OrderFilterGroup.shipByToday == "True";
			ToggleNoPackage.IsChecked = _appState.OrderFilterGroup.hasShippingPackage != null && _appState.OrderFilterGroup.hasShippingPackage == "0";
			ToggleInTotes.IsChecked = _appState.OrderFilterGroup.withShippingTotes != null && _appState.OrderFilterGroup.withShippingTotes == "True";
		}

		private void SendCombinedRequest(bool withOrderName = false)
		{
			if (withOrderName)
			{
				_vm.GetOrdersList(1, true, "15",
					_appState.OrderFilterGroup.hasShippingPackage ?? "",
					_appState.OrderFilterGroup.withShippingTotes ?? "",
					"",
					_appState.OrderFilterGroup.sortBy ?? "",
					_appState.OrderFilterGroup.shipByToday ?? "",
					_appState.OrderFilterGroup.storeName ?? "",
					_appState.OrderFilterGroup.shippingStatus ?? "",
					_appState.OrderFilterGroup.orderStatus ?? "",
					_appState.OrderFilterGroup.statusEngraving ?? "",
					SearchTextBox.Text ?? "");
			}
			else
			{
				_vm.GetOrdersList(1, true, "15",
					_appState.OrderFilterGroup.hasShippingPackage ?? "",
					_appState.OrderFilterGroup.withShippingTotes ?? "",
					"",
					_appState.OrderFilterGroup.sortBy ?? "",
					_appState.OrderFilterGroup.shipByToday ?? "True",
					_appState.OrderFilterGroup.storeName ?? "",
					_appState.OrderFilterGroup.shippingStatus ?? "waiting",
					_appState.OrderFilterGroup.orderStatus ?? "processing",
					_appState.OrderFilterGroup.statusEngraving ?? "");
			}
			
			if (_vm.Orders.Data != null)
				if (_vm.Orders.Data.Count > 0)
					ShippingItemsListView.ScrollIntoView(_vm.Orders.Data[0]);
		}

		#region EVENT HANDLERS

		private void ButtonReloadOnClick(object sender, RoutedEventArgs e)
		{
			SendCombinedRequest();
		}

		private void ToggleNoPackageOnClick(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					_appState.OrderFilterGroup.hasShippingPackage = (bool)btn.IsChecked ? "0" : "";
			SendCombinedRequest();
		}

		private void EngravingStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_appState.OrderFilterGroup.statusEngraving =
				((ComboBoxItem)EngravingStatusComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest();
		}

		private void OrderStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_appState.OrderFilterGroup.orderStatus = ((ComboBoxItem)OrderStatusComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest();
		}

		private void ShippingStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_appState.OrderFilterGroup.shippingStatus =
				((ComboBoxItem)ShippingStatusComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest();
		}

		private void StoreComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_appState.OrderFilterGroup.storeName = ((ComboBoxItem)StoreComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest();
		}

		private void ToggleInTotes_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					_appState.OrderFilterGroup.withShippingTotes = ((bool)btn.IsChecked).ToString();
			SendCombinedRequest();
		}

		private void ToggleShipByToday_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					_appState.OrderFilterGroup.shipByToday = ((bool)btn.IsChecked).ToString();
			SendCombinedRequest();
		}

		private void SortByComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_appState.OrderFilterGroup.sortBy = ((ComboBoxItem)SortByComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			if (_vm.Orders.Data.Count > 0) ShippingItemsListView.ScrollIntoView(_vm.Orders.Data[0]);
		}

		#endregion


		private void SearchTextBoxOnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			SendCombinedRequest(true);
		}

		private void ButtonSearchOnClick(object sender, RoutedEventArgs e)
		{
			SendCombinedRequest(true);
		}
	}
}
