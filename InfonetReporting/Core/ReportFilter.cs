using System.Collections.Generic;
using System.IO;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Core {
	public abstract class ReportFilter {
		protected ReportFilter() {
			Visible = true;
		}

		public string Label { get; set; }

		public bool Visible { get; set; }

		public int DisplayOrder { get; set; }

		public abstract void ApplyTo(FilterContext context, ReportContainer container);

		public abstract void WriteCriteriaOn(TextWriter w, ReportContainer container);

		public virtual void AddVisibleTo(ISet<ReportFilter> visible) {
			if (Visible)
				visible.Add(this);
		}
	}
}