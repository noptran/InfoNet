using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class MonthlyIncomeReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public MonthlyIncomeReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		private enum MonthlyIncomeRowHeader {
			None, // no row header selected
			LessThan, // less than or equal to $500
			Between, // greater than $500 and less than $1000
			GreaterThan // greater than $1000
		}

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			const decimal fiveHundredDollars = 500.0000m;
			const decimal oneThousandDollars = 1000.0000m;

			if (item.ClientTypeID == (int)ReportTableSubHeaderEnum.Adult)
				foreach (var row in Rows) {
					MonthlyIncomeRowHeader? currentIncomeRowCode;
					if (!item.IncomeSources.Any())
						currentIncomeRowCode = MonthlyIncomeRowHeader.LessThan;
					else if (item.MonthlyIncome <= fiveHundredDollars)
						currentIncomeRowCode = MonthlyIncomeRowHeader.LessThan;
					else if (item.MonthlyIncome > fiveHundredDollars && item.MonthlyIncome <= oneThousandDollars)
						currentIncomeRowCode = MonthlyIncomeRowHeader.Between;
					else if (item.MonthlyIncome > oneThousandDollars)
						currentIncomeRowCode = MonthlyIncomeRowHeader.GreaterThan;
					else
						currentIncomeRowCode = MonthlyIncomeRowHeader.None;

					if (row.Code == (int)currentIncomeRowCode)
						foreach (var currentHeader in Headers)
							if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total)
								row.Counts[currentHeader.Code.ToString()][ReportTableSubHeaderEnum.Adult.ToString()] += 1;
				}
		}
	}
}