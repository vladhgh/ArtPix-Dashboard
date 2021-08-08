using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ArtPix_Dashboard.Models.AppState;
using ArtPix_Dashboard.Models.EntityLogs;
using ArtPix_Dashboard.Models.FindBestService;
using ArtPix_Dashboard.Models.IssueReasons;
using ArtPix_Dashboard.Models.MachineAssignedItem;
using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.Models.OrderAssignedLogs;
using ArtPix_Dashboard.Models.ProductHistory;
using ArtPix_Dashboard.Models.ProductionIssue;
using ArtPix_Dashboard.Models.Requests;
using ArtPix_Dashboard.Models.Workstation;
using ToastNotifications.Messages;
using ArtPix_Dashboard.Properties;
using Microsoft.Toolkit.Uwp.Notifications;
using DataFormat = RestSharp.DataFormat;
using ArtPix_Dashboard.Models.DhlManifest;
using ArtPix_Dashboard.ViewModels;
using ArtPix_Dashboard.Models.MachineLogs;

namespace ArtPix_Dashboard.API
{

	//EMOJIS: ✅❎

	public class ArtPixAPI
	{

		#region CLIENTS AND TOKENS

		public static string BearerToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiNGU0OGQ0ZmFkZWE0MTc0OGJjMGQ3OGM4ZDQ2ZjNmODlhZTgyMWNkNmUzZjQ0OTJlNzJjNjJhMDg1MTYyYmE3ZWY2NzRlNmRjZjcyMDk3OWQiLCJpYXQiOjE2MjcyOTM0ODUsIm5iZiI6MTYyNzI5MzQ4NSwiZXhwIjoxNjU4ODI5NDg1LCJzdWIiOiIxNiIsInNjb3BlcyI6W119.37YvMLrJKvZfUltaVfrYADOLz0MuYJQezh69k0MSHztpWxm-xZ-NMnhL9PgcRTZ4De4sbMAgBCBGqcBGIoLqLUACSqAjIhU_mI0Al3IQpWnKfw0jchMQwUap1ZwHPDtteEH5Nme8w0qVc5MPibtNFP_K0t6HUBITlRUiScU9waG91__E4uEge_xuE52aMv26wlyj4q-otRatB63UaqtmcAUcgFGqcYp1bJLNZh936-LlWT6P5Vfrg4oh1n6WKxKyB7vb6-oEz1ydknaCjCkVPHb4j8UkiuTBYh6B17sd7zbrrZTDNwAEBAWwHNxzWFurtW6aZHHZ8O_vEZLZ2Tvf7aiAs_-05YUZ0RkkEB3R7cAuAf4qjvQdqJBXTh4l80NNr7VaJ1XkFC_I24U-rh6-4XEurYclK2j24v7GOGMdTQISscDpdNjxvrRIvZ-iT21N5FHlZt8Z2bIVgz3Q_C4VKV4XJkgwI_YcxVkFWGrnDytRK7OpTOkcQYYwm3zNeWmaQyQPBah2EKmZeqPaiK89UeMQ7RaHnWSg6GrS7KM6zamgm15xkLeJ_FLtf3GcRkB85qFIzqRKFxxnZg3EuXuY-JRURNEpgb0DaMwDngVt4grweKq0z5ejPsKnNQwIsmtMVZquroGPW2ezvz4yRS-ui85zIpArPzwqohw8iohqV9I";

		private static readonly RestClient Client = new ("https://order-archive.artpix3d.com/api/v1");

		private static readonly RestClient ClientCp = new ("https://confirmation.artpix3d.com/api");

		#endregion

		#region GET: PRODUCT HISTORY

		public static async Task<ProductHistoryModel> GetProductHistoryAsync(int productId)
		{
			var req = new RestRequest("/order-item-history/" + productId.ToString());
			req.AddHeader("Accept", "application/json");
			req.AlwaysMultipartFormData = true;
			var res = await ClientCp.GetAsync<ProductHistoryModel>(req);
			return res;
		}

		#endregion

		#region GET: ENGRAVING STATS

