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

	public partial class UtilitiesView
	{
		private readonly UtilitiesViewModel _vm = new UtilitiesViewModel();
		public UtilitiesView()
		{
			InitializeComponent();
			DataContext = _vm;
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
