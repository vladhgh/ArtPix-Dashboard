using ArtPix_Dashboard.Models.Order;
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

namespace ArtPix_Dashboard.ViewModels
{
	class ShippingDashboardViewModel : PropertyChangedListener
	{

		#region PROPS
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
		private ICommand _onImageClick;
		public ICommand OnImageClick
		{
			get => _onImageClick;
			set => SetProperty(ref _onImageClick, value);
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


		#endregion

		private void InitializeCommands()
		{
			OnImageClick = new DelegateCommand(OpenImage);
			ReloadList = new DelegateCommand(param => GetOrdersList());
			OnOA = new DelegateCommand(Commands.OpenOrderOnOA);
			OnCP = new DelegateCommand(Commands.OpenOrderOnCP);
			OnPrintQR = new DelegateCommand(PrintQR);
			CopyToClipboard = new DelegateCommand(param => Commands.CopyTextToClipboard(param.ToString()));
			OnVitromark = new DelegateCommand(param => Commands.OpenFileInVitroMark(param.ToString()));
		}

		public void OpenImage(object param)
		{
			var order = Orders.Data.SingleOrDefault(p => p.IdOrders == ((Product)param).IdOrders);
			var product = order?.Products.SingleOrDefault(p => p.IdProducts == ((Product) param).IdProducts);
			if (product != null)
			{
				product.IsImageExpanded = !product.IsImageExpanded;
				product.UrlRenderImgSize = product.IsImageExpanded ? 350 : 175;
			}
		}
		public void PrintQR(object obj)
		{
			var item = (List<MachineAssignItem>) obj;

			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode($"\t{item[0].OrderId}-{item[0].ProductId}-{item[0].Id}\n", QRCodeGenerator.ECCLevel.Q);
			QRCode qrCode = new QRCode(qrCodeData);
			var img = qrCode.GetGraphic(20);
			Bitmap bmp = new Bitmap(300, 125);

			RectangleF rectQR = new RectangleF(0, 0, 125, 125);
			RectangleF rectText = new RectangleF(130, 60, 170, 25);

			Graphics g = Graphics.FromImage(bmp);
			g.Clear(Color.White);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			g.DrawImage(img, rectQR);
			g.DrawString(item[0].OrderName, new Font("Consolas", 12), Brushes.Black, rectText);

			g.Flush();

			QrCodeBitmap = bmp;
			QrCode = BitmapToImageSource(bmp);

			PrintDocument pd = new PrintDocument();
			pd.OriginAtMargins = true;
			pd.DefaultPageSettings.Landscape = true;
			pd.PrintPage += pd_PrintPage;
			pd.Print();


		}

		void pd_PrintPage(object sender, PrintPageEventArgs e)
		{
			e.Graphics.DrawImage(QrCodeBitmap, -100, -100);
		}

		BitmapImage BitmapToImageSource(Bitmap bitmap)
		{
			using (MemoryStream memory = new MemoryStream())
			{
				bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
				memory.Position = 0;
				BitmapImage bitmapimage = new BitmapImage();
				bitmapimage.BeginInit();
				bitmapimage.StreamSource = memory;
				bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapimage.EndInit();
				return bitmapimage;
			}
		}

		public ShippingDashboardViewModel()
		{
			InitializeCommands();
		}

		public async void GetOrdersList(int pageNumber = 1, bool withPages = true, string perPage = "15",
			string hasShippingPackage = "", string withShippingTotes = "", string withProductionIssue = "",
			string sortBy = "", string shipByToday = "True", string storeName = "", string shippingStatus = "waiting",
			string orderStatus = "processing", string statusEngraving = "", string nameOrder = "")
		{
			IsLoading = true;
			IsLoaded = Visibility.Collapsed;
			Orders = await ArtPixAPI.GetOrdersAsync(orderStatus, shippingStatus, pageNumber.ToString(), perPage, hasShippingPackage, withShippingTotes, withProductionIssue, sortBy, shipByToday, storeName, statusEngraving, nameOrder);
			foreach (var page in Pages)
			{
				page.IsSelected = page.PageNumber == pageNumber;
			}
			Pages = withPages ? GetPages(pageNumber, perPage, hasShippingPackage, withShippingTotes, withProductionIssue, sortBy, shipByToday, storeName, shippingStatus, orderStatus, statusEngraving, nameOrder) : new ObservableCollection<PageModel>(Pages);
			IsLoading = false;
			IsLoaded = Visibility.Visible;
			CollectionViewSource.GetDefaultView(Orders.Data).Refresh();
		}
		private ObservableCollection<PageModel> GetPages(int currentPageNumber, string perPage = "15",
			string hasShippingPackage = "", string withShippingTotes = "", string withProductionIssue = "", string sortBy = "", string shipByToday = "True", string storeName = "", string shippingStatus = "waiting",
			string orderStatus = "processing", string statusEngraving = "", string nameOrder = "")
		{
			var pages = new ObservableCollection<PageModel>();
			for (var i = Orders.Meta.CurrentPage; i <= Orders.Meta.LastPage; i++)
			{
				if (i > Orders.Meta.CurrentPage + 5 && i < Orders.Meta.LastPage)
				{
					var page = new PageModel(i + 5, "...", Orders.Meta.Path + "?page=" + (i + 5))
					{
						IsSelected = i == Orders.Meta.CurrentPage,
						NavigateToSelectedPage = new DelegateCommand(param => GetOrdersList((currentPageNumber + 5),
							true, perPage, hasShippingPackage, withShippingTotes, withProductionIssue, sortBy,
							shipByToday, storeName, shippingStatus, orderStatus, statusEngraving, nameOrder))
					};
					pages.Add(page);
				}
				else
				{
					var page = new PageModel(i, i.ToString(), Orders.Meta.Path + "?page=" + i)
					{
						IsSelected = i == Orders.Meta.CurrentPage,
						NavigateToSelectedPage = new DelegateCommand(param => GetOrdersList((int) param, false, perPage,
							hasShippingPackage, withShippingTotes, withProductionIssue, sortBy, shipByToday, storeName, shippingStatus, orderStatus, statusEngraving, nameOrder))
					};
					pages.Add(page);
				}
			}
			pages = new ObservableCollection<PageModel>(pages.GroupBy(x => x.PageName).Select(g => g.First()).ToList());
			return pages;
		}
	}
}
