using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ArtPix_Dashboard.Utils.Helpers;

namespace ArtPix_Dashboard.Models.AppState
{
	public class CurrentSessionModel : PropertyChangedListener
	{
		private Visibility _mainViewProgressRingVisibility = Visibility.Visible;

		public Visibility MainViewProgressRingVisibility
		{
			get => _mainViewProgressRingVisibility;
			set => SetProperty(ref _mainViewProgressRingVisibility, value);
		}

		private bool _isBackButtonActive;

		public bool IsBackButtonActive
		{
			get => _isBackButtonActive;
			set => SetProperty(ref _isBackButtonActive, value);
		}
		
		private Visibility _loginPanelVisibility = Visibility.Collapsed;
		public Visibility LoginPanelVisibility
		{
			get => _loginPanelVisibility;
			set => SetProperty(ref _loginPanelVisibility, value);
		}
		private Visibility _shippingStatusGroupVisibility = Visibility.Visible;
		public Visibility ShippingStatusGroupVisibility
		{
			get => _shippingStatusGroupVisibility;
			set => SetProperty(ref _shippingStatusGroupVisibility, value);
		}
		private Visibility _engravingStatusGroupVisibility = Visibility.Collapsed;
		public Visibility EngravingStatusGroupVisibility
		{
			get => _engravingStatusGroupVisibility;
			set => SetProperty(ref _engravingStatusGroupVisibility, value);
		}

		private Visibility _activeMachinesGroupVisibility = Visibility.Collapsed;
		public Visibility ActiveMachinesGroupVisibility
		{
			get => _activeMachinesGroupVisibility;
			set => SetProperty(ref _activeMachinesGroupVisibility, value);
		}

		private string _statusGroup = "Shipping";

		public string StatusGroup
		{
			get => _statusGroup;
			set => SetProperty(ref _statusGroup, value);
		}

		private string _employeeName = "Vlad";

		public string EmployeeName
		{
			get => _employeeName;
			set => SetProperty(ref _employeeName, value);
		}

		private Visibility _mainNavigationViewVisibility = Visibility.Hidden;

		public Visibility MainNavigationViewVisibility
		{
			get => _mainNavigationViewVisibility;
			set => SetProperty(ref _mainNavigationViewVisibility, value);
		}

		public bool IsVitroMarkRunning => Process.GetProcessesByName("VitroMark_LE").Length > 0;
	}
}
