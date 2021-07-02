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
using Infonet.Reporting.StandardReports.ReportTables.Services.DirectServices;
using LinqKit;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class DirectClientServicesSubReport : SubReportCountBuilder<ServiceDetailOfClient, DirectServiceLineItem> {
		private HashSet<int?> _fundingSourceIds = null;
		private HashSet<int?> _svIds = null;

		public DirectClientServicesSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override void PrePerformSelect(ReportContainer container) {
			_fundingSourceIds = ReportQuery.Filters.OfType<ServiceDetailStaffFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
			_svIds = ReportQuery.Filters.OfType<ServiceDetailStaffFilter>().SingleOrDefault()?.SvIds.NotNull(ids => new HashSet<int?>(ids));
		}

		protected override IEnumerable<DirectServiceLineItem> PerformSelect(IQueryable<ServiceDetailOfClient> query) {
			return query.Select(q => new DirectServiceLineItem {
				ServiceDetailId = q.ServiceDetailID,
				Center = q.Center.CenterName,
				ClientCode = q.ClientCase.Client.ClientCode,
				ClientId = q.ClientID,
				CaseId = q.CaseID,
				ClientTypeId = q.ClientCase.Client.ClientTypeId,
				ServiceId = q.ServiceID,
				ServiceDate = q.ServiceDate,
				ShelterBegDate = q.ShelterBegDate,
				ShelterEndDate = q.ShelterEndDate,
				DaysOfShelter = DbFunctions.DiffDays(q.ShelterBegDate >= ReportContainer.StartDate ? q.ShelterBegDate : ReportContainer.StartDate, q.ShelterEndDate <= ReportContainer.EndDate ? q.ShelterEndDate : ReportContainer.EndDate) + 1,
				ReceivedHours = q.ReceivedHours,
				StaffAndFunding = StaffFunding.ServiceDetail.Invoke(q)
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Client ID", "Case ID", "Client Type", "Service", "Received Hours", "Service Date", "Shelter Begin Date", "Shelter End Date", "Days Sheltered" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, DirectServiceLineItem record) {
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
			csv.WriteField(Lookups.ProgramsAndServices[record.ServiceId].Description);
			csv.WriteField(record.ReceivedHours * averagePercentFundedPerStaff);
			csv.WriteField(record.ServiceDate, "M/d/yyyy");
			csv.WriteField(record.ShelterBegDate, "M/d/yyyy");
			csv.WriteField(record.ShelterEndDate, "M/d/yyyy");
			csv.WriteField(ServiceDetailOfClient.AllShelterIds.Contains(record.ServiceId) ? record.DaysOfShelter : null);
		}

		protected override void CreateReportTables() {
			if (ReportContainer.Provider == Provider.SA) {
				CreateSAReportTables();
				return;
			}

			var directServices = new DirectServiceReportTable("Direct Client Services", 1) {
				Headers = GetDirectServiceHeaders(),
				UseNonDuplicatedSubtotal = true,
				FundingSourceIds = _fundingSourceIds,
				SvIds = _svIds
			};
			foreach (var item in Lookups.DirectOrGroupServices[ReportContainer.Provider])
				directServices.Rows.Add(new ReportRow { Title = item.Description, Code = item.CodeId });
			ReportTableList.Add(directServices);

			if (ReportContainer.Provider == Provider.DV) {
				var shelterServices = new ShelterServiceReportTable("Shelter Services", 2) {
					Headers = GetShelterServiceHeaders(),
					UseNonDuplicatedSubtotal = true
				};
				shelterServices.Rows.Add(new ReportRow { Title = "On-Site Shelter", Code = 66, Order = 1 });
				shelterServices.Rows.Add(new ReportRow { Title = "Off-Site Shelter", Code = 65, Order = 2 });
				ReportTableList.Add(shelterServices);

				var transitionalHousing = new ShelterServiceReportTable("Transitional Housing", 3) {
					Headers = GetShelterServiceHeaders(),
					UseNonDuplicatedSubtotal = true
				};
				transitionalHousing.Rows.Add(new ReportRow { Title = "Transitional Housing", Code = 118 });
				ReportTableList.Add(transitionalHousing);
			}
		}

		private void CreateSAReportTables() {
			var directServicesGroup = new DirectServiceGroupTable("Direct Client Services", 1) { Headers = GetDirectServiceHeaders() };

			var advocacy = new DirectServiceReportTable("Advocacy", 1) {
				Headers = directServicesGroup.Headers,
				UseNonDuplicatedSubtotal = true,
				FundingSourceIds = _fundingSourceIds,
				SvIds = _svIds
			};
            advocacy.Rows.Add(new ReportRow { Code = 127, Title = "Civil Justice" });
			advocacy.Rows.Add(new ReportRow { Code = 92, Title = "Criminal Justice" });
			advocacy.Rows.Add(new ReportRow { Code = 91, Title = "Medical" });
			advocacy.Rows.Add(new ReportRow { Code = 93, Title = "General", Order = 2 });
			directServicesGroup.ReportTables.Add(advocacy);

			var consultation = new DirectServiceReportTable("Other Services", 2) {
				Headers = directServicesGroup.Headers,
				UseNonDuplicatedSubtotal = true,
				FundingSourceIds = _fundingSourceIds,
				SvIds = _svIds
			};
            consultation.Rows.Add(new ReportRow { Code = 61,  Title = "Legal Services/Attorney" });
            consultation.Rows.Add(new ReportRow { Code = 104, Title = "Parent/Guardian Consultation" });
			directServicesGroup.ReportTables.Add(consultation);

			var counseling = new DirectServiceReportTable("Counseling", 3) {
				Headers = directServicesGroup.Headers,
				UseNonDuplicatedSubtotal = true,
				FundingSourceIds = _fundingSourceIds,
				SvIds = _svIds
			};
			counseling.Rows.Add(new ReportRow { Code = 88, Title = "Family" });
			counseling.Rows.Add(new ReportRow { Code = 89, Title = "Group" });
			counseling.Rows.Add(new ReportRow { Code = 87, Title = "Telephone" });
			counseling.Rows.Add(new ReportRow { Code = 90, Title = "In-Person" });
			directServicesGroup.ReportTables.Add(counseling);

			ReportTableList.Add(directServicesGroup);
		}

		private List<ReportTableHeader> GetDirectServiceHeaders() {
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

		private List<ReportTableHeader> GetShelterServiceHeaders() {
			var subHeaders = GetClientTypeSubHeaders(ReportContainer.Provider);
			return new List<ReportTableHeader> {
				new ReportTableHeader {
					Title = "Number Of Clients Receiving Shelter",
					Code = ReportTableHeaderEnum.NumberOfClientsReceivingShelter,
					SubHeaders = subHeaders
				},
				new ReportTableHeader {
					Title = "Days Of Shelter Received",
					Code = ReportTableHeaderEnum.DaysOfShelterReceived,
					SubHeaders = subHeaders
				}
			};
		}
	}

	public class DirectServiceLineItem {
		public int? ServiceDetailId { get; set; }
		public string Center { get; set; }
		public string ClientCode { get; set; }
		public int? ClientId { get; set; }
		public int? CaseId { get; set; }
		public int? ClientTypeId { get; set; }
		public int ServiceId { get; set; }
		public DateTime? ServiceDate { get; set; }
		public double? ReceivedHours { get; set; }
		public DateTime? ShelterBegDate { get; set; }
		public DateTime? ShelterEndDate { get; set; }
		public int? DaysOfShelter { get; set; }
		public IEnumerable<StaffFunding> StaffAndFunding { get; set; }
	}
}