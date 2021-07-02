using System;

namespace Infonet.Web.ViewModels.Shared {
	public class PagedListPagination : IDateRange {
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Range { get; set; }
		public int? PageNumber { get; set; }
		public int PageSize { get; set; }
		public int? RecordCount { get; set; }
	}
}