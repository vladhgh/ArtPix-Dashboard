﻿using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.Models.Types;
using ArtPix_Dashboard.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using QRCoder;
using ArtPix_Dashboard.Views.Dialogs;
using ModernWpf.Controls;
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.Machine;
using ToastNotifications.Messages;
using Product = ArtPix_Dashboard.Models.Order.Product;

namespace ArtPix_Dashboard.ViewModels
{
	class ShippingDashboardViewModel : PropertyChangedListener
	{

		#region PROPS
		private AppStateModel _appState = new AppStateModel();
		public AppStateModel AppState
		{
			get => _appState;
			set => SetProperty(ref _appState, value);
		}
		private bool _isLoading;
		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
		}
		private Bitmap _qrCodeBitmap;
		public Bitmap QrCodeBitmap
		{
			get => _qrCodeBitmap;
			set => SetProperty(ref _qrCodeBitmap, value);
		}
		private BitmapImage _qrCode;
		public BitmapImage QrCode
		{
			get => _qrCode;
			set => SetProperty(ref _qrCode, value);
		}
		
		private Visibility _isLoaded;
		public Visibility IsLoaded
		{
			get => _isLoaded;
			set => SetProperty(ref _isLoaded, value);
		}
		private ObservableCollection<PageModel> _pages = new ObservableCollection<PageModel>();
		public ObservableCollection<PageModel> Pages
		{
			get => _pages;
			set => SetProperty(ref _pages, value);
		}
		private OrderModel _orders = new OrderModel();
		public OrderModel Orders
		{
			get => _orders;
			set => SetProperty(ref _orders, value);
		}
		#endregion

		#region COMMANDS
		private ICommand _onManualComplete;
		public ICommand OnManualComplete
		{
			get => _onManualComplete;
			set => SetProperty(ref _onManualComplete, value);
		}
		private ICommand _onUnassignMachine;
		public ICommand OnUnassignMachine
		{
			get => _onUnassignMachine;
			set => SetProperty(ref _onUnassignMachine, value);
		}
		private ICommand _onFindBestService;
		public ICommand OnFindBestService
		{
			get => _onFindBestService;
			set => SetProperty(ref _onFindBestService, value);
		}
		private ICommand _onAssignMachine;
		public ICommand OnAssignMachine
		{
			get => _onAssignMachine;
			set => SetProperty(ref _onAssignMachine, value);
		}
		
		private ICommand _onImageClick;
		public ICommand OnImageClick
		{
			get => _onImageClick;
			set => SetProperty(ref _onImageClick, value);
		}
		private ICommand _onProductHistory;
		public ICommand OnProductHistory
		{
			get => _onProductHistory;
			set => SetProperty(ref _onProductHistory, value);
		}
		
		private ICommand _reloadList;
		public ICommand ReloadList
		{
			get => _reloadList;
			set => SetProperty(ref _reloadList, value);
		}
		private ICommand _onPrintQR;
		public ICommand OnPrintQR
		{
			get => _onPrintQR;
			set => SetProperty(ref _onPrintQR, value);
		}
		
		private ICommand _onOA;
		public ICommand OnOA
		{
			get => _onOA;
			set => SetProperty(ref _onOA, value);
		}
		private ICommand _onCP;
		public ICommand OnCP
		{
			get => _onCP;
			set => SetProperty(ref _onCP, value);
		}
		private ICommand _onRedo;
		public ICommand OnRedo
		{
			get => _onRedo;
			set => SetProperty(ref _onRedo, value);
		}
		private ICommand _onRework;
		public ICommand OnRework
		{
			get => _onRework;
			set => SetProperty(ref _onRework, value);
		}
		private ICommand _onCancelIssue;
		public ICommand OnCancelIssue
		{
			get => _onCancelIssue;
			set => SetProperty(ref _onCancelIssue, value);
		}
		private ICommand _copyToClipboard;
		public ICommand CopyToClipboard
		{
			get => _copyToClipboard;
			set => SetProperty(ref _copyToClipboard, value);
		}
		private ICommand _onVitromark;
		public ICommand OnVitromark
		{
			get => _onVitromark;
			set => SetProperty(ref _onVitromark, value);
		}

