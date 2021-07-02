using System;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using Infonet.Reporting.StandardReports.Builders.Services;
using LinqKit;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.ServiceOutcome {
	public class SurveyReportTable : ReportTable<ServiceOutcomeLineItem> {
		private readonly Func<int> _eligibleClientsSelector;

		public SurveyReportTable(string title, int displayOrder, Func<int> eligibleClientsSelector) : base(title, displayOrder) {
			_eligibleClientsSelector = eligibleClientsSelector;
		}

		public ServiceOutcomeSurveyEnum SurveyType { get; set; }
        //Set column Eligible Cllients Served no matter outcome data has entered or not
        public override void PreCheckAndApply(ReportContainer reportContainer) {
            int val = _eligibleClientsSelector.Invoke();
            foreach (var row in Rows)
                foreach (ReportTableHeader header in Headers)
                    foreach (ReportTableSubHeader subheader in header.SubHeaders) {                        
                        switch (header.Code) {
                            case ReportTableHeaderEnum.OutcomeEligibleClientsServed:
                                row.Counts[header.Code.ToString()][subheader.Code.ToString()] = val;
                                break;
                            default:
                                break;
                        }
                    }
        }

        public override void CheckAndApply(ServiceOutcomeLineItem item) {
			if (item.ServiceID == SurveyType.ToInt32() || SurveyType == ServiceOutcomeSurveyEnum.All)
				foreach (ReportRow row in Rows.OrderBy(r => r.Order))
					if (row.Code == item.OutcomeID)
						foreach (ReportTableHeader header in Headers) {
							foreach (ReportTableSubHeader subheader in header.SubHeaders) {
								int val;
								switch (header.Code) {
									case ReportTableHeaderEnum.OutcomeTotalYes:
										val = item.ResponseYes;
										break;
									case ReportTableHeaderEnum.OutcomeTotalNo:
										val = item.ResponseNo;
										break;
									case ReportTableHeaderEnum.OutcomeTotalSurveys:
										val = item.ResponseYes + item.ResponseNo;
										break;
									case ReportTableHeaderEnum.OutcomeTotalRecords:
										val = item.OutcomeTotalRecords;
										break;
									case ReportTableHeaderEnum.OutcomeEligibleClientsServed:
										if (item.OutcomeID == 10 || item.OutcomeID == 11)
											val = item.EligibleChildServiceClients;
										else
											val = _eligibleClientsSelector.Invoke();

										break;

									default:
										val = 0;
										break;
								}
								if (ReportTableHeaderEnum.OutcomeEligibleClientsServed == header.Code && ReportTableSubHeaderEnum.Total == subheader.Code)
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] = val;
								else
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += val;
							}
						}
		}
	}
}