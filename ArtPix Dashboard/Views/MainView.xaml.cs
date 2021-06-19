using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArtPix_Dashboard.ViewModels;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Properties;
using Newtonsoft.Json;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ArtPix_Dashboard.Views
{
	public partial class MainView
	{
		private readonly MainViewModel _vm = new MainViewModel();

		public MainView()
		{
			DataContext = _vm;
			InitializeComponent();
		}
		private void MainViewOnLoaded(object sender, RoutedEventArgs e)
		{
			InitializeSettings();
			_vm.Initialize();
			Window.Closing += Window_Closing;
			MainNavigationView.SelectionChanged += NavigateToSelectedPage;
			_vm.AppState.SelectedItem = MainNavigationView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(x => x.Tag.ToString() == Settings.Default.LastVisitedViewTag);
		}
		private void InitializeSettings()
		{
			_vm.AppState.EmployeeName = "Supervisor";
			_vm.AppState.Top = Settings.Default.Top;
			_vm.AppState.Left = Settings.Default.Left;
			_vm.AppState.Height = Settings.Default.Height;
			_vm.AppState.Width = Settings.Default.Width;
			_vm.AppState.WindowState = Settings.Default.Maximized ? WindowState.Maximized : WindowState.Normal;
			_vm.AppState.StatusGroup = Settings.Default.StatusGroup ?? "Engraving";
			SwitchStatusPanel();
			var filterGroup =
				JsonConvert.DeserializeObject<OrderCombineFilterModel>(Settings.Default.OrderFilterGroup);
			if (filterGroup != null)
				_vm.AppState.OrderFilterGroup = new OrderCombineFilterModel
				{
					status_shipping = filterGroup.status_shipping ?? "waiting",
					status_engraving = filterGroup.status_engraving ?? "",
					status_order = filterGroup.status_order ?? "processing",
					store_name = filterGroup.store_name ?? "",
					has_shipping_package = filterGroup.has_shipping_package ?? "",
					with_shipping_totes = filterGroup.with_shipping_totes ?? "",
					with_production_issue = filterGroup.with_production_issue ?? "",
					sort_by = filterGroup.sort_by ?? "estimate_processing_max_date",
					shipByToday = filterGroup.shipByToday ?? "True",
					with_crystals = filterGroup.with_crystals ?? "3",
					name_order = filterGroup.name_order ?? ""
				};

		}
		private void MainNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			if (ContentFrame.CanGoBack)
			{
				ContentFrame.GoBack();
			}
		}
		private void NavigateToSelectedPage(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			var selected = (NavigationViewItem)args.SelectedItem;

			switch (selected.Tag)
			{
				case "ProductionIssuesView":
					ContentFrame.Navigate(typeof(ProductionIssuesView), _vm.AppState, new DrillInNavigationTransitionInfo());
					return;
				case "UtilitiesView":
					ContentFrame.Navigate(typeof(UtilitiesView), _vm.AppState, new DrillInNavigationTransitionInfo());
					return;
				case "ShippingDashboardView":
					_vm.ShippingStatusGroupVisibility = Visibility.Visible;
					_vm.EngravingStatusGroupVisibility = Visibility.Collapsed;
					_vm.ActiveMachinesGroupVisibility = Visibility.Collapsed;
					_vm.AppState.StatusGroup = "Shipping";
					ContentFrame.Navigate(typeof(ShippingDashboardView), _vm.AppState, new DrillInNavigationTransitionInfo());
					return;
				case "MachinesDashboardView":
					_vm.ShippingStatusGroupVisibility = Visibility.Collapsed;
					_vm.EngravingStatusGroupVisibility = Visibility.Visible;
					_vm.ActiveMachinesGroupVisibility = Visibility.Collapsed;
					_vm.AppState.StatusGroup = "Engraving";
					ContentFrame.Navigate(typeof(MachinesDashboardView), _vm.AppState, new DrillInNavigationTransitionInfo());
					return;
			}
		}
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Settings.Default.Top = _vm.AppState.Top;
			Settings.Default.Left = _vm.AppState.Left;
			Settings.Default.Height = _vm.AppState.Height;
			Settings.Default.Width = _vm.AppState.Width;
			Settings.Default.Maximized = _vm.AppState.WindowState == WindowState.Maximized;
			Settings.Default.LastVisitedViewTag = _vm.AppState.SelectedItem.Tag.ToString();
			Settings.Default.OrderFilterGroup = JsonConvert.SerializeObject(_vm.AppState.OrderFilterGroup, Formatting.Indented);
			Settings.Default.StatusGroup = _vm.AppState.StatusGroup;
			Settings.Default.Save();
		}


		private void SwitchStatusPanel()
		{
			Debug.WriteLine(_vm.AppState.StatusGroup);
			switch (_vm.AppState.StatusGroup)
			{
				case "Engraving":
					_vm.ShippingStatusGroupVisibility = Visibility.Collapsed;
					_vm.EngravingStatusGroupVisibility = Visibility.Collapsed;
					_vm.ActiveMachinesGroupVisibility = Visibility.Visible;
					_vm.AppState.StatusGroup = "Machines";
					break;
				case "Machines":
					_vm.ShippingStatusGroupVisibility = Visibility.Visible;
					_vm.EngravingStatusGroupVisibility = Visibility.Collapsed;
					_vm.ActiveMachinesGroupVisibility = Visibility.Collapsed;
					_vm.AppState.StatusGroup = "Shipping";
					break;
				case "Shipping":
					_vm.ShippingStatusGroupVisibility = Visibility.Collapsed;
					_vm.EngravingStatusGroupVisibility = Visibility.Visible;
					_vm.ActiveMachinesGroupVisibility = Visibility.Collapsed;
					_vm.AppState.StatusGroup = "Engraving";
					break;
			}
		}

		private void SwitchStatusPanelButtonOnClick(object sender, RoutedEventArgs e) => SwitchStatusPanel();

		private void ReadyToShipButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.status_engraving = "engrave_done&with_crystal_product_status[]=completed";
			_vm.AppState.OrderFilterGroup.shipByToday = "False";
			MainNavigationView.SelectedItem = MainNavigationView.MenuItems[1];
		}

		private void AwaitingShipmentButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.status_engraving = "";
			_vm.AppState.OrderFilterGroup.shipByToday = "False";
			_vm.AppState.OrderFilterGroup.status_order = "processing";
			_vm.AppState.OrderFilterGroup.status_shipping = "waiting";
			MainNavigationView.SelectedItem = MainNavigationView.MenuItems[1];
		}

		private void ShipByTodayButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.status_engraving = "";
			_vm.AppState.OrderFilterGroup.shipByToday = "True";
			MainNavigationView.SelectedItem = MainNavigationView.MenuItems[1];
		}

		private void WorkstationButtonOnClick(object sender, RoutedEventArgs e)
		{
			var tag = (int)((ToggleButton) sender).Tag;
			foreach (var workstation in _vm.Workstations.Data)
			{
				if (workstation.Id == tag && workstation.MachinesGroupVisibility == Visibility.Visible)
				{
					_vm.Workstations.PanelSpacing = 51;
					workstation.MachinesGroupVisibility = Visibility.Collapsed;
					return;
				}
				if (workstation.MachinesGroupVisibility == Visibility.Visible)
				{
					_vm.Workstations.PanelSpacing = 51;
					workstation.MachinesGroupVisibility = Visibility.Collapsed;
				}
				if (workstation.Id == tag)
				{
					workstation.MachinesGroupVisibility = Visibility.Visible;
					if (workstation.Id == 10 || workstation.Id == 11)
					{
						_vm.Workstations.PanelSpacing = 40;
					}
				}
				
			}
		}

		private void ReadyToEngraveButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.status_engraving = "ready_to_engrave&with_crystal_product_status[]=engrave_redo";
			_vm.AppState.OrderFilterGroup.shipByToday = "False";
			_vm.AppState.OrderFilterGroup.status_order = "processing";
			_vm.AppState.OrderFilterGroup.status_shipping = "waiting";
			MainNavigationView.SelectedItem = MainNavigationView.MenuItems[1];
		}

		private void EngravingButtonOnClick(object sender, RoutedEventArgs e)
		{
			_vm.AppState.OrderFilterGroup.status_engraving = "engrave_processing";
			_vm.AppState.OrderFilterGroup.shipByToday = "False";
			_vm.AppState.OrderFilterGroup.status_order = "processing";
			_vm.AppState.OrderFilterGroup.status_shipping = "waiting";
			MainNavigationView.SelectedItem = MainNavigationView.MenuItems[1];
		}

		private void ProductionIssuesButtonOnClick(object sender, RoutedEventArgs e)
		{
			MainNavigationView.SelectedItem = MainNavigationView.MenuItems[2];
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//GetAllMacAddressesAndIppairs();
		}
		
	}
}
