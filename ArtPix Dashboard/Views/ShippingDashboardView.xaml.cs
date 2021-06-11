using System.Collections.Specialized;
using System.Linq;
using System.Diagnostics;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using ArtPix_Dashboard.ViewModels;
using System.Windows.Navigation;
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.Machine;
using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.Properties;
using ModernWpf.Controls;
using ListView = ModernWpf.Controls.ListView;
using ArtPix_Dashboard.Utils;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Views
{

	public partial class ShippingDashboardView
	{
		private readonly ShippingDashboardViewModel _vm = new ShippingDashboardViewModel();
		public ShippingDashboardView()
		{
			InitializeComponent();
			DataContext = _vm;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_vm.AppState = (AppStateModel)e.ExtraData;
			_vm.Initialize();
			if (_vm.AppState.OrderFilterGroup == null) return;

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

			SendCombinedRequest();

			SortByComboBox.SelectionChanged += SortByComboBoxOnSelectionChanged;
			EngravingStatusComboBox.SelectionChanged += EngravingStatusComboBoxOnSelectionChanged;
			ShippingStatusComboBox.SelectionChanged += ShippingStatusComboBoxOnSelectionChanged;
			OrderStatusComboBox.SelectionChanged += OrderStatusComboBoxOnSelectionChanged;
			StoreComboBox.SelectionChanged += StoreComboBoxOnSelectionChanged;
			ToggleShipByToday.Click += ToggleShipByToday_Click;
			ToggleNoPackage.Click += ToggleNoPackageOnClick;
			ToggleInTotes.Click += ToggleInTotes_Click;
			ToggleNoCrystal.Click += ToggleNoCrystal_Click;


		}

		private async void SendCombinedRequest()
		{
			await _vm.GetOrdersList(1, 15, true, _vm.AppState.OrderFilterGroup);

			if (_vm.Orders.Data == null) return;
			if (_vm.Orders.Data.Count <= 0) return;
			ShippingItemsListView.ScrollIntoView(_vm.Orders.Data[0]);
			var scrollViewer = ShippingItemsListView.GetChildOfType<ScrollViewer>();
			if (scrollViewer == null) return;
			scrollViewer.CanContentScroll = false;
			scrollViewer.PanningMode = PanningMode.Both;

		}

		#region EVENT HANDLERS

		private void ToggleNoCrystal_Click(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					_vm.AppState.OrderFilterGroup.with_crystals = (bool)btn.IsChecked ? "0" : "3";
			SendCombinedRequest();
		}

		private void ButtonReloadOnClick(object sender, RoutedEventArgs e)
		{
			SendCombinedRequest();
		}

		private void ToggleNoPackageOnClick(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					_vm.AppState.OrderFilterGroup.has_shipping_package = (bool)btn.IsChecked ? "0" : "";
			SendCombinedRequest();
		}

		private void EngravingStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.status_engraving =
				((ComboBoxItem)EngravingStatusComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest();
		}

		private void OrderStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.status_order = ((ComboBoxItem)OrderStatusComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest();
		}

		private void ShippingStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.status_shipping =
				((ComboBoxItem)ShippingStatusComboBox.SelectedItem).Tag.ToString();
			SendCombinedRequest();
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
			_vm.AppState.OrderFilterGroup.name_order = SearchTextBox.Text ?? "";
			SendCombinedRequest();
		}

		private void ButtonSearchOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.name_order = SearchTextBox.Text ?? "";
			SendCombinedRequest();
		}


		private void Expander_OnExpanded(object sender, RoutedEventArgs e)
		{
			ShippingItemsListView.ScrollIntoView(_vm.Orders.Data.Find(i => i.NameOrder == ((Expander)sender).Tag.ToString()));
		}

		#endregion

		private void SearchTextBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			if (sender.Text != "") return;
			_vm.AppState.OrderFilterGroup.name_order = "";
			SendCombinedRequest();

		}
	}
}
