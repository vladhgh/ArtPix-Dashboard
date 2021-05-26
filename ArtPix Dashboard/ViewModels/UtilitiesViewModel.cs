using ArtPix_Dashboard.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using ArtPix_Dashboard.Models.Machine;
using ArtPix_Dashboard.Views;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ArtPix_Dashboard.Models.Types;
using ArtPix_Dashboard.Utils;
using ArtPix_Dashboard.Views.Dialogs;
using ModernWpf.Controls;
using ToastNotifications.Messages;

namespace ArtPix_Dashboard.ViewModels
{
	public class UtilitiesViewModel : PropertyChangedListener
	{
		#region PROPS
		private bool _isLoading;
		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
		}
		private Visibility _isLoaded;
		public Visibility IsLoaded
		{
			get => _isLoaded;
			set => SetProperty(ref _isLoaded, value);
		}

		#endregion

	}
}
