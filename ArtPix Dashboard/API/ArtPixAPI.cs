using ArtPix_Dashboard.Models.Types;
using ArtPix_Dashboard.Models.Machine;
using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.Models.Product;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.ProductHistory;
using ArtPix_Dashboard.Models.Shipping;
using ArtPix_Dashboard.Models.Workstation;
using RestSharp.Extensions;
using Machine = ArtPix_Dashboard.Models.Machine.Machine;
using Order = ArtPix_Dashboard.Models.Machine.Order;
using Windows.Globalization.DateTimeFormatting;
using System.Web;
using ToastNotifications.Messages;
using ArtPix_Dashboard.Properties;
using ArtPix_Dashboard.Models.Logs;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ArtPix_Dashboard.Utils
{
	public class ArtPixAPI
	{
		public static string bearerToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiZGZkMWFjZWIyN2EyOGU2ODliMjE2ZThjOGMyZTU1YTkxODgzODM5MDAwNGM2OWM1ZDE2OWU5NDQzMzRjZWRjZmRlOGJiNTljOGJjN2Y2OWUiLCJpYXQiOjE2MDAzMTg4OTksIm5iZiI6MTYwMDMxODg5OSwiZXhwIjoxNjMxODU0ODk5LCJzdWIiOiIxNiIsInNjb3BlcyI6W119.QWBy29NAIWiYQzU6QI4XLFYaZTZnxVj_Jw0z3uzv7WSapgLJTi1pa3sy57d9DEqnOG_H2bfLjrJv3yP48Ix86Q_CwkW5YME40ZlafbJzT1203Rf4fB46dFUzhtbSmVMrQfc6bYP77naN6Ev06TGjiLIZ2SYrM94scCFLdN1tJEdZdqYI3EEbABWaGIr2g1jok5G_3T9VHnrWOWYOdffaOH7w7kpl6_QTGaX5e3Z3XDYynKJAgub4xTaHptLxbNq1EpCjNvOcw1vjTV22MKj-1p3IWOWjXNtVelIOkNx1VE1w--7hAlaIaOHYoSbslqXYla1aSfCLQ6P4EPMYqvsKTArYBpnVG6LM_XQhQK-iNKmYoJa1ZIVTlSc9fQwE7z9hnmOZCcjqgc-kbN6f6DShboe0sjaAA12o-lxjbmIjPdobruDBKMIFsQ5boCrURWR2kaKIe2uATJFTImuE54C_8H2vfu2mOcH_UUPU9u3B6dLQCCk2cjLuPPj6ZOLy4DKrwafk14cvVFtMxRxu2u6ICVz0pztRzxBHo9zZTMKeW7JsZsZvhYeI4A16-7jtzhDF5lhQAHXQ_oOHsvXjLND_XDfGkTAue3Z4GS1O_7GmkvdV2URbHoD0wfNHFDvxl2ecph3u18A40jntJbgFd3ey4hHtq-IwXJaj0RM6AV_nVZ8";

		private static readonly RestClient Client = new RestClient("https://order-archive.artpix3d.com/api/v1");
		private static readonly RestClient ClientCp = new RestClient("https://confirmation.artpix3d.com/api");

		#region CP
		public static async Task<ProductHistoryModel> GetProductHistoryAsync(int productId)
		{
			var req = new RestRequest("/order-item-history/" + productId.ToString());
			req.AddHeader("Accept", "application/json");
			req.AlwaysMultipartFormData = true;
			var res = await ClientCp.GetAsync<ProductHistoryModel>(req);
			return res;
		}
		#endregion



		public static async Task<StatsModel> GetAllStatsAsync()
		{
			var machineAssignItems = await Task.WhenAll(GetMachineAssignItemsAsync("ready_to_engrave", "All" ,"1", "1"), GetMachineAssignItemsAsync("processing", "All", "1", "1"), GetMachineAssignItemsAsync("engraved", "All", "1", "1"), GetEngravedTodayItemsAsync("All", "1", "1"));
			var productionIssues = await GetProductionIssuesAsync("1", "1");
			var stats = new StatsModel
			{
				ReadyToEngraveCount = machineAssignItems[0].Meta.Total - 5,
				ProcessingCount = machineAssignItems[1].Meta.Total - 1,
				EngravedCount = machineAssignItems[2].Meta.Total,
				EngravedTodayCount = machineAssignItems[3].Meta.Total,
				IssueCount = productionIssues.Meta.Total
			};
			return stats;
		}
		public static async Task<StatsModel> GetShippingStatsAsync()
		{
			var awaitingShipmentModel = new OrderCombineFilterModel
			{
				status_order = "processing",
				status_shipping = "waiting"
			};
			var shipByTodayModel = new OrderCombineFilterModel
			{
				status_order = "processing",
				status_shipping = "waiting",
				shipByToday = "True"
			};
			var readyToShip = new OrderCombineFilterModel
			{
				status_order = "processing",
				status_shipping = "waiting",
				status_engraving = "engrave_done&amp;with_crystal_product_status[]=completed"
			};
			var orders = await Task.WhenAll(GetOrdersAsync(1, 1, awaitingShipmentModel), GetOrdersAsync(1, 1, shipByTodayModel), GetOrdersAsync(1, 1, readyToShip));
			var shippedOrders = await GetShippedTodayOrders();
			var stats = new StatsModel
			{
				AwaitingShipment = orders[0].Meta.Total,
				ShipByToday = orders[1].Meta.Total,
				ReadyToShip = orders[2].Meta.Total,
				ShippedToday = shippedOrders.Count
			};
			return stats;
		}

		public static async Task<List<Models.Workstation.Machine>> GetActiveMachines()
		{
			var items = await GetMachineAssignItemsAsync("processing", "All", "1" , "100");
			var machinesList = items.Data.Select(item => new Models.Workstation.Machine { Name = item.MachineId}).ToList();
			var i = 1;
			while (items.Meta.LastPage > i)
			{
				i++;
				items = await GetMachineAssignItemsAsync("processing", "All", i.ToString(), "100");
				machinesList.AddRange(items.Data.Select(item => new Models.Workstation.Machine {Name = item.MachineId}));
			}
			var res = machinesList.GroupBy(l => l.Name).Select(g => new Models.Workstation.Machine
			{
				Name = g.Key,
				IdMachines = Int32.Parse(g.Key),
				JobsCount = g.Select(l => l.Name).Count()
			});
			
			return res.ToList();
		}

		public static async Task<WorkstationsModel> GetWorkstations()
		{
			var req = new RestRequest("/work-stations");
			req.AddHeader("Accept", "application/json");
			var networkAddresses = Utils.GetAllMacAddressesAndIppairs();
			var workstations = await Client.GetAsync<WorkstationsModel>(req);
			var activeMachines = await GetActiveMachines();
			var online = new Dictionary<int, bool>();

			foreach (var item in Utils.MachineAddresses)
			{
				online[int.Parse(item.Value)] = networkAddresses.Find(p => p.MacAddress.ToUpper() == item.Key).MacAddress != null;
			}
			foreach (var machine in workstations.Data.SelectMany(workstation => workstation.Machines))
			{
				if (online.TryGetValue(machine.IdMachines, out bool val))
				{
					machine.NetworkStatus = val ? "Online" : "Offline";
					Console.WriteLine("Fetched value: {0}", val);
				}
				else
				{
					Console.WriteLine("No such key: {0}", machine.IdMachines);
				}
			}


			foreach (var activeMachine in activeMachines)
			{
				//Debug.WriteLine($"MACHINE {activeMachine.IdMachines} JOBS {activeMachine.JobsCount}");

				if (activeMachine.IdMachines >= 1 && activeMachine.IdMachines <= 4)
				{
					workstations.Data[0].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[0].JobsCount += activeMachine.JobsCount;
				}
				if (activeMachine.IdMachines >= 5 && activeMachine.IdMachines <= 8)
				{
					workstations.Data[1].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[1].JobsCount += activeMachine.JobsCount;
				}
				if (activeMachine.IdMachines >= 9 && activeMachine.IdMachines <= 12)
				{
					workstations.Data[2].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[2].JobsCount += activeMachine.JobsCount;
				}
				if (activeMachine.IdMachines >= 13 && activeMachine.IdMachines <= 14)
				{
					workstations.Data[3].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[3].JobsCount += activeMachine.JobsCount;
				}
				if (activeMachine.IdMachines >= 15 && activeMachine.IdMachines <= 16)
				{
					workstations.Data[4].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[4].JobsCount += activeMachine.JobsCount;
				}
				if (activeMachine.IdMachines >= 18 && activeMachine.IdMachines <= 21)
				{
					workstations.Data[5].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[5].JobsCount += activeMachine.JobsCount;
				}
				if (activeMachine.IdMachines >= 22 && activeMachine.IdMachines <= 25)
				{
					workstations.Data[6].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[6].JobsCount += activeMachine.JobsCount;
				}
				if (activeMachine.IdMachines >= 26 && activeMachine.IdMachines <= 27)
				{
					workstations.Data[7].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[7].JobsCount += activeMachine.JobsCount;
				}
				if (activeMachine.IdMachines >= 28 && activeMachine.IdMachines <= 29)
				{
					workstations.Data[8].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[8].JobsCount += activeMachine.JobsCount;
				}
				if (activeMachine.IdMachines >= 30 && activeMachine.IdMachines <= 32)
				{
					workstations.Data[9].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[9].JobsCount += activeMachine.JobsCount;
				}
				if (activeMachine.IdMachines == 33)
				{
					workstations.Data[10].Machines.ForEach(x => x.JobsCount = x.IdMachines == activeMachine.IdMachines ? activeMachine.JobsCount : x.JobsCount);
					workstations.Data[10].JobsCount += activeMachine.JobsCount;
				}
			}

			if (Settings.Default.SelectedWorkstation > 0)
			{
				workstations.Data[Settings.Default.SelectedWorkstation - 1].IsChecked = true;
				workstations.Data[Settings.Default.SelectedWorkstation - 1].MachinesGroupVisibility = System.Windows.Visibility.Visible;
			}

			return workstations;
		}


		#region ORDER
		public static async Task<Models.Order.Datum> GetOrder(string orderId = "", string orderName = "")
		{
			var request = $"/order?per_page=1{(orderId == "" ? $"&name_order={orderName}" : $"&order_id={orderId}")}";
			var req = new RestRequest(request + orderName);
			req.AddHeader("Accept", "application/json");
			req.AddHeader("Authorization", "Bearer " + bearerToken);
			req.AlwaysMultipartFormData = true;
			var res = await Client.GetAsync<OrderModel>(req);
			return res.Data.Count > 0 ? res.Data[0] : null;
		}


		public static async Task<OrderModel> GetOrdersAsync(int page, int perPage, OrderCombineFilterModel filterGroup)
		{
			var today = DateTime.Now.Date.ToString("yyyy-MM-dd");
			var request = $"/order?page={page}&per_page={perPage}";
			foreach (var propertyInfo in filterGroup.GetType().GetProperties())
			{
				var propName = propertyInfo.Name;
				var propValue = propertyInfo.GetValue(filterGroup, null);
				if (propValue == null) continue;
				if (propName == "with_crystals" && propValue.ToString() == "3") continue;
				if (propName == "name_order" && propValue.ToString() == "0") continue;
				if (propValue.ToString() == "") continue;
				switch (propName)
				{
					case "with_shipping_totes" when propValue.ToString() == "True":
						request += "&with_shipping_totes=1";
						continue;
					case "with_shipping_totes" :
						continue;
					case "status_engraving" when propValue.ToString() != "":
						request += $"&with_crystal_product_status[]={propValue}";
						continue;
					case "shipByToday" when propValue.ToString() == "True" && propValue.ToString() != "":
						request += "&with_ship_by_min=true&ship_by_min_before=" + today + " 23:59:59";
						continue;
					case "shipByToday":
						continue;
					case "store_name":
						request += $"&store_name[]={propValue}";
						continue;
					default:
						request += $"&{propName}={propValue}";
						break;
				}
			}
			//Debug.WriteLine(request);
			var req = new RestRequest(request);
			req.AddHeader("Accept", "application/json");
			req.AddHeader("Authorization", "Bearer " + bearerToken);
			req.AlwaysMultipartFormData = true;
			return await Client.GetAsync<OrderModel>(req);
		}

		#endregion

		#region MACHINE

		public static async Task<MachineModel> GetMachines(int productId)
		{
			var req = new RestRequest("/machines?product_id=" + productId);
			req.AddHeader("Accept", "application/json");
			return await Client.GetAsync<MachineModel>(req);
		}


		public static async Task<MachineAssignItemModel> GetMachineAssignItemsAsync(string status, string machineId, string page = "1", string perPage = "15")
		{
			var request = machineId == "All" ? new RestRequest("/machine/assign-item?per_page=" + perPage + "&page=" + page + "&status=" + status + "&sort_by=ended_at_desc", DataFormat.Json) : new RestRequest("/machine/assign-item?per_page=" + perPage + "&page=" + page + "&status=" + status + "&machine_id=" + machineId + "&sort_by=ended_at_desc", DataFormat.Json);
			var res = await Client.GetAsync<MachineAssignItemModel>(request);
			return res;
		}

		public static async Task<MachineAssignItemModel> GetEngravedTodayItemsAsync(string machineId, string page = "1", string perPage = "15")
		{
			var today = DateTime.Now.Date.ToString("yyyy/MM/dd");
			var request = machineId == "All" ? new RestRequest("/machine/assign-item?page=" + page + "&per_page=" + perPage + "&status=success&ended_after=" + today + " 12:00:00&sort_by=ended_at_desc", DataFormat.Json) : new RestRequest("/machine/assign-item?page=" + page + "&per_page=" + perPage + "&status=success&ended_after=" + today + " 12:00:00&machine_id=" + machineId + "&sort_by=ended_at_desc", DataFormat.Json);
			var res = await Client.GetAsync<MachineAssignItemModel>(request);
			return res;
		}

		public static async Task ResolveProductionIssueAsync(ResolveErrorRequestModel requestBody)
		{
			var request = new RestRequest("/resolveError").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			var res = await Client.PostAsync<string>(request);
			Debug.WriteLine(res);

		}
		public static async Task RemoveCurrentJobsFromMachineAsync(int machineId)
		{
			var request = new RestRequest($"/removeCurrentJobs?machine_id={machineId}", Method.GET).AddHeader("Accept", "application/json");
			var res = await Client.ExecuteAsync(request);
			if (res.Content == "0")
			{
				Utils.Notifier.ShowError($"No jobs on machine {machineId} to remove!");
			} else
			{
				Utils.Notifier.ShowSuccess($"{res.Content} jobs were removed from machine {machineId} successfully!");
			}

		}

		public static async Task ChangeMachineAssignItemStatusAsync(NewStatusModel requestBody)
		{
			var request = new RestRequest("/machine/assign-item/status").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			var res = await Client.PostAsync<string>(request);
		}

		#endregion

		#region PRODUCTS

		public static async Task UnassignMachineAssignItemAsync(AssignProcessingModel requestBody)
		{
			var request = new RestRequest("/product/machine/unassign").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			request.AddHeader("Authorization", "Bearer " + bearerToken);
			var res = await Client.DeleteAsync<string>(request);
		}

		public static async Task CancelProductionIssueAsync(ResolveErrorRequestModel requestBody)
		{
			var request = new RestRequest("/product/production-issue/" + requestBody.machine_assign_error_id + "/cancel").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			var res = await Client.PostAsync<string>(request);
		}
		public static async Task<ProductionIssueModel> GetProductionIssuesAsync(string page = "1", string perPage = "15", string orderId = "All", string sortBy = "")
		{
			var request = orderId == "All"
				? new RestRequest("/product/production-issue?per_page=" + perPage + "&page=" + page + "&sort_by=" + sortBy, DataFormat.Json)
				: new RestRequest("/product/production-issue?per_page=" + perPage + "&page=" + page + "&sort_by=" + sortBy + "&order_id=" + orderId, DataFormat.Json);
			request.AddHeader("Cache-Control", "no-cache");
			request.AddHeader("Content-Type", "application/json");
			var res = await Client.GetAsync<ProductionIssueModel>(request);
			return res;
		}
		public static async Task<IssueReasonsModel> GetIssueReasonsAsync()
		{
			var request = new RestRequest("/production-issues/reasons", DataFormat.Json);
			var res = await Client.GetAsync<IssueReasonsModel>(request);
			return res;
		}
		public static async Task ProductAssignProcessing(AssignProcessingModel requestBody)
		{
			var request = new RestRequest("/product/machine/assign-processing").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			request.AddHeader("Authorization", "Bearer " + bearerToken);
			var res = await Client.PostAsync<string>(request);
			Debug.WriteLine(res);
		}
		public static async Task ProductReEngrave(AssignProcessingModel requestBody)
		{
			var request = new RestRequest("/machine/assign-item/re-engrave").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			request.AddHeader("Authorization", "Bearer " + bearerToken);
			var res = await Client.PostAsync<string>(request);
			Debug.WriteLine(res);
		}
		#endregion

		#region SHIPPING
		public static async Task<FindBestServiceModel> FindBestServiceAsync(FindBestServiceRequestModel requestBody)
		{
			var request = new RestRequest("/shipping/findBestService", Method.POST);
			request.AddJsonBody(requestBody);
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Cookie", "XSRF-TOKEN=eyJpdiI6Ik9yRWVJaGdrMHU1QXpKeURcL0QrYndnPT0iLCJ2YWx1ZSI6IkkzRGloZ1M1cUdQK3pEb0VkZXVsWWVtNlJwSGpQVkM5U3dYOEhWbzR5bnFnMm1PTFBMeFlkZWorMlo5S2c0NE10aGxUWkNvbElXeG5YOExUbzFYTnRKOER3ZlZUYnM2U0paNndqMmx6QnF4MXNKK3loeXBIb0NDTG5wTHQ5aXVWIiwibWFjIjoiZGEyZjkzMWYzMGEyNWU2MTcyNGU3ZTNmNWZmOGVlN2QwNDNlOTcxNGVlMDQxZmM5NWQ0NzAyZmMyNzgwYjlkNCJ9; orderarchive_session=eyJpdiI6IjlPQlNrTmRMNFBlWjh6OGE4Qlg4OXc9PSIsInZhbHVlIjoiQTNmODhjaDZ6c2pwaXp1eDZHUGp0SlZGUWdcL1JEdG8wT2VkMFwvQ2VZRGRYSDhmWXJtc1wvMk92TWlvZHNDdm5SYmNKTW5Ocm1YQnp2Nk5CRWFDcHdoVlJhMnhGV3lRWnE1VTBEV2t5c0hFYWdUQ29oWVhHbzhXUmhhUGlTOHJFTGIiLCJtYWMiOiJiYTJjOGRjNjIyZjVmNTBjNTQxYTViYzY3NjkxMmMwMGNhZTEyZjMyMzVmYzAwYjc4MzJiNDlhMzBmNmE2ZTg1In0%3D");
			IRestResponse response = await Client.ExecuteAsync(request);
			return JsonConvert.DeserializeObject<FindBestServiceModel>(response.Content);
			//Debug.WriteLine(res.Success);
		}
		public static async Task<List<Models.Shipping.Datum>> GetShippedTodayOrders()
		{
			var today = DateTime.Now.Date.ToString("yyyy-MM-dd");
			var orderList = new List<Models.Shipping.Datum>();
			var req = new RestRequest("/shipping/order-assign?per_page=100");
			//while last element in the list check updated at date and if it's earlier then 7AM of today's date then stop API calls
			req.AddHeader("Accept", "application/json");
			req.AddHeader("Authorization", "Bearer " + bearerToken);
			req.AlwaysMultipartFormData = true;
			var res = await Client.GetAsync<OrderAssignModel>(req);
			foreach (var order in res.Data)
			{
				if (DateTime.Parse(order.Order.UpdatedAt, CultureInfo.CurrentUICulture).AddHours(-5) >
						DateTime.Parse(today + " 7:00:00", CultureInfo.CurrentUICulture))
					orderList.Add(order);
			}
			var i = 1;
			while (DateTime.Parse(res.Data[res.Data.Count - 1].Order.UpdatedAt, CultureInfo.CurrentUICulture).AddHours(-5)
				> DateTime.Parse(today + " 7:00:00", CultureInfo.CurrentUICulture))
			{
				i++;
				req = new RestRequest($"/shipping/order-assign?per_page=100&page={i}");
				res = await Client.GetAsync<OrderAssignModel>(req);
				res.Data.ForEach(x =>
				{
					if (DateTime.Parse(x.Order.UpdatedAt, CultureInfo.CurrentUICulture).AddHours(-5) >
					    DateTime.Parse(today + " 7:00:00", CultureInfo.CurrentUICulture))
						orderList.Add(x);
				});

			}
			var result = orderList.GroupBy(l => l.User).Select(g => new Models.Shipping.Datum
			{
				User = g.Key,
				ShippedOrdersCount = g.Select(l => l.User).Count()
			});
			var sort = new List<Models.Shipping.Datum>(result);
			foreach (var item in sort)
			{
				Debug.WriteLine($"USER: {item.User}, ORDERS SHIPPED: {item.ShippedOrdersCount}");
			}

			return orderList;
		}
		#endregion

		#region LOGS

		public static async Task GetEntityLogsAsync()
		{
			var request = new RestRequest("/entity-logs?entity_type=machine_assign_item&per_page=100", DataFormat.Json);
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Content-Type", "application/json");
			var res = await Client.GetAsync<EntityLogsModel>(request);
			DateTime startEngravingTime = new DateTime();
			DateTime endEngravingTime = new DateTime(); ;
			TimeSpan engravingGap = new TimeSpan();
			var previousLog = new Models.Logs.Datum();
			var lastIssueLog = new Models.Logs.Datum();

			foreach (var issueLog in res.Data.ToList())
			{
				if (issueLog.Type == "machine_issue" && Settings.Default.LastIssueEntityLogId < issueLog.Id)
				{
					new ToastContentBuilder()
						.AddArgument("action", "viewConversation")
						.AddArgument("conversationId", 9813)
						.AddText($"Machine {issueLog.Data.Machine}: Issue Created")
						.AddText($"Issue: {issueLog.Data.Error}\nEmployee: {issueLog.Data.User}")
						.AddButton(new ToastButton()
							.SetContent("Show Details")
							.AddArgument("action", "like")
							.SetBackgroundActivation())
						.Show();
					Settings.Default.LastIssueEntityLogId = issueLog.Id;
				}
			}


			foreach (var log in res.Data)
			{
				// Check if log id is the same as the last processed one - stop
				if (log.Id <= Settings.Default.LastEntityLogId)
				{
					return;
				}

				//Start looking for engraving start. Previous log must be null.
				if (log.Type == "engraving_start" && previousLog.EventDate == null)
				{
					if (int.Parse(log.Data.Machine) > 25)
					{
						startEngravingTime = DateTime.Parse(log.EventDate).Add(-(TimeSpan.Parse(log.Data.LaserTime)));
					} else
					{
						startEngravingTime = DateTime.Parse(log.EventDate);
					}
					previousLog = previousLog.EventDate != null ? previousLog : log;
					Debug.WriteLine($"START ENGRAVING: {startEngravingTime}");
					continue;
				}
				if (log.Type == "engraving_end" && startEngravingTime.Day == DateTime.Now.Day)
				{
					if (previousLog.Data.Machine == log.Data.Machine)
					{
						endEngravingTime = DateTime.Parse(log.EventDate);
						Debug.WriteLine($"END ENGRAVING: {endEngravingTime}");
					}
				}
				if (endEngravingTime.Day == DateTime.Now.Day && startEngravingTime.Day == DateTime.Now.Day)
				{
					engravingGap = startEngravingTime - endEngravingTime;
					Debug.WriteLine($"ENGRAVING GAP: {engravingGap}\n");
					new ToastContentBuilder()
						.AddArgument("action", "viewConversation")
						.AddArgument("conversationId", 9813)
						.AddText($"Machine {previousLog.Data.Machine}: Time gap between end and start engraving exceeded!")
						.AddText($"Start Engraving: {startEngravingTime}\nEnd Engraving: {endEngravingTime}\nTime Gap: {engravingGap}\nEmployee: {previousLog.Data.User}")
						.AddButton(new ToastButton()
							.SetContent("Got it!")
							.AddArgument("action", "like")
							.SetBackgroundActivation())
						.Show();
					startEngravingTime = new DateTime();
					endEngravingTime = new DateTime();
					Settings.Default.LastEntityLogId = previousLog.Id;
				}

			}


			
		}

		#endregion
	}
}
