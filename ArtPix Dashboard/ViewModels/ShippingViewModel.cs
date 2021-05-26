using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.Utils;

namespace ArtPix_Dashboard.ViewModels
{
	public class ShippingViewModel : PropertyChangedListener
	{

		#region COMMANDS

		private ICommand _onImageClick;
		public ICommand OnImageClick
		{
			get => _onImageClick;
			set => SetProperty(ref _onImageClick, value);
		}

		#endregion

		#region PROPS

		private Datum _order = new Datum();
		public Datum Order
		{
			get => _order;
			set => SetProperty(ref _order, value);
		}

		private Visibility _listVisibility;

		public Visibility ListVisibility
		{
			get => _listVisibility;
			set => SetProperty(ref _listVisibility, value);
		}
		private Visibility _startupMessageVisibility;

		public Visibility StartupMessageVisibility
		{
			get => _startupMessageVisibility;
			set => SetProperty(ref _startupMessageVisibility, value);
		}

		#endregion



		public ShippingViewModel()
		{
			ListVisibility = Visibility.Collapsed;
			StartupMessageVisibility = Visibility.Visible;
			OnImageClick = new DelegateCommand(OpenImage);
		}

		public void OpenImage(object param)
		{
			Debug.WriteLine("I work");
			var product = Order.Products.SingleOrDefault(p => p.IdProducts == (int)param);
			if (product == null) return;
			product.IsImageExpanded = !product.IsImageExpanded;
			product.UrlRenderImgSize = product.IsImageExpanded ? 350 : 175;
		}

		public async void GetOrder(string orderName = "", string orderId = "")
		{
			StartupMessageVisibility = Visibility.Hidden;
			ListVisibility = Visibility.Visible;
			Order = await ArtPixAPI.GetOrder("", orderName);
		}
	}
}
