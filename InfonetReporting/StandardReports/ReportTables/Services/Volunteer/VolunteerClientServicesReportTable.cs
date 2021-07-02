using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.Volunteer {
	public class VolunteerClientServicesReportTable : ReportTable<ClientServiceLineItem> {
		private ISet<int?> _fundingSourceIds = null;
		private ISet<int?> _volunteerSvIds = null;

		public VolunteerClientServicesReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public IEnumerable<int?> FundingSourceIds {
			get { return _fundingSourceIds; }
			set { _fundingSourceIds = value.NotNull(v => new HashSet<int?>(v)); }
		}

		public IEnumerable<int?> VolunteerSvIds {
			get { return _volunteerSvIds; }
			set { _volunteerSvIds = value.NotNull(v => new HashSet<int?>(v)); }
		}

		public override void CheckAndApply(ClientServiceLineItem item) {
			double averagePercentFundedPerStaff = 1;
			if (_fundingSourceIds != null) {
				int percentFundedSum = item.StaffAndFunding.Where(sf => sf.FundingSourceId != null && _fundingSourceIds.Contains(sf.FundingSourceId) && _volunteerSvIds.Contains(sf.SvId)).Sum(sf => sf.PercentFund ?? 0);
				int staffCount = item.StaffAndFunding.Select(sf => sf.SvId).Distinct().ToArray().Length;
				averagePercentFundedPerStaff = percentFundedSum / 100.0 / staffCount;
			}

			foreach (var row in Rows)
				switch ((ReportTableHeaderEnum)row.Code) {
					case ReportTableHeaderEnum.HoursOfService:
						row.Counts[TotalHeader][TotalSubHeader] += item.ReceivedHours * averagePercentFundedPerStaff ?? 0;
						break;
					case ReportTableHeaderEnum.NumberOfContacts:
						row.Counts[TotalHeader][TotalSubHeader] += 1;
						break;
				}
		}
	}
}