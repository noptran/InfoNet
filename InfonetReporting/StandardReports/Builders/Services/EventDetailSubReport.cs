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
using Infonet.Reporting.StandardReports.ReportTables.Services.CommunityGroup;
using LinqKit;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class EventDetailSubReport : SubReportCountBuilder<EventDetail, EventDetailLineItem> {
		private HashSet<int?> _fundingSourceIds = null;

		public EventDetailSubReport(SubReportSelection subReportType) : base(subReportType) {
			IsInGroup = true;
			IsEndOfGroup = false;
		}

		protected override void PrePerformSelect(ReportContainer container) {
			_fundingSourceIds = ReportQuery.Filters.OfType<EventDetailFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
		}

		protected override IEnumerable<EventDetailLineItem> PerformSelect(IQueryable<EventDetail> query) {
			var staffPredicate = PredicateBuilder.New<EventDetailStaff>(true);
			var svIds = ReportQuery.Filters.OfType<EventDetailStaffFilter>().SingleOrDefault()?.SvIds;
			if (svIds != null)
				staffPredicate.And(pds => svIds.Contains(pds.SVID));

			return query.Select(q => new EventDetailLineItem {
				IcsId = q.ICS_ID,
				Center = q.Center.CenterName,
				ProgramId = q.ProgramID,
				EventName = q.EventName,
				EventDate = q.EventDate,
				EventHours = q.EventHours,
				NumOfPeopleReached = q.NumPeopleReached,
				Staff = q.EventDetailStaff.AsQueryable().Where(staffPredicate).Select(eds => new StaffLineItem {
					SvId = eds.SVID,
					ConductHours = eds.HoursConduct ?? 0,
					PrepHours = eds.HoursPrep ?? 0,
					TravelHours = eds.HoursTravel ?? 0,
					Funding = StaffFunding.EventDetailStaff.Invoke(eds)
				})
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Program", "Event Name", "Event Date", "Number of People Reached", "Number of Staff", "Event Hours", "Staff Conduct Hours", "Staff Travel Hours", "Staff Prep Hours" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, EventDetailLineItem record) {
			csv.WriteField(record.IcsId);
			csv.WriteField(record.Center);
			csv.WriteField(Lookups.ProgramsAndServices[record.ProgramId]?.Description);
			csv.WriteField(record.EventName);
			csv.WriteField(record.EventDate, "M/d/yyyy");
			csv.WriteField(record.NumOfPeopleReached);
			csv.WriteField(record.Staff.Select(s => s.SvId).Distinct().Count());
			csv.WriteField(record.EventHours);
			csv.WriteField(_fundingSourceIds == null
				? record.Staff.Sum(s => s.ConductHours)
				: record.Staff.Sum(s => s.ConductHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0)));
			csv.WriteField(_fundingSourceIds == null
				? record.Staff.Sum(s => s.TravelHours)
				: record.Staff.Sum(s => s.TravelHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0)));
			csv.WriteField(_fundingSourceIds == null
				? record.Staff.Sum(s => s.PrepHours)
				: record.Staff.Sum(s => s.PrepHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0)));
		}

		protected override void CreateReportTables() {
			var eventTable = new EventDetailReportTable("Event Information", 8) {
				Headers = GetHeaders(),
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true,
				FundingSourceIds = _fundingSourceIds
			};
			eventTable.Rows.AddRange(Lookups.EventTypes[ReportContainer.Provider].Select(i => new ReportRow { Title = i.Description, Code = i.CodeId }));
			ReportTableList.Add(eventTable);
		}

		private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.EventNumberOfEvents, Title = "Number of Events", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.EventNumberOfPeopleReached, Title = "Number of People Reached", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.NumberOfStaff, Title = "Number of Staff", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.EventHours, Title = "Event Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.StaffConductHours, Title = "Staff Conduct Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.StaffTravelHours, Title = "Staff Travel Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.StaffPreparationHours, Title = "Staff Preperation Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}
	}

	public class EventDetailLineItem {
		public int IcsId { get; set; }
		public string Center { get; set; }
		public int? ProgramId { get; set; }
		public string EventName { get; set; }
		public DateTime? EventDate { get; set; }
		public double? EventHours { get; set; }
		public int? NumOfPeopleReached { get; set; }
		public IEnumerable<StaffLineItem> Staff { get; set; }
	}
}