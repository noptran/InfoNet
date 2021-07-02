using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ViewModels;
using RazorEngine.Templating;

namespace Infonet.Reporting.Core {
	public abstract class SubReportCountBuilder<TInfonetContextType, TReportLineItemType> : ISubReportBuilder<TInfonetContextType> {
		#region Constructors
		protected SubReportCountBuilder(SubReportSelection subReportType) {
			ReportTableList = new List<IReportTable>();
			SubReportType = subReportType;
			DisplayOrder = subReportType.ToInt32();
		}
		#endregion

		#region Properties
		public string ReportName { get; set; }
		public ReportContainer ReportContainer { get; set; }
		public ReportQuery<TInfonetContextType> ReportQuery { get; set; }
		public int DisplayOrder { get; set; }
		public List<IReportTable> ReportTableList { get; set; }
		public bool IsInGroup { get; set; }
		public bool IsEndOfGroup { get; set; }
		public SubReportSelection SubReportType { get; set; }
		#endregion

		#region Overridable Methods
		protected virtual void PrePerformSelect(ReportContainer container) { }
		protected abstract IEnumerable<TReportLineItemType> PerformSelect(IQueryable<TInfonetContextType> query);
		protected abstract string[] CsvHeaders { get; }
		protected abstract void WriteCsvRecord(CsvWriter csv, TReportLineItemType record);
		protected abstract void CreateReportTables();
		#endregion

		#region Public Methods
		public void Write(IOrderedQueryable<TInfonetContextType> query, TextWriter html, TextWriter csv) {
			PrePerformSelect(ReportContainer);
			var q = PerformSelect(query);
			if (html != null) {
				CreateReportTables();
				InitializeRows();
				foreach (var each in ReportTableList)
					each.PreCheckAndApply(ReportContainer);
			}

			using (var csw = csv == null ? null : CsvWriter.WriteHeaders(csv, CsvHeaders, false))
				foreach (var item in q) {
					if (csw != null) {
						WriteCsvRecord(csw, item);
						csw.WriteEol();
					}

					if (html != null)
						foreach (var group in ReportTableList.Cast<ReportTable<TReportLineItemType>>()) {
							group.Provider = ReportContainer.Provider;
							group.CheckAndApply(item);
						}
				}

			if (html == null)
				return;

			foreach (var group in ReportTableList)
				group.PostCheckAndApply(ReportContainer);

			if (!ReportContainer.GroupedSubReports.ContainsKey(SubReportType)) {
				var subreports = new List<IReportTable>();
				subreports.AddRange(ReportTableList);
				ReportContainer.GroupedSubReports.Add(SubReportType, subreports);
			} else {
				ReportContainer.GroupedSubReports[SubReportType].AddRange(ReportTableList);
			}
			if (!IsInGroup || IsEndOfGroup) {
				var model = new SubReportCountViewModel {
					DisplayOrder = DisplayOrder,
					ReportContainer = ReportContainer,
					SubReportType = SubReportType,
					Query = ReportQuery,
					AppliedOrdering = ReportQuery.OrderDisplay
				};
				ReportContainer.Razor.RunCompile("_SubReportCount", html, typeof(SubReportCountViewModel), model);
			}
		}
		#endregion

		#region Protected Methods
		private void InitializeRows() {
			foreach (var group in ReportTableList) {
				InitializeRows(group);
				foreach (var child in group.ReportTables) {
					InitializeRows(child);
					if (child.UseNonDuplicatedSubtotal)
						InitializeNonDuplicatedSubTotalRow(child);
				}
				if (group.UseNonDuplicatedSubtotal)
					InitializeNonDuplicatedSubTotalRow(group);
			}
		}

		private void InitializeRows(IReportTable reportTable) {
			foreach (var row in reportTable.Rows) {
				foreach (var header in reportTable.Headers) {
					var innerDict = new Dictionary<string, double>();
					foreach (var subheader in header.SubHeaders)
						if (innerDict.ContainsKey(subheader.Code.ToString()) == false)
							innerDict.Add(subheader.Code.ToString(), 0);
					if (row.Counts.ContainsKey(header.Code.ToString()) == false)
						row.Counts.Add(header.Code.ToString(), innerDict);
				}
			}
		}

		private void InitializeNonDuplicatedSubTotalRow(IReportTable reportTable) {
			foreach (var header in reportTable.Headers) {
				var innerDict = new Dictionary<string, double>();
				foreach (var subheader in header.SubHeaders)
					if (innerDict.ContainsKey(subheader.Code.ToString()) == false)
						innerDict.Add(subheader.Code.ToString(), 0);
				reportTable.NonDuplicatedSubtotalRow.Counts.Add(header.Code.ToString(), innerDict);
			}
		}

		protected virtual List<ReportTableSubHeader> GetClientTypeSubHeaders(Provider provider) {
			var subHeaders = new List<ReportTableSubHeader>();
			foreach (var type in Lookups.ClientType[provider])
				subHeaders.Add(new ReportTableSubHeader { Title = type.Description, Code = (ReportTableSubHeaderEnum)type.CodeId });
			subHeaders.Add(new ReportTableSubHeader { Title = "Total", Code = ReportTableSubHeaderEnum.Total });
			return subHeaders;
		}

		protected List<ReportTableSubHeader> GetSingleSubHeader(ReportTableSubHeaderEnum subheader) {
			return new List<ReportTableSubHeader> { new ReportTableSubHeader { Title = subheader.GetDisplayName(), Code = subheader } };
		}

		protected List<ReportTableSubHeader> GetSingleAndTotalSubHeaders(ReportTableSubHeaderEnum subheader) {
			return new List<ReportTableSubHeader> {
				new ReportTableSubHeader { Title = subheader.GetDisplayName(), Code = subheader },
				new ReportTableSubHeader { Title = ReportTableSubHeaderEnum.Total.GetDisplayName(), Code = ReportTableSubHeaderEnum.Total }
			};
		}

		protected virtual List<ReportTableHeader> GetNewAndOngoingHeaders() {
			var subheaders = GetClientTypeSubHeaders(ReportContainer.Provider);
			return new List<ReportTableHeader> {
				new ReportTableHeader("New", ReportTableHeaderEnum.New, subheaders),
				new ReportTableHeader("Ongoing", ReportTableHeaderEnum.Ongoing, subheaders),
				new ReportTableHeader("Total", ReportTableHeaderEnum.Total, subheaders)
			};
		}

		protected List<ReportTableHeader> GetNewAndOngoingHeaders(ReportTableSubHeaderEnum clientType, bool includeTotal = true) {
			var subheaders = includeTotal ? GetSingleAndTotalSubHeaders(clientType) : GetSingleSubHeader(clientType);
			return new List<ReportTableHeader> {
				new ReportTableHeader("New", ReportTableHeaderEnum.New, subheaders),
				new ReportTableHeader("Ongoing", ReportTableHeaderEnum.Ongoing, subheaders),
				new ReportTableHeader("Total", ReportTableHeaderEnum.Total, subheaders)
			};
		}

		protected ReportRow GetReportRowFromLookup(LookupCode item) {
			return new ReportRow(item, ReportContainer.Provider);
		}
		#endregion
	}
}