using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.Hotline {
	public class HotlineReportTable : ReportTable<HotlineLineItem> {
		public HotlineReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(HotlineLineItem item) {
			foreach (var row in Rows.Where(r => r.Code == item.CallTypeId))
				foreach (var header in Headers)
					row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += item.NumberOfContacts ?? 0;
		}
	}
}