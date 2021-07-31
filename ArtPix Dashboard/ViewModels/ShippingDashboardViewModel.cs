using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using QRCoder;
using ModernWpf.Controls;
using ToastNotifications.Messages;
using Product = ArtPix_Dashboard.Models.Order.Product;
using System.Globalization;
using ArtPix_Dashboard.Dialogs;
using ArtPix_Dashboard.Models.AppState;
using ArtPix_Dashboard.Models.Common;
using ArtPix_Dashboard.Models.MachineAssignedItem;
using ArtPix_Dashboard.Models.Requests;
using ArtPix_Dashboard.Utils;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.Views;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using System.IO;

namespace ArtPix_Dashboard.ViewModels
{
	public class ShippingDashboardViewModel : PropertyChangedListener
	{
		//EMOJIS: ✅❎🔄️

		#region PROPERTIES

		private Product SelectedItem;
		private Models.Order.Datum SelectedOrder;
		private FrameworkElement progressRingImage;
		private FrameworkElement orderExpander;


		private AppStateModel _appState = new AppStateModel();
		public AppStateModel AppState
		{
			get => _appState;
			set => SetProperty(ref _appState, value);
		}

		private ShippingDashboardView _view;
		public ShippingDashboardView View
		{
			get => _view;
			set => SetProperty(ref _view, value);
		}
		
