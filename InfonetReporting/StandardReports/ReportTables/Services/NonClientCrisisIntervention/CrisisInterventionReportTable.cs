using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.NonClientCrisisIntervention {
	public class CrisisInterventionReportTable : ReportTable<CrisisInterventionLineItem> {
		public CrisisInterventionReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}

		public override void CheckAndApply(CrisisInterventionLineItem item) {
			foreach (ReportRow row in Rows) {
                foreach (ReportTableHeader header in Headers) {
                    switch (header.Code) {
                        case ReportTableHeaderEnum.InPersonContacts:
                            if (item.CallTypeId == (int)CrisisInterventionCallTypeEnum.InPerson)
                                row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += item.NumberOfContacts ?? 0;
                            break;
                        case ReportTableHeaderEnum.CrisisInterventionPhoneContacts:
                            if (item.CallTypeId == (int)CrisisInterventionCallTypeEnum.Phone)
                                row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += item.NumberOfContacts ?? 0;
                            break;
                        case ReportTableHeaderEnum.Total:
                            row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += item.NumberOfContacts ?? 0;
                            break;
                    }
                }
			}
		}
	}
}