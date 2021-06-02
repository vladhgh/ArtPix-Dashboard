using ArtPix_Dashboard.Models.Types;
using ArtPix_Dashboard.Models.Machine;
using ArtPix_Dashboard.Models.Order;
using ArtPix_Dashboard.Models.Product;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArtPix_Dashboard.Models;
using ArtPix_Dashboard.Models.ProductHistory;
using ArtPix_Dashboard.Models.Shipping;
using RestSharp.Extensions;

namespace ArtPix_Dashboard.Utils
{
	public class ArtPixAPI
	{
		public static string bearerToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiZGZkMWFjZWIyN2EyOGU2ODliMjE2ZThjOGMyZTU1YTkxODgzODM5MDAwNGM2OWM1ZDE2OWU5NDQzMzRjZWRjZmRlOGJiNTljOGJjN2Y2OWUiLCJpYXQiOjE2MDAzMTg4OTksIm5iZiI6MTYwMDMxODg5OSwiZXhwIjoxNjMxODU0ODk5LCJzdWIiOiIxNiIsInNjb3BlcyI6W119.QWBy29NAIWiYQzU6QI4XLFYaZTZnxVj_Jw0z3uzv7WSapgLJTi1pa3sy57d9DEqnOG_H2bfLjrJv3yP48Ix86Q_CwkW5YME40ZlafbJzT1203Rf4fB46dFUzhtbSmVMrQfc6bYP77naN6Ev06TGjiLIZ2SYrM94scCFLdN1tJEdZdqYI3EEbABWaGIr2g1jok5G_3T9VHnrWOWYOdffaOH7w7kpl6_QTGaX5e3Z3XDYynKJAgub4xTaHptLxbNq1EpCjNvOcw1vjTV22MKj-1p3IWOWjXNtVelIOkNx1VE1w--7hAlaIaOHYoSbslqXYla1aSfCLQ6P4EPMYqvsKTArYBpnVG6LM_XQhQK-iNKmYoJa1ZIVTlSc9fQwE7z9hnmOZCcjqgc-kbN6f6DShboe0sjaAA12o-lxjbmIjPdobruDBKMIFsQ5boCrURWR2kaKIe2uATJFTImuE54C_8H2vfu2mOcH_UUPU9u3B6dLQCCk2cjLuPPj6ZOLy4DKrwafk14cvVFtMxRxu2u6ICVz0pztRzxBHo9zZTMKeW7JsZsZvhYeI4A16-7jtzhDF5lhQAHXQ_oOHsvXjLND_XDfGkTAue3Z4GS1O_7GmkvdV2URbHoD0wfNHFDvxl2ecph3u18A40jntJbgFd3ey4hHtq-IwXJaj0RM6AV_nVZ8";

		public static RestClient client = new RestClient("https://order-archive.artpix3d.com/api/v1");
		public static RestClient clientCP = new RestClient("https://confirmation.artpix3d.com/api");

		public static async Task<ProductHistoryModel> GetProductHistoryAsync(int productId)
		{
			var req = new RestRequest("/order-item-history/" + productId.ToString());
			req.AddHeader("Accept", "application/json");
			req.AlwaysMultipartFormData = true;
			var res = await clientCP.GetAsync<ProductHistoryModel>(req);
			return res;
		}

		public static async Task<StatsModel> GetAllStatsAsync()
		{
			var machineAssignItems = await Task.WhenAll(GetMachineAssignItemsAsync("ready_to_engrave", "All" ,"1", "1"), GetMachineAssignItemsAsync("processing", "All", "1", "1"), GetMachineAssignItemsAsync("engraved", "All", "1", "1"), GetEngravedTodayItemsAsync("All", "1", "1"));
			var productionIssues = await GetProductionIssuesAsync("1", "1");
			var stats = new StatsModel
			{
				ReadyToEngraveCount = machineAssignItems[0].Meta.Total - 5,
				ProcessingCount = machineAssignItems[1].Meta.Total,
				EngravedCount = machineAssignItems[2].Meta.Total,
				EngravedTodayCount = machineAssignItems[3].Meta.Total,
				IssueCount = productionIssues.Meta.Total
			};
			return stats;
		}

		public static async Task<List<Machine>> GetActiveMachines()
		{
			var items = await GetMachineAssignItemsAsync("processing", "All", "1" , "100");
			var machinesList = items.Data.Select(item => new Machine {Name = item.MachineId}).ToList();
			var res = machinesList.GroupBy(l => l.Name).Select(g => new Machine
			{
				Name = g.Key,
				JobsCount = g.Select(l => l.Name).Count()
			});
			var sort = new List<Machine>(res);
			
			return sort.OrderBy(x => x.Name).ToList();
		}


