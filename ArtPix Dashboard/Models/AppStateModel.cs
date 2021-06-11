using ArtPix_Dashboard.ViewModels;
using ArtPix_Dashboard.Views;
using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace ArtPix_Dashboard.Models
{
	public class AppStateModel : PropertyChangedListener
	{
		#region WINDOW
		private double _top;
		public double Top
		{
			get => _top;
			set => SetProperty(ref _top, value);
		}
		private double _left;
		public double Left
		{
			get => _left;
			set => SetProperty(ref _left, value);
		}
		private double _height;
		public double Height
		{
			get => _height;
			set => SetProperty(ref _height, value);
		}
		private double _width;
		public double Width
		{
			get => _width;
			set => SetProperty(ref _width, value);
		}
		private WindowState _windowState;
		public WindowState WindowState
		{
			get => _windowState;
			set => SetProperty(ref _windowState, value);
		}
		#endregion

		#region PROPS
		private OrderCombineFilterModel _orderFilterGroup = new OrderCombineFilterModel();
		public OrderCombineFilterModel OrderFilterGroup
		{
			get => _orderFilterGroup;
			set => SetProperty(ref _orderFilterGroup, value);
		}
		private string _statusGroup;
		public string StatusGroup
		{
			get => _statusGroup;
			set => SetProperty(ref _statusGroup, value);
		}
		private string _employeeName;
		public string EmployeeName
		{
			get => _employeeName;
			set => SetProperty(ref _employeeName, value);
		}

		private bool _isMainViewLoading = true;
		public bool IsMainViewLoading
		{
			get => _isMainViewLoading;
			set => SetProperty(ref _isMainViewLoading, value);
		}
		private Visibility _mainNavigationViewVisibility;
		public Visibility MainNavigationViewVisibility
		{
			get => _mainNavigationViewVisibility;
			set => SetProperty(ref _mainNavigationViewVisibility, value);
		}
		

		private Type _lastVisitedViewType;
		public Type LastVisitedViewType
		{
			get => _lastVisitedViewType;
			set => SetProperty(ref _lastVisitedViewType, value);
		}

		private NavigationViewItem _selectedItem;

		public NavigationViewItem SelectedItem
		{
			get => _selectedItem;
			set
			{
				SetProperty(ref _selectedItem, value);
				LastVisitedViewType = typeof(MainView).Assembly.GetType("ArtPix_Dashboard.Views." + (string)value.Tag);
			}
		}

		public bool IsVitroMarkRunning => Process.GetProcessesByName("VitroMark_LE").Length > 0;

		#endregion
	}
}
