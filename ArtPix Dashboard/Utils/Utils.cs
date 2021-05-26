using System;
using System.Threading;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace ArtPix_Dashboard.Utils
{
	public static class Utils
	{
		public static bool IsBase64Encoded(String str)
		{
			try
			{
				byte[] data = Convert.FromBase64String(str);
				return (str.Replace(" ", "").Length % 4 == 0);
			}
			catch
			{
				return false;
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
