using System.Linq;

namespace Infonet.Reporting.Core {
	public interface IReportOrder<T> : IReportOrderBase {
		IOrderedQueryable<T> ApplyOrder(IQueryable<T> query);
		IOrderedQueryable<T> ApplyOrder(IOrderedQueryable<T> query);
	}
}