using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.Product;
using ArtPix_Dashboard.Models.ProductHistory;
using ArtPix_Dashboard.Utils;
using ArtPix_Dashboard.ViewModels;
using System;

namespace ArtPix_Dashboard.Views.Dialogs
{
    public partial class ProductHistoryDialog
    {
        private ProductHistoryViewModel _vm = new ProductHistoryViewModel();
        private class ProductHistoryViewModel : PropertyChangedListener
		{
            private ProductHistoryModel _productHistory = new ProductHistoryModel();
            public ProductHistoryModel ProductHistory
			{
                get => _productHistory;
                set => SetProperty(ref _productHistory, value);
			}

			public async void GetProductHistory(int productId)
			{
                ProductHistory = await ArtPixAPI.GetProductHistoryAsync(productId);
            }
		}

        public ProductHistoryDialog(int productId)
        {
            InitializeComponent();
            DataContext = _vm;
            _vm.GetProductHistory(productId);
        }
        
		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            this.Hide();
		}
	}
}
