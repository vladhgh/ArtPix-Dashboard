using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;

namespace ArtPix_Dashboard.Models.Common
{
	public class PageModel : PropertyChangedListener
	{
		public string PageName { get; set; }
		public int PageNumber { get; set; }

		private bool _isSelected;

		public bool IsSelected
		{
			get => _isSelected;
			set => SetProperty(ref _isSelected, value);
		}
		public string PageUrl { get; set; }
		public DelegateCommand NavigateToSelectedPage { get; set; }
	}
}
