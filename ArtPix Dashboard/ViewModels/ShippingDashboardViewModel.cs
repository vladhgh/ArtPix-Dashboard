using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.Models.Types;
using ArtPix_Dashboard.API;
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
using System.Drawing.Imaging;
using System.Threading;

namespace ArtPix_Dashboard.ViewModels
{
	class ShippingDashboardViewModel : PropertyChangedListener
	{

		#region PROPS

		private Product SelectedItem;
		private Models.Order.Datum SelectedOrder;

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

		private void InitializeCommands()
		{
			OnUnassignMachine = new DelegateCommand(OpenUnassignDialog);
			OnManualComplete = new DelegateCommand(OpenManualCompleteDialog);
			OnAssignMachine = new DelegateCommand(OpenAssignMachineDialog);
			OnImageClick = new DelegateCommand(OpenImage);
			OnUnassignAllJobs = new DelegateCommand(OpenUnassignAllDialog);
			ReloadList = new DelegateCommand(async param => await GetOrdersList());
			OnOA = new DelegateCommand(Commands.OpenOrderOnOA);
			OnCP = new DelegateCommand(Commands.OpenOrderOnCP);
			OnPrintQR = new DelegateCommand(PrintQR);
			OnRedo = new DelegateCommand(OpenRedoDialog);
			OnRework = new DelegateCommand(OpenReworkDialog);
			CopyToClipboard = new DelegateCommand(param => Commands.CopyTextToClipboard(param.ToString()));
			OnVitromark = new DelegateCommand(param => Commands.OpenFileInVitroMark(param.ToString()));
			OnProductHistory = new DelegateCommand(OpenProductHistoryDialog);
			OnFindBestService = new DelegateCommand(FindBestServiceButtonOnClick);
			OnReEngrave = new DelegateCommand(OpenReEngraveDialog);
		}

		private async void OpenUnassignAllDialog(object param)
		{
			var dialog = new UnassignAllDialog();
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				ToggleMainProgressRingVisibility();
				await ArtPixAPI.RemoveCurrentJobsFromMachineAsync((int)param);
				await GetOrdersList(1, 15, true, AppState.OrderFilterGroup);
				ToggleMainProgressRingVisibility();
			}
		}

		private void ToggleMainProgressRingVisibility()
		{
			if (ProgressRingVisibility == Visibility.Collapsed || ProgressRingVisibility == Visibility.Hidden)
			{
				IsLoaded = Visibility.Hidden;
				ProgressRingVisibility = Visibility.Visible;
			} else
			{
				IsLoaded = Visibility.Visible;
				ProgressRingVisibility = Visibility.Collapsed;
			}
		}

