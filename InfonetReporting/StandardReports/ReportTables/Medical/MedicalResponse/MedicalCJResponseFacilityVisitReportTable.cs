using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.MedicalResponse {
	public class MedicalCJResponseFacilityVisitReportTable : ReportTable<MedicalSystemInvolvementLineItem> {
		public MedicalCJResponseFacilityVisitReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}
		public override void CheckAndApply(MedicalSystemInvolvementLineItem item) {

			foreach (ReportRow row in Rows) {
                bool unassignedApplys = false;

                //unassigned is counted only when medical visit is yes
                if (item.MedicalVisitId == 1 || row.Code != null)
                    unassignedApplys = true;

                if (unassignedApplys) 
                    if (row.Code == item.MedicalVisitId) 
					foreach (ReportTableHeader header in Headers) 
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total) 
							foreach (ReportTableSubHeader subheader in header.SubHeaders) 
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
			}
		}
	}
}