using System.Collections.Generic;

namespace Infonet.Reporting.Core {
	public interface IReportRow {
		string Title { get; set; }
		int? Code { get; set; }
		Dictionary<string, Dictionary<string, double>> Counts { get; set; }
	}	
}