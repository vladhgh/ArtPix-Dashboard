using ArtPix_Dashboard.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtPix_Dashboard.Models.Types
{
	public class PageModel : PropertyChangedListener
	{
		public PageModel(int pageNumber, string pageName, string pageUrl)
		{
			PageName = pageName;
			PageNumber = pageNumber;
			PageUrl = pageUrl;
		}
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
