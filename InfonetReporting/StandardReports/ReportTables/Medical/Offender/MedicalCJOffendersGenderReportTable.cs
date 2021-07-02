using System.Collections.Generic;
using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.Offender
{
    public class MedicalCJOffendersGenderReportTable : ReportTable<MedicalCJOffendersLineItem>
    {
        private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, Dictionary<int?, HashSet<int?>>>> _offenderIds = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, Dictionary<int?, HashSet<int?>>>>();

        public MedicalCJOffendersGenderReportTable(string title, int displayOrder) : base(title, displayOrder) { }

        public override void PreCheckAndApply(ReportContainer container) {
            foreach (var header in Headers) {
                var innerDict = new Dictionary<ReportTableSubHeaderEnum, Dictionary<int?, HashSet<int?>>>();
                foreach (var subheader in header.SubHeaders) {
                    var rowDict = new Dictionary<int?, HashSet<int?>>();
                    foreach (var row in Rows)
                        rowDict.Add(row.Code, new HashSet<int?>());
                    innerDict.Add(subheader.Code, rowDict);
                }
                _offenderIds.Add(header.Code, innerDict);
            }
        }


        public override void CheckAndApply(MedicalCJOffendersLineItem item) {
            foreach (var row in Rows.Where(r => r.Code == item.GenderID))
                foreach (var currentHeader in Headers)
                    if (currentHeader.Code == item.ClientStatus || currentHeader.Code == ReportTableHeaderEnum.Total)
                        foreach (var currentSubheader in currentHeader.SubHeaders) {
                            var currentSet = _offenderIds[currentHeader.Code][currentSubheader.Code][row.Code];
                            if (currentSubheader.Code == ReportTableSubHeaderEnum.Total && currentSet.Add(item.OffenderID))
                                row.Counts[currentHeader.Code.ToString()][currentSubheader.Code.ToString()] = currentSet.Count;
                        }
        }
    }
}