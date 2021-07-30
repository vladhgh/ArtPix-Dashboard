using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.ProductHistory
{
	public class Datum
	{

		public Visibility UserNameVisibility => User == null ? Visibility.Collapsed : Visibility.Visible;

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("service")]
		public string Service { get; set; }

		private string _message;
		[JsonProperty("message")]
		public string Message
		{
			get
			{
				_message = Regex.Replace(_message, "<.*?>", String.Empty);
				if (User != null)
				{
					_message = Regex.Replace(_message, "User: " + User.Name, String.Empty);
				}
				return _message;
			}
			set => _message = value;
		}

		[JsonProperty("user")]
		public User User { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		private string _updatedAt;

		[JsonProperty("updated_at")]
		public string UpdatedAt
		{
			get
			{
				var lastUpdated = DateTime.Parse(_updatedAt, CultureInfo.CurrentUICulture);
				return lastUpdated.AddHours(-5).ToString(CultureInfo.CurrentUICulture);
			}
			set => _updatedAt = value;
		}
	}
}
