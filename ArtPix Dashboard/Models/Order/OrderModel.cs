using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using ArtPix_Dashboard.ViewModels;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using ArtPix_Dashboard.API;
using ArtPix_Dashboard.Models.Logs;
using RestSharp;
using DataFormat = System.Windows.DataFormat;

namespace ArtPix_Dashboard.Models.Order
{
	
	public class ShipstationOrder
	{
		[JsonProperty("id_orders")]
		public int IdOrders { get; set; }

		[JsonProperty("shipstation_id")]
		public int ShipstationId { get; set; }
	}

	[Serializable()]
	public class Comment
	{
		[JsonProperty("product_id")]
		public int ProductId { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("comment")]
		public string Message { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	[Serializable()]
	public class CrystalType
	{
		[JsonProperty("id_crystals")]
		public int IdCrystals { get; set; }

		[JsonProperty("index")]
		public string Index { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("form")]
		public string Form { get; set; }

		[JsonProperty("size")]
		public string Size { get; set; }

		[JsonProperty("price")]
		public double Price { get; set; }

		[JsonProperty("sku")]
		public string Sku { get; set; }

		public int has_3d { get; set; }

		public string Is3d => has_3d == 1 ? "3D" : "2D";

		[JsonProperty("weight_gram")]
		public double WeightGram { get; set; }

		[JsonProperty("size_mm_x")]
		public int SizeMmX { get; set; }

		[JsonProperty("size_mm_y")]
		public int SizeMmY { get; set; }

		[JsonProperty("size_mm_z")]
		public int SizeMmZ { get; set; }
	}

	[Serializable()]
	public class MachineAssignItem
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("machine_assign_id")]
		public int MachineAssignId { get; set; }

		public string machine_id { get; set; }

		[JsonProperty("product_id")]
		public int ProductId { get; set; }

		[JsonProperty("order_id")]
		public int OrderId { get; set; }

		[JsonProperty("order_name")]
		public string OrderName { get; set; }

		[JsonProperty("started_at")]
		public object StartedAt { get; set; }

		[JsonProperty("ended_at")]
		public object EndedAt { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		[JsonProperty("machine_slot")]
		public object MachineSlot { get; set; }

		[JsonProperty("is_qr_printed")]
		public bool IsQrPrinted { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}
	[Serializable()]
	public class Retouch
	{
		[JsonProperty("id_retouchers")]
		public int IdRetouchers { get; set; }

		[JsonProperty("link_to_3d_model")]
		public string LinkTo3dModel { get; set; }

		[JsonProperty("id_user_retoucher")]
		public object IdUserRetoucher { get; set; }

		[JsonProperty("filename")]
		public string Filename { get; set; }

		[JsonProperty("file_url")]
		public string FileUrl { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class BillingAddress
	{
		[JsonProperty("id_address")]
		public int IdAddress { get; set; }

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("last_name")]
		public string LastName { get; set; }

		[JsonProperty("company")]
		public string Company { get; set; }

		[JsonProperty("address_1")]
		public string Address { get; set; }

		[JsonProperty("address_2")]
		public string Address2 { get; set; }

		[JsonProperty("city")]
		public string City { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("postcode")]
		public string Postcode { get; set; }

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("is_verified")]
		public object IsVerified { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class ShippingAddress
	{
		[JsonProperty("id_address")]
		public int IdAddress { get; set; }

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("last_name")]
		public string LastName { get; set; }

		[JsonProperty("company")]
		public string Company { get; set; }

		public string address_1 { get; set; }

		public string address_2 { get; set; }

		[JsonProperty("city")]
		public string City { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("postcode")]
		public string Postcode { get; set; }

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("is_verified")]
		public object IsVerified { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class Customers
	{
		[JsonProperty("id_customers")]
		public int IdCustomers { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("phone")]
		public string Phone { get; set; }

		[JsonProperty("billing_address")]
		public BillingAddress BillingAddress { get; set; }

		[JsonProperty("shipping_address")]
		public ShippingAddress ShippingAddress { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class Box
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }


		[JsonProperty("weight")]
		public double Weight { get; set; }

		[JsonProperty("length_inch")]
		public int LengthInch { get; set; }

		[JsonProperty("width_inch")]
		public int WidthInch { get; set; }

		[JsonProperty("height_inch")]
		public int HeightInch { get; set; }

		[JsonProperty("weight_pound")]
		public double WeightPound { get; set; }

		[JsonProperty("cost")]
		public double Cost { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class WrappingPaper
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("weight")]
		public double Weight { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class ShippingPackage
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("box_id")]
		public int? BoxId { get; set; }

		[JsonProperty("wrapping_paper_id")]
		public int? WrappingPaperId { get; set; }