		private async void OpenRedoDialog(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Models.Order.Product)param).IdOrders);
			var product = order.Products.SingleOrDefault(p => p.MachineAssignItemId == ((Models.Order.Product)param).MachineAssignItemId);
			if (product != null)
			{
				var machines = await ArtPixAPI.GetMachines(product.IdProducts);
				var dialog = new RedoDialog(machines.Data);
				var result = await dialog.ShowAsync();
				if (result != ContentDialogResult.Primary) return;

				ToggleOrderProgressRing(order.IdOrders);
				var requestBody = new ResolveErrorRequestModel
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
					var x = (Models.Machine.Machine)dialog.Combo2.SelectedItem;
					var body = new AssignProcessingModel
					{
						machine = x.Name,
						product_id = product.IdProducts,
						order_id = product.IdOrders,
						order_name = order.NameOrder
					};
					await ArtPixAPI.ProductAssignProcessing(body);
					Utils.Utils.Notifier.ShowSuccess("Assigned To Machine Successfully!");
				}
				if (System.IO.Directory.Exists($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{product.IdProducts}"))
				{
					System.IO.Directory.Delete($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{product.IdProducts}", true);
					Utils.Utils.Notifier.ShowSuccess($"Local Files Removed Successfully For Product {product.IdProducts}!");
				}
				await UpdateOrderInfoAsync(product.IdOrders);
				ToggleOrderProgressRing(order.IdOrders);
			}

		}

		private async void OpenReworkDialog(object param)
		{
			//var dialog = new ReworkDialog();
			//var result = await dialog.ShowAsync();
			//switch (result)
			//{
			//	case ContentDialogResult.Primary:
			//		{
			//			IsLoading = true;
			//			var item = Issues.Data.SingleOrDefault(p => p.MachineAssignItemId == (int)param);
			//			var requestBody = new ResolveErrorRequestModel
			//			{
			//				machine_assign_error_id = item.Id,
			//				machine_assign_item_id = item.MachineAssignItemId,
			//				id_products = item.ProductId,
			//				machine_id = item.MachineId,
			//				user = "Supervisor",
			//				issue_type = "Retoucher Error",
			//				status_error = "retouch",
			//				message = dialog.MessageBox.Text
			//			};
			//			await ArtPixAPI.ResolveProductionIssueAsync(requestBody);
			//			Utils.Notifier.ShowInformation(item.ProductId + " Product Sent To Retoucher Rework Successfully!");
			//			IsLoading = false;
			//			if (System.IO.Directory.Exists($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{item.ProductId}"))
			//			{
			//				System.IO.Directory.Delete($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{item.ProductId}", true);
			//				Utils.Notifier.ShowSuccess($"Local Files Removed Successfully For Product {item.ProductId}!");
			//			}
			//			break;
			//		}
			//	case ContentDialogResult.Secondary:
			//		{
			//			IsLoading = true;
			//			var item = Issues.Data.SingleOrDefault(p => p.MachineAssignItemId == (int)param);
			//			var requestBody = new ResolveErrorRequestModel
			//			{
			//				machine_assign_error_id = item.Id,
			//				machine_assign_item_id = item.MachineAssignItemId,
			//				id_products = item.ProductId,
			//				machine_id = item.MachineId,
			//				user = "Supervisor",
			//				issue_type = "Retoucher Error",
			//				status_error = "looxis",
			//				message = dialog.MessageBox.Text

			//			};
			//			await ArtPixAPI.ResolveProductionIssueAsync(requestBody);
			//			Utils.Notifier.ShowInformation(item.ProductId + " Product Sent To Looxis Rework Successfully!");
			//			IsLoading = false;
			//			if (System.IO.Directory.Exists($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{item.ProductId}"))
			//			{
			//				System.IO.Directory.Delete($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{item.ProductId}", true);
			//				Utils.Notifier.ShowSuccess($"Local Files Removed Successfully For Product {item.ProductId}!");
			//			}
			//			break;
			//		}
			//	case ContentDialogResult.None:
			//		break;
			//	default:
			//		throw new ArgumentOutOfRangeException();
			//}
		}

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
			qr = ResizeImage(qr, 95, 95);
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

		public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
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
				ToggleOrderProgressRing(order.IdOrders);
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
				} else
				{
					var body = new AssignProcessingModel
					{
						user = "Supervisor",
						machine_assign_item_id = product.MachineAssignItemId
					};
					await ArtPixAPI.ProductReEngrave(body);
					Utils.Utils.Notifier.ShowSuccess($"Product re-engrave success!");
				}
				await UpdateOrderInfoAsync(order.IdOrders);
				ToggleOrderProgressRing(order.IdOrders);
			}
		}

		private async Task UpdateOrderInfoAsync(int orderId)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == orderId);
			var updatedOrder = await ArtPixAPI.GetOrder(orderId.ToString());
			order.Status = updatedOrder.Status;
			order.StatusOrderColor = Utils.Utils.SelectStatusColor(order.Status);
			order.UpdatedAt = updatedOrder.UpdatedAt;
			for (var i = 0; i < order.Products.Count; i++)
			{
				order.Products[i].Status = updatedOrder.Products[i].Status;
				order.Products[i].ManualCompleteButtonVisibility =
					updatedOrder.Products[i].ManualCompleteButtonVisibility;
				order.Products[i].UpdatedAt = updatedOrder.Products[i].UpdatedAt;
				order.Products[i].StatusColor = Utils.Utils.SelectStatusColor(order.Products[i].Status);
			}
		}

		private void ToggleOrderProgressRing(int orderId)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == orderId);
			if (order.IsOrderLoading)
			{
				order.ExpanderVisibility = Visibility.Visible;
				order.IsOrderLoading = false;
			} else
			{
				order.ExpanderVisibility = Visibility.Hidden;
				order.IsOrderLoading = true;
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
				order.ExpanderVisibility = Visibility.Hidden;
				order.IsOrderLoading = true;
				var newStatus = new NewStatusModel
				{
					machine_assign_item_id = item.MachineAssignItemId,
					new_status = "success_manually",
					order_id = item.IdOrders,
					order_name = order.NameOrder,
					product_id = item.IdProducts
				};
				await ArtPixAPI.ChangeMachineAssignItemStatusAsync(newStatus);
				var updatedOrder = await ArtPixAPI.GetOrder(((Product)param).IdOrders.ToString());
				//order = updatedOrder;
				order.Status = updatedOrder.Status;
				order.StatusOrderColor = Utils.Utils.SelectStatusColor(order.Status);
				order.UpdatedAt = updatedOrder.UpdatedAt;
				for (var i = 0; i < order.Products.Count; i++)
				{
					order.Products[i].Status = updatedOrder.Products[i].Status;
					order.Products[i].ManualCompleteButtonVisibility =
						updatedOrder.Products[i].ManualCompleteButtonVisibility;
					order.Products[i].UpdatedAt = updatedOrder.Products[i].UpdatedAt;
					order.Products[i].StatusColor = Utils.Utils.SelectStatusColor(order.Products[i].Status);

				}
				order.ExpanderVisibility = Visibility.Visible;
				order.IsOrderLoading = false;
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
			var product = order?.Products.SingleOrDefault(p => p.MachineAssignItemId == ((Product) param).MachineAssignItemId);
			if (product != null)
			{
				var dialog = new PhotoPreviewDialog(product);
				var result = await dialog.ShowAsync();
			}
		}

		public void Initialize(AppStateModel appState)
		{
			AppState = appState;
			InitializeCommands();
		}

		public async Task GetOrdersList(int pageNumber = 1, int perPage = 15, bool withPages = true, OrderCombineFilterModel filterGroup = null)
		{
			ToggleMainProgressRingVisibility();
			Orders = await ArtPixAPI.GetOrdersAsync(pageNumber, perPage, filterGroup);
			if (withPages)
			{
				Pages = await GetPages(pageNumber, perPage, filterGroup);
			}
			ToggleMainProgressRingVisibility();
		}
		
		private async Task<ObservableCollection<PageModel>> GetPages(int currentPageNumber, int perPage = 15, OrderCombineFilterModel filterGroup = null)
		{
			var pages = new ObservableCollection<PageModel>();
			await Task.Run(() =>
			{
				for (var i = Orders.Meta.CurrentPage; i <= Orders.Meta.LastPage; i++)
				{
					if (i > Orders.Meta.CurrentPage + 5 && i < Orders.Meta.LastPage)
					{
						var page = new PageModel(i + 5, "...", Orders.Meta.Path + "?page=" + (i + 5))
						{
							IsSelected = i == Orders.Meta.CurrentPage,
							NavigateToSelectedPage = new DelegateCommand(async param => await GetOrdersList((currentPageNumber + 6),
								perPage, true, filterGroup))
						};
						pages.Add(page);
					}
					else
					{
						var page = new PageModel(i, i.ToString(), Orders.Meta.Path + "?page=" + i)
						{
							IsSelected = i == Orders.Meta.CurrentPage,
							NavigateToSelectedPage = new DelegateCommand(async param => await GetOrdersList((int)param, perPage, false, filterGroup))
						};
						pages.Add(page);
					}
				}
				pages = new ObservableCollection<PageModel>(pages.GroupBy(x => x.PageName).Select(g => g.First()).ToList());
			});
			return pages;
		}
	}
}
