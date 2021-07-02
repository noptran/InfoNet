using System.Collections.Generic;
using Infonet.Core.Collections;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.DirectServices {
	public class DirectServiceGroupTable : ReportTableGroup<DirectServiceLineItem> {
		public DirectServiceGroupTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override double GrandTotalFor(ReportTableHeaderEnum header, ReportTableSubHeaderEnum subheader) {
			if (header != ReportTableHeaderEnum.NumberOfClientsReceivingServices)
				return base.GrandTotalFor(header, subheader);

			var result = new HashSet<int>();
            foreach (var each in ReportTables)
                result.AddRange(((DirectServiceReportTable)each).UniqueClientsByType[header][subheader]);
            return result.Count;
		}
	}
}