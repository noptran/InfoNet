using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Medical.OrderOfProtection;

namespace Infonet.Reporting.StandardReports.Builders.MedicalCJ {
	public class MedicalCJOrdersOfProtectionSubReport : SubReportCountBuilder<OrderOfProtection, MedicalCJOrderofProtectionLineItem> {
		public MedicalCJOrdersOfProtectionSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override IEnumerable<MedicalCJOrderofProtectionLineItem> PerformSelect(IQueryable<OrderOfProtection> query) {
			var db = ReportContainer.InfonetContext;

			var ret = query.Select(q => new MedicalCJOrderofProtectionLineItem {
				Center = db.T_Center.Where(c => c.CenterID == q.LocationID).Select(c => c.CenterName).FirstOrDefault(),
				ClientID = q.ClientId,
				ClientCode = q.ClientCase.Client.ClientCode,
				TypeOfOPID = q.TypeOfOPID,
				ForumID = q.ForumID,
				OP_ID = q.OP_ID,
				StatusID = q.StatusID,
				OpActivities = q.OrderOfProtectionActivities,
				DateVacated = q.DateVacated,
				MaxNewExpirationDate = q.OrderOfProtectionActivities.Max(op => op.NewExpirationDate),
				OriginalExpirationDate = q.OriginalExpirationDate,
				ValidUpgradedOpActivities = q.OrderOfProtectionActivities.Where(op => op.OpActivityDate.HasValue && op.OpActivityDate >= ReportContainer.StartDate.Value && op.OpActivityDate <= ReportContainer.EndDate.Value && (op.OpActivityCodeID == (int)OpActivityId.EmergencyToInterim || op.OpActivityCodeID == (int)OpActivityId.EmergencyToPlenary || op.OpActivityCodeID == (int)OpActivityId.InterimToPlenary)),
				HasOpActivityInRange = q.OrderOfProtectionActivities.Any(op => op.OpActivityDate.HasValue && op.OpActivityDate >= ReportContainer.StartDate.Value && op.OpActivityDate <= ReportContainer.EndDate.Value && (op.OpActivityCodeID == (int)OpActivityId.EmergencyToInterim || op.OpActivityCodeID == (int)OpActivityId.EmergencyToPlenary || op.OpActivityCodeID == (int)OpActivityId.InterimToPlenary)),
				NumberOfUpgrades = q.OrderOfProtectionActivities.Any() ? q.OrderOfProtectionActivities.Where(op => op.OpActivityDate.HasValue && op.OpActivityDate >= ReportContainer.StartDate.Value && op.OpActivityDate <= ReportContainer.EndDate.Value && (op.OpActivityCodeID == (int)OpActivityId.EmergencyToInterim || op.OpActivityCodeID == (int)OpActivityId.EmergencyToPlenary || op.OpActivityCodeID == (int)OpActivityId.InterimToPlenary)).Select(a => a.OpActivityCodeID).Count() : 0,
				IsActiveOrder = q.DateIssued.HasValue && q.DateIssued <= ReportContainer.EndDate,
				IsUpgradedOrder = q.OrderOfProtectionActivities.Any(op => op.OpActivityCodeID == (int)OpActivityId.EmergencyToInterim || op.OpActivityCodeID == (int)OpActivityId.EmergencyToPlenary || op.OpActivityCodeID == (int)OpActivityId.InterimToPlenary),
				IsNewFiled = q.DateFiled.HasValue && q.DateFiled >= ReportContainer.StartDate && q.DateFiled <= ReportContainer.EndDate,
				IsNewIssued = q.DateIssued.HasValue && q.DateIssued >= ReportContainer.StartDate && q.DateIssued <= ReportContainer.EndDate,
				IsValidOrder = (q.DateVacated ?? (q.OrderOfProtectionActivities.Any(op => op.NewExpirationDate.HasValue) ? q.OrderOfProtectionActivities.Max(op => op.NewExpirationDate) : q.OriginalExpirationDate ?? ReportContainer.EndDate.Value)) >= ReportContainer.StartDate
			});

			return ret;
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Client ID", "Status of Order", "Type of Order", "Forum of Order", "Order Activities", "Valid Upgrade Activities", "Is Valid Order", "Is Active Order", "Is New Order Issued", "Is New Order Filed", "Is Upgraded Order", "Date Vacated" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, MedicalCJOrderofProtectionLineItem record) {
			csv.WriteField(record.OP_ID);
			csv.WriteField(record.Center);
			csv.WriteField(record.ClientCode);
			csv.WriteField(Lookups.OrderOfProtectionStatus[record.StatusID]?.Description);
			csv.WriteField(Lookups.OrderOfProtectionType[record.TypeOfOPID]?.Description);
			csv.WriteField(Lookups.OrderOfProtectionForum[record.ForumID]?.Description);
			csv.WriteField(string.Join("|", record.OpActivities.Select(a => Lookups.OrderOfProtectionActivity[a.OpActivityCodeID].Description)));
			csv.WriteField(string.Join("|", record.ValidUpgradedOpActivities.Select(a => Lookups.OrderOfProtectionActivity[a.OpActivityCodeID].Description)));
			csv.WriteField(record.IsValidOrder);
			csv.WriteField(record.IsActiveOrder);
			csv.WriteField(record.IsNewIssued);
			csv.WriteField(record.IsNewFiled);
			csv.WriteField(record.IsUpgradedOrder);
			csv.WriteField(record.DateVacated, "M/d/yyyy");
		}

