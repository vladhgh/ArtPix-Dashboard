using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;

namespace ArtPix_Dashboard.Models.AppState
{
	public class EngravingStatsModel : PropertyChangedListener
	{

		private int _engravedTodayCount;
		public int EngravedTodayCount
		{
			get => _engravedTodayCount;
			set => SetProperty(ref _engravedTodayCount, value);
		}

		private int _awaitingModelCount;
		public int AwaitingModelCount
		{
			get => _awaitingModelCount;
			set => SetProperty(ref _awaitingModelCount, value);
		}


		private int _processingCount;
		public int ProcessingCount
		{
			get => _processingCount;
			set => SetProperty(ref _processingCount, value);
		}

		private int _issueCount;
		public int IssueCount
		{
			get => _issueCount;
			set => SetProperty(ref _issueCount, value);
		}

		private int _readyToEngraveCount;
		public int ReadyToEngraveCount
		{
			get => _readyToEngraveCount;
			set => SetProperty(ref _readyToEngraveCount, value);
		}

		
	}
}
