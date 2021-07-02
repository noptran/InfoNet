using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.MedicalResponse {
	public class MedicalCJResponseSeverityOfAbuseReportTable : ReportTable<MedicalSystemInvolvementClientConflictLineItem> {
		public MedicalCJResponseSeverityOfAbuseReportTable(string title, int displayOrder) : base(title, displayOrder) {
            UniqueCases = new HashSet<string>();
        }

        private HashSet<string> UniqueCases { get; }

        public override void CheckAndApply(MedicalSystemInvolvementClientConflictLineItem item) {
            string caseIdentifier = $"{item.ClientId}:{item.ClientId}";
            if (item.ClientConflictScale != null) {                
				foreach (ReportRow row in Rows) {
					bool hasAbuse = false;
					switch (row.Code) {
						case (int)ClientConflictScaleEnum.BeatUp:
							hasAbuse = item.ClientConflictScale.BeatUp;
							break;
						case (int)ClientConflictScaleEnum.Choked:
							hasAbuse = item.ClientConflictScale.Choked;
							break;
						case (int)ClientConflictScaleEnum.Hit:
							hasAbuse = item.ClientConflictScale.Hit;
							break;
						case (int)ClientConflictScaleEnum.Kicked:
							hasAbuse = item.ClientConflictScale.Kicked;
							break;
						case (int)ClientConflictScaleEnum.Pushed:
							hasAbuse = item.ClientConflictScale.Pushed;
							break;
						case (int)ClientConflictScaleEnum.Slapped:
							hasAbuse = item.ClientConflictScale.Slapped;
							break;
						case (int)ClientConflictScaleEnum.Threatened:
							hasAbuse = item.ClientConflictScale.Threatened;
							break;
						case (int)ClientConflictScaleEnum.Threw:
							hasAbuse = item.ClientConflictScale.Threw;
							break;
						case (int)ClientConflictScaleEnum.Used:
							hasAbuse = item.ClientConflictScale.Used;
							break;
					}
					if (hasAbuse) {
						foreach (ReportTableHeader header in Headers) {
							if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total) {
								foreach (ReportTableSubHeader subheader in header.SubHeaders) {
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
                                    if (UseNonDuplicatedSubtotal && !UniqueCases.Contains(caseIdentifier))
                                        NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
                                }
							}
						}                        
                        UniqueCases.Add(caseIdentifier);
                    }
				}                
            }
		}
	}
}