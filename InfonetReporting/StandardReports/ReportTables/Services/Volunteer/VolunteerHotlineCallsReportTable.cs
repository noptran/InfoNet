using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.Volunteer {
	public class VolunteerHotlineCallsReportTable : ReportTable<HotlineItem> {
		private ISet<int?> _fundingSourceIds = null;

		public VolunteerHotlineCallsReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public IEnumerable<int?> FundingSourceIds {
			get { return _fundingSourceIds; }
			set { _fundingSourceIds = value.NotNull(v => new HashSet<int?>(v)); }
		}

		public override void CheckAndApply(HotlineItem item) {
			double percentFunded = 1;
			if (_fundingSourceIds != null) {
				int percentFundedSum = item.StaffAndFunding.Where(sf => sf.FundingSourceId != null && _fundingSourceIds.Contains(sf.FundingSourceId)).Sum(sf => sf.PercentFund ?? 0);
				percentFunded = percentFundedSum / 100.0;
			}

			int divisor = Provider == Provider.DV ? 60 : 1;
			foreach (var row in Rows)
				switch ((ReportTableHeaderEnum)row.Code) {
					case ReportTableHeaderEnum.HoursOfService:
						row.Counts[TotalHeader][TotalSubHeader] += (item.TotalTime ?? 0) / divisor * percentFunded;
						break;
					case ReportTableHeaderEnum.NumberOfContacts:
						row.Counts[TotalHeader][TotalSubHeader] += item.NumberOfContacts ?? 0;
						break;
				}
		}
	}
}