		private bool _isLoading;
		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
		}
		private bool _isOrderLoading;
		public bool IsOrderLoading
		{
			get => _isOrderLoading;
			set => SetProperty(ref _isOrderLoading, value);
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
		private Visibility _noResultsTextVisibility = Visibility.Collapsed;
		public Visibility NoResultsTextVisibility
		{
			get => _noResultsTextVisibility;
			set => SetProperty(ref _noResultsTextVisibility, value);
		}
		
		private Visibility _progressRingVisibility = Visibility.Collapsed;
		public Visibility ProgressRingVisibility
		{
			get => _progressRingVisibility;
			set => SetProperty(ref _progressRingVisibility, value);
		}
		private bool _paginationBackButtonVisibility;
		public bool PaginationBackButtonVisibility
		{
			get => _paginationBackButtonVisibility;
			set => SetProperty(ref _paginationBackButtonVisibility, value);
		}
		private bool _paginationForwardButtonVisibility;
		public bool PaginationForwardButtonVisibility
		{
			get => _paginationForwardButtonVisibility;
			set => SetProperty(ref _paginationForwardButtonVisibility, value);
		}


		private List<PageModel> _pages = new ();
		public List<PageModel> Pages
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
		private MachineAssignItemModel _engravedTodayItems = new();
		public MachineAssignItemModel EngravedTodayItems
		{
			get => _engravedTodayItems;
			set => SetProperty(ref _engravedTodayItems, value);
		}
		#endregion

		#region COMMANDS

		private ICommand _onManualComplete;
		public ICommand OnManualComplete
		{
			get => _onManualComplete;
			set => SetProperty(ref _onManualComplete, value);
		}
		private ICommand _onCreateDHLManifest;
		public ICommand OnCreateDHLManifest
		{
			get => _onCreateDHLManifest;
			set => SetProperty(ref _onCreateDHLManifest, value);
		}
		
		private ICommand _onAssignJobs;
		public ICommand OnAssignJobs
		{
			get => _onAssignJobs;
			set => SetProperty(ref _onAssignJobs, value);
		}
		
		private ICommand _onUnassignAllJobs;
		public ICommand OnUnassignAllJobs
		{
			get => _onUnassignAllJobs;
			set => SetProperty(ref _onUnassignAllJobs, value);
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
		private ICommand _onAddIssue;
		public ICommand OnAddIssue
		{
			get => _onAddIssue;
			set => SetProperty(ref _onAddIssue, value);
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

		#region CONSTRUCTOR INITIALIZATION

		private void InitializeCommands()
		{
			OnUnassignMachine = new DelegateCommand(OpenUnassignDialog);
			OnManualComplete = new DelegateCommand(OpenManualCompleteDialog);
			OnAssignMachine = new DelegateCommand(OpenAssignMachineDialog);
			OnImageClick = new DelegateCommand(OpenImage);
			OnAddIssue = new DelegateCommand(OpenAddIssueDialog);
			OnAssignJobs = new DelegateCommand(OpenAssignJobsDialog);
			OnUnassignAllJobs = new DelegateCommand(OpenUnassignAllDialog);
			ReloadList = new DelegateCommand(async param => await GetOrdersList(AppState.CombinedFilter));
			OnOA = new DelegateCommand(Commands.OpenOrderOnOA);
			OnCP = new DelegateCommand(Commands.OpenOrderOnCP);
			OnCreateDHLManifest = new DelegateCommand(OpenDhlManifestDialog);
			OnPrintQR = new DelegateCommand(PrintQR);
			OnRedo = new DelegateCommand(OpenRedoDialog);
			OnRework = new DelegateCommand(OpenReworkDialog);
			CopyToClipboard = new DelegateCommand(param => Commands.CopyTextToClipboard(param.ToString()));
			OnVitromark = new DelegateCommand(param => Commands.OpenFileInVitroMark(param.ToString()));
			OnProductHistory = new DelegateCommand(OpenProductHistoryDialog);
			OnFindBestService = new DelegateCommand(FindBestServiceButtonOnClick);
			OnReEngrave = new DelegateCommand(OpenReEngraveDialog);
		}

		public void Initialize(AppStateModel appState, ShippingDashboardView view)
		{
			View = view;
			AppState = appState;
			InitializeCommands();
		}

		#endregion

		#region DHL MANIFEST DIALOG - NOT DONE - ❎

		private async void OpenDhlManifestDialog(object param)
		{
			//var dialog = new DhlManifestDialog();
			//var result = await dialog.ShowAsync();
			//if (result != ContentDialogResult.Primary) return;
			//if (string.IsNullOrEmpty(dialog.PrinterSelection.Text))
			//{
			//	Utils.Utils.Notifier.ShowError("Printer has to be selected!\nPlease try again!");
			//	return;
			//}
			//var x = await ArtPixAPI.CreateDhlManifest(new CreateDhlManifestRequest()
			//{
			//	international_containers_count = Int32.Parse(dialog.Combo1.Text),
			//	domestic_containers_count = Int32.Parse(dialog.Combo2.Text),
			//});
			//foreach(var manifest in x.Manifests)
			//{
			//	byte[] sPDFDecoded = Convert.FromBase64String(manifest.FileData);

			//	File.WriteAllBytes("\\\\artpix\\wh\\testManifest.pdf", sPDFDecoded);

			//}
		}

		#endregion

		#region ASSIGN JOBS DIALOG - DONE - ✅

		private async void OpenAssignJobsDialog(object param)
		{
			var dialog = new AssignJobsDialog();
			var result = await dialog.ShowAsync();
			if (result != ContentDialogResult.Primary) return;
			if (string.IsNullOrEmpty(dialog.Combo1.Text))
			{
				Utils.Utils.Notifier.ShowError("Jobs count has to be selected!\nPlease try again!");
				return;
			}
			ToggleMainLoadingAnimation(1);
			await ArtPixAPI.GetNextOrderAsync(AppState.CombinedFilter.machine, dialog.Combo1.Text);
			await GetOrdersList(AppState.CombinedFilter);
			ToggleMainLoadingAnimation(0);
		}

		#endregion

		#region ORDERS LIST INITIALIZATION - DONE - ✅

		public async Task GetOrdersList(CombinedFilterModel combinedFilter)
		{

			if (AppState.CombinedFilter.SelectedFilterGroup == "Engraved Today")
			{
				EngravedTodayItems = await ArtPixAPI.GetEngravedTodayItemsAsync("All", combinedFilter.pageNumber.ToString(), combinedFilter.perPage.ToString());
				Orders.Meta = EngravedTodayItems.Meta;
				Orders.Data = new List<Models.Order.Datum>();
				View.ShippingItemsListView.ItemsSource = EngravedTodayItems.Data;
				foreach (var item in EngravedTodayItems.Data)
				{
					Orders.Data.Add(new Models.Order.Datum());
				}
				if (combinedFilter.withPages)
				{
					Pages = await GetPages(combinedFilter);
				}
				return;
			}
			Orders = await ArtPixAPI.GetOrdersAsync(combinedFilter);
			if (combinedFilter.withPages)
			{
				Pages = await GetPages(combinedFilter);
			}
		}

		#endregion

		#region PAGES LIST INITIALIZATION

		private async Task<List<PageModel>> GetPages(CombinedFilterModel combinedFilter)
		{
			var pages = new List<PageModel>();
			await Task.Run(() =>
			{
				for (var i = Orders.Meta.CurrentPage; i <= Orders.Meta.LastPage; i++)
				{
					var page = new PageModel()
					{
						PageName = i.ToString(),
						PageNumber = i,
						PageUrl = Orders.Meta.Path + "?page=" + i,
						IsSelected = i == Orders.Meta.CurrentPage,
					};
					pages.Add(page);
				}
			});
			return pages;
		}

		#endregion

		#region UNASSIGN ALL DIALOG - DONE - ✅

		private async void OpenUnassignAllDialog(object param)
		{
			var dialog = new UnassignAllDialog();
			var result = await dialog.ShowAsync();
			if (result != ContentDialogResult.Primary) return;
			ToggleMainLoadingAnimation(1);
			await ArtPixAPI.RemoveCurrentJobsFromMachineAsync(param.ToString());
			await GetOrdersList(AppState.CombinedFilter);
			ToggleMainLoadingAnimation(0);
			Animation.FadeIn(View.NoResultsText);
		}

		#endregion

		#region ADD ISSUE DIALOG - DONE - ✅

		private async void OpenAddIssueDialog(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Models.Order.Product)param).IdOrders);
			var product = order.Products.SingleOrDefault(p => p.MachineAssignItemId == ((Models.Order.Product)param).MachineAssignItemId);
			var dialog = new AddIssueDialog();
			var result = await dialog.ShowAsync();
			if (result != ContentDialogResult.Primary) return;
			if (String.IsNullOrEmpty(dialog.Combo1.Text)) return;
			ToggleOrderLoadingAnimation(1, order.NameOrder);
			var body = new AddProductionIssueRequest()
			{
				machine_assign_item_id = product.MachineAssignItemId,
				user = "Supervisor",
				error = dialog.MessageBox.Text,
				source = "packing_station",
				issue_title = dialog.Combo1.Text,
				production_issue_reason_id = ((Models.IssueReasons.Datum)dialog.Combo1.SelectedItem).Id
			};
			await ArtPixAPI.AddProductionIssueAsync(body);
			await UpdateOrderInfoAsync(product.IdOrders);
			ToggleOrderLoadingAnimation(0, order.NameOrder);
		}

