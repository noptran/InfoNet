using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum ReportOutputType {
		[Display(Name = "HTML")] Html = 1,
		[Display(Name = "CSV")] Csv = 2,
		[Display(Name = "PDF")] Pdf = 3
	}
}