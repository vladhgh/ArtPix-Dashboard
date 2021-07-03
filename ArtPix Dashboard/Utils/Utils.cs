using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ListView = ModernWpf.Controls.ListView;

namespace ArtPix_Dashboard.Utils
{
	public static class Utils
	{

		public struct MacIpPair
		{
			public string MacAddress;
			public string IpAddress;
		}

		public static Dictionary<string, string> MachineAddresses = new Dictionary<string, string>()
		{
			{ "94-de-80-fc-3a-fb", "1" },
		};

		public static string GetMacByIp(string ip)
		{
			var macIpPairs = GetAllMacAddressesAndIppairs();
			int index = macIpPairs.FindIndex(x => x.IpAddress == ip);
			if (index >= 0)
			{
				return macIpPairs[index].MacAddress.ToUpper();
			}
			else
			{
				return null;
			}
		}

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
				Debug.WriteLine($"MAC: {m.Groups["mac"].Value} IP: {m.Groups["ip"].Value}");
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


	public static class WOL
	{

		public static async Task WakeOnLan(string macAddress)
		{
			byte[] magicPacket = BuildMagicPacket(macAddress);
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces().Where((n) =>
				n.NetworkInterfaceType != NetworkInterfaceType.Loopback && n.OperationalStatus == OperationalStatus.Up))
			{
				IPInterfaceProperties iPInterfaceProperties = networkInterface.GetIPProperties();
				foreach (MulticastIPAddressInformation multicastIPAddressInformation in iPInterfaceProperties.MulticastAddresses)
				{
					IPAddress multicastIpAddress = multicastIPAddressInformation.Address;
					if (multicastIpAddress.ToString().StartsWith("ff02::1%", StringComparison.OrdinalIgnoreCase)) // Ipv6: All hosts on LAN (with zone index)
					{
						UnicastIPAddressInformation unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
							u.Address.AddressFamily == AddressFamily.InterNetworkV6 && !u.Address.IsIPv6LinkLocal).FirstOrDefault();
						if (unicastIPAddressInformation != null)
						{
							await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);
							break;
						}
					}
					else if (multicastIpAddress.ToString().Equals("224.0.0.1")) // Ipv4: All hosts on LAN
					{
						UnicastIPAddressInformation unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
							u.Address.AddressFamily == AddressFamily.InterNetwork && !iPInterfaceProperties.GetIPv4Properties().IsAutomaticPrivateAddressingActive).FirstOrDefault();
						if (unicastIPAddressInformation != null)
						{
							await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);
							break;
						}
					}
				}
			}
		}

		static byte[] BuildMagicPacket(string macAddress) // MacAddress in any standard HEX format
		{
			macAddress = Regex.Replace(macAddress, "[: -]", "");
			byte[] macBytes = new byte[6];
			for (int i = 0; i < 6; i++)
			{
				macBytes[i] = Convert.ToByte(macAddress.Substring(i * 2, 2), 16);
			}

			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter bw = new BinaryWriter(ms))
				{
					for (int i = 0; i < 6; i++)  //First 6 times 0xff
					{
						bw.Write((byte)0xff);
					}
					for (int i = 0; i < 16; i++) // then 16 times MacAddress
					{
						bw.Write(macBytes);
					}
				}
				return ms.ToArray(); // 102 bytes magic packet
			}
		}

		static async Task SendWakeOnLan(IPAddress localIpAddress, IPAddress multicastIpAddress, byte[] magicPacket)
		{
			using (UdpClient client = new UdpClient(new IPEndPoint(localIpAddress, 0)))
			{
				await client.SendAsync(magicPacket, magicPacket.Length, multicastIpAddress.ToString(), 9);
			}
		}
	}

}
