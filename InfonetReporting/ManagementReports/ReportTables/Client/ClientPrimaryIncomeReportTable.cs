using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.Builders;

namespace Infonet.Reporting.ManagementReports.ReportTables.Client {
	public class ClientPrimaryIncomeReportTable : ReportTable<IncomeLineItem> {
		public decimal LowerBounds { get; set; }
		public decimal? UpperBounds { get; set; }
		public decimal? LastLowerBounds { get; set; }
		public ClientPrimaryIncomeReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(IncomeLineItem item) {
			if (item.AnnualIncome.HasValue)
				foreach (var row in Rows)
					if (item.PrimaryIncomeSourceId == row.Code)
						if (LastLowerBounds.HasValue && UpperBounds == null) {
							if (item.AnnualIncome >= LowerBounds && item.AnnualIncome >= LastLowerBounds)
								foreach (var header in Headers)
									if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
										row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += 1;
						} else if (UpperBounds.HasValue) {
							if (item.AnnualIncome >= LowerBounds && item.AnnualIncome <= UpperBounds)
								foreach (var header in Headers)
									if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
										row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += 1;
						}
		}
	}
}