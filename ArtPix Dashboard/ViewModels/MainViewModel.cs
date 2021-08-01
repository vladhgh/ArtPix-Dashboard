using ArtPix_Dashboard.API;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using ArtPix_Dashboard.Models.AppState;
using ArtPix_Dashboard.Models.Workstation;
using ArtPix_Dashboard.Utils.Helpers;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

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
			set
			{
				var selectedWorkstation = new Datum();
				if (_workstationStats.Data != null)
				{
					foreach (var workstation in _workstationStats.Data)
					{
						if (workstation.IsChecked)
						{
							selectedWorkstation = workstation;
						}
					}
					SetProperty(ref _workstationStats, value);
					if (selectedWorkstation.Id <= 0) return;
					var newSelectedWorkstation = _workstationStats.Data.FirstOrDefault((x) => x.Id == selectedWorkstation.Id);
					newSelectedWorkstation.IsChecked = selectedWorkstation.IsChecked;
					newSelectedWorkstation.MachinesGroupVisibility = selectedWorkstation.MachinesGroupVisibility;
					newSelectedWorkstation.Machines = selectedWorkstation.Machines;
					return;
				}
				SetProperty(ref _workstationStats, value);
			}
		}

		private AppStateModel _appState = new ();

		public AppStateModel AppState
		{
			get => _appState;
			set => SetProperty(ref _appState, value);
		}

		#endregion

		private IObservable<long> entityLogsTimer = Observable.Interval(TimeSpan.FromSeconds(15));

		#region INITIALIZER

		public async Task Initialize()
		{
			EngravingStats = await ArtPixAPI.GetEngravingStatsAsync();
			ShippingStats = await ArtPixAPI.GetShippingStatsAsync();
			WorkstationStats = await ArtPixAPI.GetWorkstationStats();
			SetTimers();
		}

		#endregion

		public async void SetGetEntityLogsTimer()
		{
			entityLogsTimer.Subscribe(async tick => await ArtPixAPI.GetEntityLogsAsync());
		}


		private async Task SynthesizeAudioAsync()
		{
			if (ShippingStats.ShipByToday > 0 && DateTime.Now.Hour >= 17)
			{
				var config = SpeechConfig.FromSubscription("7c41fdfabd744deb8ff197f39b2b63e9", "eastus");
				using var synthesizer = new SpeechSynthesizer(config);
				await synthesizer.SpeakTextAsync($"There are {ShippingStats.ShipByToday} more orders to ship. Please, ship them now!");
			};
		}





		#region SET STATS UPDATE TIMERS

		private void SetTimers()
		{
			
			var engravingStatsTimer = Observable.Interval(TimeSpan.FromSeconds(30));
			engravingStatsTimer.Subscribe(async tick => EngravingStats = await ArtPixAPI.GetEngravingStatsAsync());
			var shippingStatsTimer = Observable.Interval(TimeSpan.FromSeconds(30));
			shippingStatsTimer.Subscribe(async tick => ShippingStats = await ArtPixAPI.GetShippingStatsAsync());
			var workstationsStatsTimer = Observable.Interval(TimeSpan.FromSeconds(30));
			workstationsStatsTimer.Subscribe(async tick => WorkstationStats = await ArtPixAPI.GetWorkstationStats());
			//var checkForOrdersToShipTimer = Observable.Interval(TimeSpan.FromMinutes(15));
			//checkForOrdersToShipTimer.Subscribe(async tick => await SynthesizeAudioAsync());
		}

		#endregion

		

	}
}
