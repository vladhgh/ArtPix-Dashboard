using System;
using ArtPix_Dashboard.ViewModels;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ArtPix_Dashboard.Views
{

    public partial class MachinesDashboardView
    {
        private readonly MachinesDashboardViewModel _vm = new MachinesDashboardViewModel();

        public MachinesDashboardView()
        {
	        DataContext = _vm;
	        InitializeComponent();
		}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
	        _vm.Initialize();
			MachineComboBox.SelectionChanged += MachineComboBox_OnSelectionChanged;
			EngravingStatusComboBox.SelectionChanged += EngravingStatusComboBox_OnSelectionChanged;
        }


		private void EngravingStatusComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
	        _vm.GetMachineAssignItems((EngravingStatusComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString(), (MachineComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString());
	        if (_vm.MachineAssignedItems.Data.Count > 0) LaserMachineItemsListView.ScrollIntoView(_vm.MachineAssignedItems.Data[0]);
        }

        private void MachineComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
	        _vm.GetMachineAssignItems((EngravingStatusComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString(), (MachineComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString());
	        if (_vm.MachineAssignedItems.Data.Count > 0) LaserMachineItemsListView.ScrollIntoView(_vm.MachineAssignedItems.Data[0]);
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
	        if (_vm.MachineAssignedItems.Data.Count > 0) LaserMachineItemsListView.ScrollIntoView(_vm.MachineAssignedItems.Data[0]);
        }

        private void ActiveMachineButton_OnClick(object sender, RoutedEventArgs e)
        {
	        var x = ((Button) sender).Tag.ToString();
	        MachineComboBox.SelectedIndex = int.Parse(x);
            EngravingStatusComboBox.SelectedIndex = 2;
        }
    }
}
