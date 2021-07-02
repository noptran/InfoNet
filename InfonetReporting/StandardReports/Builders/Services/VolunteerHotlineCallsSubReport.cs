using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Core.IO;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using Infonet.Reporting.StandardReports.ReportTables.Services.Volunteer;
using LinqKit;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class VolunteerHotlineCallsSubReport : SubReportCountBuilder<PhoneHotline, HotlineItem> {
		private HashSet<int?> _fundingSourceIds = null;

		public VolunteerHotlineCallsSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
		}

		protected override void PrePerformSelect(ReportContainer container) {
			_fundingSourceIds = ReportQuery.Filters.OfType<HotlineFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
		}

		protected override IEnumerable<HotlineItem> PerformSelect(IQueryable<PhoneHotline> query) {
			return query.Select(h => new HotlineItem {
				Id = h.PH_ID,
				Center = h.Center.CenterName,
				Volunteer = (h.StaffVolunteer.FirstName + " " + h.StaffVolunteer.LastName).Trim(),
				Date = h.Date,
				TotalTime = h.TotalTime,
				NumberOfContacts = h.NumberOfContacts,
				StaffAndFunding = StaffFunding.Hotline.Invoke(h)
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Volunteer", "Date", "Total Time", "Number of Contacts" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, HotlineItem record) {
			double percentFunded = 1;
			if (_fundingSourceIds != null) {
				int percentFundedSum = record.StaffAndFunding.Where(sf => sf.FundingSourceId != null && _fundingSourceIds.Contains(sf.FundingSourceId)).Sum(sf => sf.PercentFund ?? 0);
				percentFunded = percentFundedSum / 100.0;
			}

			csv.WriteField(record.Id);
			csv.WriteField(record.Center);
			csv.WriteField(record.Volunteer);
			csv.WriteField(record.Date, "M/d/yyyy");
			csv.WriteField(record.TotalTime * percentFunded);
			csv.WriteField(record.NumberOfContacts);
		}

		protected override void CreateReportTables() {
			var hotlineGroup = new VolunteerHotlineCallsReportTable("Hotline Calls", 2) {
				Headers = GetHeaders(),
				HideSubheaders = true,
				HideSubtotal = true,
				FundingSourceIds = _fundingSourceIds
			};
			hotlineGroup.Rows.Add(new ReportRow { Code = (int)ReportTableHeaderEnum.HoursOfService, Title = "Hours Of Service" });
			hotlineGroup.Rows.Add(new ReportRow { Code = (int)ReportTableHeaderEnum.NumberOfContacts, Title = "Number Of Contacts" });
			ReportTableList.Add(hotlineGroup);
		}

		private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> { new ReportTableHeader { Code = ReportTableHeaderEnum.Total, Title = string.Empty, SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } } };
		}
	}

	public class HotlineItem {
		public int? Id { get; set; }
		public string Center { get; set; }
		public string Volunteer { get; set; }
		public DateTime? Date { get; set; }
		public double? TotalTime { get; set; }
		public int? NumberOfContacts { get; set; }
		public IEnumerable<StaffFunding> StaffAndFunding { get; set; }
	}
}