		protected override void CreateReportTables() {
			var numberAndPercentHeader = GetHeaders();

			var newOpsFiled = new MedicalCJOPNewOrdersReportTable("", 1);
			newOpsFiled.PreHeader = "New Orders of Protection";
			newOpsFiled.Headers = numberAndPercentHeader;
			newOpsFiled.HideSubheaders = true;
			newOpsFiled.HideSubtotal = true;
			newOpsFiled.Rows.Add(new ReportRow { Title = "Number Of orders filed this reporting period", Code = null });
			ReportTableList.Add(newOpsFiled);

			var newOpsVictims = new MedicalCJOPVictimsFiledOrdersReportTable("", 2);
			newOpsVictims.Headers = numberAndPercentHeader;
			newOpsVictims.HideSubheaders = true;
			newOpsVictims.HideSubtotal = true;
			newOpsVictims.Rows.Add(new ReportRow { Title = "Number Of victims who filed orders this period", Code = null });
			ReportTableList.Add(newOpsVictims);

			var statusOfOP = new MedicalCJOPStatusOfOrdersReportTable("Status of Orders Filed This Period", 3);
			statusOfOP.Headers = numberAndPercentHeader;
			statusOfOP.HideSubheaders = true;
			statusOfOP.HideSubtotal = true;
			foreach (var item in Lookups.OrderOfProtectionStatus[ReportContainer.Provider])
				statusOfOP.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(statusOfOP);

			var typeOfOP = new MedicalCJOPTypeOfOrdersReportTable("Type of Orders Issued This Period", 4);
			typeOfOP.Headers = numberAndPercentHeader;
			typeOfOP.HideSubheaders = true;
			typeOfOP.HideSubtotal = true;
			foreach (var item in Lookups.OrderOfProtectionType[ReportContainer.Provider])
				typeOfOP.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(typeOfOP);

			var forumOfOP = new MedicalCJOPForumOfOrdersReportTable("Forum of Orders Issued This Period", 5);
			forumOfOP.Headers = numberAndPercentHeader;
			forumOfOP.HideSubheaders = true;
			forumOfOP.HideSubtotal = true;
			foreach (var item in Lookups.OrderOfProtectionForum[ReportContainer.Provider])
				forumOfOP.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(forumOfOP);

			var ordersUpgraded = new MedicalCJOPOrderUpgradesReportTable("", 6);
			ordersUpgraded.PreHeader = "Upgrade in the Type of Order";
			ordersUpgraded.Headers = numberAndPercentHeader;
			ordersUpgraded.HideSubheaders = true;
			ordersUpgraded.HideSubtotal = true;
			ordersUpgraded.Rows.Add(new ReportRow { Title = "Number of orders upgraded made this period", Code = null });
			ReportTableList.Add(ordersUpgraded);

			var orderUpgradesMade = new MedicalCJOPOrderUpgradesMadeReportTable("", 7);
			orderUpgradesMade.Headers = numberAndPercentHeader;
			orderUpgradesMade.HideSubheaders = true;
			orderUpgradesMade.HideSubtotal = true;
			orderUpgradesMade.Rows.Add(new ReportRow { Title = "Number of upgrades made this period", Code = null });
			ReportTableList.Add(orderUpgradesMade);

			var orderUpgradeTypes = new MedicalCJOPTypeOfUpgradeReportTable("Type Of Upgrades", 8);
			orderUpgradeTypes.Headers = numberAndPercentHeader;
			orderUpgradeTypes.HideSubheaders = true;
			orderUpgradeTypes.Rows.Add(new ReportRow { Title = "Emergency to Interim", Code = (int)OpActivityId.EmergencyToInterim });
			orderUpgradeTypes.Rows.Add(new ReportRow { Title = "Emergency to Plenary", Code = (int)OpActivityId.EmergencyToPlenary });
			orderUpgradeTypes.Rows.Add(new ReportRow { Title = "Interim to Plenary", Code = (int)OpActivityId.InterimToPlenary });
			ReportTableList.Add(orderUpgradeTypes);

			var activeOrders = new MedicalCJOPActiveOrdersReportTable("", 9);
			activeOrders.PreHeader = "Active Orders of Protection Only";
			activeOrders.Headers = numberAndPercentHeader;
			activeOrders.HideSubheaders = true;
			activeOrders.HideSubtotal = true;
			activeOrders.Rows.Add(new ReportRow { Title = "Number of active orders", Code = null });
			ReportTableList.Add(activeOrders);

			var victimsWithActiveOrders = new MedicalCJOPVictimsWithActiveOrdersReportTable("", 10);
			victimsWithActiveOrders.Headers = numberAndPercentHeader;
			victimsWithActiveOrders.HideSubheaders = true;
			victimsWithActiveOrders.HideSubtotal = true;
			victimsWithActiveOrders.Rows.Add(new ReportRow { Title = "Number of victims with active orders", Code = null });
			ReportTableList.Add(victimsWithActiveOrders);

			var orderActivityTable = new MedicalCJOPOrderActivityReportTable("Order Activity", 11);
			orderActivityTable.Headers = numberAndPercentHeader;
			orderActivityTable.HideSubheaders = true;
			orderActivityTable.HideSubtotal = true;
			orderActivityTable.StartDate = ReportContainer.StartDate;
			orderActivityTable.EndDate = ReportContainer.EndDate;
			orderActivityTable.Rows.Add(new ReportRow { Title = "Expired", Code = (int)OPOrderActivity.Expired });
			orderActivityTable.Rows.Add(new ReportRow { Title = "Vacated", Code = (int)OPOrderActivity.Vacated });
			orderActivityTable.Rows.Add(new ReportRow { Title = "Extended", Code = (int)OPOrderActivity.Extended });
			orderActivityTable.Rows.Add(new ReportRow { Title = "Modified", Code = (int)OPOrderActivity.Modified });
			ReportTableList.Add(orderActivityTable);

			var orderViolationTable = new MedicalCJOPViolationsReportTable("Violations", 11);
			orderViolationTable.Headers = numberAndPercentHeader;
			orderViolationTable.HideSubheaders = true;
			orderViolationTable.HideSubtotal = true;
			orderViolationTable.StartDate = ReportContainer.StartDate;
			orderViolationTable.EndDate = ReportContainer.EndDate;
			orderViolationTable.Rows.Add(new ReportRow { Title = "Orders without any violation", Code = (int)Violations.NoViolation });
			orderViolationTable.Rows.Add(new ReportRow { Title = "Orders violated without police charge", Code = (int)Violations.ViolationWithoutPoliceCharge });
			orderViolationTable.Rows.Add(new ReportRow { Title = "Orders violated with police charge", Code = (int)Violations.ViolationWithPoliceCharge });
			ReportTableList.Add(orderViolationTable);
		}

