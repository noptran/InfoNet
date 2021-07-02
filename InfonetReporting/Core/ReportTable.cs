using System.Collections.Generic;
using Infonet.Data.Looking;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.Core {
	public abstract class ReportTable<TReportLineItemType> : IReportTable {
		protected static readonly string TotalHeader = ReportTableHeaderEnum.Total.ToString();
		protected static readonly string TotalSubHeader = ReportTableSubHeaderEnum.Total.ToString();

		protected ReportTable(string title, int displayOrder) {
			Title = title;
			DisplayOrder = displayOrder;
			Headers = new List<ReportTableHeader>();
			Rows = new List<ReportRow>();
			NonDuplicatedSubtotalRow = new ReportRow();
			ReportTables = new List<IReportTable>();
		}

		public string Title { get; set; }
		public string PreHeader { get; set; }
		public string Footer { get; set; }
		public int DisplayOrder { get; set; }
		public bool HideHeaders { get; set; }
		public bool HideSubheaders { get; set; }
		public bool HideSubtotal { get; set; }
		public bool HideTitle { get; set; }
		public bool HideZeroValues { get; set; }
		public bool UseNonDuplicatedSubtotal { get; set; }
        public bool UseNonDuplicatedSubtotalLabel { get; set; }
        public List<IReportTable> ReportTables { get; }
		public List<ReportTableHeader> Headers { get; set; }
		public List<ReportRow> Rows { get; }
		public ReportRow NonDuplicatedSubtotalRow { get; }
		public Provider Provider { get; set; }

		public virtual void PreCheckAndApply(ReportContainer container) { }
		public abstract void CheckAndApply(TReportLineItemType item);
		public virtual void PostCheckAndApply(ReportContainer container) { }

		protected Dictionary<string, Dictionary<string, double>> GetBlankDictionary(IEnumerable<ReportTableHeader> headers) {
			var newDict = new Dictionary<string, Dictionary<string, double>>();
			foreach (var header in headers) {
				var innerDict = new Dictionary<string, double>();
				foreach (var subheader in header.SubHeaders)
					innerDict.Add(subheader.Code.ToString(), 0);
				newDict.Add(header.Code.ToString(), innerDict);
			}
			return newDict;
		}

		public virtual double GrandTotalFor(ReportTableHeaderEnum header, ReportTableSubHeaderEnum subheader) {
			if (UseNonDuplicatedSubtotal)
				return NonDuplicatedSubtotalRow.Counts[header.ToString()][subheader.ToString()];

			double subtotal = 0;
			foreach (var innerTable in ReportTables)
				foreach (var row in innerTable.Rows)
					subtotal += row.Counts[header.ToString()][subheader.ToString()];
			return subtotal;
		}
	}
}