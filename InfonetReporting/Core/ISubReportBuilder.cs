using System.IO;
using System.Linq;

namespace Infonet.Reporting.Core {
	public interface ISubReportBuilder<TInfonetContextType> : ISubReportBuilderBase {
		ReportQuery<TInfonetContextType> ReportQuery { get; set; }
		void Write(IOrderedQueryable<TInfonetContextType> query, TextWriter html, TextWriter csv);
	}
}