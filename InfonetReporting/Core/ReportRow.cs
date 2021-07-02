using System.Collections.Generic;
using Infonet.Data.Looking;

namespace Infonet.Reporting.Core {
	public class ReportRow : IReportRow {
		public ReportRow() {
			Counts = new Dictionary<string, Dictionary<string, double>>();
		}

		public ReportRow(LookupCode item, Provider provider) {
			Counts = new Dictionary<string, Dictionary<string, double>>();
			Title = item.Description;
			Code = item.CodeId;
			Order = item.Entries.ToDictionary()[provider].DisplayOrder;
		}

		public string Title { get; set; }
		public int? Code { get; set; }
		public double Order { get; set; }
		public Dictionary<string, Dictionary<string, double>> Counts { get; set; }
	}
}