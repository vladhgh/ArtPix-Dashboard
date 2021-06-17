﻿using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.Types;
using ArtPix_Dashboard.Properties;
using ArtPix_Dashboard.Utils;
using ArtPix_Dashboard.Views;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using ArtPix_Dashboard.Models.Workstation;
using Microsoft.Toolkit.Uwp.Notifications;
using Datum = ArtPix_Dashboard.Models.Workstation.Datum;
using Machine = ArtPix_Dashboard.Models.Machine.Machine;

namespace ArtPix_Dashboard.ViewModels
{
    public class MainViewModel : PropertyChangedListener
    {
	    #region PROPS
	    private StatsModel _engravingStats = new StatsModel();
	    public StatsModel EngravingStats
		{
			get => _engravingStats;
			set => SetProperty(ref _engravingStats, value);
		}
	    private StatsModel _shippingStats = new StatsModel();
	    public StatsModel ShippingStats
	    {
		    get => _shippingStats;
		    set => SetProperty(ref _shippingStats, value);
	    }
		private Visibility _shippingStatusGroupVisibility = Visibility.Collapsed;
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


		private AppStateModel _appState = new AppStateModel();
		public AppStateModel AppState
		{
			get => _appState;
			set => SetProperty(ref _appState, value);
		}

		private List<Models.Workstation.Machine> _activeMachinesList = new List<Models.Workstation.Machine>();
		public List<Models.Workstation.Machine> ActiveMachinesList
		{
			get => _activeMachinesList;
			set => SetProperty(ref _activeMachinesList, value);
		}
		private WorkstationsModel _workstations = new WorkstationsModel();
		public WorkstationsModel Workstations
		{
			get => _workstations;
			set => SetProperty(ref _workstations, value);
		}

		private Visibility _activeMachinesGroupVisibility = Visibility.Collapsed;
		public Visibility ActiveMachinesGroupVisibility
		{
			get => _activeMachinesGroupVisibility;
			set => SetProperty(ref _activeMachinesGroupVisibility, value);
		}

		#endregion

		public async void Initialize()
		{
			AppState.IsMainViewLoading = true;
			AppState.MainNavigationViewVisibility = Visibility.Hidden;
			try
			{
				EngravingStats = await ArtPixAPI.GetAllStatsAsync();
				ShippingStats = await ArtPixAPI.GetShippingStatsAsync();
				Workstations = await ArtPixAPI.GetWorkstations();
			} catch (Exception e)
			{
				Debug.WriteLine("EXCEPTION UNHANDLED: " + e.Message);
			}
			AppState.MainNavigationViewVisibility = Visibility.Visible;
			AppState.IsMainViewLoading = false;
			var timer = Observable.Interval(TimeSpan.FromSeconds(30));
			timer.Do(x => Debug.WriteLine("!ENGRAVING STATS LOADED!")).Subscribe(async tick => EngravingStats = await ArtPixAPI.GetAllStatsAsync());
			var timer2 = Observable.Interval(TimeSpan.FromSeconds(60));
			timer2.Do(x => Debug.WriteLine("!SHIPPING STATS LOADED!")).Subscribe(async tick => ShippingStats = await ArtPixAPI.GetShippingStatsAsync());
			/*new ToastContentBuilder()
				.AddArgument("action", "viewConversation")
				.AddArgument("conversationId", 9813)
				.AddText("Andrew sent you a picture")
				.AddText("Check this out, The Enchantments in Washington!")
				.Show();*/
		}
    }
}
