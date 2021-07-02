using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ViewModels;
using RazorEngine.Templating;

namespace Infonet.Reporting.Core {
	public abstract class SubReportDataBuilder<TInfonetContextType, TReportLineItemType> : ISubReportBuilder<TInfonetContextType> {
		protected SubReportDataBuilder(SubReportSelection subReportType) {
			SubReportType = subReportType;
			ColumnSelections = new SortedSet<SubReportColumnSelection>();
			GroupingSelections = new List<SubReportTableGroupingSelection>();
			DisplayOrder = subReportType.ToInt32();
		}

		public SubReportSelection SubReportType { get; set; }
		public string ReportName { get; set; }
		public ReportContainer ReportContainer { get; set; }
		public ReportQuery<TInfonetContextType> ReportQuery { get; set; }
		public int DisplayOrder { get; set; }
		private DateTime ReportRanTimestamp { get; set; }
		public ISet<SubReportColumnSelection> ColumnSelections { get; }
		public List<SubReportTableGroupingSelection> GroupingSelections { get; }
		public List<IReportTable> ReportTableList { get; set; }
		
		protected ReportColumnSelectionsEnum FirstColumn {
			get { return ColumnSelections.First().ColumnSelection; }
		}

		public void Write(IOrderedQueryable<TInfonetContextType> query, TextWriter html, TextWriter csv) {
			ReportRanTimestamp = DateTime.Now;
			var q = PerformSelect(query);
			var sb = new StringBuilder();

			if(csv != null)
				csv.WriteLine(BuildTrueCSVHeaders());

			using (var en = new LookaheadEnumerator<TReportLineItemType>(q))
				while (en.MoveNext()) {
					var item = en.Current;
					PrepareRecord(item);
					if (csv != null)
						csv.WriteLine(BuildTrueCSVLine(item));
					if (html != null)
						BuildLegacyHtmlRow(item, sb, en.IsFirst, en.IsLast);
				}
			if (html == null)
				return;

			BuildLegacyHtmlSummaryRow(sb);
			var model = new SubReportDataViewModel {
				Output = sb.ToString(),
				DisplayOrder = DisplayOrder,
				ReportContainer = ReportContainer,
				ReportRanTimestamp = ReportRanTimestamp,
				ColumnSelections = ColumnSelections.ToList(),
				GroupingSelections = GroupingSelections,
				Query = ReportQuery,
				AppliedOrdering = ReportQuery.OrderDisplay,
				SubReportType = SubReportType
			};
			ReportContainer.Razor.RunCompile("_SubReportData", html, typeof(SubReportDataViewModel), model);
		}

		protected abstract void BuildLegacyHtmlSummaryRow(StringBuilder sb);
		protected abstract void BuildLegacyHtmlRow(TReportLineItemType record, StringBuilder sb, bool isFirst, bool isLast);
		protected abstract IEnumerable<TReportLineItemType> PerformSelect(IOrderedQueryable<TInfonetContextType> query);
		protected virtual void PrepareRecord(TReportLineItemType record) { }
		protected abstract string BuildTrueCSVLine(TReportLineItemType record);

		protected virtual string BuildTrueCSVHeaders() {
			var sb = new StringBuilder();

			foreach (var columnSelection in ColumnSelections) {
				if (sb.Length != 0) {
					sb.Append(",");
				}

				sb.AppendQuotedCSVData(columnSelection.ColumnSelection.GetShortName());
			}

			return sb.ToString();
		}
	}

	public class SubReportColumnSelection : IComparable, IComparable<SubReportColumnSelection> {
		public SubReportColumnSelection(ReportColumnSelectionsEnum columnSelection, int order) {
			ColumnSelection = columnSelection;
			Order = order;
		}

		public ReportColumnSelectionsEnum ColumnSelection { get; }
		public int Order { get; }

		public int CompareTo(object other) {
			return CompareTo((SubReportColumnSelection)other);
		}

		public int CompareTo(SubReportColumnSelection other) {
			int result = Order.CompareTo(other.Order);
			if (result == 0)
				result = ColumnSelection.CompareTo(other.ColumnSelection);
			return result;
		}
	}

	public class SubReportTableGroupingSelection {
		public ReportOrderSelectionsEnum GroupingSelection { get; set; }
		public int Order { get; set; }
	}
}