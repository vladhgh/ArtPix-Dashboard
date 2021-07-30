using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtPix_Dashboard.Utils.Helpers;
using ArtPix_Dashboard.ViewModels;

namespace ArtPix_Dashboard.Models.AppState
{
	public class ShippingStatsModel : PropertyChangedListener
	{
		private int _awaitingShipment;
		public int AwaitingShipment
		{
			get => _awaitingShipment;
			set => SetProperty(ref _awaitingShipment, value);
		}
		private int _readyToShip;
		public int ReadyToShip
		{
			get => _readyToShip;
			set => SetProperty(ref _readyToShip, value);
		}
		private int _shipByToday;
		public int ShipByToday
		{
			get => _shipByToday;
			set => SetProperty(ref _shipByToday, value);
		}
		private int _shippedToday;
		public int ShippedToday
		{
			get => _shippedToday;
			set => SetProperty(ref _shippedToday, value);
		}
		private string _ordersShipped;
		public string OrdersShipped
		{
			get => _ordersShipped;
			set => SetProperty(ref _ordersShipped, value);
		}
	}
}
