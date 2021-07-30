using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ArtPix_Dashboard.Utils.Helpers;

namespace ArtPix_Dashboard.Models.AppState
{
	public class WindowPropertiesModel : PropertyChangedListener
	{
		public WindowPropertiesModel()
		{
			Top = 0;
			Left = 0;
			Width = 1920;
			Height = 1080;
		}

		private double _top;
		public double Top
		{
			get => _top;
			set => SetProperty(ref _top, value);
		}
		private double _left;
		public double Left
		{
			get => _left;
			set => SetProperty(ref _left, value);
		}
		private double _height;
		public double Height
		{
			get => _height;
			set => SetProperty(ref _height, value);
		}
		private double _width;
		public double Width
		{
			get => _width;
			set => SetProperty(ref _width, value);
		}
		private WindowState _windowState;
		public WindowState WindowState
		{
			get => _windowState;
			set => SetProperty(ref _windowState, value);
		}
	}
}
