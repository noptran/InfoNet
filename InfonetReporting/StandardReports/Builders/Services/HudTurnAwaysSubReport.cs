using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Services.Hud;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class HudTurnAwaysSubReport : SubReportCountBuilder<TurnAwayService, TurnAwayLineItem> {
		public HudTurnAwaysSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override IEnumerable<TurnAwayLineItem> PerformSelect(IQueryable<TurnAwayService> query) {
			return query.Select(q => new TurnAwayLineItem {
				Id = q.Id,
				LocationId = q.LocationId,
				AdultsNo = q.AdultsNo,
				ChildrenNo = q.ChildrenNo,
				ReferralMadeId = q.ReferralMadeId,
				CenterName = q.Center.CenterName
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center Name", "Adult Count", "Child Count", "Referral Made" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, TurnAwayLineItem record) {
			csv.WriteField(record.Id);
			csv.WriteField(record.CenterName);
			csv.WriteField(record.AdultsNo);
			csv.WriteField(record.ChildrenNo);
			csv.WriteField(Lookups.YesNo[record.ReferralMadeId]?.Description);
		}

		protected override void CreateReportTables() {
			var turnAways = new HudTurnAwaysReportTable("Turn Away Information", 5) {
				Headers = GetHeaders(),
				HideSubheaders = true
			};
			foreach (var center in ReportContainer.Centers)
				turnAways.Rows.Add(new ReportRow { Title = center.CenterName, Code = center.CenterId });
			ReportTableList.Add(turnAways);
		}

		private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.TurnAwayAdultCount, Title = "Adult Counts", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.TurnAwayChildCount, Title = "Child Counts", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.TurnAwayReferralYes, Title = "Yes", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.TurnAwayReferralNo, Title = "No", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}
	}

	public class TurnAwayLineItem {
		public int? Id { get; set; }
		public int? LocationId { get; set; }
		public int? AdultsNo { get; set; }
		public int? ChildrenNo { get; set; }
		public int? ReferralMadeId { get; set; }
		public string CenterName { get; set; }
	}
}