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
                    // TODO: Navigate to order with issue
                    MessageBox.Show("Toast activated. Args: " + toastArgs.Argument);
                });
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
	        MainView = new MainView();
            MainView.Show();
	        Debug.WriteLine("STARTUP");
        }
    }
}
