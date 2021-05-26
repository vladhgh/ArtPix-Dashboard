using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArtPix_Dashboard.ViewModels;

namespace ArtPix_Dashboard.Views
{
	/// <summary>
	/// Interaction logic for ShippingView.xaml
	/// </summary>
	public partial class ShippingView
	{
		private readonly ShippingViewModel _vm = new ShippingViewModel();
		public ShippingView()
		{
			InitializeComponent();
			DataContext = _vm;
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(searchTextBox.Text))
			{
				_vm.GetOrder(searchTextBox.Text);
			}
		}
	}
}