		public static async Task<EngravingStatsModel> GetEngravingStatsAsync()
		{
			var machineAssignItems = await Task.WhenAll(
				GetOrdersAsync(new CombinedFilterModel("Ready To Engrave"){perPage = 1}),
				GetOrdersAsync(new CombinedFilterModel("Engraving In Progress"){perPage = 1}),
				GetOrdersAsync(new CombinedFilterModel("Awaiting Model"){perPage = 1}),
				GetOrdersAsync(new CombinedFilterModel("Production Issues") { perPage = 1 }));
			var engravedToday = await GetEngravedTodayItemsAsync("", "1", "1");
			var stats = new EngravingStatsModel
			{
				ReadyToEngraveCount = machineAssignItems[0].Meta.Total,
				ProcessingCount = machineAssignItems[1].Meta.Total,
				AwaitingModelCount = machineAssignItems[2].Meta.Total,
				EngravedTodayCount = engravedToday.Meta.Total,
				IssueCount = machineAssignItems[3].Meta.Total
			};
			return stats;
		}

		#endregion

		#region GET: SHIPPING  STATS

		public static async Task<ShippingStatsModel> GetShippingStatsAsync()
		{
			var awaitingShipmentModel = new CombinedFilterModel("Awaiting Shipment");
			var shipByTodayModel = new CombinedFilterModel("Ship By Today");
			var readyToShip = new CombinedFilterModel("Ready To Ship");
			var orders = await Task.WhenAll(GetOrdersAsync(awaitingShipmentModel), GetOrdersAsync(shipByTodayModel), GetOrdersAsync(readyToShip));
			var shippedOrders = await GetShippedTodayOrders();

			var result = shippedOrders.GroupBy(l => l.User).Select(g => new Models.OrderAssignedLogs.Datum
			{
				User = g.Key,
				ShippedOrdersCount = g.Select(l => l.User).Count()
			});
			var sort = new List<Models.OrderAssignedLogs.Datum>(result);
			var ordersShippedByUser = sort.Aggregate("", (current, item) => current + $"USER: {item.User}, ORDERS SHIPPED: {item.ShippedOrdersCount}\n");

			var stats = new ShippingStatsModel
			{
				AwaitingShipment = orders[0].Meta.Total,
				ShipByToday = orders[1].Meta.Total,
				ReadyToShip = orders[2].Meta.Total,
				ShippedToday = shippedOrders.Count,
				OrdersShipped = ordersShippedByUser
			};
			return stats;
		}

		#endregion

		#region PATCH: UPDATE SHIPPING ADDRESS

		public static async Task<OrderModel> UpdateShippingAddress(string orderId)
		{
			var request = $"/order/{orderId}/shipping-address";
			var res = await Client.PatchAsync<OrderModel>(new RestRequest(request, DataFormat.Json).AddHeader("Accept", "application/json").AddHeader("Authorization", "Bearer " + BearerToken));
			return res;
		}

		#endregion

		#region GET: ACTIVE MACHINES

		//TODO: CHANGE ENDPOINT TO /ORDER

		public static async Task<List<Machine>> GetActiveMachines()
		{
			var items = await GetMachineAssignItemsAsync("processing", "All", "1" , "100");
			var machinesList = items.Data.Select(item => new Machine { Name = item.MachineId}).ToList();
			var i = 1;
			while (items.Meta.LastPage > i)
			{
				i++;
				items = await GetMachineAssignItemsAsync("processing", "All", i.ToString(), "100");
				machinesList.AddRange(items.Data.Select(item => new Machine {Name = item.MachineId}));
			}
			var res = machinesList.GroupBy(l => l.Name).Select(g => new Machine
			{
				Name = g.Key,
				IdMachines = Int32.Parse(g.Key),
				JobsCount = g.Select(l => l.Name).Count()
			});
			
			return res.ToList();
		}


		#endregion

		#region GET: WORKSTATIONS

		//TODO: GET CURRENTLY ENGRAVING ORDERS FROM GET /ORDER ENDPOINT AND ENSURE PROPER FUNCTIONING

