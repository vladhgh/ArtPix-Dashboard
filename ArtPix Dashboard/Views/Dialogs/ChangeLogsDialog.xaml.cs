using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.Product;
using ArtPix_Dashboard.Models.ProductHistory;
using ArtPix_Dashboard.Utils;
using ArtPix_Dashboard.ViewModels;
using System;

namespace ArtPix_Dashboard.Views.Dialogs
{
    public partial class ChangeLogsDialog
    {

        public ChangeLogsDialog()
        {
            InitializeComponent();
        }
        
		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            this.Hide();
		}
	}
}
