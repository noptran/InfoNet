using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.Builders;

namespace Infonet.Reporting.ManagementReports.ReportTables.Client {
	public class ClientAnnualIncomeReportTable : ReportTable<IncomeLineItem> {
		public ClientAnnualIncomeReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public decimal[] LowerBounds { get; set; }

		public decimal?[] UpperBounds { get; set; }

		public override void CheckAndApply(IncomeLineItem item) {
			if (item.AnnualIncome.HasValue)
				foreach (var row in Rows)
					if (item.AnnualIncome >= LowerBounds[row.Code.Value] && item.AnnualIncome <= (UpperBounds[row.Code.Value] ?? decimal.MaxValue))
						foreach (var header in Headers)
							if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
								row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += 1;
		}
	}
}