using System.Collections.Generic;
using ArtPix_Dashboard.Models.AppState;
using ArtPix_Dashboard.Models.Workstation;
using ArtPix_Dashboard.Utils.Helpers;

namespace ArtPix_Dashboard.Dialogs
{
    public partial class DailyReportDialog : ModernWpf.Controls.ContentDialog
    {
	    public AppStateModel AppState { get; set; }
	    public DailyReportDialog(AppStateModel appState)
	    {
		    InitializeComponent();
		    DataContext = this;
		    AppState = appState;
	    }

		private void CreateReportButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{

		}
		private void GoBackButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{

		}
	}

	public class DailyReportDialogViewModel : PropertyChangedListener
	{

	}
}
