using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.CommunityGroup {
	public class PublicationReportTable : ReportTable<PublicationLineItem> {
		private ISet<int?> _fundingSourceIds = null;

		public PublicationReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public IEnumerable<int?> FundingSourceIds {
			get { return _fundingSourceIds; }
			set { _fundingSourceIds = value.NotNull(v => new HashSet<int?>(v)); }
		}

		public override void CheckAndApply(PublicationLineItem item) {
			foreach (var row in Rows.Where(r => r.Code == item.ProgramId))
				foreach (var header in Headers) {
					foreach (var subheader in header.SubHeaders) {
						double val = 0;
						switch (header.Code) {
							case ReportTableHeaderEnum.PublicationNumberOfSegments:
								val = item.NumberOfSegments;
								break;
							case ReportTableHeaderEnum.PublicationPrepareHours:
								val = item.PrepareHours;
								break;
							case ReportTableHeaderEnum.PublicationStaffPrepareHours:
								val = _fundingSourceIds == null
									? item.Staff.Sum(s => s.PrepHours)
									: item.Staff.Sum(s => s.PrepHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0));
								break;
						}
						row.Counts[header.Code.ToString()][subheader.Code.ToString()] += val;
					}
				}
		}
	}
}