using ArtPix_Dashboard.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Forms;
using ArtPix_Dashboard.Models.Order;
using QRCoder;
using System.Drawing;
using System.Drawing.Printing;

namespace ArtPix_Dashboard.Utils
{
	public static class Commands
	{

		public static void CopyTextToClipboard(string text)
		{
			Clipboard.Clear();
			Clipboard.SetText(text);
			//notifier.ShowSuccess(text + " Copied Succesfully!");
		}
		public static void OpenFileInVitroMark(object fileName)
		{
			var process = Process.GetProcessesByName("VitroMark_LE")[0];
			Clipboard.Clear();
			Clipboard.SetText("\\\\artpix\\main-storage\\" + fileName.ToString());
			WinAPI.SetForegroundWindow(process.MainWindowHandle);
			var buttonHandle = WinAPI.FindWindowEx(process.MainWindowHandle, IntPtr.Zero, null, "Open");
			WinAPI.SendMessage(buttonHandle, 0x0201, IntPtr.Zero, IntPtr.Zero);
			WinAPI.SendMessage(buttonHandle, 0x0202, IntPtr.Zero, IntPtr.Zero);
			Thread.Sleep(500);
			SendKeys.SendWait("^v");
			SendKeys.SendWait("{ENTER}");
		}
		public static void OpenOrderOnCP(object orderName)
		{
			string target = "https://confirmation.artpix3d.com/archives?search=" + orderName.ToString() + "&searchOptions=order_number";
			try
			{
				Process.Start(target);
			}
			catch (Win32Exception noBrowser)
			{
				if (noBrowser.ErrorCode == -2147467259)
					MessageBox.Show(noBrowser.Message);
			}
			catch (Exception other)
			{
				MessageBox.Show(other.Message);
			}
		}
		public static void OpenOrderOnOA(object orderName)
		{
			string target = "https://order-archive.artpix3d.com/dashboard/orders?search=" + orderName.ToString();
			try
			{
				Process.Start(target);
			}
			catch (Win32Exception noBrowser)
			{
				if (noBrowser.ErrorCode == -2147467259)
					MessageBox.Show(noBrowser.Message);
			}
			catch (Exception other)
			{
				MessageBox.Show(other.Message);
			}
		}

	}
}