		private List<ReportTableHeader> GetHeaders() {
			var headers = new List<ReportTableHeader>();
			headers.Add(new ReportTableHeader { Code = ReportTableHeaderEnum.Number, Title = "Number", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } });
			headers.Add(new ReportTableHeader { Code = ReportTableHeaderEnum.Percent, Title = "%", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } });
			return headers;
		}
	}

	public class MedicalCJOrderofProtectionLineItem {
		public string Center { get; set; }
		public int? ClientID { get; set; }
		public string ClientCode { get; set; }
		public int? OP_ID { get; set; }
		public int? ForumID { get; set; }
		public int? TypeOfOPID { get; set; }
		public bool IsValidOrder { get; set; }
		public int? StatusID { get; set; }
		public bool IsNewIssued { get; set; }
		public bool? IsNewFiled { get; set; }
		public bool HasOpActivityInRange { get; set; }
		public DateTime? DateVacated { get; set; }
		public IEnumerable<OpActivity> ValidUpgradedOpActivities { get; set; }
		public bool IsUpgradedOrder { get; internal set; }
		public int NumberOfUpgrades { get; internal set; }
		public bool IsActiveOrder { get; internal set; }
		public DateTime? MaxNewExpirationDate { get; internal set; }
		public DateTime? OriginalExpirationDate { get; internal set; }
		public IEnumerable<OpActivity> OpActivities { get; internal set; }
	}
}