using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.NonClientCrisisIntervention {
	public class CrisisInterventionTotalHoursReportTable : ReportTable<CrisisInterventionLineItem> {
		private ISet<int?> _fundingSourceIds = null;

		public CrisisInterventionTotalHoursReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public IEnumerable<int?> FundingSourceIds {
			get { return _fundingSourceIds; }
			set { _fundingSourceIds = value.NotNull(v => new HashSet<int?>(v)); }
		}

		public override void CheckAndApply(CrisisInterventionLineItem item) {
			double percentFunded = 1;
			if (_fundingSourceIds != null) {
				int percentFundedSum = item.StaffAndFunding.Where(sf => sf.FundingSourceId != null && _fundingSourceIds.Contains(sf.FundingSourceId)).Sum(sf => sf.PercentFund ?? 0);
				percentFunded = percentFundedSum / 100.0;
			}

			foreach (var row in Rows)
				if (item.CallTypeId != null && item.TotalTime != null)
					foreach (var header in Headers)
						switch (header.Code) {
							case ReportTableHeaderEnum.InPersonContacts:
								if (item.CallTypeId == (int)CrisisInterventionCallTypeEnum.InPerson)
									row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += item.TotalTime.Value * percentFunded;
								break;
							case ReportTableHeaderEnum.CrisisInterventionPhoneContacts:
								if (item.CallTypeId == (int)CrisisInterventionCallTypeEnum.Phone)
									row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += item.TotalTime.Value * percentFunded;
								break;
							case ReportTableHeaderEnum.Total:
								row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += item.TotalTime.Value * percentFunded;
								break;
						}
		}
	}
}