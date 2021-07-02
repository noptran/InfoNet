﻿using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.SpecialNeeds {
	public class DisablingConditionReportTable : ReportTable<ClientInformationSpecialNeedsLineItem> {
        public DisablingConditionReportTable(string title, int displayOrder) : base(title, displayOrder) {
        }
        public override void CheckAndApply(ClientInformationSpecialNeedsLineItem item) {
			foreach (ReportRow row in Rows) {
                bool thisAnswerApplies = false;
                switch (row.Code) {
                    case (int)ShortAnswerEnum.Yes:
                        if ((item.ADLProblem ?? false) || (item.Deaf ?? false) ||
                            (item.DevelopmentalDisabled ?? false) || (item.Immobile ?? false) ||
                            (item.WheelChair ?? false) || (item.VisualProblem ?? false))
                            thisAnswerApplies = true;
                        break;
                    case (int)ShortAnswerEnum.NotReported:
                        if (item.NotReported ?? false)
                            thisAnswerApplies = true;
                        break;
                    case (int)ShortAnswerEnum.Unknown:
                        if (item.UnknownSpecialNeeds ?? false)
                            thisAnswerApplies = true;
                        break;
                    default:
                        if (item.ADLProblem != true && item.Deaf != true &&
                            item.DevelopmentalDisabled != true && item.Immobile != true &&
                            item.WheelChair != true && item.VisualProblem != true &&
                            item.NotReported != true && item.UnknownSpecialNeeds != true)
                            thisAnswerApplies = true;
                        break;
                }
                if (thisAnswerApplies) { 
                    foreach (ReportTableHeader currentHeader in Headers) {
                        // Check New vs. Ongoing - allow Total
                        if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total) {
                            foreach (ReportTableSubHeader currentSubHeader in currentHeader.SubHeaders) {
                                // Check if Client Type matches
                                if (item.ClientTypeID == (int)currentSubHeader.Code || currentSubHeader.Code == ReportTableSubHeaderEnum.Total) {
                                    row.Counts[currentHeader.Code.ToString()][currentSubHeader.Code.ToString()] += 1;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}