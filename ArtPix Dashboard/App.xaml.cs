using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Store;
using Windows.Foundation.Collections;
using ArtPix_Dashboard.Views;
using ModernWpf.Media.Animation;
using ArtPix_Dashboard.Models.AppState;

namespace ArtPix_Dashboard
{
    public partial class App : Application
    {
	    internal MainView MainView;

        public App()
		{
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                ValueSet userInput = toastArgs.UserInput;

                Application.Current.Dispatcher.Invoke(delegate
                {
	                try
	                {
		                // TODO: Navigate to order with issue
	                    //MessageBox.Show("Toast activated. Args: " + toastArgs.Argument);
	                    var action = toastArgs.Argument.Split(';')[0].Split('=')[1];
	                    var param = toastArgs.Argument.Split(';')[1].Split('=')[1];
	                    if (action == "openIssue")
						{
	                        MainView.MainViewModel.AppState.CombinedFilter = new CombinedFilterModel("Search", "", "", param);
	                        MainView.SetActiveButton(null, "None");
	                        MainView.ContentFrame.Navigate(MainView.ShippingView, MainView.MainViewModel.AppState);
	                    }
	                }
	                catch (Exception ex)
	                {
		                MessageBox.Show("TOAST ON ACTIVATED EXCEPTION " + ex.Message);
	                }
                });
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
	        MainView = new MainView();
	        MainView.Show();
        }
    }
}
