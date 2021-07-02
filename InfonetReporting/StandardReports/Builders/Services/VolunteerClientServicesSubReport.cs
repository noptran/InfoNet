using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using Infonet.Reporting.StandardReports.ReportTables.Services.Volunteer;
using LinqKit;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class VolunteerClientServicesSubReport : SubReportCountBuilder<ServiceDetailOfClient, ClientServiceLineItem> {
		private readonly Dictionary<int?, string> _volunteerNames = new Dictionary<int?, string>();
		private HashSet<int?> _fundingSourceIds = null;
		private HashSet<int?> _svIds = null;

		public VolunteerClientServicesSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
		}

		protected override void PrePerformSelect(ReportContainer container) {
			_fundingSourceIds = ReportQuery.Filters.OfType<ServiceDetailStaffFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
			_svIds = ReportQuery.Filters.OfType<ServiceDetailStaffFilter>().SingleOrDefault()?.SvIds.NotNull(ids => new HashSet<int?>(ids));

			var locationIds = container.CenterIds;
			var volunteers = container.InfonetContext.T_StaffVolunteer.Where(sv => locationIds.Contains(sv.CenterId) && sv.TypeId == 2);
			if (_svIds != null)
				volunteers = volunteers.Where(sv => _svIds.Contains(sv.SvId));
			foreach (var each in volunteers)
				_volunteerNames[each.SvId] = (each.FirstName + " " + each.LastName).Trim();
		}

		protected override IEnumerable<ClientServiceLineItem> PerformSelect(IQueryable<ServiceDetailOfClient> query) {
			return query.Select(q => new ClientServiceLineItem {
				ServiceDetailId = q.ServiceDetailID,
				Center = q.Center.CenterName,
				ServiceId = q.ServiceID,
				ServiceDate = q.ServiceDate,
				ReceivedHours = q.ReceivedHours,
				StaffAndFunding = StaffFunding.ServiceDetail.Invoke(q)
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Service Name", "Service Date", "Volunteer(s)", "Received Hours" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ClientServiceLineItem record) {
			var recordSvIds = record.StaffAndFunding.Select(sf => sf.SvId).Distinct().ToArray();
			var recordVolunteerSvIds = _volunteerNames.Keys.Intersect(recordSvIds);

			double averagePercentFundedPerStaff = 1;
			if (_fundingSourceIds != null) {
				int staffCount = recordSvIds.Length;
				int percentFundedSum = record.StaffAndFunding.Where(sf => sf.FundingSourceId != null && _fundingSourceIds.Contains(sf.FundingSourceId) && _volunteerNames.ContainsKey(sf.SvId)).Sum(sf => sf.PercentFund ?? 0);
				averagePercentFundedPerStaff = percentFundedSum / 100.0 / staffCount;
			}

			csv.WriteField(record.ServiceDetailId);
			csv.WriteField(record.Center);
			csv.WriteField(Lookups.ProgramsAndServices[record.ServiceId].Description);
			csv.WriteField(record.ServiceDate, "M/d/yyyy");
			csv.WriteField(string.Join("|", recordVolunteerSvIds.Select(i => _volunteerNames[i]).OrderBy(sn => sn)));
			csv.WriteField(record.ReceivedHours * averagePercentFundedPerStaff);
		}

		protected override void CreateReportTables() {
			var table = new VolunteerClientServicesReportTable("Direct Client Services", 1) {
				PreHeader = "Volunteer Information (Subset of all services reflected above) ",
				Headers = GetHeaders(),
				HideSubheaders = true,
				HideSubtotal = true,
				FundingSourceIds = _fundingSourceIds,
				VolunteerSvIds = _volunteerNames.Keys
			};
			table.Rows.Add(new ReportRow { Code = (int)ReportTableHeaderEnum.HoursOfService, Title = "Hours Of Direct Client Service" });
			table.Rows.Add(new ReportRow { Code = (int)ReportTableHeaderEnum.NumberOfContacts, Title = "Number Of Contacts" });
			ReportTableList.Add(table);
		}

		private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> { new ReportTableHeader { Code = ReportTableHeaderEnum.Total, Title = string.Empty, SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } } };
		}
	}

	public class ClientServiceLineItem {
		public int? ServiceDetailId { get; set; }
		public string Center { get; set; }
		public int ServiceId { get; set; }
		public DateTime? ServiceDate { get; set; }
		public double? ReceivedHours { get; set; }
		public IEnumerable<StaffFunding> StaffAndFunding { get; set; }
	}
}