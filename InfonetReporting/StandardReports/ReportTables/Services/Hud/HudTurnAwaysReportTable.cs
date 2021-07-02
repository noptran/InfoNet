using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.Hud {
	public class HudTurnAwaysReportTable : ReportTable<TurnAwayLineItem> {
		public HudTurnAwaysReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(TurnAwayLineItem item) {
			foreach (var row in Rows.Where(r => r.Code == item.LocationId))
				foreach (var header in Headers)
					switch (header.Code) {
						case ReportTableHeaderEnum.TurnAwayAdultCount:
							if (item.AdultsNo != null)
								row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += (double)item.AdultsNo;
							break;
						case ReportTableHeaderEnum.TurnAwayChildCount:
							if (item.ChildrenNo != null)
								row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += (double)item.ChildrenNo;
							break;
						case ReportTableHeaderEnum.TurnAwayReferralYes:
							if (item.ReferralMadeId == 1)
								row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += (double)((item.ChildrenNo + item.AdultsNo) * item.ReferralMadeId);
							break;
						case ReportTableHeaderEnum.TurnAwayReferralNo:
							if (item.ReferralMadeId == 2)
								row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += (double)((item.ChildrenNo + item.AdultsNo) * item.ReferralMadeId);
							break;
					}
		}
	}
}