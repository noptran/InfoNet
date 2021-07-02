namespace Infonet.Reporting.Core {
	public interface IReportOrderBase {
		int DisplayOrder { get; set; }
		string ReportOrderAsString { get; }
		bool HideOrder { get; set; }
	}
}