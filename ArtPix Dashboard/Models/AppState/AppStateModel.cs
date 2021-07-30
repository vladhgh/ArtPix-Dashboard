using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using ArtPix_Dashboard.Utils.Helpers;

namespace ArtPix_Dashboard.Models.AppState
{

	public class AppStateModel : PropertyChangedListener
	{

		private WindowPropertiesModel _windowProperties = new();

		public WindowPropertiesModel WindowProperties
		{
			get => _windowProperties;
			set => SetProperty(ref _windowProperties, value);
		}

		private CombinedFilterModel _combinedFilter = new ();

		public CombinedFilterModel CombinedFilter
		{
			get => _combinedFilter;
			set => SetProperty(ref _combinedFilter, value);
		}

		private CurrentSessionModel _currentSession = new();

		public CurrentSessionModel CurrentSession
		{
			get => _currentSession;
			set => SetProperty(ref _currentSession, value);
		}

		private List<string> _navigationStack = new();

		public List<string> NavigationStack
		{
			get => _navigationStack;
			set => SetProperty(ref _navigationStack, value);
		}

	}
}
