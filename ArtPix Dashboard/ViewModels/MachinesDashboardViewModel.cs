using ArtPix_Dashboard.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using ArtPix_Dashboard.Models.Machine;
using ArtPix_Dashboard.Views;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using ArtPix_Dashboard.Models.Types;
using ArtPix_Dashboard.Utils;
using ArtPix_Dashboard.Views.Dialogs;
using ModernWpf.Controls;
using ToastNotifications.Messages;
using static ArtPix_Dashboard.Utils.Utils;

namespace ArtPix_Dashboard.ViewModels
{
    public class MachinesDashboardViewModel : PropertyChangedListener
    {
		#region PROPS
		private bool _isLoading;
		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
		}
		private Visibility _isLoaded;
		public Visibility IsLoaded
		{
			get => _isLoaded;
			set => SetProperty(ref _isLoaded, value);
		}
		private ObservableCollection<PageModel> _pages = new ObservableCollection<PageModel>();
		public ObservableCollection<PageModel> Pages
		{
			get => _pages;
			set => SetProperty(ref _pages, value);
		}
		private MachineAssignItemModel _machineAssignedItems = new MachineAssignItemModel();
		public MachineAssignItemModel MachineAssignedItems
		{
			get => _machineAssignedItems;
			set => SetProperty(ref _machineAssignedItems, value);
		}
		private AppStateModel _appState = new AppStateModel();
		public AppStateModel AppState
		{
			get => _appState;
			set => SetProperty(ref _appState, value);
		}

		public List<string> EngravingMachines
		{
			get
			{
				var list = new List<string>();
				for (var i = 1; i < 34; i++)
				{
					list.Add($"Machine {i}");
				}

				return list;
			}
		}

		private List<Machine> _activeMachinesList = new List<Machine>();

		public List<Machine> ActiveMachinesList
		{
			get => _activeMachinesList;
			set => SetProperty(ref _activeMachinesList, value);
		}
		#endregion

		#region COMMANDS
		private ICommand _onUnassignMachine;
		public ICommand OnUnassignMachine
		{
			get => _onUnassignMachine;
			set => SetProperty(ref _onUnassignMachine, value);
		}
		private ICommand _onAssignMachine;
		public ICommand OnAssignMachine
		{
			get => _onAssignMachine;
			set => SetProperty(ref _onAssignMachine, value);
		}
		private ICommand _reloadList;
		public ICommand ReloadList
		{
			get => _reloadList;
			set => SetProperty(ref _reloadList, value);
		}
		private ICommand _onOA;
		public ICommand OnOA
		{
			get => _onOA;
			set => SetProperty(ref _onOA, value);
		}
		private ICommand _onCP;
		public ICommand OnCP
		{
			get => _onCP;
			set => SetProperty(ref _onCP, value);
		}
		private ICommand _onRedo;
		public ICommand OnRedo
		{
			get => _onRedo;
			set => SetProperty(ref _onRedo, value);
		}
		private ICommand _onRework;
		public ICommand OnRework
		{
			get => _onRework;
			set => SetProperty(ref _onRework, value);
		}
		private ICommand _onCancelIssue;
		public ICommand OnCancelIssue
		{
			get => _onCancelIssue;
			set => SetProperty(ref _onCancelIssue, value);
		}
		private ICommand _copyToClipboard;
		public ICommand CopyToClipboard
		{
			get => _copyToClipboard;
			set => SetProperty(ref _copyToClipboard, value);
		}
		private ICommand _onVitromark;
		public ICommand OnVitromark
		{
			get => _onVitromark;
			set => SetProperty(ref _onVitromark, value);
		}
		private ICommand _onManualComplete;
		public ICommand OnManualComplete
		{
			get => _onManualComplete;
			set => SetProperty(ref _onManualComplete, value);
		}
		


		#endregion

		private string _status = "ready_to_engrave";
		private string _machine = "All";

		private void InitializeCommands()
		{
			ReloadList = new DelegateCommand(async param => await GetMachineAssignItems());
			OnOA = new DelegateCommand(Commands.OpenOrderOnOA);
			OnCP = new DelegateCommand(Commands.OpenOrderOnCP);
			OnUnassignMachine = new DelegateCommand(OpenUnassignDialog);
			OnManualComplete = new DelegateCommand(OpenManualCompleteDialog);
			OnAssignMachine = new DelegateCommand(OpenAssignMachineDialog);
			CopyToClipboard = new DelegateCommand(param => Commands.CopyTextToClipboard(param.ToString()));
			OnVitromark = new DelegateCommand(param => Commands.OpenFileInVitroMark(param.ToString()));
		}


