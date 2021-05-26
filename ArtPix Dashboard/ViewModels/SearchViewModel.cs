using ArtPix_Dashboard.Models.Order;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtPix_Dashboard.Utils;

namespace ArtPix_Dashboard.ViewModels
{
	public class SearchViewModel : PropertyChangedListener
	{
		private Datum _order = new Datum();
		public Datum Order
		{
			get => _order;
			set => SetProperty(ref _order, value);
		}
		private ObservableCollection<Product> _products = new ObservableCollection<Product>();
		public ObservableCollection<Product> Products
		{
			get => _products;
			set => SetProperty(ref _products, value);
		}

		public async void Initialize(string id)
		{
			var res = await ArtPixAPI.GetOrder(id);
			if (res != null)
			{
				foreach (var i in res.Products)
				{
					Products.Add(i);
				}
			}
		}
	}
}
