

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ArtPix_Dashboard.ViewModels;
using System.Windows.Navigation;
using ArtPix_Dashboard.Models.Order;

namespace ArtPix_Dashboard.Views
{

	public partial class ShippingDashboardView
	{
		private readonly ShippingDashboardViewModel _vm = new ShippingDashboardViewModel();
		private bool shipByToday = true;
		private bool inTotes = false;

		public ShippingDashboardView()
		{
			InitializeComponent();
			DataContext = _vm;
		}
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			SortByComboBox.SelectionChanged += SortByComboBox_SelectionChanged;
			EngravingStatusComboBox.SelectionChanged += EngravingStatusComboBoxOnSelectionChanged;
			ShippingStatusComboBox.SelectionChanged += ShippingStatusComboBoxOnSelectionChanged;
			OrderStatusComboBox.SelectionChanged += OrderStatusComboBoxOnSelectionChanged;
			StoreComboBox.SelectionChanged += StoreComboBoxOnSelectionChanged;
			ToggleShipByToday.Click += ToggleShipByToday_Click;
			ToggleInTotes.Click += ToggleInTotes_Click;
		}

		private void EngravingStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SendCombinedRequest();
		}

		private void OrderStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e) => SendCombinedRequest();

		private void SendCombinedRequest()
		{
			_vm.GetOrdersList(1, true, "15", "", inTotes.ToString(), "",
				((ComboBoxItem)SortByComboBox.SelectedItem).Tag.ToString(),
				shipByToday.ToString(),
				((ComboBoxItem)StoreComboBox.SelectedItem).Tag.ToString(),
				((ComboBoxItem)ShippingStatusComboBox.SelectedItem).Tag.ToString(),
				((ComboBoxItem)OrderStatusComboBox.SelectedItem).Tag.ToString(),
				((ComboBoxItem)EngravingStatusComboBox.SelectedItem).Tag.ToString());
			if (_vm.Orders.Data.Count > 0) ShippingItemsListView.ScrollIntoView(_vm.Orders.Data[0]);
		}

		private void ShippingStatusComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e) => SendCombinedRequest();

		private void StoreComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e) => SendCombinedRequest();

		private void ToggleInTotes_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					inTotes = (bool)btn.IsChecked;
			SendCombinedRequest();
		}

		private void ToggleShipByToday_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is ToggleButton btn)
				if (btn.IsChecked != null)
					shipByToday = (bool) btn.IsChecked;
			SendCombinedRequest();
		}

		private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => SendCombinedRequest();

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			if (_vm.Orders.Data.Count > 0) ShippingItemsListView.ScrollIntoView(_vm.Orders.Data[0]);
		}
	}
}
