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
	public class PublicationSubReport : SubReportCountBuilder<PublicationDetail, PublicationLineItem> {
		private HashSet<int?> _fundingSourceIds = null;

		public PublicationSubReport(SubReportSelection subReportType) : base(subReportType) {
			IsInGroup = true;
			IsEndOfGroup = true;
		}

		protected override void PrePerformSelect(ReportContainer container) {
			_fundingSourceIds = ReportQuery.Filters.OfType<PublicationDetailFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
		}

		protected override IEnumerable<PublicationLineItem> PerformSelect(IQueryable<PublicationDetail> query) {
			var staffPredicate = PredicateBuilder.New<PublicationDetailStaff>(true);
			var svIds = ReportQuery.Filters.OfType<PublicationDetailStaffFilter>().SingleOrDefault()?.SvIds;
			if (svIds != null)
				staffPredicate.And(pds => svIds.Contains(pds.SVID));

			return query.Select(q => new PublicationLineItem {
				IcsId = q.ICS_ID,
				Center = q.Center.CenterName,
				ProgramId = q.ProgramID,
				Title = q.Title,
				PublicationDate = q.PDate,
				NumberOfSegments = q.NumOfBrochure ?? 0,
				PrepareHours = q.PrepareHours ?? 0,
				Staff = q.PublicationDetailStaff.AsQueryable().Where(staffPredicate).Select(pds => new StaffLineItem {
					SvId = (int)pds.SVID,
					PrepHours = pds.HoursPrep ?? 0,
					Funding = StaffFunding.PublicationDetailStaff.Invoke(pds)
				})
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Program", "Title", "Publication Date", "Number of Segments", "Prepare Hours", "Staff Prepare Hours" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, PublicationLineItem record) {
			csv.WriteField(record.IcsId);
			csv.WriteField(record.Center);
			csv.WriteField(Lookups.ProgramsAndServices[record.ProgramId].Description);
			csv.WriteField(record.Title);
			csv.WriteField(record.PublicationDate, "M/d/yyyy");
			csv.WriteField(record.NumberOfSegments);
			csv.WriteField(record.PrepareHours);
			csv.WriteField(_fundingSourceIds == null
				? record.Staff.Sum(s => s.PrepHours)
				: record.Staff.Sum(s => s.PrepHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0)));
		}

		protected override void CreateReportTables() {
			var pubGroup = new PublicationReportTable("Publication Information", 9) {
				Headers = GetHeaders(),
				HideSubheaders = true,
				FundingSourceIds = _fundingSourceIds
			};
			pubGroup.Rows.AddRange(Lookups.PublicationTypes[ReportContainer.Provider].Select(t => new ReportRow { Code = t.CodeId, Title = t.Description }));
			ReportTableList.Add(pubGroup);
		}

		private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.PublicationNumberOfSegments, Title = "Number of Segments", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.PublicationPrepareHours, Title = "Prepare Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.PublicationStaffPrepareHours, Title = "Staff Prepare Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}
	}

	public class PublicationLineItem {
		public int IcsId { get; set; }
		public string Center { get; set; }
		public int ProgramId { get; set; }
		public string Title { get; set; }
		public DateTime? PublicationDate { get; set; }
		public int NumberOfSegments { get; set; }
		public double PrepareHours { get; set; }
		public IEnumerable<StaffLineItem> Staff { get; set; }
	}
}