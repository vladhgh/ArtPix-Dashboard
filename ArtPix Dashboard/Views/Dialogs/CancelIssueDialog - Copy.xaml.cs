
using System.Drawing;
using System.Windows.Input;
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.ViewModels;
using Image = System.Windows.Controls.Image;

namespace ArtPix_Dashboard.Views.Dialogs
{

    public partial class PhotoPreviewDialog
    {
        public Product Product { get; set; }
        public PhotoPreviewDialog(Product item)
        {
            InitializeComponent();
            DataContext = this;
            this.Product = item;
        }
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
	        this.Hide();
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
	        var img = sender as Image;
	        img.Opacity = 0.8;
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
	        var img = sender as Image;
	        img.Opacity = 1;
        }
    }
}