		[JsonProperty("wrapping_paper_length")]
		public int? WrappingPaperLength { get; set; }

		[JsonProperty("wrapping_paper_count")]
		public int? WrappingPaperCount { get; set; }

		[JsonProperty("fedex_box_alias")]
		public string FedexBoxAlias { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		public string VersionStatusColor => VersionStatus == "Package Not Found" ? "DarkRed" : "DarkGreen";
		public Visibility ShippingPackageInformationVisibility => VersionStatus == "Package Not Found" ? Visibility.Collapsed : Visibility.Visible;

		private string _versionStatus;

		[JsonProperty("version_status")]
		public string VersionStatus
		{
			get => _versionStatus == "not_approved" || _versionStatus == "waiting" ? "Package Not Found" : "Package Approved";
			set => _versionStatus = value;
		}

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }

		[JsonProperty("box")]
		public Box Box { get; set; }

		[JsonProperty("wrapping_paper")]
		public WrappingPaper WrappingPaper { get; set; }
	}

	public class ShippingOrderInfo
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("order_id")]
		public int OrderId { get; set; }

		[JsonProperty("shipping_package_id")]
		public int ShippingPackageId { get; set; }

		[JsonProperty("provider")]
		public object Provider { get; set; }

		[JsonProperty("real_provider")]
		public object RealProvider { get; set; }

		[JsonProperty("service")]
		public object Service { get; set; }

		[JsonProperty("real_service")]
		public object RealService { get; set; }

		[JsonProperty("cost")]
		public object Cost { get; set; }

		[JsonProperty("real_cost")]
		public object RealCost { get; set; }

		[JsonProperty("pickup_at")]
		public object PickupAt { get; set; }

		[JsonProperty("real_pickup_at")]
		public object RealPickupAt { get; set; }

		[JsonProperty("delivery_at")]
		public object DeliveryAt { get; set; }

		[JsonProperty("real_delivery_at")]
		public object RealDeliveryAt { get; set; }

		[JsonProperty("provider_estimate_delivery_at")]
		public object ProviderEstimateDeliveryAt { get; set; }

		[JsonProperty("tracking_number")]
		public object TrackingNumber { get; set; }

		[JsonProperty("label_created_at")]
		public object LabelCreatedAt { get; set; }

		[JsonProperty("shipping_package")]
		public ShippingPackage ShippingPackage { get; set; }
	}

	public class Payment
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("order_id")]
		public int OrderId { get; set; }

		[JsonProperty("payment_method_title")]
		public string PaymentMethodTitle { get; set; }

		[JsonProperty("last_card_digits")]
		public string LastCardDigits { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class Deadline
	{
		[JsonProperty("ship_by")]
		public string ShipBy { get; set; }

		[JsonProperty("model_3d")]
		public string Model3d { get; set; }

		[JsonProperty("retoucher")]
		public string Retoucher { get; set; }

		[JsonProperty("engraving")]
		public string Engraving { get; set; }

		[JsonProperty("ship_by_chicago")]
		public string ShipByChicago { get; set; }

		[JsonProperty("model_3d_chicago")]
		public string Model3dChicago { get; set; }

		[JsonProperty("retoucher_chicago")]
		public string RetoucherChicago { get; set; }

		[JsonProperty("engraving_chicago")]
		public string EngravingChicago { get; set; }
	}

	public class ShippingSlot
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("row_name")]
		public int RowName { get; set; }

		[JsonProperty("column_name")]
		public string ColumnName { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	public class ShippingTote
	{
		public Visibility SlotButtonVisibility => ShippingSlot == null ? Visibility.Collapsed : Visibility.Visible;

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }

		[JsonProperty("shipping_slot")]
		public ShippingSlot ShippingSlot { get; set; }
	}

	public class Links
	{
		[JsonProperty("first")]
		public string First { get; set; }

		[JsonProperty("last")]
		public string Last { get; set; }

		[JsonProperty("prev")]
		public string Prev { get; set; }

		[JsonProperty("next")]
		public string Next { get; set; }
	}

	public class Meta
	{
		[JsonProperty("current_page")]
		public int CurrentPage { get; set; }

		[JsonProperty("from")]
		public int From { get; set; }

		[JsonProperty("last_page")]
		public int LastPage { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("per_page")]
		public int PerPage { get; set; }

		[JsonProperty("to")]
		public int To { get; set; }

		[JsonProperty("total")]
		public int Total { get; set; }
	}

	public class OrderModel
	{

		[JsonProperty("data")]
		public List<Datum> Data { get; set; }

		[JsonProperty("links")]
		public Links Links { get; set; }

		[JsonProperty("meta")]
		public Meta Meta { get; set; }
	}

}
