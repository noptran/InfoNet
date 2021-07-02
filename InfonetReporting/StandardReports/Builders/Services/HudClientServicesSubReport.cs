using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Infonet.Core;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using Infonet.Reporting.StandardReports.ReportTables.Services.Hud;
using LinqKit;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class HudClientServicesSubReport : SubReportCountBuilder<ServiceDetailOfClient, HudDirectServiceLineItem> {
		private HashSet<int?> _fundingSourceIds = null;
		private HashSet<int?> _svIds = null;

		public HudClientServicesSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override void PrePerformSelect(ReportContainer container) {
			_fundingSourceIds = ReportQuery.Filters.OfType<ServiceDetailStaffFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
			_svIds = ReportQuery.Filters.OfType<ServiceDetailStaffFilter>().SingleOrDefault()?.SvIds.NotNull(ids => new HashSet<int?>(ids));
		}

		protected override IEnumerable<HudDirectServiceLineItem> PerformSelect(IQueryable<ServiceDetailOfClient> query) {
			return query.Select(q => new HudDirectServiceLineItem {
				ServiceDetailId = q.ServiceDetailID,
				Center = q.Center.CenterName,
				ClientCode = q.ClientCase.Client.ClientCode,
				ClientId = q.ClientID,
				CaseId = q.CaseID,
				ClientTypeId = q.ClientCase.Client.ClientTypeId,
				ServiceId = q.ServiceID,
				HudServices = q.TLU_Codes_ProgramsAndServices.HudServices.Select(hs => hs.HudServiceId),
				ServiceDate = q.ServiceDate,
				ShelterBegDate = q.ShelterBegDate,
				ShelterEndDate = q.ShelterEndDate,
				DaysOfShelter = DbFunctions.DiffDays(q.ShelterBegDate >= ReportContainer.StartDate ? q.ShelterBegDate : ReportContainer.StartDate, q.ShelterEndDate <= ReportContainer.EndDate ? q.ShelterEndDate : ReportContainer.EndDate) + 1,
				ReceivedHours = q.ReceivedHours,
				StaffAndFunding = StaffFunding.ServiceDetail.Invoke(q)
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Client ID", "Case ID", "Client Type", "HUD Service(s)", "Received Hours", "Service Date", "Shelter Begin Date", "Shelter End Date", "Days Sheltered" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, HudDirectServiceLineItem record) {
			double averagePercentFundedPerStaff = 1;
			if (_fundingSourceIds != null) {
				int staffCount = record.StaffAndFunding.Select(sf => sf.SvId).Distinct().Count();
				int percentFundedSum = record.StaffAndFunding.Where(sf => sf.FundingSourceId != null && _fundingSourceIds.Contains(sf.FundingSourceId) && (_svIds?.Contains(sf.SvId) ?? true)).Sum(sf => sf.PercentFund ?? 0);
				averagePercentFundedPerStaff = percentFundedSum / 100.0 / staffCount;
			}

			csv.WriteField(record.ServiceDetailId);
			csv.WriteField(record.Center);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(Lookups.ClientType[record.ClientTypeId]?.Description);
			csv.WriteField(string.Join("|", record.HudServices.Select(hs => Lookups.HudServices[hs]).OrderBy(lc => lc.Entries[ReportContainer.Provider]).Select(lc => lc.Description)));
			csv.WriteField(record.ReceivedHours * averagePercentFundedPerStaff);
			csv.WriteField(record.ServiceDate, "M/d/yyyy");
			csv.WriteField(record.ShelterBegDate, "M/d/yyyy");
			csv.WriteField(record.ShelterEndDate, "M/d/yyyy");
			csv.WriteField(ServiceDetailOfClient.AllShelterIds.Contains(record.ServiceId) ? record.DaysOfShelter : null);
		}

		protected override void CreateReportTables() {
			var directServices = new HudClientServicesReportTable("Direct Client Services", 1) {
				Headers = GetHeaders(),
				UseNonDuplicatedSubtotal = true,
				FundingSourceIds = _fundingSourceIds,
				SvIds = _svIds
			};
			foreach (var item in Lookups.HudServices[ReportContainer.Provider])
				directServices.Rows.Add(new ReportRow { Title = item.Description, Code = item.CodeId, Order = item.Entries[ReportContainer.Provider].DisplayOrder });
			ReportTableList.Add(directServices);
		}

		private List<ReportTableHeader> GetHeaders() {
			var subHeaders = GetClientTypeSubHeaders(ReportContainer.Provider);
			return new List<ReportTableHeader> {
				new ReportTableHeader {
					Title = "Number of Clients Receiving Service",
					Code = ReportTableHeaderEnum.NumberOfClientsReceivingServices,
					SubHeaders = subHeaders
				},
				new ReportTableHeader {
					Title = "Number of Contacts",
					Code = ReportTableHeaderEnum.NumberOfContacts,
					SubHeaders = subHeaders
				},
				new ReportTableHeader {
					Title = "Hours of Service",
					Code = ReportTableHeaderEnum.HoursOfService,
					SubHeaders = subHeaders
				}
			};
		}
	}

	public class HudDirectServiceLineItem {
		public int? ServiceDetailId { get; set; }
		public string Center { get; set; }
		public string ClientCode { get; set; }
		public int? ClientId { get; set; }
		public int? CaseId { get; set; }
		public int? ClientTypeId { get; set; }
		public int ServiceId { get; set; }
		public IEnumerable<int> HudServices { get; set; }
		public DateTime? ServiceDate { get; set; }
		public double? ReceivedHours { get; set; }
		public DateTime? ShelterBegDate { get; set; }
		public DateTime? ShelterEndDate { get; set; }
		public int? DaysOfShelter { get; set; }
		public IEnumerable<StaffFunding> StaffAndFunding { get; set; }
	}
}