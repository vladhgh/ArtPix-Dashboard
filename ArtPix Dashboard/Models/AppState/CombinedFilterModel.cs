using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;

namespace ArtPix_Dashboard.Models.AppState
{
	public class CombinedFilterModel : PropertyChangedListener
	{

		public CombinedFilterModel(string statusGroup = "", string machineId = "", string nameOrder = "", string orderId = "")
		{
			switch (statusGroup)
			{
				case "Search":
					{
						SelectedFilterGroup = "Search";
						name_order = string.IsNullOrEmpty(nameOrder) ? "" : nameOrder;
						order_id = string.IsNullOrEmpty(orderId) ? "" : orderId;
						machine = "";
						status_engraving = "";
						status_order = "";
						status_shipping = "";
						with_crystals = "3";
						has_shipping_package = "";
						with_shipping_totes = "False";
						shipByToday = "False";
						sort_by = "estimate_processing_max_date";
						store_name = "";
						IsFilterGroupEnabled = false;
						IsClearSearchButtonEnabled = true;
						IsShipByTodayButtonEnabled = false;
						return;
					}

				case "Production Issues":
					{
						SelectedFilterGroup = "Production Issues";
						name_order = "";
						order_id = "";
						machine = "";
						status_engraving = "engrave_issue";
						status_shipping = "waiting";
						status_order = "processing";
						with_crystals = "3";
						has_shipping_package = "";
						with_shipping_totes = "False";
						shipByToday = "False";
						sort_by = "estimate_processing_max_date";
						store_name = "";
						IsFilterGroupEnabled = false;
						IsShipByTodayButtonEnabled = true;
						return;
					}

				case "Engraving":
					{

						SelectedFilterGroup = "Engraving In Progress";
						name_order = "";
						order_id = "";
						machine = "";
						status_engraving = "engrave_processing";
						status_shipping = "waiting";
						status_order = "processing";
						with_crystals = "3";
						has_shipping_package = "";
						with_shipping_totes = "False";
						shipByToday = "False";
						sort_by = "estimate_processing_max_date";
						store_name = "";
						IsFilterGroupEnabled = false;
						IsShipByTodayButtonEnabled = true;
						MachineComboBoxVisibility = Visibility.Visible;
						return;
					}

				case "Engraved Today":
					{

						SelectedFilterGroup = "Engraved Today";
						name_order = "";
						order_id = "";
						machine = "";
						status_engraving = "engrave_processing";
						status_shipping = "waiting";
						status_order = "processing";
						with_crystals = "3";
						has_shipping_package = "";
						with_shipping_totes = "False";
						shipByToday = "False";
						sort_by = "estimate_processing_max_date";
						store_name = "";
						IsFilterGroupEnabled = false;
						IsShipByTodayButtonEnabled = false;
						MachineComboBoxVisibility = Visibility.Visible;
						return;
					}

				case "Awaiting Model":
					{
						SelectedFilterGroup = "Awaiting Model";
						name_order = "";
						order_id = "";
						machine = "";
						status_engraving = "3d_model_in_progress&with_crystal_product_status[]=3d_model_pending&with_crystal_product_status[]=3d_model_rework&with_crystal_product_status[]=retoucher_in_progress&with_crystal_product_status[]=retoucher_pending&with_crystal_product_status[]=retoucher_rework";
						status_shipping = "waiting";
						status_order = "processing";
						with_crystals = "3";
						has_shipping_package = "";
						with_shipping_totes = "False";
						shipByToday = "False";
						sort_by = "estimate_processing_max_date";
						store_name = "";
						IsFilterGroupEnabled = false;
						IsShipByTodayButtonEnabled = true;
						return;
					}

				case "Ready To Engrave":
					{
						SelectedFilterGroup = "Ready To Engrave";
						name_order = "";
						order_id = "";
						machine = "";
						status_engraving = "ready_to_engrave&amp;with_crystal_product_status[]=engrave_redo";
						status_shipping = "waiting";
						status_order = "processing";
						with_crystals = "3";
						has_shipping_package = "";
						with_shipping_totes = "False";
						shipByToday = "False";
						sort_by = "estimate_processing_max_date";
						store_name = "";
						IsFilterGroupEnabled = false;
						IsShipByTodayButtonEnabled = true;
						MachineComboBoxVisibility = Visibility.Visible;
						return;
					}

				case "Machine":
					{
						SelectedFilterGroup = "Machine " + machineId;
						machine = machineId;
						name_order = "";
						order_id = "";
						status_engraving = "";
						status_shipping = "waiting";
						status_order = "processing";
						with_crystals = "3";
						has_shipping_package = "";
						with_shipping_totes = "False";
						shipByToday = "False";
						sort_by = "estimate_processing_max_date";
						store_name = "";
						IsFilterGroupEnabled = false;
						IsShipByTodayButtonEnabled = false;
						UnAssignJobsFromMachineButtonVisibility = Visibility.Visible;
						PcPowerButtonsVisibility = Visibility.Visible;
						return;
					}

				case "Ready To Ship":
					{
						SelectedFilterGroup = "Ready To Ship";
						machine = "";
						name_order = "";
						order_id = "";
						status_engraving = "engrave_done&amp;with_crystal_product_status[]=completed";
						status_shipping = "waiting";
						status_order = "processing";
						with_crystals = "3";
						has_shipping_package = "";
						with_shipping_totes = "False";
						shipByToday = "False";
						sort_by = "estimate_processing_max_date";
						store_name = "Amazon&store_name[]=WordPress";
						IsFilterGroupEnabled = true;
						IsShipByTodayButtonEnabled = true;
						CreateManifestButtonVisibility = Visibility.Visible;
						StoreComboBoxVisibility = Visibility.Visible;

						return;
					}

				case "Awaiting Shipment":
					{
						SelectedFilterGroup = "Awaiting Shipment";
						machine = "";
						name_order = "";
						order_id = "";
						status_engraving = "";
						status_shipping = "waiting";
						status_order = "processing";
						with_crystals = "3";
						has_shipping_package = "";
						with_shipping_totes = "False";
						shipByToday = "False";
						sort_by = "estimate_processing_max_date";
						store_name = "";
						IsFilterGroupEnabled = true;
						IsShipByTodayButtonEnabled = true;
						CreateManifestButtonVisibility = Visibility.Visible;
						StoreComboBoxVisibility = Visibility.Visible;
						return;
					}

				case "Ship By Today":
					{

						SelectedFilterGroup = "Ship By Today";
						machine = "";
						name_order = "";
						order_id = "";
						status_engraving = "";
						status_shipping = "waiting";
						status_order = "processing";
						with_crystals = "3";
						has_shipping_package = "";
						with_shipping_totes = "False";
						shipByToday = "True";
						sort_by = "estimate_processing_max_date";
						store_name = "Amazon&store_name[]=WordPress";
						IsFilterGroupEnabled = true;
						IsShipByTodayButtonEnabled = true;
						CreateManifestButtonVisibility = Visibility.Visible;
						StoreComboBoxVisibility = Visibility.Visible;
						return;
					}

			}
		}

		
		private bool _isClearSearchButtonEnabled = false;

