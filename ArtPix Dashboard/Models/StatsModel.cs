using ArtPix_Dashboard.ViewModels;

namespace ArtPix_Dashboard.Models.Types
{
	public class StatsModel : PropertyChangedListener
	{
		#region COUNTERS
		private int _totalCount;
		public int TotalCount
		{
			get => ReadyToEngraveCount + ProcessingCount;
			set => SetProperty(ref _totalCount, value);
		}

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

		private int _engravedCount;
		public int EngravedCount
		{
			get => _engravedCount;
			set => SetProperty(ref _engravedCount, value);
		}

		private int _redoCount;
		public int RedoCount
		{
			get => _redoCount;
			set => SetProperty(ref _redoCount, value);
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

		private int _reworkCount;
		public int ReworkCount
		{
			get => _reworkCount;
			set => SetProperty(ref _reworkCount, value);
		}
		#endregion
	}
}
