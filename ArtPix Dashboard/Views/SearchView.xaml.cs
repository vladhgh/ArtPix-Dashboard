using ArtPix_Dashboard.ViewModels;
using System.Windows.Navigation;

namespace ArtPix_Dashboard.Views
{
	public partial class SearchView
	{
		private SearchViewModel _vm = new SearchViewModel();
		public SearchView()
		{
			InitializeComponent();
			DataContext = _vm;
		}
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			var i = e.ExtraData.ToString();
			 _vm.Initialize(i);
		}
	}
}
