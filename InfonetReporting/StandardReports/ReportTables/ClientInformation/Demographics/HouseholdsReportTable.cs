using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class HouseholdsReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public HouseholdsReportTable(string title, int displayOrder) : base(title, displayOrder) {
			UniqueHouseholds = new HashSet<string>();
		}

		private HashSet<string> UniqueHouseholds { get; }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			UniqueHouseholds.Add(item.HouseholdID);
			foreach (var row in Rows)
				row.Counts[ReportTableHeaderEnum.Total.ToString()][ReportTableSubHeaderEnum.Total.ToString()] = UniqueHouseholds.Count;
		}
	}
}