		#region ORDER
		public static async Task<Models.Order.Datum> GetOrder(string orderId = "", string orderName = "")
		{
			var request = $"/order?per_page=1{(orderId == "" ? $"&name_order={orderName}" : $"&order_id={orderId}")}";
			var req = new RestRequest(request + orderName);
			req.AddHeader("Accept", "application/json");
			req.AddHeader("Authorization", "Bearer " + bearerToken);
			req.AlwaysMultipartFormData = true;
			var res = await client.GetAsync<OrderModel>(req);
			return res.Data.Count > 0 ? res.Data[0] : null;
		}
		public static async Task<OrderModel> GetOrdersAsync(string statusOrder = "", string statusShipping = "", string page = "1", string perPage = "15",
			string hasShippingPackage = "", string withShippingTotes = "", string withProductionIssue = "", string sortBy = "", string shipByToday = "",
			string storeName = "", string statusEngraving = "", string nameOrder = "", string withCrystal = "3")
		{
			var today = DateTime.Now.Date.ToString("yyyy-MM-dd");
			var request = $"/order?page={page}&per_page={perPage}";
			if (withCrystal != "3")
			{
				request += $"&with_crystals={withCrystal}";
			}
			if (statusOrder != "")
			{
				request += $"&status_order={statusOrder}";
			}
			if (statusShipping != "")
			{
				request += $"&status_shipping={statusShipping}";
			}
			if (nameOrder != "")
			{
				request += $"&name_order={nameOrder}";
			}
			if (shipByToday == "True")
			{
				request += ("&estimate_processing_max_date_before=" + today + " 23:59:59");
			}

			if (statusEngraving != "")
			{
				request += $"&with_crystal_product_status[]={statusEngraving}";
			}
			if (storeName != "")
			{
				request += $"&store_name[]={storeName}";
			}
			if (withShippingTotes == "True")
			{
				request += "&with_shipping_totes=1";
			}
			if (sortBy != "")
			{
				request += $"&sort_by={sortBy}";
			}
			if (hasShippingPackage != "")
			{
				request += $"&has_shipping_package={hasShippingPackage}";
			}
			if (withProductionIssue != "")
			{
				request += $"&with_production_issue={withProductionIssue}";
			}
			var req = new RestRequest(request);
			req.AddHeader("Accept", "application/json");
			req.AddHeader("Authorization", "Bearer " + bearerToken);
			req.AlwaysMultipartFormData = true;
			return await client.GetAsync<OrderModel>(req);
		}

		#endregion

		#region MACHINE

		public static async Task<MachineModel> GetMachines(int productId)
		{
			var req = new RestRequest("/machines?product_id=" + productId);
			req.AddHeader("Accept", "application/json");
			return await client.GetAsync<MachineModel>(req);
		}


		public static async Task<MachineAssignItemModel> GetMachineAssignItemsAsync(string status, string machineId, string page = "1", string perPage = "15")
		{
			var request = machineId == "All" ? new RestRequest("/machine/assign-item?per_page=" + perPage + "&page=" + page + "&status=" + status + "&sort_by=ended_at_desc", DataFormat.Json) : new RestRequest("/machine/assign-item?per_page=" + perPage + "&page=" + page + "&status=" + status + "&machine_id=" + machineId + "&sort_by=ended_at_desc", DataFormat.Json);
			var res = await client.GetAsync<MachineAssignItemModel>(request);
			return res;
		}

		public static async Task<MachineAssignItemModel> GetEngravedTodayItemsAsync(string machineId, string page = "1", string perPage = "15")
		{
			var today = DateTime.Now.Date.ToString("yyyy/MM/dd");
			var request = machineId == "All" ? new RestRequest("/machine/assign-item?page=" + page + "&per_page=" + perPage + "&status=success&ended_after=" + today + " 12:00:00&sort_by=ended_at_desc", DataFormat.Json) : new RestRequest("/machine/assign-item?page=" + page + "&per_page=" + perPage + "&status=success&ended_after=" + today + " 12:00:00&machine_id=" + machineId + "&sort_by=ended_at_desc", DataFormat.Json);
			var res = await client.GetAsync<MachineAssignItemModel>(request);
			return res;
		}

		public static async Task ResolveProductionIssueAsync(ResolveErrorRequestModel requestBody)
		{
			var request = new RestRequest("/resolveError").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			var res = await client.PostAsync<string>(request);
			Debug.WriteLine(res);

		}
		
		public static async Task ChangeMachineAssignItemStatusAsync(NewStatusModel requestBody)
		{
			var request = new RestRequest("/machine/assign-item/status").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			var res = await client.PostAsync<string>(request);
		}

		#endregion

		#region PRODUCTS

		public static async Task UnassignMachineAssignItemAsync(AssignProcessingModel requestBody)
		{
			var request = new RestRequest("/product/machine/unassign").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			request.AddHeader("Authorization", "Bearer " + bearerToken);
			var res = await client.DeleteAsync<string>(request);
		}

		public static async Task CancelProductionIssueAsync(ResolveErrorRequestModel requestBody)
		{
			var request = new RestRequest("/product/production-issue/" + requestBody.machine_assign_error_id + "/cancel").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			var res = await client.PostAsync<string>(request);
		}
		public static async Task<ProductionIssueModel> GetProductionIssuesAsync(string page = "1", string perPage = "15", string orderId = "All", string sortBy = "")
		{
			var request = orderId == "All"
				? new RestRequest("/product/production-issue?per_page=" + perPage + "&page=" + page + "&sort_by=" + sortBy, DataFormat.Json)
				: new RestRequest("/product/production-issue?per_page=" + perPage + "&page=" + page + "&sort_by=" + sortBy + "&order_id=" + orderId, DataFormat.Json);
			request.AddHeader("Cache-Control", "no-cache");
			request.AddHeader("Content-Type", "application/json");
			var res = await client.GetAsync<ProductionIssueModel>(request);
			return res;
		}
		public static async Task<IssueReasonsModel> GetIssueReasonsAsync()
		{
			var request = new RestRequest("/production-issues/reasons", DataFormat.Json);
			var res = await client.GetAsync<IssueReasonsModel>(request);
			return res;
		}
		public static async Task ProductAssignProcessing(AssignProcessingModel requestBody)
		{
			var request = new RestRequest("/product/machine/assign-processing").AddJsonBody(requestBody).AddHeader("Accept", "application/json");
			request.AddHeader("Authorization", "Bearer " + bearerToken);
			var res = await client.PostAsync<string>(request);
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
			IRestResponse response = await client.ExecuteAsync(request);
			return JsonConvert.DeserializeObject<FindBestServiceModel>(response.Content);
			//Debug.WriteLine(res.Success);
		}
		#endregion
	}
}
