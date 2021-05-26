using ArtPix_Dashboard.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Forms;
using ArtPix_Dashboard.Models.Order;
using QRCoder;

namespace ArtPix_Dashboard.ViewModels
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
			IntPtr ButtonHandle = WinAPI.FindWindowEx(process.MainWindowHandle, IntPtr.Zero, null, "Open");
			WinAPI.SendMessage(ButtonHandle, 0x0201, IntPtr.Zero, IntPtr.Zero);
			WinAPI.SendMessage(ButtonHandle, 0x0202, IntPtr.Zero, IntPtr.Zero);
			Thread.Sleep(500);
			SendKeys.SendWait("^v");
			SendKeys.SendWait("{ENTER}");
		}
		public static void OpenOrderOnCP(object orderName)
		{
			string target = "https://confirmation.artpix3d.com/archives?search=" + orderName.ToString() + "&searchOptions=name";
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

		public static void PrintQR(object obj)
		{
			foreach (var item in (List<MachineAssignItem>) obj)
			{
				Debug.WriteLine(item.Id);
			}
			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
			QRCode qrCode = new QRCode(qrCodeData);
			var qrCodeImage = qrCode.GetGraphic(20);
		}
	}
}
