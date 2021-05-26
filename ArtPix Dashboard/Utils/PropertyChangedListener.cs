using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ArtPix_Dashboard.ViewModels
{
	public class PropertyChangedListener : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (!EqualityComparer<T>.Default.Equals(field, newValue))
			{
				field = newValue;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
				return true;
			}
			return false;
		}
	}
}
