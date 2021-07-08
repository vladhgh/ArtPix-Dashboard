using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ListView = ModernWpf.Controls.ListView;

namespace ArtPix_Dashboard.Utils
{

	public class MultiplyConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double result = 1.0;
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] is double)
					result *= (double)values[i];
			}

			return result;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new Exception("Not implemented");
		}
	}

	public static class Utils
	{

		public struct MacIpPair
		{
			public string MacAddress;
			public string IpAddress;
		}

		public static Dictionary<string, string> MachineAddresses = new Dictionary<string, string>()
		{
			{ "94-DE-80-FC-3A-FB", "1" },
			{ "B4-2E-99-5D-61-5C", "2" },
			{ "B4-2E-99-D9-65-DA", "3" },
			{ "B4-2E-99-B6-A8-60", "4" },
			{ "B4-2E-99-D9-64-A1", "5" },
			{ "B4-2E-99-D9-67-8B", "6" },
			{ "18-C0-4D-12-B9-D9", "7" },
			{ "B4-2E-99-B8-C7-30", "8" },
			{ "B4-2E-99-5D-5A-7D", "9" },
			{ "B4-2E-99-75-46-79", "10" },
			{ "B4-2E-99-5D-61-64", "11" },
			{ "B4-2E-99-5D-61-10", "12" },
			{ "E0-D5-5E-CE-E5-99", "13" },
			{ "B4-2E-99-20-7A-98", "14" },
			{ "E0-D5-5E-CE-E5-72", "15" },
			{ "78-24-AF-8F-D6-AB", "16" },
			{ "14-DD-A9-56-B1-7F", "17" },
			{ "B4-2E-99-B8-C7-53", "18" },
			{ "14-DD-A9-25-20-7E", "19" },
			{ "B4-2E-99-75-46-7E", "20" },
			{ "B4-2E-99-5D-60-4E", "21" },
			{ "B4-2E-99-5D-61-57", "22" },
			{ "B4-2E-99-B8-C6-67", "23" },
			{ "B4-2E-99-5D-61-5E", "24" },
			{ "B4-2E-99-75-46-7D", "25" },
			{ "00-D8-61-7E-B4-91", "26" },
			{ "00-D8-61-7F-A0-94", "27" },
			{ "00-D8-61-7F-A0-B2", "28" },
			{ "00-D8-61-7F-A0-BE", "29" },
			{ "00-D8-61-7F-A0-96", "30" },
			{ "00-D8-61-7E-B4-B1", "31" },
			{ "00-D8-61-7F-A0-AC", "32" },
			{ "00-19-0F-37-0C-34", "33" }
		};

		public static List<MacIpPair> GetAllMacAddressesAndIppairs()
		{
			List<MacIpPair> mip = new List<MacIpPair>();
			Process pProcess = new Process();
			pProcess.StartInfo.FileName = "arp";
			pProcess.StartInfo.Arguments = "-a ";
			pProcess.StartInfo.UseShellExecute = false;
			pProcess.StartInfo.RedirectStandardOutput = true;
			pProcess.StartInfo.CreateNoWindow = true;
			pProcess.Start();
			string cmdOutput = pProcess.StandardOutput.ReadToEnd();
			string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

			foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
			{
				mip.Add(new MacIpPair()
				{
					MacAddress = m.Groups["mac"].Value,
					IpAddress = m.Groups["ip"].Value
				});
				//Debug.WriteLine($"MAC: {m.Groups["mac"].Value} IP: {m.Groups["ip"].Value}");
			}

			return mip;
		}
		

		public static T DeepCopy<T>(T other)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Context = new StreamingContext(StreamingContextStates.Clone);
				formatter.Serialize(ms, other);
				ms.Position = 0;
				return (T)formatter.Deserialize(ms);
			}
		}

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
			//Debug.WriteLine("LOOKING FOR SCROLLVIEWER");
			if (scrollViewer != null)
			{
				//Debug.WriteLine("SCROLLVIEWER FOUND");
				scrollViewer.CanContentScroll = false;
				scrollViewer.PanningDeceleration = 2;
				scrollViewer.PanningRatio = 0.75;
				scrollViewer.PanningMode = PanningMode.VerticalOnly;
			}
			else
			{
				//Debug.WriteLine("SCROLLVIEWER IS NULL");
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
