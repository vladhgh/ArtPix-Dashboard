using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.ViewModels;
using System.Windows.Navigation;
using ModernWpf.Controls;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using MessageBox = System.Windows.MessageBox;

namespace ArtPix_Dashboard.Views
{
	public partial class ProductionIssuesView
	{
		private static readonly ProductionIssuesViewModel _vm = new ProductionIssuesViewModel();


		public ProductionIssuesView()
		{
			InitializeComponent();
			DataContext = _vm;
		}
		private string inputString;
		private bool tabPressed;

		public void KeyPressEventListener(object sender, System.Windows.Input.KeyEventArgs e)
		{
			Debug.WriteLine("FIRED");
			if (e.Key == Key.Tab && !tabPressed)
			{
				tabPressed = true;
				e.Handled = true;
			}

			if (e.Key != Key.Tab && tabPressed)
			{
				if ((e.Key >= Key.D0) && (e.Key <= Key.D9))
				{
					inputString += e.Key.ToString().ToCharArray()[1];
					e.Handled = true;
				}

				if (e.Key == Key.OemMinus)
				{
					inputString += "-";
					e.Handled = true;
				}
			}

			if (e.Key == Key.Enter)
			{
				tabPressed = false;
				_vm.GetIssuesList(1, true, inputString.Split('-')[0]);
				inputString = "";
				e.Handled = true;
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			ProductionIssuesPage.PreviewKeyDown += KeyPressEventListener;
			ProductionIssuesPage.Focus();
			_vm.Initialize(e.ExtraData as AppStateModel);
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			_vm.GetIssuesList(1, true, searchTextBox.Text == "" ? "All" : searchTextBox.Text);
		}
		private void PageButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (_vm.Issues.Data.Count > 0) IssuesListView.ScrollIntoView(_vm.Issues.Data[0]);
		}

	}
}
