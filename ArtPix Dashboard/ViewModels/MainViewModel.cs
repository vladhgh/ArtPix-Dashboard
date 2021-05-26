using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.Types;
using ArtPix_Dashboard.Properties;
using ArtPix_Dashboard.Utils;
using ArtPix_Dashboard.Views;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

namespace ArtPix_Dashboard.ViewModels
{
    public class MainViewModel : PropertyChangedListener
    {

		private StatsModel _stats = new StatsModel();
		public StatsModel Stats
		{
			get => _stats;
			set => SetProperty(ref _stats, value);
		}

		private AppStateModel _appState = new AppStateModel();
		public AppStateModel AppState
		{
			get => _appState;
			set => SetProperty(ref _appState, value);
		}

		public async void Initiallize(MainView view)
		{
			AppState.IsLoading = true;
			view.MainNavigationView.Visibility = Visibility.Hidden;
			try
			{
				InitializeSettings();
				Stats = await ArtPixAPI.GetAllStatsAsync();
				InitializeUI(view);
			} catch (Exception e)
			{
				Debug.WriteLine("EXCEPTION UNHANDLED: " + e.Message);
			}
			view.MainNavigationView.Visibility = Visibility.Visible;
			AppState.IsLoading = false;
			var timer = Observable.Interval(TimeSpan.FromSeconds(30));
			timer.Do(x => Debug.WriteLine("!STATS LOADED!")).Subscribe(async tick => Stats = await ArtPixAPI.GetAllStatsAsync());

		}
		private void InitializeSettings()
		{
			AppState.EmployeeName = "Vlad Prokopev";
			AppState.Top = Settings.Default.Top;
			AppState.Left = Settings.Default.Left;
			AppState.Height = Settings.Default.Height;
			AppState.Width = Settings.Default.Width;
			AppState.WindowState = Settings.Default.Maximized ? WindowState.Maximized : WindowState.Normal;
		}
		private void InitializeUI(MainView view)
		{
			AppState.contentFrame = view.contentFrame;
			AppState.contentFrame.ContentTransitions = null;
			view.Window.Closing += Window_Closing;
			//view.PreviewKeyDown += KeyPressEventListener;
			view.MainNavigationView.SelectionChanged += NavigateToSelectedPage;
			AppState.SelectedItem = view.MainNavigationView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(x => x.Tag.ToString() == Settings.Default.LastVisitedViewTag);
		}

		public void KeyPressEventListener(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Tab)
			{
				e.Handled = true;
			}
		}



		private void NavigateToSelectedPage(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			var selected = (NavigationViewItem)args.SelectedItem;
			switch (selected.Tag)
			{
				case "ProductionIssuesView":
					AppState.contentFrame.Navigate(typeof(ProductionIssuesView), AppState, new DrillInNavigationTransitionInfo());
					return;
				case "UtilitiesView":
					AppState.contentFrame.Navigate(typeof(UtilitiesView), AppState, new DrillInNavigationTransitionInfo());
					return;
				case "ShippingDashboardView":
					AppState.contentFrame.Navigate(typeof(ShippingDashboardView), null, new DrillInNavigationTransitionInfo());
					return;
				case "ShippingView":
					AppState.contentFrame.Navigate(typeof(ShippingView), null, new DrillInNavigationTransitionInfo());
					return;
				case "MachinesDashboardView":
					AppState.contentFrame.Navigate(typeof(MachinesDashboardView), null, new DrillInNavigationTransitionInfo());
					return;
			}
		}
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Settings.Default.Top = AppState.Top;
			Settings.Default.Left = AppState.Left;
			Settings.Default.Height = AppState.Height;
			Settings.Default.Width = AppState.Width;
			Settings.Default.Maximized = AppState.WindowState == WindowState.Maximized;
			Settings.Default.LastVisitedViewTag = AppState.SelectedItem.Tag.ToString();
			Settings.Default.Save();
		}
    }
}