		private ICommand _onReEngrave;
		public ICommand OnReEngrave
		{
			get => _onReEngrave;
			set => SetProperty(ref _onReEngrave, value);
		}

		#endregion

		private void InitializeCommands()
		{
			OnUnassignMachine = new DelegateCommand(OpenUnassignDialog);
			OnManualComplete = new DelegateCommand(OpenManualCompleteDialog);
			OnAssignMachine = new DelegateCommand(OpenAssignMachineDialog);
			OnImageClick = new DelegateCommand(OpenImage);
			ReloadList = new DelegateCommand(async param => await GetOrdersList());
			OnOA = new DelegateCommand(Commands.OpenOrderOnOA);
			OnCP = new DelegateCommand(Commands.OpenOrderOnCP);
			CopyToClipboard = new DelegateCommand(param => Commands.CopyTextToClipboard(param.ToString()));
			OnVitromark = new DelegateCommand(param => Commands.OpenFileInVitroMark(param.ToString()));
			OnProductHistory = new DelegateCommand(OpenProductHistoryDialog);
			OnFindBestService = new DelegateCommand(FindBestServiceButtonOnClick);
			OnReEngrave = new DelegateCommand(OpenReEngraveDialog);
		}

		private async void OpenReEngraveDialog(object param)
		{
			var product = (Product)param;
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == product.IdOrders);
			var machines = await ArtPixAPI.GetMachines(product.IdProducts);
			var dialog = new ReEngraveDialog(machines.Data);
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				if (!string.IsNullOrEmpty(dialog.Combo2.Text))
				{
					var x = (Models.Machine.Machine)dialog.Combo2.SelectedItem;
					var body = new AssignProcessingModel
					{
						machine = x.Name,
						user = "Supervisor",
						machine_assign_item_id = product.MachineAssignItemId

					};
					await ArtPixAPI.ProductReEngrave(body);
					Utils.Utils.Notifier.ShowSuccess($"Product re-engrave success!");
				}
				//order = await ArtPixAPI.GetOrder(((Product)param).IdOrders.ToString());
				var index = Orders.Data.IndexOf(order);
				Orders.Data[index] = await ArtPixAPI.GetOrder(((Product)param).IdOrders.ToString());
			}
		}

		private async void OpenAssignMachineDialog(object param)
		{
			var product = (Product)param;
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == product.IdOrders);
			var machines = await ArtPixAPI.GetMachines(product.IdProducts);
			var dialog = new AssignMachineDialog(machines.Data);
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				if (!string.IsNullOrEmpty(dialog.Combo2.Text))
				{
					var x = (Models.Machine.Machine)dialog.Combo2.SelectedItem;
					var body = new AssignProcessingModel
					{
						machine = x.Name,
						product_id = product.IdProducts,
						order_id = product.IdOrders,
						order_name = order.NameOrder
					};
					await ArtPixAPI.ProductAssignProcessing(body);
					Utils.Utils.Notifier.ShowSuccess($"Assigned To Machine {body.machine} Successfully!");
				}
				var index = Orders.Data.IndexOf(order);
				Orders.Data[index] = await ArtPixAPI.GetOrder(((Product)param).IdOrders.ToString());
			}
		}

		private async void OpenManualCompleteDialog(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Product)param).IdOrders);
			var item = order.Products.SingleOrDefault(p => p.IdProducts == ((Product)param).IdProducts);
			var dialog = new ManualCompleteDialog();
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				IsLoaded = Visibility.Hidden;
				IsLoading = true;
				var newStatus = new NewStatusModel
				{
					machine_assign_item_id = item.MachineAssignItemId,
					new_status = "success_manually",
					order_id = item.IdOrders,
					order_name = order.NameOrder,
					product_id = item.IdProducts
				};
				await ArtPixAPI.ChangeMachineAssignItemStatusAsync(newStatus);
				//order = await ArtPixAPI.GetOrder(((Product)param).IdOrders.ToString());
			}
		}
		private async void OpenUnassignDialog(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Product)param).IdOrders);
			var item = order.Products.SingleOrDefault(p => p.IdProducts == ((Product)param).IdProducts);
			var dialog = new UnassignDialog();
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				IsLoaded = Visibility.Hidden;
				IsLoading = true;
				var body = new AssignProcessingModel
				{
					order_id = item.IdOrders,
					order_name = order.NameOrder,
					product_id = item.IdProducts
				};
				await ArtPixAPI.UnassignMachineAssignItemAsync(body);
				//order = await ArtPixAPI.GetOrder(param.ToString());
			}
		}
		private async void FindBestServiceButtonOnClick(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == (int)param);
			if (order != null)
			{
				order.IsShippingInformationLoading = true;
				var res = await ArtPixAPI.FindBestServiceAsync(new FindBestServiceRequestModel
					{order_id = param.ToString()});
				order.ShippingOrderInfo = (await ArtPixAPI.GetOrder(param.ToString())).ShippingOrderInfo;
				order.IsShippingInformationLoading = false;
				order.IsShippingServiceFound = "Shipping Service Found";
				order.IsShippingServiceFoundColor = "DarkGreen";
				order.ShippingInformationPanelVisibility = Visibility.Visible;
			}
		}

		private async void OpenProductHistoryDialog(object param)
		{
			var dialog = new ProductHistoryDialog((int)param);
			await dialog.ShowAsync();
		}

		public async void OpenImage(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Product)param).IdOrders);
			var product = order?.Products.SingleOrDefault(p => p.IdProducts == ((Product) param).IdProducts);
			if (product != null)
			{
				var dialog = new PhotoPreviewDialog(product);
				var result = await dialog.ShowAsync();
			}
		}


		public void Initialize()
		{
			InitializeCommands();
		}

		public async Task GetOrdersList(int pageNumber = 1, int perPage = 15, bool withPages = true, OrderCombineFilterModel filterGroup = null)
		{
			IsLoading = true;
			IsLoaded = Visibility.Collapsed;
			Orders = await ArtPixAPI.GetOrdersAsync(pageNumber, perPage, filterGroup);
			foreach (var page in Pages)
			{
				page.IsSelected = page.PageNumber == pageNumber;
			}
			Pages = withPages ? GetPages(pageNumber, perPage, filterGroup) : new ObservableCollection<PageModel>(Pages);
			IsLoading = false;
			IsLoaded = Visibility.Visible;
		}
		private ObservableCollection<PageModel> GetPages(int currentPageNumber, int perPage = 15, OrderCombineFilterModel filterGroup = null)
		{
			var pages = new ObservableCollection<PageModel>();
			for (var i = Orders.Meta.CurrentPage; i <= Orders.Meta.LastPage; i++)
			{
				if (i > Orders.Meta.CurrentPage + 5 && i < Orders.Meta.LastPage)
				{
					var page = new PageModel(i + 5, "...", Orders.Meta.Path + "?page=" + (i + 5))
					{
						IsSelected = i == Orders.Meta.CurrentPage,
						NavigateToSelectedPage = new DelegateCommand(async param => await GetOrdersList((currentPageNumber + 5),
							perPage, true, filterGroup))
					};
					pages.Add(page);
				}
				else
				{
					var page = new PageModel(i, i.ToString(), Orders.Meta.Path + "?page=" + i)
					{
						IsSelected = i == Orders.Meta.CurrentPage,
						NavigateToSelectedPage = new DelegateCommand(async param => await GetOrdersList((int) param, perPage, false, filterGroup))
					};
					pages.Add(page);
				}
			}
			pages = new ObservableCollection<PageModel>(pages.GroupBy(x => x.PageName).Select(g => g.First()).ToList());
			return pages;
		}
	}
}
