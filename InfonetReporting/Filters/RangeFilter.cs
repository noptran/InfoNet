using System.IO;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Filters {
	public abstract class RangeFilter<T> : ReportFilter where T : struct {
		protected RangeFilter(T? from, T? to) {
			Label = "Range";
			From = from;
			To = to;
		}

		public T? From { get; set; }

		public T? To { get; set; }

		public string Format { get; set; }

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			if (From == null && To == null) {
				w.Write("<any>");
			} else if (From != null && To != null) {
				WriteOn(w, From, Format);
				if (!From.Equals(To)) {
					w.Write(" - ");
					WriteOn(w, To, Format);
				}
			} else if (From != null) {
				w.Write(">= ");
				WriteOn(w, From, Format);
			} else {
				w.Write("<= ");
				WriteOn(w, To, Format);
			}
		}

		private static void WriteOn<TItem>(TextWriter w, TItem item, string formatOrNull) {
			if (formatOrNull == null)
				w.Write(item);
			else
				w.Write(formatOrNull, item);
		}
	}
}