		public bool IsClearSearchButtonEnabled

		{
			get => _isClearSearchButtonEnabled;
			set => SetProperty(ref _isClearSearchButtonEnabled, value);
		}

		private bool _isShipByTodayButtonEnabled = true;

		public bool IsShipByTodayButtonEnabled

		{
			get => _isShipByTodayButtonEnabled;
			set => SetProperty(ref _isShipByTodayButtonEnabled, value);
		}

		private Visibility _storeComboBoxVisibility = Visibility.Collapsed;

		public Visibility StoreComboBoxVisibility

		{
			get => _storeComboBoxVisibility;
			set => SetProperty(ref _storeComboBoxVisibility, value);
		}

		private Visibility _unAssignJobsFromMachineButtonVisibility = Visibility.Collapsed;

		public Visibility UnAssignJobsFromMachineButtonVisibility
		{
			get => _unAssignJobsFromMachineButtonVisibility;
			set => SetProperty(ref _unAssignJobsFromMachineButtonVisibility, value);
		}
		private Visibility _machineComboBoxVisibility = Visibility.Collapsed;

		public Visibility MachineComboBoxVisibility
		{
			get => _machineComboBoxVisibility;
			set => SetProperty(ref _machineComboBoxVisibility, value);
		}
		private Visibility _createManifestButtonVisibility = Visibility.Collapsed;

		public Visibility CreateManifestButtonVisibility
		{
			get => _createManifestButtonVisibility;
			set => SetProperty(ref _createManifestButtonVisibility, value);
		}

		private Visibility _pcPowerButtonsVisibility = Visibility.Collapsed;

		public Visibility PcPowerButtonsVisibility
		{
			get => _pcPowerButtonsVisibility;
			set => SetProperty(ref _pcPowerButtonsVisibility, value);
		}


		private bool _isFilterGroupEnabled = true;

		public bool IsFilterGroupEnabled
		{
			get => _isFilterGroupEnabled;
			set => SetProperty(ref _isFilterGroupEnabled, value);
		}

		private string _selectedFilterGroup = "Awaiting Shipment";

		public string SelectedFilterGroup
		{
			get => _selectedFilterGroup;
			set => SetProperty(ref _selectedFilterGroup, value);
		}

		private string _machine;

		public string machine
		{
			get => _machine;
			set => SetProperty(ref _machine, value);
		}

		private int _pageNumber = 1;

		public int pageNumber
		{
			get => _pageNumber;
			set => SetProperty(ref _pageNumber, value);
		}

		private int _perPage = 15;

		public int perPage
		{
			get => _perPage;
			set => SetProperty(ref _perPage, value);
		}

		private bool _withPages = true;

		public bool withPages
		{
			get => _withPages;
			set => SetProperty(ref _withPages, value);
		}

		private string _order_id;

		public string order_id
		{
			get => _order_id;
			set => SetProperty(ref _order_id, value);
		}
		private string _name_order;

		public string name_order
		{
			get => _name_order;
			set => SetProperty(ref _name_order, value);
		}

		private string _status_engraving;
		public string status_engraving
		{
			get => _status_engraving;
			set => SetProperty(ref _status_engraving, value);
		}

		private string _status_shipping = "waiting";
		public string status_shipping
		{
			get => _status_shipping;
			set => SetProperty(ref _status_shipping, value);
		}

		private string _status_order = "processing";
		public string status_order
		{
			get => _status_order;
			set => SetProperty(ref _status_order, value);
		}

		private string _with_crystals;
		public string with_crystals
		{
			get => _with_crystals;
			set => SetProperty(ref _with_crystals, value);
		}

		private string _has_shipping_package;
		public string has_shipping_package
		{
			get => _has_shipping_package;
			set => SetProperty(ref _has_shipping_package, value);
		}

		private string _with_shipping_totes;
		public string with_shipping_totes
		{
			get => _with_shipping_totes;
			set => SetProperty(ref _with_shipping_totes, value);
		}

		private string _sort_by;
		public string sort_by
		{
			get => _sort_by;
			set => SetProperty(ref _sort_by, value);
		}

		private string _shipByToday;
		public string shipByToday
		{
			get => _shipByToday;
			set => SetProperty(ref _shipByToday, value);
		}

		private string _store_name;
		public string store_name
		{
			get => _store_name;
			set => SetProperty(ref _store_name, value);
		}

	}
}
