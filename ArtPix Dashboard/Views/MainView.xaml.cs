using System;
using System.Diagnostics;
using ArtPix_Dashboard.ViewModels;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace ArtPix_Dashboard.Views
{
	public partial class MainView : Window
	{
		private readonly MainViewModel _vm = new MainViewModel();


		public MainView()
		{
			DataContext = _vm;
			InitializeComponent();
			_vm.Initialize(this);
		}

		private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			contentFrame.Navigate(typeof(SearchView), args.QueryText, new DrillInNavigationTransitionInfo());
		}
		private void MainNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			if (contentFrame.CanGoBack)
			{
				contentFrame.GoBack();
			}
		}
	}
}
