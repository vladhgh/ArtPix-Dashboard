using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ListView = ModernWpf.Controls.ListView;

namespace ArtPix_Dashboard.Utils
{
	public static class Utils
	{
		public static T GetChildOfType<T>(this DependencyObject depObj)
			where T : DependencyObject
		{
			if (depObj == null) return null;

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				var child = VisualTreeHelper.GetChild(depObj, i);

				var result = (child as T) ?? GetChildOfType<T>(child);
				if (result != null) return result;
			}
			return null;
		}

		public static void EnableTouchScrollForListView(ListView listView)
		{
			ScrollViewer scrollViewer = GetChildOfType<ScrollViewer>(listView);
			Debug.WriteLine("LOOKING FOR SCROLLVIEWER");
			if (scrollViewer != null)
			{
				Debug.WriteLine("SCROLLVIEWER FOUND");
				scrollViewer.CanContentScroll = false;
				scrollViewer.PanningDeceleration = 2;
				scrollViewer.PanningRatio = 0.75;
				scrollViewer.PanningMode = PanningMode.VerticalOnly;
			}
			else
			{
				Debug.WriteLine("SCROLLVIEWER IS NULL");
			}
		}
		#region NOTIFIER
		public static Notifier Notifier = new Notifier(cfg =>
		{
			cfg.PositionProvider = new WindowPositionProvider(
				parentWindow: Application.Current.MainWindow,
				corner: Corner.BottomRight,
				offsetX: 10,
				offsetY: 50);

			cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
				notificationLifetime: TimeSpan.FromSeconds(3),
				maximumNotificationCount: MaximumNotificationCount.FromCount(5));

			cfg.DisplayOptions.Width = 250; // set the notifications width
			cfg.Dispatcher = Application.Current.Dispatcher;
		});
		#endregion
	}
}
