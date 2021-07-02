using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.ReportTables.StaffService;

namespace Infonet.Reporting.ManagementReports.Builders {
	public class Staff_DirectClientServicesSubReport : SubReportCountBuilder<ServiceDetailOfClient, ProgramsAndServicesDirectClientServicesLineItem> {
		public Staff_DirectClientServicesSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
			IsEndOfGroup = true;
		}

		protected override IEnumerable<ProgramsAndServicesDirectClientServicesLineItem> PerformSelect(IQueryable<ServiceDetailOfClient> query) {
			return query.Select(q => new ProgramsAndServicesDirectClientServicesLineItem {
				Id = q.ServiceDetailID,
				ClientID = q.ClientID,
				ClientCode = q.ClientCase.Client.ClientCode,
				CaseID = q.CaseID,
				CenterName = q.Center.CenterName,
				ClientTypeId = q.ClientCase.Client.ClientTypeId,
				ServiceID = q.ServiceID,
				ServiceDate = q.ServiceDate,
				ReceivedHours = q.ReceivedHours
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Client ID", "Case ID", "Client Type", "Service Name", "Received Hours", "Service Date" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ProgramsAndServicesDirectClientServicesLineItem record) {
			csv.WriteField(record.Id);
			csv.WriteField(record.CenterName);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseID);
			csv.WriteField(Lookups.ClientType[record.ClientTypeId]?.Description);
			csv.WriteField(Lookups.ProgramsAndServices[record.ServiceID].Description);
			csv.WriteField(record.ReceivedHours);
			csv.WriteField(record.ServiceDate, "M/d/yyyy");
		}

		protected override void CreateReportTables() {
			var directServices = new DirectClientServicesReportTable("Direct Client Services", 6);
			directServices.Headers = GetHeaders();
			directServices.HideSubheaders = true;
			directServices.PreHeader = "Direct Client Service (All Clients)";
			//KMS DO eliminate
			IEnumerable<ReportLookup> services = ReportContainer.InfonetContext.Database.SqlQuery<ReportLookup>(@"
							SELECT directServicesResults.Description, directServicesResults.ServiceID AS CodeID 
							FROM (	SELECT dbo.LOOKUPLIST_ItemAssignment.CodeID as ServiceID, dbo.TLU_Codes_ProgramsAndServices.Description AS Description , (CASE WHEN dbo.TLU_Codes_ProgramsAndServices.Description = 'Other Advocacy' then 1 ELSE 0 END) as OtherAtBottom 
									FROM dbo.LOOKUPLIST_Tables 
									INNER JOIN dbo.LOOKUPLIST_ItemAssignment 
									ON dbo.LOOKUPLIST_Tables.TableID = dbo.LOOKUPLIST_ItemAssignment.TableID 
									INNER JOIN dbo.TLU_Codes_ProgramsAndServices 
									ON dbo.LOOKUPLIST_ItemAssignment.CodeID = dbo.TLU_Codes_ProgramsAndServices.CodeID 
									WHERE dbo.LOOKUPLIST_ItemAssignment.ProviderID = @p0 
									AND dbo.LOOKUPLIST_ItemAssignment.TableID = 30 
									AND (dbo.TLU_Codes_ProgramsAndServices.IsService = 1 OR dbo.TLU_Codes_ProgramsAndServices.IsGroupService = 1)) directServicesResults 
							WHERE ServiceID NOT IN(65,66,118) 
							ORDER BY OtherAtBottom, Description", (int)ReportContainer.Provider);

			foreach (var item in services.Distinct())
				directServices.Rows.Add(new ReportRow { Title = item.Description, Code = item.CodeId });
			directServices.UseNonDuplicatedSubtotal = true;
			ReportTableList.Add(directServices);
		}

		private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.Hours, Title = "Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.NumberOfClients, Title = "Number Of Clients", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}
	}

	public class ProgramsAndServicesDirectClientServicesLineItem {
		public int? Id { get; set; }
		public int? ClientID { get; set; }
		public string ClientCode { get; set; }
		public int? CaseID { get; set; }
		public int? ClientTypeId { get; set; }
		public int ServiceID { get; set; }
		public DateTime? ServiceDate { get; set; }
		public string CenterName { get; set; }
		public double? ReceivedHours { get; set; }
	}
}