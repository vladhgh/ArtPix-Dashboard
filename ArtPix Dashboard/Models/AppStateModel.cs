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
			get { return _top; }
			set { SetProperty(ref _top, value); }
		}
		private double _left;
		public double Left
		{
			get { return _left; }
			set { SetProperty(ref _left, value); }
		}
		private double _height;
		public double Height
		{
			get { return _height; }
			set { SetProperty(ref _height, value); }
		}
		private double _width;
		public double Width
		{
			get { return _width; }
			set { SetProperty(ref _width, value); }
		}
		private WindowState _windowState;
		public WindowState WindowState
		{
			get { return _windowState; }
			set { SetProperty(ref _windowState, value); }
		}
		#endregion



		#region PROPS
		private OrderCombineFilterModel _orderFilterGroup = new OrderCombineFilterModel();
		public OrderCombineFilterModel OrderFilterGroup
		{
			get => _orderFilterGroup;
			set => SetProperty(ref _orderFilterGroup, value);
		}

		private string _employeeQR;
		public string EmployeeQR
		{
			get => _employeeQR;
			set => SetProperty(ref _employeeQR, value);
		}

		private string _employeeName;
		public string EmployeeName
		{
			get => _employeeName;
			set => SetProperty(ref _employeeName, value);
		}

		private bool _isLoading = true;
		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
		}
		private bool _tabPressed = true;
		public bool TabPressed
		{
			get => _tabPressed;
			set => SetProperty(ref _tabPressed, value);
		}

		private string _inputString = "";
		public string InputString
		{
			get => _inputString;
			set => SetProperty(ref _inputString, value);
		}

		private Type _lastVisitedViewType;
		public Type LastVisitedViewType
		{
			get { return _lastVisitedViewType; }
			set { SetProperty(ref _lastVisitedViewType, value); }
		}

		private NavigationViewItem _selectedItem;

		public Frame contentFrame;

		public NavigationViewItem SelectedItem
		{
			get => _selectedItem;
			set
			{
				SetProperty(ref _selectedItem, value);
				LastVisitedViewType = typeof(MainView).Assembly.GetType("ArtPix_Dashboard.Views." + (string)value.Tag);
			}
		}

		public bool IsVitroMarkRunning
		{
			get => Process.GetProcessesByName("VitroMark_LE").Length > 0;
		}

		#endregion
	}
}
