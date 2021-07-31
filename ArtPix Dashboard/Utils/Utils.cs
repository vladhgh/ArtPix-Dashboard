using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using ArtPix_Dashboard.API;
using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.Views;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace ArtPix_Dashboard.Utils
{

	#region ACRYLIC BACKGROUND PROPERTIES

	internal enum AccentState
	{
		ACCENT_DISABLED = 0,
		ACCENT_ENABLE_GRADIENT = 1,
		ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
		ACCENT_ENABLE_BLURBEHIND = 3,
		ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
		ACCENT_INVALID_STATE = 5
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct AccentPolicy
	{
		public AccentState AccentState;
		public uint AccentFlags;
		public uint GradientColor;
		public uint AnimationId;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct WindowCompositionAttributeData
	{
		public WindowCompositionAttribute Attribute;
		public IntPtr Data;
		public int SizeOfData;
	}

	internal enum WindowCompositionAttribute
	{
		WCA_ACCENT_POLICY = 19
	}

	#endregion

	public static class Utils
	{

		#region NOTIFIER
		public static Notifier Notifier = new(cfg =>
		{
			cfg.PositionProvider = new WindowPositionProvider(
				parentWindow: Application.Current.MainWindow,
				corner: Corner.BottomRight,
				offsetX: 12,
				offsetY: 55);

			cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
				notificationLifetime: TimeSpan.FromSeconds(3),
				maximumNotificationCount: MaximumNotificationCount.FromCount(5));

			cfg.DisplayOptions.Width = 250; // set the notifications width
			cfg.Dispatcher = Application.Current.Dispatcher;
		});
		#endregion

		#region LOCAL MACHINE ADDRESS PROPERTIES
		
		public struct MacIpPair
		{
			public string MacAddress;
			public string IpAddress;
		}
		
		public static Dictionary<string, string>MachineAddresses = new()
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

		#endregion

		#region ENABLE ACRYLIC BACKGROUND - DONE - ✅

		private static readonly uint BlurBackgroundColor = 0x990000;

		private static readonly uint BlurOpacity = 0;

		public static void EnableBlur(MainView view)
		{
			var windowHelper = new WindowInteropHelper(view);

			var accent = new AccentPolicy
			{
				AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
				GradientColor = (BlurOpacity << 24) | (BlurBackgroundColor & 0xFFFFFF)
			};

			var accentStructSize = Marshal.SizeOf(accent);

			var accentPtr = Marshal.AllocHGlobal(accentStructSize);
			Marshal.StructureToPtr(accent, accentPtr, false);

			var data = new WindowCompositionAttributeData
			{
				Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
				SizeOfData = accentStructSize,
				Data = accentPtr
			};

			WinAPI.SetWindowCompositionAttribute(windowHelper.Handle, ref data);

			Marshal.FreeHGlobal(accentPtr);
		}

		#endregion

		#region RESIZE IMAGE - DONE - ✅

		public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}

		#endregion

		#region SEND WAKE ON LAN - DONE - ✅

		public static void SendWakeOnLan(PhysicalAddress target)
		{
			var header = Enumerable.Repeat(byte.MaxValue, 6);
			var data = Enumerable.Repeat(target.GetAddressBytes(), 16).SelectMany(mac => mac);

			var magicPacket = header.Concat(data).ToArray();

			var client = new UdpClient();

			client.Send(magicPacket, magicPacket.Length, new IPEndPoint(IPAddress.Broadcast, 9));
		}

		#endregion

		#region GET LOCAL MACHINE ADDRESS - DONE - ✅

		public static string GetLocalMachineAddress(string machineId)
		{
			switch (machineId)
			{
				case "1":
					return "\\\\1-AB1-LL";
				case "2":
					return "\\\\2-AB1-LL";
				case "3":
					return "\\\\3-AB1-LL";
				case "4":
					return "\\\\4-AB1-LL";
				case "5":
					return "\\\\5-AB1-LL";
				case "6":
					return "\\\\6-AB1-LL";
				case "7":
					return "\\\\7-AB1-LL";
				case "8":
					return "\\\\8-AB1-LL";
				case "9":
					return "\\\\9-AB1-LL";
				case "10":
					return "\\\\10-AB1-LL";
				case "11":
					return "\\\\11-AB1-LL";
				case "12":
					return "\\\\12-AB1-LL";
				case "13":
					return "\\\\13-AB2-SL";
				case "14":
					return "\\\\14-AB2-SL";
				case "15":
					return "\\\\15-AB2-SL";
				case "16":
					return "\\\\16-AB3-SL";
				case "17":
					return "\\\\17-AB4-SL";
				case "18":
					return "\\\\18-AB2-SL";
				case "19":
					return "\\\\19-AB2-SL";
				case "20":
					return "\\\\20-AB1-LL";
				case "21":
					return "\\\\21-AB1-LL";
				case "22":
					return "\\\\22-AB1-LL";
				case "23":
					return "\\\\23-AB1-LL";
				case "24":
					return "\\\\24-AB1-LL";
				case "25":
					return "\\\\25-AB1-LL";
				default:
					return "none";
			}
		}

		#endregion

		#region SELECT STATUS COLOR

		public static string SelectStatusColor(string status)
		{
			switch (status)
			{
				case "photoshop": return "#bf6900";
				case "issue": return "DarkRed";
				case "3d_model_in_progress": return "SteelBlue";
				case "3d_model_pending": return "#bf6900";
				case "retoucher_in_progress": return "SteelBlue";
				case "retoucher_pending": return "#bf6900";
				case "waiting_to_confirm": return "#bf6900";
				case "wait_model": return "#bf6900";
				case "engrave_issue": return "DarkRed";
				case "engrave_processing": return "SteelBlue";
				case "engrave_done": return "DarkGreen";
				case "on_hold": return "#494949";
				case "error": return "DarkRed";
				case "shipping_label_printed": return "DarkGreen";
				case "In Photoshop": return "#bf6900";
				case "success": return "DarkGreen";
				case "success_manually": return "DarkGreen";
				case "Completed Manually": return "DarkGreen";
				case "Customer Service Issue": return "DarkRed";
				case "3D Model In Progress": return "SteelBlue";
				case "3D Model Pending": return "#bf6900";
				case "Retouch In Progress": return "SteelBlue";
				case "Retouch Pending": return "#bf6900";
				case "Awaiting Confirmation": return "#bf6900";
				case "Awaiting Model": return "#bf6900";
				case "Ready To Engrave": return "#494949";
				case "On Hold": return "#494949";
				case "Engraving Issue": return "DarkRed";
				case "retoucher_issue": return "DarkRed";
				case "Engraving In Progress": return "SteelBlue";
				case "Engraving Done": return "DarkGreen";
				case "Ready To Ship": return "DarkGreen";
				case "Shipped": return "DarkGreen";
				default: return "#494949";
			}
		}

		#endregion

		#region SELECT STATUS TEXT

		public static string SelectStatusText(string status)
		{
			switch (status)
			{
				case "photoshop": return "In Photoshop";
				case "issue": return "Customer Service Issue";
				case "3d_model_in_progress": return "3D Model In Progress";
				case "3d_model_pending": return "3D Model Pending";
				case "retoucher_in_progress": return "Retouch In Progress";
				case "retoucher_pending": return "Retouch Pending";
				case "waiting_to_confirm": return "Awaiting Confirmation";
				case "wait_model": return "Awaiting Model";
				case "ready_to_engrave": return "Ready To Engrave";
				case "success": return "Engraving Done";
				case "on_hold": return "On Hold";
				case "success_manually": return "Completed Manually";
				case "engrave_issue": return "Engraving Issue";
				case "error": return "Engraving Issue";
				case "retoucher_issue": return "Retouch Issue";
				case "processing": return "Engraving In Progress";
				case "engraved": return "Engraving In Progress";
				case "engrave_processing": return "Engraving In Progress";
				case "engrave_redo": return "Ready To Engrave";
				case "engrave_done": return "Engraving Done";
				case "shipping_label_printed": return "Shipped";
				default: return status;
			}
		}

		#endregion

		#region CHECK IS CRYSTAL - DONE - ✅

		public static bool IsCrystal(Product product) => product.CrystalType.Type == "Crystal" ||
		                                                 product.CrystalType.Type == "Keychain" ||
		                                                 product.CrystalType.Type == "Necklace" ||
		                                                 product.CrystalType.Type == "Wine Stopper" ||
		                                                 product.CrystalType.Type.Contains("Fingerprint");

		#endregion

		#region GET ALL MAC AND IP ADDRESS PAIRS - DONE - ✅

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

		#endregion

		#region GET LIST OF CHILDREN - DONE - ✅

		public static List<FrameworkElement> GetChildren(DependencyObject parent)
		{
			List<FrameworkElement> controls = new List<FrameworkElement>();

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				if (child is FrameworkElement)
				{
					controls.Add(child as FrameworkElement);
				}
				controls.AddRange(GetChildren(child));
			}
			return controls;
		}

		#endregion

		#region CREATE DEEP COPY - DONE - ✅

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

		#endregion

		#region GET CHILD OF TYPE - DONE - ✅
		
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

		#endregion

		#region GET SCROLL VIEWER - DONE - ✅

		public static DependencyObject GetScrollViewer(DependencyObject o)
		{
			// Return the DependencyObject if it is a ScrollViewer
			if (o is ScrollViewer)
			{ return o; }

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
			{
				var child = VisualTreeHelper.GetChild(o, i);

				var result = GetScrollViewer(child);
				if (result == null)
				{
					continue;
				}
				else
				{
					return result;
				}
			}
			return null;
		}

		#endregion

	}

}
