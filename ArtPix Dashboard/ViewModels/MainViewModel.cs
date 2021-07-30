using ArtPix_Dashboard.API;
using System;
using System.Reactive.Linq;
using System.Windows;
using ArtPix_Dashboard.Models.AppState;
using ArtPix_Dashboard.Models.Workstation;
using ArtPix_Dashboard.Utils.Helpers;
using System.Threading.Tasks;

namespace ArtPix_Dashboard.ViewModels
{

	public class MainViewModel : PropertyChangedListener
    {

	    #region PROPERTIES

	    private EngravingStatsModel _engravingStats = new ();

	    public EngravingStatsModel EngravingStats
		{
			get => _engravingStats;
			set => SetProperty(ref _engravingStats, value);
		}
	    private ShippingStatsModel _shippingStats = new ();

	    public ShippingStatsModel ShippingStats
	    {
		    get => _shippingStats;
		    set => SetProperty(ref _shippingStats, value);
	    }

		private WorkstationsModel _workstationStats = new ();

		public WorkstationsModel WorkstationStats
		{
			get => _workstationStats;
			set => SetProperty(ref _workstationStats, value);
		}

		private AppStateModel _appState = new ();

		public AppStateModel AppState
		{
			get => _appState;
			set => SetProperty(ref _appState, value);
		}

		#endregion

		#region INITIALIZER

		public async void Initialize()
		{
			ToggleLoadingAnimation(1);

			EngravingStats = await ArtPixAPI.GetEngravingStatsAsync();
			ShippingStats = await ArtPixAPI.GetShippingStatsAsync();
			WorkstationStats = await ArtPixAPI.GetWorkstationStats();
			await ArtPixAPI.GetEntityLogsAsync();

			SetTimers();

			ToggleLoadingAnimation(0);

		}
		
		#endregion

		#region TOGGLE LOADING ANIMATION

		private void ToggleLoadingAnimation(int kind)
		{
			if (kind == 0)
			{
				AppState.CurrentSession.MainNavigationViewVisibility = Visibility.Visible;
				AppState.CurrentSession.MainViewProgressRingVisibility = Visibility.Hidden;
			}

			if (kind == 1)
			{
				AppState.CurrentSession.MainNavigationViewVisibility = Visibility.Hidden;
				AppState.CurrentSession.MainViewProgressRingVisibility = Visibility.Visible;
			}
		}

		#endregion

		#region SET STATS UPDATE TIMERS - DONE - ✅

		private void SetTimers()
		{
			var engravingStatsTimer = Observable.Interval(TimeSpan.FromSeconds(30));
			engravingStatsTimer.Subscribe(async tick => EngravingStats = await ArtPixAPI.GetEngravingStatsAsync());
			var shippingStatsTimer = Observable.Interval(TimeSpan.FromSeconds(30));
			shippingStatsTimer.Subscribe(async tick => ShippingStats = await ArtPixAPI.GetShippingStatsAsync());
			var workstationsStatsTimer = Observable.Interval(TimeSpan.FromSeconds(30));
			workstationsStatsTimer.Subscribe(async tick => WorkstationStats = await ArtPixAPI.GetWorkstationStats());
			var entityLogsTimer = Observable.Interval(TimeSpan.FromSeconds(15));
			entityLogsTimer.Subscribe(async tick => await ArtPixAPI.GetEntityLogsAsync());
		}

		#endregion

	}
}
