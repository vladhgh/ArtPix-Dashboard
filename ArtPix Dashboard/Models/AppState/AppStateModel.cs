using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using ArtPix_Dashboard.Utils.Helpers;

namespace ArtPix_Dashboard.Models.AppState
{

	public class AppStateModel : PropertyChangedListener
	{
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

		private string _employeeName = "Dev";

		public string EmployeeName
		{
			get => _employeeName;
			set => SetProperty(ref _employeeName, value);
		}

		private int _sessionTimeOut = 24 * 60 * 60;

		public int SessionTimeOut
		{
			get => _sessionTimeOut;
			set => SetProperty(ref _sessionTimeOut, value);
		}

		private List<string> _navigationStack = new();

		public List<string> NavigationStack
		{
			get => _navigationStack;
			set => SetProperty(ref _navigationStack, value);
		}

	}
}
