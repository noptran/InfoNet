using System.Collections.Generic;
using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.Offender
{
    public class MedicalCJOffendersTotalOffendersReportTable : ReportTable<MedicalCJOffendersLineItem>
    {
        private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, Dictionary<string, HashSet<int?>>>> _offenderIds = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, Dictionary<string, HashSet<int?>>>>();

        public MedicalCJOffendersTotalOffendersReportTable(string title, int displayOrder) : base(title, displayOrder) { }
        public override void PreCheckAndApply(ReportContainer container){
            foreach (var header in Headers) {
                var innerDict = new Dictionary<ReportTableSubHeaderEnum, Dictionary<string, HashSet<int?>>>();
                foreach (var subheader in header.SubHeaders) {
                    var rowDict = new Dictionary<string, HashSet<int?>>();
                    foreach (var row in Rows)
                        rowDict.Add(row.Title, new HashSet<int?>());
                    innerDict.Add(subheader.Code, rowDict);
                }
                _offenderIds.Add(header.Code, innerDict);
            }
        }



        public override void CheckAndApply(MedicalCJOffendersLineItem item) {
            foreach (var row in Rows.Where(r => r.Title == "Total Offenders"))
                foreach (var currentHeader in Headers)
                    if (currentHeader.Code == item.ClientStatus || currentHeader.Code == ReportTableHeaderEnum.Total)
                        foreach (var currentSubheader in currentHeader.SubHeaders) {
                            var currentSet = _offenderIds[currentHeader.Code][currentSubheader.Code][row.Title];
                            if ((item.OffenderID == (int)currentSubheader.Code || currentSubheader.Code == ReportTableSubHeaderEnum.Total) && currentSet.Add(item.OffenderID))
                                row.Counts[currentHeader.Code.ToString()][currentSubheader.Code.ToString()] = currentSet.Count;
                        }
        }
    }
}