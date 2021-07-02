using Infonet.Reporting.StandardReports.Builders.Services;
using System.Linq;

namespace Infonet.Reporting.Core {
	public class ReportTableGroup<TLineItemType> : ReportTable<TLineItemType> {
		public ReportTableGroup(string title, int displayOrder) : base(title, displayOrder) { }

		public override void PreCheckAndApply(ReportContainer container) {
			foreach (var each in ReportTables)
				each.PreCheckAndApply(container);
		}

        public override void CheckAndApply(TLineItemType item) {
            foreach (var each in ReportTables.Cast<ReportTable<TLineItemType>>())
                each.CheckAndApply(item);
        }

        public override void PostCheckAndApply(ReportContainer container) {
			foreach (var each in ReportTables)
				each.PostCheckAndApply(container);
		}
	}
}