		#endregion

		#region REDO DIALOG - DONE - IMPROVEMENT NEEDED - 🔄️

		private async void OpenRedoDialog(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Models.Order.Product)param).IdOrders);
			var product = order.Products.SingleOrDefault(p => p.MachineAssignItemId == ((Models.Order.Product)param).MachineAssignItemId);
			var machines = await ArtPixAPI.GetMachines(product.IdProducts);
			var dialog = new RedoDialog(machines);
			var result = await dialog.ShowAsync();
			if (result != ContentDialogResult.Primary) return;
			ToggleOrderLoadingAnimation(1, order.NameOrder);
			var requestBody = new ResolveProductionIssueRequest()
			{
				machine_assign_error_id = product.MachineAssignErrorId,
				machine_assign_item_id = product.MachineAssignItemId,
				id_products = product.IdProducts,
				machine_id = Int32.Parse(product.MachineId),
				user = "Supervisor",
				issue_type = dialog.Combo1.SelectedValue.ToString(),
				status_error = "redo",
				message = "Testing"
			};
			await ArtPixAPI.ResolveProductionIssueAsync(requestBody);
			Utils.Utils.Notifier.ShowSuccess("Issue Resolved Successfully!");
			if (!string.IsNullOrEmpty(dialog.Combo2.Text))
			{
				var x = (Models.Workstation.Machine)dialog.Combo2.SelectedItem;
				var body = new ChangeMachineAssignItemStatusRequest()
				{
					machine = x.Name,
					product_id = product.IdProducts,
					order_id = product.IdOrders,
					order_name = order.NameOrder
				};
				await ArtPixAPI.ProductAssignProcessing(body);
				Utils.Utils.Notifier.ShowSuccess("Assigned To Machine Successfully!");
			}
			if (System.IO.Directory.Exists($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{product.MachineAssignItemId}"))
			{
				System.IO.Directory.Delete($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{product.MachineAssignItemId}", true);
				Utils.Utils.Notifier.ShowSuccess($"Local Files Removed Successfully For Product {product.MachineAssignItemId}!");
			}
			await UpdateOrderInfoAsync(product.IdOrders);
			ToggleOrderLoadingAnimation(0, order.NameOrder);
		}

		#endregion

		#region REWORK DIALOG - DONE - ✅
		private async void OpenReworkDialog(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Models.Order.Product)param).IdOrders);
			var product = order.Products.SingleOrDefault(p => p.MachineAssignItemId == ((Models.Order.Product)param).MachineAssignItemId);
			var dialog = new ReworkDialog();
			var result = await dialog.ShowAsync();
			switch (result)
			{
				case ContentDialogResult.Primary:
					{
						ToggleOrderLoadingAnimation(1, order.NameOrder);
						var requestBody = new ResolveProductionIssueRequest()
						{
							machine_assign_error_id = product.MachineAssignErrorId,
							machine_assign_item_id = product.MachineAssignItemId,
							id_products = product.IdProducts,
							machine_id = Int32.Parse(product.MachineId),
							user = "Supervisor",
							issue_type = "Retoucher Error",
							status_error = "retouch",
							message = dialog.MessageBox.Text
						};
						await ArtPixAPI.ResolveProductionIssueAsync(requestBody);
						Utils.Utils.Notifier.ShowInformation("3D Model Sent To Retoucher For Rework Successfully!");
						if (System.IO.Directory.Exists($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{product.MachineAssignItemId}"))
						{
							System.IO.Directory.Delete($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{product.MachineAssignItemId}", true);
							Utils.Utils.Notifier.ShowSuccess($"Local Files Removed Successfully For Product {product.MachineAssignItemId}!");
						}
						await UpdateOrderInfoAsync(order.IdOrders);
						ToggleOrderLoadingAnimation(0, order.NameOrder);
						return;
					}
				case ContentDialogResult.Secondary:
					{
						ToggleOrderLoadingAnimation(1, order.NameOrder);
						var requestBody = new ResolveProductionIssueRequest()
						{
							machine_assign_error_id = product.MachineAssignErrorId,
							machine_assign_item_id = product.MachineAssignItemId,
							id_products = product.IdProducts,
							machine_id = Int32.Parse(product.MachineId),
							user = "Supervisor",
							issue_type = "Retoucher Error",
							status_error = "looxis",
							message = dialog.MessageBox.Text
						};
						await ArtPixAPI.ResolveProductionIssueAsync(requestBody);
						Utils.Utils.Notifier.ShowInformation("3D Model Sent To Looxis For Rework Successfully!");
						if (System.IO.Directory.Exists($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{product.MachineAssignItemId}"))
						{
							System.IO.Directory.Delete($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{product.MachineAssignItemId}", true);
							Utils.Utils.Notifier.ShowSuccess($"Local Files Removed Successfully For Product {product.MachineAssignItemId}!");
						}

						await UpdateOrderInfoAsync(order.IdOrders);
						ToggleOrderLoadingAnimation(0, order.NameOrder);
						return;
					}
				default: return;
			}
		}



		#endregion

		#region PRINT QR - DONE - POSSIBLE IMPROVEMENT: ADD MORE DETAILS TO LABEL - ✅

		public void PrintQR(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Models.Order.Product)param).IdOrders);
			var product = order.Products.SingleOrDefault(p => p.MachineAssignItemId == ((Models.Order.Product)param).MachineAssignItemId);
			if (product is Product)
			{
				SelectedOrder = order;
				SelectedItem = product;
				PrintDocument pd = new PrintDocument();
				pd.PrinterSettings.DefaultPageSettings.PaperSize = new PaperSize("QR", 62, 29);
				pd.OriginAtMargins = false;
				pd.PrintPage += PrintPage;
				pd.Print();
			}

			Utils.Utils.Notifier.ShowSuccess($" QR code for product {product.IdProducts} printed successfully!");
		}

		public void PrintPage(object o, PrintPageEventArgs e)
		{
			var item = SelectedItem;
			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode("\t" + $"{item.IdOrders}-{item.IdProducts}-{item.MachineAssignItemId}" + "\n", QRCodeGenerator.ECCLevel.Q);
			QRCode qrCode = new QRCode(qrCodeData);
			var qr = qrCode.GetGraphic(20);
			qr = Utils.Utils.ResizeImage(qr, 95, 95);
			var img = (System.Drawing.Image)qr;
			System.Drawing.Point loc = new System.Drawing.Point(0, 0);
			Rectangle rect = new Rectangle(2, 2, 225, 88);
			e.Graphics.DrawImage(img, loc);
			if (SelectedOrder.NameOrder.Length > 6)
			{
				e.Graphics.DrawString($"{SelectedOrder.NameOrder}", new Font("Consolas", 9), new SolidBrush(Color.Black), 95, 45);
			}
			else
			{
				e.Graphics.DrawString($"{SelectedOrder.NameOrder}", new Font("Consolas", 12), new SolidBrush(Color.Black), 125, 45);
			}
		}

		#endregion

		#region REENGRAVE DIALOG - DONE - ✅

		private async void OpenReEngraveDialog(object param)
		{
			var product = (Product)param;
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == product.IdOrders);
			var machines = await ArtPixAPI.GetMachines(product.IdProducts);
			var body = new ChangeMachineAssignItemStatusRequest();
			var dialog = new ReEngraveDialog(machines);
			var result = await dialog.ShowAsync();
			if (result != ContentDialogResult.Primary) return;
			ToggleOrderLoadingAnimation(1, order.NameOrder);
			body = !string.IsNullOrEmpty(dialog.Combo2.Text)
				? new ChangeMachineAssignItemStatusRequest()
				{
					machine = ((Models.Workstation.Machine) dialog.Combo2.SelectedItem).Name,
					user = "Supervisor",
					machine_assign_item_id = product.MachineAssignItemId
				}
				: new ChangeMachineAssignItemStatusRequest()
				{
					user = "Supervisor",
					machine_assign_item_id = product.MachineAssignItemId
				};
			await ArtPixAPI.ProductReEngrave(body);
			await UpdateOrderInfoAsync(order.IdOrders);
			ToggleOrderLoadingAnimation(0, order.NameOrder);
			Utils.Utils.Notifier.ShowSuccess($"Product re-engrave success!");
		}

		#endregion

		#region UPDATE ORDER INFO AFTER DOING CHANGES - WORK IN PROGRESS - 🔄️

		private async Task UpdateOrderInfoAsync(int orderId)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == orderId);
			var updatedOrder = await ArtPixAPI.GetOrder(orderId.ToString());
			for (var i = 0; i < order.Products.Count; i++)
			{
				order.Products[i].Employee = updatedOrder.Products[i].Employee;
				order.Products[i].MachineId = updatedOrder.Products[i].MachineId;

				order.Products[i].Status = updatedOrder.Products[i].Status;
				order.Products[i].StatusColor = Utils.Utils.SelectStatusColor(order.Products[i].Status);

				order.Products[i].AssignMachineButtonVisibility = updatedOrder.Products[i].AssignMachineButtonVisibility;
				order.Products[i].UnAssignMachineButtonVisibility = updatedOrder.Products[i].UnAssignMachineButtonVisibility;
				order.Products[i].ManualCompleteButtonVisibility = updatedOrder.Products[i].ManualCompleteButtonVisibility;
				order.Products[i].CrystalIssueButtonVisibility = updatedOrder.Products[i].CrystalIssueButtonVisibility;
				order.Products[i].ProductionIssueButtonsVisibility = updatedOrder.Products[i].ProductionIssueButtonsVisibility;
				order.Products[i].ReEngraveButtonVisibility = updatedOrder.Products[i].ReEngraveButtonVisibility;
				order.Products[i].MachineButtonVisibility = updatedOrder.Products[i].MachineButtonVisibility;

				order.Products[i].UpdatedAt = DateTime.Parse(updatedOrder.Products[i].UpdatedAt, CultureInfo.CurrentUICulture).AddHours(5).ToString(CultureInfo.CurrentUICulture); //FIX
				order.Products[i].UpdatedAtAge = "";
				order.Products[i].UpdatedAtAgeColor = "";

				if (order.Products[i].Status == "Engraving Issue")
				{
					var productionIssue =
						await ArtPixAPI.GetProductionIssueAsync(order.Products[i].MachineAssignItemId.ToString());
					order.Products[i].MachineAssignErrorId = productionIssue.Data[0].Id;
					order.Products[i].Status = productionIssue.Data[0].ProductionIssueReason.Reason;
					order.Products[i].Employee = productionIssue.Data[0].User;
				}

			}
			order.Status = updatedOrder.Status;
			order.StatusOrderColor = Utils.Utils.SelectStatusColor(order.Status);
			order.UpdatedAt = DateTime.Parse(updatedOrder.UpdatedAt, CultureInfo.CurrentUICulture).AddHours(5).ToString(CultureInfo.CurrentUICulture); //FIX
			order.UpdatedAtAge = "";
		}

		#endregion

		#region ASSIGN MACHINE DIALOG - TESTING - 🔄️

		private async void OpenAssignMachineDialog(object param)
		{
			var product = (Product)param;
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == product.IdOrders);
			var machines = await ArtPixAPI.GetMachines(product.IdProducts);
			var dialog = new AssignMachineDialog(machines);
			var result = await dialog.ShowAsync();
			if (result != ContentDialogResult.Primary) return;
			if (string.IsNullOrEmpty(dialog.Combo2.Text))
			{
				Utils.Utils.Notifier.ShowError("Machine has to be selected!\nPlease try again!");
				return;
			}
			ToggleOrderLoadingAnimation(1, order.NameOrder);
			var x = (Models.Workstation.Machine)dialog.Combo2.SelectedItem;
			var body = new ChangeMachineAssignItemStatusRequest()
			{
				machine = x.Name,
				product_id = product.IdProducts,
				order_id = product.IdOrders,
				order_name = order.NameOrder
			};
			await ArtPixAPI.ProductAssignProcessing(body);
			Utils.Utils.Notifier.ShowSuccess($"Assigned To Machine {body.machine} Successfully!");
			await UpdateOrderInfoAsync(order.IdOrders);
			ToggleOrderLoadingAnimation(0, order.NameOrder);
		}

		#endregion

		#region MANUAL COMPLETED DIALOG - NOT DONE - NEED TO ADD PRINT QR CHECKBOX - ❎

		private async void OpenManualCompleteDialog(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Product)param).IdOrders);
			var item = order.Products.SingleOrDefault(p => p.MachineAssignItemId == ((Product)param).MachineAssignItemId);
			var dialog = new ManualCompleteDialog();
			var result = await dialog.ShowAsync();
			if (result != ContentDialogResult.Primary) return;
			ToggleOrderLoadingAnimation(1, order.NameOrder);
			var newStatus = new ChangeMachineAssignItemStatusRequest()
			{
				machine_assign_item_id = item.MachineAssignItemId,
				new_status = "success_manually",
				order_id = item.IdOrders,
				order_name = order.NameOrder,
				product_id = item.IdProducts
			};
			await ArtPixAPI.ChangeMachineAssignItemStatusAsync(newStatus);
			await UpdateOrderInfoAsync(order.IdOrders);
			ToggleOrderLoadingAnimation(0, order.NameOrder);
		}

		#endregion

		#region UNASSIGN DIALOG - TESTING - 🔄️

		private async void OpenUnassignDialog(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Product)param).IdOrders);
			var item = order.Products.SingleOrDefault(p => p.MachineAssignItemId == ((Product)param).MachineAssignItemId);
			var machines = await ArtPixAPI.GetMachines(item.IdProducts);
			var dialog = new UnassignDialog(machines);
			var result = await dialog.ShowAsync();
			if (result != ContentDialogResult.Primary) return;
			ToggleOrderLoadingAnimation(1, order.NameOrder);
			var body = new ChangeMachineAssignItemStatusRequest()
			{
				order_id = item.IdOrders,
				order_name = order.NameOrder,
				product_id = item.IdProducts
			};
			await ArtPixAPI.UnassignMachineAssignItemAsync(body);
			if (!string.IsNullOrEmpty(dialog.Combo2.Text))
			{
				var x = (Models.Workstation.Machine)dialog.Combo2.SelectedItem;
				var body2 = new ChangeMachineAssignItemStatusRequest()
				{
					machine = x.Name,
					product_id = item.IdProducts,
					order_id = item.IdOrders,
					order_name = order.NameOrder
				};
				await ArtPixAPI.ProductAssignProcessing(body2);
				Utils.Utils.Notifier.ShowSuccess($"Assigned To Machine {body2.machine} Successfully!");
			}
			await UpdateOrderInfoAsync(order.IdOrders);
			ToggleOrderLoadingAnimation(0, order.NameOrder);

		}

		#endregion

		#region ANIMATION CONTROL - DONE - ✅

		private void ToggleMainLoadingAnimation(int kind)
		{
			if (kind == 0)
			{
				Animation.FadeOut(View.ProgressRingImage);
				Animation.FadeIn(View.ShippingItemsListView);
				return;
			}

			if (kind == 1)
			{
				Animation.FadeOut(View.ShippingItemsListView);
				Animation.FadeIn(View.ProgressRingImage);
			}
		}

		private void ToggleOrderLoadingAnimation(int kind, string nameOrder)
		{
			var ch = Utils.Utils.GetChildren(View.ShippingItemsListView);
			
			foreach (var element in ch)
			{
				if (element.Tag == null) continue;
				if (String.IsNullOrEmpty(element.Tag.ToString())) continue;
				if (element.Tag == nameOrder)
				{
					if (element.GetType().ToString() == "System.Windows.Controls.Expander")
					{
						orderExpander = (FrameworkElement)element;
						Debug.WriteLine(element.Name);
					}
					if (element.GetType().ToString() == "System.Windows.Controls.Image")
					{
						progressRingImage = (FrameworkElement)element;
						Debug.WriteLine(element.Name);
					}

				}
			}
			if (kind == 0)
			{
				Animation.FadeOut(progressRingImage);
				Animation.FadeIn(orderExpander);
				return;
			}

			if (kind == 1)
			{
				Animation.FadeOut(orderExpander);
				Animation.FadeIn(progressRingImage);
			}
		}

		#endregion

		#region FIND BEST SERVICE COMMAND - WORK IN PROGRESS - 🔄️

		private async void FindBestServiceButtonOnClick(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == (int)param);
			if (order != null)
			{
				order.IsShippingInformationLoading = true;
				var res = await ArtPixAPI.FindBestServiceAsync(new FindBestServiceRequest()
					{order_id = param.ToString()});
				order.ShippingOrderInfo = (await ArtPixAPI.GetOrder(param.ToString())).ShippingOrderInfo;
				order.IsShippingInformationLoading = false;
				order.IsShippingServiceFound = "Shipping Service Found";
				order.IsShippingServiceFoundColor = "DarkGreen";
				order.ShippingInformationPanelVisibility = Visibility.Visible;
			}
		}

		#endregion

		#region PRODUCT HISTORY DIALOG - DONE - ✅

		private async void OpenProductHistoryDialog(object param)
		{
			var dialog = new ProductHistoryDialog((int)param);
			await dialog.ShowAsync();
		}

		#endregion

		#region PRODUCT IMAGE PREVIEW DIALOG - DONE - ✅

		public async void OpenImage(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Product)param).IdOrders);
			var product = order?.Products.SingleOrDefault(p => p.MachineAssignItemId == ((Product) param).MachineAssignItemId);
			if (!Utils.Utils.IsCrystal(product)) return;
			if (product == null) return;
			var dialog = new PhotoPreviewDialog(product);
			var result = await dialog.ShowAsync();
		}
		

		#endregion
	}
}