		private async void OpenAssignMachineDialog(object param)
		{
			var item = MachineAssignedItems.Data.SingleOrDefault(p => p.Id == (int)param);
			var machines = await ArtPixAPI.GetMachines(item.ProductId);
			var dialog = new AssignMachineDialog(machines.Data);
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				IsLoaded = Visibility.Hidden;
				IsLoading = true;
				if (!string.IsNullOrEmpty(dialog.Combo2.Text))
				{
					var x = (Models.Machine.Machine)dialog.Combo2.SelectedItem;
					var body = new AssignProcessingModel
					{
						machine = x.Name,
						product_id = item.ProductId,
						order_id = item.OrderId,
						order_name = item.OrderName
					};
					/*Debug.WriteLine(body.machine);
					Debug.WriteLine(body.product_id);
					Debug.WriteLine(body.order_id);
					Debug.WriteLine(body.order_name);*/
					await ArtPixAPI.ProductAssignProcessing(body);
					Notifier.ShowSuccess($"Assigned To Machine {body.machine} Successfully!");
					ActiveMachinesList = await ArtPixAPI.GetActiveMachines();
				}
				await GetMachineAssignItems(_status, _machine, MachineAssignedItems.Meta.CurrentPage, false);
			}
		}

		private async void OpenManualCompleteDialog(object param)
		{
			var item = MachineAssignedItems.Data.SingleOrDefault(p => p.Id == (int) param);
			var dialog = new ManualCompleteDialog();
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				IsLoaded = Visibility.Hidden;
				IsLoading = true;
				var newStatus = new NewStatusModel
				{
					machine_assign_item_id = item.Id,
					new_status = "success_manually",
					order_id = item.OrderId,
					order_name = item.OrderName,
					product_id = item.ProductId
				};
				await ArtPixAPI.ChangeMachineAssignItemStatusAsync(newStatus);
				await GetMachineAssignItems(_status, _machine , MachineAssignedItems.Meta.CurrentPage, false);
			}
		}
		private async void OpenUnassignDialog(object param)
		{
			var item = MachineAssignedItems.Data.SingleOrDefault(p => p.Id == (int)param);
			var dialog = new UnassignDialog();
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				IsLoaded = Visibility.Hidden;
				IsLoading = true;
				var body = new AssignProcessingModel
				{
					order_id = item.OrderId,
					order_name = item.OrderName,
					product_id = item.ProductId
				};
				await ArtPixAPI.UnassignMachineAssignItemAsync(body);
				await GetMachineAssignItems(_status, _machine, MachineAssignedItems.Meta.CurrentPage, false);
			}
		}

		public async Task GetMachineAssignItems(string status = "ready_to_engrave", string machineId = "All" , int pageNumber = 1, bool withPages = true)
		{
			IsLoaded = Visibility.Hidden;
			IsLoading = true;
			_status = status;
			_machine = machineId;
			MachineAssignedItems = status == "engraved_today"
				? await ArtPixAPI.GetEngravedTodayItemsAsync(machineId, pageNumber.ToString())
				: await ArtPixAPI.GetMachineAssignItemsAsync(status, machineId, pageNumber.ToString());
			foreach (var page in Pages)
			{
				page.IsSelected = page.PageNumber == pageNumber;
			}
			Pages = withPages ? GetPages(pageNumber, status, machineId) : new ObservableCollection<PageModel>(Pages);
			IsLoading = false;
			IsLoaded = Visibility.Visible;
		}

		public async void GetActiveMachines()
		{
			ActiveMachinesList = await ArtPixAPI.GetActiveMachines();
			var timer = Observable.Interval(TimeSpan.FromSeconds(30));
			timer.Do(x => Debug.WriteLine("!MACHINES LOADED!")).Subscribe(async tick => ActiveMachinesList = await ArtPixAPI.GetActiveMachines());
			
		}

		public async Task Initialize(AppStateModel appState)
		{
			AppState = appState;
			GetActiveMachines();
			await GetMachineAssignItems();
			InitializeCommands();
		}
		private ObservableCollection<PageModel> GetPages(int currentPageNumber, string currentStatus, string machineId)
		{
			var pages = new ObservableCollection<PageModel>();
			for (var i = MachineAssignedItems.Meta.CurrentPage; i <= MachineAssignedItems.Meta.LastPage; i++)
			{

				if (i > MachineAssignedItems.Meta.CurrentPage + 5 && i < MachineAssignedItems.Meta.LastPage)
				{
					var page = new PageModel(i + 5, "...", MachineAssignedItems.Meta.Path + "?page=" + (i + 5))
					{
						IsSelected = i == MachineAssignedItems.Meta.CurrentPage,
						NavigateToSelectedPage = new DelegateCommand(async param =>
							await GetMachineAssignItems(currentStatus, machineId , (currentPageNumber + 5), true))
					};
					pages.Add(page);
				}
				else
				{
					var page = new PageModel(i, i.ToString(), MachineAssignedItems.Meta.Path + "?page=" + i.ToString())
					{
						IsSelected = i == MachineAssignedItems.Meta.CurrentPage,
						NavigateToSelectedPage = new DelegateCommand(async param =>
							await GetMachineAssignItems(currentStatus, machineId , (int) param, false))
					};
					pages.Add(page);
				}
			}
			pages = new ObservableCollection<PageModel>(pages.GroupBy(x => x.PageName).Select(g => g.First()).ToList());
			return pages;
		}
    }
}
