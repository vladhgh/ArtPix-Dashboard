using System;
using System.Runtime.InteropServices;

namespace ArtPix_Dashboard.API
{
	public class WinAPI
	{
		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern string SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
	}
}
