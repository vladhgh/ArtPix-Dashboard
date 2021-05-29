using System;
using ArtPix_Dashboard.ViewModels;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ArtPix_Dashboard.Models;

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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
	        await _vm.Initialize(e.ExtraData as AppStateModel);
			MachineComboBox.SelectionChanged += MachineComboBox_OnSelectionChanged;
			EngravingStatusComboBox.SelectionChanged += EngravingStatusComboBox_OnSelectionChanged;
			Utils.Utils.EnableTouchScrollForListView(LaserMachineItemsListView);
		}

        private async void SendCombinedRequest(bool withOrderName = false)
        {
	        if (withOrderName)
	        {
				await _vm.GetMachineAssignItems((EngravingStatusComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString(), (MachineComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString());
			}
	        else
	        {
				await _vm.GetMachineAssignItems((EngravingStatusComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString(), (MachineComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString());
			}

	        if (_vm.MachineAssignedItems.Data != null)
		        if (_vm.MachineAssignedItems.Data.Count > 0)
		        {
			        LaserMachineItemsListView.ScrollIntoView(_vm.MachineAssignedItems.Data[0]);
			        Utils.Utils.EnableTouchScrollForListView(LaserMachineItemsListView);

		        }

        }


		private void EngravingStatusComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SendCombinedRequest();
		}

        private void MachineComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
	        SendCombinedRequest();

		}
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
	        if (_vm.MachineAssignedItems.Data.Count > 0)
	        {
		        Utils.Utils.EnableTouchScrollForListView(LaserMachineItemsListView);
				LaserMachineItemsListView.ScrollIntoView(_vm.MachineAssignedItems.Data[0]);
	        }
        }

        private void ActiveMachineButton_OnClick(object sender, RoutedEventArgs e)
        {
	        var x = ((Button) sender).Tag.ToString();
	        MachineComboBox.SelectedIndex = int.Parse(x);
            EngravingStatusComboBox.SelectedIndex = 2;
        }

        private void ButtonReloadOnClick(object sender, RoutedEventArgs e) => SendCombinedRequest();
		
    }
}
