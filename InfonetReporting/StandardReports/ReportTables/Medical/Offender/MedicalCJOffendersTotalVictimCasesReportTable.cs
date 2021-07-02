using System.Collections.Generic;
using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.Offender {
	public class MedicalCJOffendersTotalVictimCasesReportTable : ReportTable<MedicalCJOffendersLineItem> {
        private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, Dictionary<string, HashSet<int?>>>> _clientIds = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, Dictionary<string, HashSet<int?>>>>();
        public MedicalCJOffendersTotalVictimCasesReportTable(string title, int displayOrder) : base(title, displayOrder) { }

        public override void PreCheckAndApply(ReportContainer container) {
            foreach (var header in Headers) {
                var innerDict = new Dictionary<ReportTableSubHeaderEnum, Dictionary<string, HashSet<int?>>>();
                foreach (var subheader in header.SubHeaders) {
                    var rowDict = new Dictionary<string, HashSet<int?>>();
                    foreach (var row in Rows)
                        rowDict.Add(row.Title, new HashSet<int?>());
                    innerDict.Add(subheader.Code, rowDict);
                }
                _clientIds.Add(header.Code, innerDict);
            }
        }

        public override void CheckAndApply(MedicalCJOffendersLineItem item) { 
            foreach (var row in Rows.Where(r => r.Title == "Total Victims"))
                foreach (var currentHeader in Headers)
                    if (currentHeader.Code == item.ClientStatus || currentHeader.Code == ReportTableHeaderEnum.Total)
                        foreach (var currentSubheader in currentHeader.SubHeaders) {
                            var currentSet = _clientIds[currentHeader.Code][currentSubheader.Code][row.Title];
                            if ((item.ClientID == (int)currentSubheader.Code || currentSubheader.Code == ReportTableSubHeaderEnum.Total) && currentSet.Add(item.ClientID))
                                row.Counts[currentHeader.Code.ToString()][currentSubheader.Code.ToString()] = currentSet.Count;
                        }
        }
    }
}