
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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

        private void Toggle2Dto3DPic_OnClick(object sender, RoutedEventArgs e)
        {
	        ToggleOriginalPic.IsChecked = false;
	        ToggleORenderPic.IsChecked = false;
	        ToggleOptimizedPic.IsChecked = false;
	        ToggleOUploadPic.IsChecked = false;
	        ToggleRenderPic.IsChecked = false;
	        ProductImage.Source = new BitmapImage(new Uri(Product.UrlShapeImg)); 
        }

        private void ToggleOptimizedPic_OnClick(object sender, RoutedEventArgs e)
        {
	        Toggle2Dto3DPic.IsChecked = false;
	        ToggleORenderPic.IsChecked = false;
	        ToggleOriginalPic.IsChecked = false;
	        ToggleOUploadPic.IsChecked = false;
	        ToggleRenderPic.IsChecked = false;
	        ProductImage.Source = new BitmapImage(new Uri(Product.UrlOptimizeImg));
        }

        private void ToggleOriginalPic_OnClick(object sender, RoutedEventArgs e)
        {
	        ToggleOptimizedPic.IsChecked = false;
	        ToggleORenderPic.IsChecked = false;
	        Toggle2Dto3DPic.IsChecked = false;
	        ToggleOUploadPic.IsChecked = false;
	        ToggleRenderPic.IsChecked = false;
	        ProductImage.Source = new BitmapImage(new Uri(Product.UrlOriginalImg));
        }

        private void ToggleRenderPic_OnClick(object sender, RoutedEventArgs e)
        {
	        ToggleOptimizedPic.IsChecked = false;
	        ToggleORenderPic.IsChecked = false;
	        Toggle2Dto3DPic.IsChecked = false;
	        ToggleOUploadPic.IsChecked = false;
	        ToggleOriginalPic.IsChecked = false;
	        ProductImage.Source = new BitmapImage(new Uri(Product.UrlRenderImg));
        }

        private void ToggleOUploadPic_OnClick(object sender, RoutedEventArgs e)
        {
			ToggleOptimizedPic.IsChecked = false;
			ToggleORenderPic.IsChecked = false;
			Toggle2Dto3DPic.IsChecked = false;
			ToggleOriginalPic.IsChecked = false;
			ToggleRenderPic.IsChecked = false;
			ProductImage.Source = new BitmapImage(new Uri(Product.UrlOriginalOriginal));
		}

        private void ToggleORenderPic_OnClick(object sender, RoutedEventArgs e)
        {
			ToggleOptimizedPic.IsChecked = false;
			ToggleOriginalPic.IsChecked = false;
			Toggle2Dto3DPic.IsChecked = false;
			ToggleOUploadPic.IsChecked = false;
			ToggleRenderPic.IsChecked = false;
			ProductImage.Source = new BitmapImage(new Uri(Product.UrlOriginalRender));
		}
    }
}