		public static async Task<WorkstationsModel> GetWorkstationStats()
		{
			var req = new RestRequest("/work-stations");
			//Debug.WriteLine($"API GET: /work-stations");
			req.AddHeader("Accept", "application/json");
			var networkAddresses = Utils.Utils.GetAllMacAddressesAndIppairs();
			var workstations = await Client.GetAsync<WorkstationsModel>(req);
			var activeMachines = await GetActiveMachines();
			var online = new Dictionary<int, bool>();

			foreach (var item in Utils.Utils.MachineAddresses)
			{
				online[int.Parse(item.Value)] = networkAddresses.Find(p => p.MacAddress.ToUpper() == item.Key).MacAddress != null;
			}
			foreach (var machine in workstations.Data.SelectMany(workstation => workstation.Machines))
			{
				if (online.TryGetValue(machine.IdMachines, out bool val))
				{
					machine.NetworkStatus = val ? "Online" : "Offline";
					//Debug.WriteLine("Fetched value: {0}", val);
				}
				else
				{
					Debug.WriteLine("No such key: {0}", machine.IdMachines);
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

		#endregion

		#region GET: ORDER

		public static async Task<Models.Order.Datum> GetOrder(string orderId = "", string orderName = "")
		{
			var request = $"/order?per_page=1{(orderId == "" ? $"&name_order={orderName}" : $"&order_id={orderId}")}";
			var req = new RestRequest(request + orderName);
			req.AddHeader("Accept", "application/json");
			req.AddHeader("Authorization", "Bearer " + BearerToken);
			req.AlwaysMultipartFormData = true;
			Debug.WriteLine($"API GET: {request}");
			var res = await Client.GetAsync<OrderModel>(req);
			return res.Data.Count > 0 ? res.Data[0] : null;
		}

		#endregion

		#region POST: CREATE DHL MANIFEST

		public static async Task<DhlManifestModel> CreateDhlManifest(CreateDhlManifestRequest request)
		{
			var apiRequest = "/shipping/shipping-manifests/dhl";
			var req = new RestRequest(apiRequest).AddJsonBody(request).AddHeader("Accept", "application/json").AddHeader("Authorization", "Bearer " + BearerToken);
			req.AlwaysMultipartFormData = true;
			Debug.WriteLine($"API POST: {apiRequest}");
			var res =  await Client.PostAsync<DhlManifestModel>(req);
			return res;
		}

		#endregion

		#region GET: ORDERS

		public static async Task<OrderModel> GetOrdersAsync(CombinedFilterModel combinedFilter)
		{
		var today = DateTime.Now.Date.ToString("yyyy-MM-dd");
			var request = $"/order?page={combinedFilter.pageNumber}&per_page={combinedFilter.perPage}";
			foreach (var propertyInfo in combinedFilter.GetType().GetProperties())
			{
				var propName = propertyInfo.Name;
				var propValue = propertyInfo.GetValue(combinedFilter, null);
				if (propValue == null) continue;
				if (propertyInfo.PropertyType == typeof(Visibility)) continue;
				if (propertyInfo.PropertyType == typeof(bool)) continue;
				if (propName == "with_crystals" && propValue.ToString() == "3") continue;
				if (propName == "name_order" && propValue.ToString() == "0") continue;
				if (propValue.ToString() == "") continue;
				if (propName == "SelectedFilterGroup" || propName == "pageNumber" || propName == "perPage") continue;
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
			Debug.WriteLine($"API GET: {request}");
			var req = new RestRequest(request);
			req.AddHeader("Accept", "application/json");
			req.AddHeader("Authorization", "Bearer " + BearerToken);
			req.AlwaysMultipartFormData = true;
			return await Client.GetAsync<OrderModel>(req);
		}

		#endregion

		#region GET: MACHINES

		public static async Task<List<Machine>> GetMachines(int productId)
		{
			var req = new RestRequest("/machines?product_id=" + productId);
			req.AddHeader("Accept", "application/json");
			var res = await Client.GetAsync<MachineModel>(req);
			return res.Data;
		}

		#endregion

		#region GET: NEXT ORDERS

		public static async Task GetNextOrderAsync(string machineId, string count)
		{
			var req = new RestRequest($"/getNextOrders?machine_id={machineId}&count={count}");
			req.AddHeader("Accept", "application/json");
			Debug.WriteLine($"API GET: /getNextOrders?machine_id={machineId}&count={count} ");
			var res = await Client.GetAsync<string>(req);
			Debug.WriteLine(res);
		}

		#endregion

		#region GET: MACHINE ASSIGNED ITEMS - DONE - ✅

		public static async Task<MachineAssignItemModel> GetMachineAssignItemsAsync(string status, string machineId, string page = "1", string perPage = "15")
		{
			var request = machineId == "All" ? "/machine/assign-item?per_page=" + perPage + "&page=" + page + "&status=" + status + "&sort_by=ended_at_desc" : "/machine/assign-item?per_page=" + perPage + "&page=" + page + "&status=" + status + "&machine_id=" + machineId + "&sort_by=ended_at_desc";
			//Debug.WriteLine($"API GET: {request}");
			var req = new RestRequest(request, DataFormat.Json);
			var res = await Client.GetAsync<MachineAssignItemModel>(req);
			return res;
		}

		#endregion

		#region GET: ENGRAVED TODAY ITEMS

		public static async Task<MachineAssignItemModel> GetEngravedTodayItemsAsync(string machineId, string page = "1", string perPage = "15")
		{
			var today = DateTime.Now.Date.ToString("yyyy/MM/dd");
			var request = machineId == "" ? ("/machine/assign-item?page=" + page + "&per_page=" + perPage + "&status=success&ended_after=" + today + " 12:00:00&sort_by=ended_at_desc")
				: ("/machine/assign-item?page=" + page + "&per_page=" + perPage + "&status=success&ended_after=" + today + " 12:00:00&machine_id=" + machineId + "&sort_by=ended_at_desc");
			Debug.WriteLine($"API GET: {request}");
			var req = new RestRequest(request, DataFormat.Json);
			var res = await Client.GetAsync<MachineAssignItemModel>(req);
			return res;
		}

		#endregion

		#region POST: RESOLVE PRODUCTION ISSUE - DONE - ✅

		public static async Task ResolveProductionIssueAsync(ResolveProductionIssueRequest request)
		{
			var apiRequest = new RestRequest("/resolveError").AddJsonBody(request).AddHeader("Accept", "application/json");
			Debug.WriteLine($"API POST: /resolveError");
			var response = await Client.PostAsync<string>(apiRequest);
			Debug.WriteLine($"API RESPONSE: {response}");

		}

		#endregion

		#region GET: REMOVE CURRENT JOBS FROM MACHINE - DONE - ✅

		public static async Task RemoveCurrentJobsFromMachineAsync(string machineId)
		{
			var request = new RestRequest($"/removeCurrentJobs?machine_id={machineId}", Method.GET).AddHeader("Accept", "application/json");
			Debug.WriteLine($"API GET: /removeCurrentJobs?machine_id={machineId}");
			var res = await Client.ExecuteAsync(request);
			if (res.Content == "0")
			{
				Utils.Utils.Notifier.ShowError($"No jobs on machine {machineId} to remove!");
			} else
			{
				Utils.Utils.Notifier.ShowSuccess($"{res.Content} jobs were removed from machine {machineId} successfully!");
			}
		}

		#endregion

		#region POST: CHANGE MACHINE ASSIGNED ITEM STATUS - DONE - ✅

		public static async Task ChangeMachineAssignItemStatusAsync(ChangeMachineAssignItemStatusRequest request)
		{
			var apiRequest = new RestRequest("/machine/assign-item/status").AddJsonBody(request).AddHeader("Accept", "application/json");
			Debug.WriteLine($"API POST: /machine/assign-item/status");
			var response = await Client.PostAsync<string>(apiRequest);
			Debug.WriteLine($"API RESPONSE: {response}");
		}

		#endregion

		#region DELETE: UNASSIGN MACHINE ASSIGNED ITEM - DONE - ✅

		public static async Task UnassignMachineAssignItemAsync(ChangeMachineAssignItemStatusRequest requestBody)
		{
			var request = new RestRequest("/product/machine/unassign").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			Debug.WriteLine($"API DELETE: {request}");
			request.AddHeader("Authorization", "Bearer " + BearerToken);
			var res = await Client.DeleteAsync<string>(request);
			Debug.WriteLine($"API RESPONSE: {res}");
		}

		#endregion

		#region POST: CANCEL PRODUCTION ISSUE - DONE  - ✅ - NOT USED

		public static async Task CancelProductionIssueAsync(ResolveProductionIssueRequest requestBody)
		{
			var request = new RestRequest("/product/production-issue/" + requestBody.machine_assign_error_id + "/cancel").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			var res = await Client.PostAsync<string>(request);
		}

		#endregion

		#region GET: PRODUCTION ISSUES - DONE - ✅

		public static async Task<ProductionIssueModel> GetProductionIssuesAsync(string page = "1", string perPage = "15", string orderId = "All", string sortBy = "")
		{
			var request = orderId == "All"
				? new RestRequest("/product/production-issue?per_page=" + perPage + "&page=" + page + "&sort_by=" + sortBy, DataFormat.Json)
				: new RestRequest("/product/production-issue?per_page=" + perPage + "&page=" + page + "&sort_by=" + sortBy + "&order_id=" + orderId, DataFormat.Json);
			request.AddHeader("Cache-Control", "no-cache");
			request.AddHeader("Content-Type", "application/json");
			Debug.WriteLine("API GET: " + "/product/production-issue?per_page=" + perPage + "&page=" + page + "&sort_by=" + sortBy + "&order_id=" + orderId);
			var res = await Client.GetAsync<ProductionIssueModel>(request);
			return res;
		}

		#endregion

		#region POST: ADD PRODUCTION ISSUE - DONE - ✅

		public static async Task AddProductionIssueAsync(AddProductionIssueRequest request)
		{
			var apiRequest = new RestRequest("/product/production-issue", DataFormat.Json).AddJsonBody(request).AddHeader("Cache-Control", "no-cache").AddHeader("Content-Type", "application/json");
			await Client.PostAsync<string>(apiRequest);

		}

		#endregion

		#region GET: ISSUE REASONS - DONE - ✅

		public static async Task<IssueReasonsModel> GetIssueReasonsAsync()
		{
			var request = new RestRequest("/production-issues/reasons", DataFormat.Json);
			var res = await Client.GetAsync<IssueReasonsModel>(request);
			return res;
		}

		#endregion

		#region POST: ASSIGN MACHINE - DONE - ✅

		public static async Task ProductAssignProcessing(ChangeMachineAssignItemStatusRequest requestBody)
		{
			var request = new RestRequest("/product/machine/assign-processing").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			request.AddHeader("Authorization", "Bearer " + BearerToken);
			Debug.WriteLine($"API POST: /product/machine/assign-processing");
			var res = await Client.PostAsync<string>(request);
			Debug.WriteLine($"API RESPONSE: {res}");
		}

		#endregion

		#region POST: RE-ENGRAVE PRODUCT

		public static async Task ProductReEngrave(ChangeMachineAssignItemStatusRequest requestBody)
		{
			var request = new RestRequest("/machine/assign-item/re-engrave").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			request.AddHeader("Authorization", "Bearer " + BearerToken);
			Debug.WriteLine($"API POST: /machine/assign-item/re-engrave");
			var res = await Client.PostAsync<string>(request);
			Debug.WriteLine($"API RESPONSE: {res}");
		}

		#endregion

		#region POST: FIND BEST SERVICE

		public static async Task<FindBestServiceModel> FindBestServiceAsync(FindBestServiceRequest requestBody)
		{
			var request = new RestRequest("/shipping/findBestService", Method.POST);
			request.AddJsonBody(requestBody);
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Cookie", "XSRF-TOKEN=eyJpdiI6Ik9yRWVJaGdrMHU1QXpKeURcL0QrYndnPT0iLCJ2YWx1ZSI6IkkzRGloZ1M1cUdQK3pEb0VkZXVsWWVtNlJwSGpQVkM5U3dYOEhWbzR5bnFnMm1PTFBMeFlkZWorMlo5S2c0NE10aGxUWkNvbElXeG5YOExUbzFYTnRKOER3ZlZUYnM2U0paNndqMmx6QnF4MXNKK3loeXBIb0NDTG5wTHQ5aXVWIiwibWFjIjoiZGEyZjkzMWYzMGEyNWU2MTcyNGU3ZTNmNWZmOGVlN2QwNDNlOTcxNGVlMDQxZmM5NWQ0NzAyZmMyNzgwYjlkNCJ9; orderarchive_session=eyJpdiI6IjlPQlNrTmRMNFBlWjh6OGE4Qlg4OXc9PSIsInZhbHVlIjoiQTNmODhjaDZ6c2pwaXp1eDZHUGp0SlZGUWdcL1JEdG8wT2VkMFwvQ2VZRGRYSDhmWXJtc1wvMk92TWlvZHNDdm5SYmNKTW5Ocm1YQnp2Nk5CRWFDcHdoVlJhMnhGV3lRWnE1VTBEV2t5c0hFYWdUQ29oWVhHbzhXUmhhUGlTOHJFTGIiLCJtYWMiOiJiYTJjOGRjNjIyZjVmNTBjNTQxYTViYzY3NjkxMmMwMGNhZTEyZjMyMzVmYzAwYjc4MzJiNDlhMzBmNmE2ZTg1In0%3D");
			IRestResponse response = await Client.ExecuteAsync(request);
			Debug.WriteLine(response.Content);
			return JsonConvert.DeserializeObject<FindBestServiceModel>(response.Content);
		}

		#endregion

		#region GET: SHIPPED TODAY ORDERS

		public static async Task<List<Models.OrderAssignedLogs.Datum>> GetShippedTodayOrders()
		{
			var today = DateTime.Now.Date.ToString("yyyy-MM-dd");
			var orderList = new List<Models.OrderAssignedLogs.Datum>();
			var req = new RestRequest("/shipping/order-assign?per_page=100");
			//while last element in the list check updated at date and if it's earlier then 7AM of today's date then stop API calls
			req.AddHeader("Accept", "application/json");
			req.AddHeader("Authorization", "Bearer " + BearerToken);
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
			return orderList;
		}

		#endregion

		#region GET: ENTITY LOGS

		public static async Task GetEntityLogsAsync()
		{
			var request = new RestRequest("/entity-logs?entity_type=machine_assign_item&per_page=25", DataFormat.Json);
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Content-Type", "application/json");
			var res = await Client.GetAsync<EntityLogsModel>(request);
			DateTime startEngravingTime = new DateTime();
			DateTime endEngravingTime = new DateTime(); ;
			TimeSpan engravingGap = new TimeSpan();
			var previousLog = new Models.EntityLogs.Datum();
			var lastIssueLog = new Models.EntityLogs.Datum();

			foreach (var issueLog in res.Data.ToList())
			{
				if (issueLog.Type == "machine_issue" && Settings.Default.LastIssueEntityLogId < issueLog.Id)
				{
					new ToastContentBuilder()
						.AddArgument("action", "openIssue")
						.AddArgument("orderId", issueLog.Data.OrderIds[0])
						.AddText($"Machine {issueLog.Data.Machine}: Issue Created")
						.AddText($"Issue: {issueLog.Data.Error}\nEmployee: {issueLog.Data.User}")
						.AddButton(new ToastButton()
							.SetContent("Show Details")
							.SetBackgroundActivation())
						.Show();
					Settings.Default.LastIssueEntityLogId = issueLog.Id;
				}
			}

			/*
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
					if (int.Parse(log.Data.Machine) > 25 && int.Parse(log.Data.Machine) != 34)
					{
						startEngravingTime = DateTime.Parse(log.EventDate).Add(-(TimeSpan.Parse(log.Data.LaserTime)));
					} else
					{
						startEngravingTime = DateTime.Parse(log.EventDate);
					}
					previousLog = !String.IsNullOrEmpty(previousLog.EventDate) ? previousLog : log;
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

			}*/
		}

		#endregion

		#region GET: ENGRAVED TODAY ITEMS FROM ENTITY LOGS

		public static async Task<EntityLogsModel> GetEngravedTodayItemsEntityLogsAsync(int jobsCount)
		{
			var today = DateTime.Now.Date.ToString("yyyy/MM/dd") + " 12:00:00";
			var request = new RestRequest($"/entity-logs?page=1&entity_type=machine_assign_item&type=engraving_end&per_page={jobsCount + 50}", DataFormat.Json);
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Content-Type", "application/json");
			var logs = await Client.GetAsync<EntityLogsModel>(request);
			foreach (var log in logs.Data.ToList())
			{
				if (DateTime.Parse(log.EventDate, CultureInfo.CurrentUICulture) < DateTime.Parse(today, CultureInfo.CurrentUICulture))
				{
					logs.Data.Remove(log);
				}
			}

			return logs;

		}

		#endregion

		#region GET: MACHINE LOGS

		public static async Task<MachineLogsModel> GetMachineLogsLogsAsync(int page, int perPage, string employee)
		{
			var today = DateTime.Now.Date.ToString("yyyy-MM-dd") + " 12:00:00";
			var request = new RestRequest($"/machine-logs?type_logs=get_next_crystal_request&machine_user={employee}&date_after={today}&per_page={perPage}&page={page}", DataFormat.Json);
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Content-Type", "application/json");
			MachineLogsModel machineLogs = await Client.GetAsync<MachineLogsModel>(request);
			return machineLogs;

		}

		#endregion

		#region GET: PRODUCTION ISSUE

		public static async Task<ProductionIssueModel> GetProductionIssueAsync(string machineAssignItemId)
		{
			var request = new RestRequest($"/product/production-issue?machine_assign_item_id={machineAssignItemId}",
				RestSharp.DataFormat.Json);
			Debug.WriteLine($"API GET: {request}");
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Content-Type", "application/json");
			var res = await Client.GetAsync<ProductionIssueModel>(request);
			return res;
		}

		#endregion

	}
}
