using System.Linq;

namespace Infonet.Reporting.Core {
	public abstract class ReportOrder<TInfonetContextType> : IReportOrder<TInfonetContextType> {
		public int DisplayOrder { get; set; }

		public bool HideOrder { get; set; }

		public abstract string ReportOrderAsString { get; }

		public abstract IOrderedQueryable<TInfonetContextType> ApplyOrder(IOrderedQueryable<TInfonetContextType> query);

		public abstract IOrderedQueryable<TInfonetContextType> ApplyOrder(IQueryable<TInfonetContextType> query);
	}
}