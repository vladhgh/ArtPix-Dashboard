using System.Collections.ObjectModel;
using ArtPix_Dashboard.Models.Types;
using ArtPix_Dashboard.Models.Product;
using ArtPix_Dashboard.Utils;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;
using System;
using System.ComponentModel;
using ArtPix_Dashboard.Views.Dialogs;
using ModernWpf.Controls;
using ArtPix_Dashboard.Models;
using System.Windows;
using System.Threading.Tasks;
using RestSharp;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using ToastNotifications.Messages;
using static ArtPix_Dashboard.Utils.Utils;
using System.Drawing.Printing;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;
using QRCoder;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ArtPix_Dashboard.ViewModels
{
	public class ProductionIssuesViewModel : PropertyChangedListener
	{

		#region PROPS

		private bool _isLoading;
		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
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
		private ProductionIssueModel _issues = new ProductionIssueModel();
		public ProductionIssueModel Issues
		{
			get => _issues;
			set => SetProperty(ref _issues, value);

		}
		private IssueReasonsModel _allIssueReasons = new IssueReasonsModel();
		public IssueReasonsModel AllIssueReasons
		{
			get => _allIssueReasons;
			set 
			{
				SetProperty(ref _allIssueReasons, value);
				var all = new Models.Types.Datum { Reason = "All Reasons", Id = 0, Stage = "All Stages", Type = "All Types" };
				IssueReasons = new ObservableCollection<Models.Types.Datum>(_allIssueReasons.Data.GroupBy(x => x.Reason).Select(p => p.First()).ToList());
				IssueReasons.Insert(0, all);
				IssueStages = new ObservableCollection<Models.Types.Datum>(_allIssueReasons.Data.GroupBy(x => x.Stage).Select(p => p.First()).ToList());
				IssueStages.Insert(0, all);
				IssueTypes = new ObservableCollection<Models.Types.Datum>(_allIssueReasons.Data.GroupBy(x => x.Type).Select(p => p.First()).ToList());
				IssueTypes.Insert(0, all);
			}
		}
		private ObservableCollection<Models.Types.Datum> _issueStages = new ObservableCollection<Models.Types.Datum>();
		public ObservableCollection<Models.Types.Datum> IssueStages
		{
			get => _issueStages;
			set => SetProperty(ref _issueStages, value);
		}
		private ObservableCollection<Models.Types.Datum> _issueTypes = new ObservableCollection<Models.Types.Datum>();
		public ObservableCollection<Models.Types.Datum> IssueTypes
		{
			get => _issueTypes;
			set => SetProperty(ref _issueTypes, value);
		}
		private ObservableCollection<Models.Types.Datum> _issueReasons = new ObservableCollection<Models.Types.Datum>();
		public ObservableCollection<Models.Types.Datum> IssueReasons
		{
			get => _issueReasons;
			set => SetProperty(ref _issueReasons, value);
		}
		private AppStateModel _appState = new AppStateModel();
		public AppStateModel AppState
		{
			get => _appState;
			set => SetProperty(ref _appState, value);
		}
		#endregion

		#region COMMANDS
		private ICommand _onImageClick;
		public ICommand OnImageClick
		{
			get => _onImageClick;
			set => SetProperty(ref _onImageClick, value);
		}
		private ICommand _onManualComplete;
		public ICommand OnManualComplete
		{
			get => _onManualComplete;
			set => SetProperty(ref _onManualComplete, value);
		}
		private ICommand _reloadList;
		public ICommand ReloadList
		{
			get => _reloadList;
			set => SetProperty(ref _reloadList, value);
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
		

		#endregion

		public Models.Product.Datum SelectedItem;

		private void InitializeCommands()
		{
			OnImageClick = new DelegateCommand(OpenImage);
			ReloadList = new DelegateCommand( param => GetIssuesList());
			OnOA = new DelegateCommand(Commands.OpenOrderOnOA);
			OnCP = new DelegateCommand(Commands.OpenOrderOnCP);
			OnRedo = new DelegateCommand(OpenRedoDialog);
			OnRework = new DelegateCommand(OpenReworkDialog);
			OnCancelIssue = new DelegateCommand(OpenCancelIssueDialog);
			CopyToClipboard = new DelegateCommand(param => Commands.CopyTextToClipboard(param.ToString()));
			OnManualComplete = new DelegateCommand(OpenManualCompleteDialog);
			OnVitromark = new DelegateCommand(param => Commands.OpenFileInVitroMark(param.ToString()));
		}

		private void OpenImage(object param)
		{
			var item = Issues.Data.SingleOrDefault(p => p.Id == (int)param);
			if (item != null)
			{
				item.Product.IsImageExpanded = !item.Product.IsImageExpanded;
				item.Product.UrlRenderImgSize = item.Product.IsImageExpanded ? 450 : 175;
			}
		}

		public async void GetIssuesList(int pageNumber = 1, bool withPages = true, string orderId = "All")
		{
			IsLoaded = Visibility.Hidden;
			IsLoading = true;
			Issues = await ArtPixAPI.GetProductionIssuesAsync(pageNumber.ToString(), "15", orderId);
			foreach (var page in Pages)
			{
				page.IsSelected = page.PageNumber == pageNumber;
			}
			Pages = withPages ? GetPages(pageNumber) : new ObservableCollection<PageModel>(Pages);
			IsLoading = false;
			IsLoaded = Visibility.Visible;
		}

		public async Task Initialize(AppStateModel appState)
		{
			AppState = appState;
			AllIssueReasons = await ArtPixAPI.GetIssueReasonsAsync();
			GetIssuesList();
			InitializeCommands();
		}

		private ObservableCollection<PageModel> GetPages(int currentPageNumber)
		{
			var pages = new ObservableCollection<PageModel>();
			for (var i = Issues.Meta.CurrentPage; i <= Issues.Meta.LastPage; i++)
			{

				if (i > Issues.Meta.CurrentPage + 5 && i < Issues.Meta.LastPage)
				{
					var page = new PageModel(i + 5, "...", Issues.Meta.Path + "?page=" + (i + 5))
					{
						IsSelected = i == Issues.Meta.CurrentPage,
						NavigateToSelectedPage =
							new DelegateCommand(param => GetIssuesList((currentPageNumber + 5), true))
					};
					pages.Add(page);
				}
				else
				{
					var page = new PageModel(i, i.ToString(), Issues.Meta.Path + "?page=" + i.ToString())
					{
						IsSelected = i == Issues.Meta.CurrentPage,
						NavigateToSelectedPage = new DelegateCommand(param => GetIssuesList((int) param, false))
					};
					pages.Add(page);
				}
			}
			pages = new ObservableCollection<PageModel>(pages.GroupBy(x => x.PageName).Select(g => g.First()).ToList());
			return pages;
		}

		private async void OpenManualCompleteDialog(object param)
		{
			var item = Issues.Data.SingleOrDefault(p => p.Id == (int)param);
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
					order_id = item.Order.Id,
					order_name = item.Order.Name,
					product_id = item.ProductId
				};
				await ArtPixAPI.ChangeMachineAssignItemStatusAsync(newStatus);
				GetIssuesList(Issues.Meta.CurrentPage, false);
			}
		}

		private async void OpenRedoDialog(object param)
		{
			var item = Issues.Data.SingleOrDefault(p => p.MachineAssignItemId == (int)param);
			if (item != null)
			{
				var machines = await ArtPixAPI.GetMachines(item.ProductId);
				var dialog = new RedoDialog(machines.Data);
				var result = await dialog.ShowAsync();
				if (result != ContentDialogResult.Primary) return;
				IsLoaded = Visibility.Hidden;
				IsLoading = true;
				var requestBody = new ResolveErrorRequestModel {
					machine_assign_error_id = item.Id,
					machine_assign_item_id = item.MachineAssignItemId,
					id_products = item.ProductId,
					machine_id = item.MachineId,
					user = "Supervisor",
					issue_type = dialog.Combo1.SelectedValue.ToString(),
					status_error = "redo",
					message = "Testing"
				};
				await ArtPixAPI.ResolveProductionIssueAsync(requestBody);
				Notifier.ShowSuccess("Issue Resolved Successfully!");
				if (!string.IsNullOrEmpty(dialog.Combo2.Text))
				{
					var x = (Models.Machine.Machine)dialog.Combo2.SelectedItem;
					var body = new AssignProcessingModel
					{
						machine = x.Name,
						product_id = item.ProductId,
						order_id = item.Order.Id,
						order_name = item.Order.Name
					};
					await ArtPixAPI.ProductAssignProcessing(body);
					Notifier.ShowSuccess("Assigned To Machine Successfully!");
				}
				if (System.IO.Directory.Exists($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{item.ProductId}"))
				{
					System.IO.Directory.Delete($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{item.ProductId}", true);
					Notifier.ShowSuccess($"Local Files Removed Successfully For Product {item.ProductId}!");
				}
				GetIssuesList(Issues.Meta.CurrentPage, false);
			}
			IsLoading = false;
			IsLoaded = Visibility.Visible;
		}
		private async void OpenReworkDialog(object param)
		{
			var dialog = new ReworkDialog();
			var result = await dialog.ShowAsync();
			switch (result)
			{
				case ContentDialogResult.Primary:
				{
					IsLoading = true;
					var item = Issues.Data.SingleOrDefault(p => p.MachineAssignItemId == (int)param);
					var requestBody = new ResolveErrorRequestModel{
						machine_assign_error_id = item.Id,
						machine_assign_item_id = item.MachineAssignItemId,
						id_products = item.ProductId,
						machine_id = item.MachineId,
						user = "Supervisor",
						issue_type = "Retoucher Error",
						status_error = "retouch",
						message = dialog.MessageBox.Text
					};
					await ArtPixAPI.ResolveProductionIssueAsync(requestBody);
					GetIssuesList(Issues.Meta.CurrentPage, false);
					Notifier.ShowInformation(item.ProductId + " Product Sent To Retoucher Rework Successfully!");
					IsLoading = false;
					if (System.IO.Directory.Exists($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{item.ProductId}"))
					{
						System.IO.Directory.Delete($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{item.ProductId}", true);
						Notifier.ShowSuccess($"Local Files Removed Successfully For Product {item.ProductId}!");
					}
					break;
				}
				case ContentDialogResult.Secondary:
				{
					IsLoading = true;
					var item = Issues.Data.SingleOrDefault(p => p.MachineAssignItemId == (int)param);
					var requestBody = new ResolveErrorRequestModel{
						machine_assign_error_id = item.Id,
						machine_assign_item_id = item.MachineAssignItemId,
						id_products = item.ProductId,
						machine_id = item.MachineId,
						user = "Supervisor",
						issue_type = "Retoucher Error",
						status_error = "looxis",
						message = dialog.MessageBox.Text

					};
					await ArtPixAPI.ResolveProductionIssueAsync(requestBody);
					GetIssuesList(Issues.Meta.CurrentPage, false);
					Notifier.ShowInformation(item.ProductId + " Product Sent To Looxis Rework Successfully!");
					IsLoading = false;
					if (System.IO.Directory.Exists($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{item.ProductId}"))
					{
						System.IO.Directory.Delete($"\\\\artpix\\MAIN-JOBS-STORAGE\\Orders\\{item.ProductId}", true);
						Notifier.ShowSuccess($"Local Files Removed Successfully For Product {item.ProductId}!");
					}
					break;
				}
				case ContentDialogResult.None:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private async void OpenCancelIssueDialog(object param)
		{
			var item = Issues.Data.SingleOrDefault(p => p.MachineAssignItemId == (int)param);
			SelectedItem = item;
			var dialog = new CancelIssueDialog(item);
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				IsLoading = true;
				var requestBody = new ResolveErrorRequestModel{
					machine_assign_error_id = item.Id,
					user = "Supervisor",
					message = "Life Is Good!"
				};
				if ((bool)dialog.PrintShippingLabel.IsChecked)
				{
					PrintDocument pd = new PrintDocument();
					pd.PrinterSettings.DefaultPageSettings.PaperSize = new PaperSize("QR",62, 29);
					pd.OriginAtMargins = false;
					pd.PrintPage += PrintPage;
					pd.Print();
				}
				
				await ArtPixAPI.CancelProductionIssueAsync(requestBody);
				GetIssuesList(Issues.Meta.CurrentPage, false);
				Notifier.ShowSuccess(item.ProductId + " Issue Canceled Successfully!");
				IsLoading = false;
			}
		}
		public void PrintPage(object o, PrintPageEventArgs e)
		{
			var item = SelectedItem;
			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode("\t" + $"{item.Order.Id}-{item.ProductId}-{item.MachineAssignItemId}" + "\n", QRCodeGenerator.ECCLevel.Q);
			QRCode qrCode = new QRCode(qrCodeData);
			var qr = qrCode.GetGraphic(20);
			qr = ResizeImage(qr, 95, 95);
			var img = (System.Drawing.Image)qr;
			System.Drawing.Point loc = new System.Drawing.Point(0, 0);
			Rectangle rect = new Rectangle(2, 2, 225, 88);
			e.Graphics.DrawImage(img, loc);
			Pen blackPen = new Pen(Color.Black, 3);
			//e.Graphics.DrawRectangle(blackPen, rect);
			if (SelectedItem.Order.Name.Length > 6)
			{
				e.Graphics.DrawString($"{SelectedItem.Order.Name}", new Font("Consolas", 9), new SolidBrush(Color.Black), 95, 45);
			}
			else
			{
				e.Graphics.DrawString($"{SelectedItem.Order.Name}", new Font("Consolas", 12), new SolidBrush(Color.Black), 125, 45);
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

	}
}
