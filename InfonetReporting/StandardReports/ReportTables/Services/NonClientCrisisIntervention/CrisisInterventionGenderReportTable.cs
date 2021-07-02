using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.NonClientCrisisIntervention {
	public class CrisisInterventionGenderReportTable : ReportTable<CrisisInterventionLineItem> {
		public CrisisInterventionGenderReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}

		public override void CheckAndApply(CrisisInterventionLineItem item) {
            ReportTableHeaderEnum callType = item.CallTypeId == (int)CrisisInterventionCallTypeEnum.InPerson ? ReportTableHeaderEnum.InPersonContacts : ReportTableHeaderEnum.CrisisInterventionPhoneContacts;
            foreach (ReportRow row in Rows) {
                if (row.Code == item.GenderId) {
                    foreach (ReportTableHeader header in Headers) {
                        if (callType == header.Code || header.Code == ReportTableHeaderEnum.Total) {
                            row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += item.NumberOfContacts ?? 0;
                        }
                    }
                }
            }
		}
	}
}