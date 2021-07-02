using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infonet.Core;
using Infonet.Core.IO;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using LinqKit;

namespace Infonet.Reporting.ManagementReports.Builders {
    public class StaffClientServiceInformationSubReport : SubReportDataBuilder<ServiceDetailOfClient, StaffClientServiceInformationLineItem> {
        private readonly ISet<int?> _previousClientIds = new HashSet<int?>();
        private readonly ISet<int?> _previousSvIds = new HashSet<int?>();
        private readonly ISet<int?> _totalClientIds = new HashSet<int?>();
        private readonly ISet<int?> _totalSvIds = new HashSet<int?>();

        private HashSet<int?> _fundingSourceIds = null;
        private HashSet<int?> _svIds = null;

        private Dictionary<string, StaffTracker> staffNamesDictionary = new Dictionary<string, StaffTracker>();

        public StaffClientServiceInformationSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
            PreviousGroupValue = string.Empty;
            PreviousGroupCount = 1;
        }

        public RecordDetailOrderSelectionsEnum DetailOrGroupSelection { get; set; }
        private StaffClientServiceInformationLineItem PreviousLineItem { get; set; }
        private int PreviousGroupCount { get; set; }
        private string PreviousGroupValue { get; set; }
        private int PreviousServiceCount { get; set; }
        private double PreviousHoursCount { get; set; }
        private int TotalServiceCount { get; set; }
        private double TotalHoursCount { get; set; }

        protected override void BuildLegacyHtmlRow(StaffClientServiceInformationLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
            IsStaffFirst = ReportColumnSelectionsEnum.Staff == FirstColumn;
            if (!IsStaffFirst) {          
                if (!isFirst && GroupingSelections.Any()) {
                    if (IsGrouped(record)) {
                        PreviousGroupCount++;
                        PreviousLineItem.ServiceHours += record.ServiceHours;
                    }
                    else {
                        ApplyItemRow(PreviousLineItem, sb);
                        PreviousLineItem = record;
                        PreviousGroupCount = 1;
                    }
                    string currentGroupValue = GetCurrentGroupValue(record);
                    if (!PreviousGroupValue.Trim().Equals(currentGroupValue.Trim(), StringComparison.OrdinalIgnoreCase)) {
                        ApplySummaryRow(sb);
                        ResetPreviousGroupValues();
                    }
                }
                else {
                    PreviousLineItem = record;
                }
                SetGroupTotals(record);
            }
            SetTotals(record);
        }

        private bool IsGrouped(StaffClientServiceInformationLineItem record, bool isStaffIncluded = true) {
            if (DetailOrGroupSelection == RecordDetailOrderSelectionsEnum.RecordDetail)
                return false;

            bool isGrouped = true;
            foreach (var columnSelection in ColumnSelections) {
                switch (columnSelection.ColumnSelection) {
                    case ReportColumnSelectionsEnum.ClientCode:
                        isGrouped = PreviousLineItem.ClientCode == record.ClientCode;
                        break;
                    case ReportColumnSelectionsEnum.ServiceName:
                        isGrouped = PreviousLineItem.ServiceStr == record.ServiceStr;
                        break;
                    case ReportColumnSelectionsEnum.Staff:
                        if (isStaffIncluded) 
                            isGrouped = PreviousLineItem.StaffNamesStr == record.StaffNamesStr;                      
                        break;
                    case ReportColumnSelectionsEnum.ServiceDate:
                        isGrouped = PreviousLineItem.ServiceDate == record.ServiceDate;
                        break;
                }
                if (!isGrouped)
                    return false;
            }
            return true;
        }

        private string GetCurrentGroupValue(StaffClientServiceInformationLineItem record) {
            string currentGroupValue = string.Empty;
            switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
                case ReportOrderSelectionsEnum.Staff:
                    currentGroupValue = record.StaffNamesStr;
                    break;
                case ReportOrderSelectionsEnum.Client:
                    currentGroupValue = record.ClientCode ?? "";
                    break;
                case ReportOrderSelectionsEnum.Service:
                    currentGroupValue = record.ServiceStr ?? "";
                    break;
            }
            return currentGroupValue;
        }

        private void ResetPreviousGroupValues() {
            PreviousGroupValue = null;
            _previousSvIds.Clear();
            PreviousServiceCount = 0;
            _previousClientIds.Clear();
            PreviousHoursCount = 0.0;
        }

        class StaffTracker {
            public StaffTracker(string name) {
                Name = name;
                Records = new List<StaffClientServiceInformationLineItem>();
                IsProcessed = false;
            }
            public string Name { get; set; }
            public List<StaffClientServiceInformationLineItem> Records { get; set; }
            public bool IsProcessed { get; set; }
        }

        bool IsStaffFirst { get; set; }

        private void ApplyItemRow(StaffClientServiceInformationLineItem record, StringBuilder sb, string alternateName = null) {
            if (!IsStaffFirst) 
                GenerateHtml(record, sb);            
        }

        private void GenerateHtml(StaffClientServiceInformationLineItem record, StringBuilder sb, string alternateName = null) {
            string serviceNameFormat = PreviousGroupCount > 1 ? "{0} ({1})" : "{0}";
            sb.Append("<tr>");
            if (record != null)
                foreach (var columnSelection in ColumnSelections)
                    switch (columnSelection.ColumnSelection) {
                        case ReportColumnSelectionsEnum.ClientCode:
                            sb.Append("<td>" + record.ClientCode + "</td>");
                            break;
                        case ReportColumnSelectionsEnum.ServiceName:
                            sb.Append("<td>" + string.Format(serviceNameFormat, record.ServiceStr, PreviousGroupCount) + "</td>");
                            break;
                        case ReportColumnSelectionsEnum.Staff:
                            if (alternateName != null) {
                                sb.Append("<td>" + alternateName + "</td>");
                            }
                            else {
                                sb.Append("<td>" + record.StaffNamesStr + "</td>");
                            }
                            break;
                        case ReportColumnSelectionsEnum.ServiceHours:
                            sb.Append($"<td>{record.ServiceHours:n2}</td>");
                            break;
                        case ReportColumnSelectionsEnum.ServiceDate:
                            sb.Append("<td>" + (record.ServiceDate?.ToShortDateString() ?? "") + "</td>");
                            break;
                    }
            sb.Append("</tr>");
        }

        private void ApplySummaryRow(StringBuilder sb) {
            if (ColumnSelections.Count <= 1)
                return;

            sb.Append("<tr class='subtotal'>");
            foreach (var columnSelection in ColumnSelections)
                if (columnSelection.ColumnSelection != FirstColumn)
                    switch (columnSelection.ColumnSelection) {
                        case ReportColumnSelectionsEnum.ClientCode:
                            sb.Append("<td><b>" + _previousClientIds.Count + " Client" + (_previousClientIds.Count > 1 ? "s" : string.Empty) + "</b></td>");
                            break;
                        case ReportColumnSelectionsEnum.ServiceName:
                            sb.Append("<td><b>" + PreviousServiceCount + " Service" + (PreviousServiceCount > 1 ? "s" : string.Empty) + "</b></td>");
                            break;
                        case ReportColumnSelectionsEnum.Staff:
                            sb.Append("<td><b>" + _previousSvIds.Count + " Staff</b></td>");
                            break;
                        case ReportColumnSelectionsEnum.ServiceHours:
                            sb.Append($"<td><b>{PreviousHoursCount:n2} Hour" + (PreviousHoursCount != 1 ? "s" : string.Empty) + "</b></td>");
                            break;
                        case ReportColumnSelectionsEnum.ServiceDate:
                            sb.Append("<td></td>");
                            break;
                    }
                else
                    sb.Append("<td></td>");
            sb.Append("</tr>");
        }

        private void SetGroupTotals(StaffClientServiceInformationLineItem record, string alternateName = null) {
            if (GroupingSelections.Any())
                switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
                    case ReportOrderSelectionsEnum.Staff:
                        if (alternateName != null) {
                            PreviousGroupValue = alternateName;
                        }
                        else {
                            PreviousGroupValue = record.StaffNamesStr;
                        }

                        break;
                    case ReportOrderSelectionsEnum.Client:
                        PreviousGroupValue = record.ClientCode ?? "";
                        break;
                    case ReportOrderSelectionsEnum.Service:
                        PreviousGroupValue = record.ServiceStr ?? "";
                        break;
                }
            _previousClientIds.Add(record.ClientId);
            foreach (var each in record.StaffNames)
                _previousSvIds.Add(each.SvId);
            PreviousServiceCount++;
            PreviousHoursCount += record.ServiceHours ?? 0.0;
        }

        private void SetTotals(StaffClientServiceInformationLineItem record) {
            _totalClientIds.Add(record.ClientId);
            foreach (var each in record.StaffNames)
                _totalSvIds.Add(each.SvId);
            TotalServiceCount++;
            TotalHoursCount += record.ServiceHours ?? 0.0;
        }

        protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
            if (!IsStaffFirst) {
                ApplyItemRow(PreviousLineItem, sb);
                ApplySummaryRow(sb);
            }
            else {
                StaffFirstProcess(sb);
            }

            sb.Append("<tr class='grandtotal'>");
            foreach (var columnSelection in ColumnSelections) {
                bool isFirst = columnSelection.ColumnSelection == FirstColumn;
                switch (columnSelection.ColumnSelection) {
                    case ReportColumnSelectionsEnum.ClientCode:
                        sb.Append("<td><b>" + (isFirst ? "Grand Total: " : string.Empty) + _totalClientIds.Count + " Clients</b></td>");
                        break;
                    case ReportColumnSelectionsEnum.ServiceName:
                        sb.Append("<td><b>" + (isFirst ? "Grand Total: " : string.Empty) + TotalServiceCount + " Services</b></td>");
                        break;
                    case ReportColumnSelectionsEnum.Staff:
                        sb.Append("<td><b>" + (isFirst ? "Grand Total: " : string.Empty) + _totalSvIds.Count + " Staff</b></td>");
                        break;
                    case ReportColumnSelectionsEnum.ServiceHours:
                        sb.Append($"<td><b>{TotalHoursCount:n2} Hours</b></td>");
                        break;
                    case ReportColumnSelectionsEnum.ServiceDate:
                        sb.Append("<td></td>");
                        break;
                }
            }
            sb.Append("</tr>");
        }

        //Edits the final output if Staff Name is selected first
        private void StaffFirstProcess(StringBuilder sb) {
            ResetPreviousGroupValues();
            sb.Clear();
            var allStaff = staffNamesDictionary.Values.OrderBy(x => x.Name);

            TotalHoursCount = 0;
            TotalServiceCount = 0;
            PreviousServiceCount =0;
            PreviousLineItem = new StaffClientServiceInformationLineItem();

            foreach (var currentStaff in allStaff) {
                bool isFirst = true;
                bool isStaffIncluded = false;
                var orderedRec = currentStaff.Records.OrderBy(q => 0);

                if (ReportQuery.Orders.Any()) 
                    foreach (var order in ReportQuery.Orders.OrderBy(x => x.DisplayOrder))
                        switch (order.GetType().Name) {
                            case "ServiceDetailOfClientClientCodeReportOrder":
                                orderedRec = orderedRec.ThenBy(q => q.ClientCode);
                                break;
                            case "ServiceDetailOfClientServiceNameReportOrder":
                                orderedRec = orderedRec.ThenBy(q => q.ServiceStr);
                                break;
                        }
                else 
                    orderedRec = orderedRec.ThenBy(q => q.ServiceStr).ThenBy(q => q.ClientCode);
                //DO: give some order by default?

                var names = new List<StaffClientServiceInformationLineItem.StaffName> {
                            new StaffClientServiceInformationLineItem.StaffName {
                                Name = currentStaff.Name
                            }
                        };

                foreach (StaffClientServiceInformationLineItem record in orderedRec) {                 
                    if (!isFirst && GroupingSelections.Any()) {
                        if (IsGrouped(record, isStaffIncluded)) {
                            PreviousGroupCount++;
                            PreviousLineItem.ServiceHours += record.ServiceHours;
                        }
                        else {                            
                            GenerateHtml(PreviousLineItem, sb,currentStaff.Name);
                            //PreviousLineItem = record;
                            PreviousLineItem = new StaffClientServiceInformationLineItem {
                                ServiceStr = record.ServiceStr,
                                ServiceHours = record.ServiceHours,
                                ServiceDate = record.ServiceDate,
                                ClientCode = record.ClientCode,
                                ClientId = record.ClientId,
                            };
                            PreviousLineItem.StaffNames = names;
                            PreviousGroupCount = 1;
                        }                       
                    }
                    else {                       
                        PreviousLineItem = new StaffClientServiceInformationLineItem {
                            ServiceStr = record.ServiceStr,
                            ServiceHours = record.ServiceHours,
                            ServiceDate = record.ServiceDate,
                            ClientCode = record.ClientCode,
                            ClientId = record.ClientId
                        };
                        PreviousLineItem.StaffNames = names;
                    }
                    
                    isFirst = false;
                    SetGroupTotals(record);
                    SetTotals(record);
                }
                GenerateHtml(PreviousLineItem, sb,currentStaff.Name);
                PreviousGroupCount = 1;
                //Apply summary for each staff
                ApplySummaryRow(sb);
                ResetPreviousGroupValues();
            } //All Staff
        }

 
        protected override string BuildTrueCSVLine(StaffClientServiceInformationLineItem record) {
            bool applyComma = false;
            var sb = new StringBuilder();
            foreach (var columnSelection in ColumnSelections) {
                if (applyComma)
                    sb.Append(",");
                switch (columnSelection.ColumnSelection) {
                    case ReportColumnSelectionsEnum.ClientCode:
                        sb.AppendQuotedCSVData(record.ClientCode);
                        break;
                    case ReportColumnSelectionsEnum.ServiceName:
                        sb.AppendQuotedCSVData(record.ServiceStr);
                        break;
                    case ReportColumnSelectionsEnum.Staff:
                        sb.AppendQuotedCSVData(record.StaffNamesStr);
                        break;
                    case ReportColumnSelectionsEnum.ServiceHours:
                        sb.AppendQuotedCSVData($"{record.ServiceHours:n2}");
                        break;
                    case ReportColumnSelectionsEnum.ServiceDate:
                        sb.AppendQuotedCSVData(record.ServiceDate?.ToShortDateString() ?? string.Empty);
                        break;
                }
                applyComma = true;
            }
            return sb.ToString();
        }

        protected override IEnumerable<StaffClientServiceInformationLineItem> PerformSelect(IOrderedQueryable<ServiceDetailOfClient> query) {
            //grab the staff and funding filter criteria to use in PrepareRecord(...)
            _fundingSourceIds = ReportQuery.Filters.OfType<ServiceDetailStaffFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
            _svIds = ReportQuery.Filters.OfType<ServiceDetailStaffFilter>().SingleOrDefault()?.SvIds.NotNull(ids => new HashSet<int?>(ids));

            //duplicates some logic from PrepareRecord, but factoring it out would make this report differ even more from the other reports with this logic
            var staffFundingFilter = PredicateBuilder.New<StaffFunding>(true);
            if (_fundingSourceIds != null)
                staffFundingFilter.And(sf => sf.FundingSourceId != null && sf.PercentFund > 0 && _fundingSourceIds.Contains(sf.FundingSourceId));
            if (_svIds != null)
                staffFundingFilter.And(sf => _svIds.Contains(sf.SvId));

            var db = ReportContainer.InfonetContext;

            IEnumerable<StaffClientServiceInformationLineItem> ret = query.Select(q => new StaffClientServiceInformationLineItem {
                ClientId = q.ClientID,
                ClientCode = q.ClientCase.Client.ClientCode,
                ServiceDate = q.ServiceDate,
                ServiceHours = q.ReceivedHours,
                ServiceStr = q.TLU_Codes_ProgramsAndServices.Description,
                StaffNames = db.T_StaffVolunteer.Where(sv => StaffFunding.ServiceDetail.Invoke(q).AsQueryable().Where(staffFundingFilter).Select(sf => sf.SvId).Contains(sv.SvId)).Select(sv => new StaffClientServiceInformationLineItem.StaffName { SvId = sv.SvId, Name = sv.FirstName + " " + sv.LastName }).OrderBy(sn => sn.Name),
                StaffAndFunding = StaffFunding.ServiceDetail.Invoke(q)
            });

            if (ReportQuery.Orders.Any()) {
                var orderedRet = ret.OrderBy(q => 0);
                foreach (var order in ReportQuery.Orders)
                    switch (order.GetType().Name) {
                        case "ServiceDetailOfClientClientCodeReportOrder":
                            orderedRet = orderedRet.ThenBy(q => q.ClientCode);
                            break;
                        case "ServiceDetailOfClientServiceNameReportOrder":
                            orderedRet = orderedRet.ThenBy(q => q.ServiceStr); //KMS DO this isn't technically correct....would need to sort by display order and then service name
                            break;
                        case "ServiceDetailOfClientStaffNameReportOrder":
                            orderedRet = orderedRet.ThenBy(q => q.StaffNames.First().Name); //KMS DO this doesn't sort properly when multiple staff groups start with the same staff name
                            break;
                    }
                return orderedRet;
            }
            return ret;
        }

        protected override void PrepareRecord(StaffClientServiceInformationLineItem record) {
            record.StaffNamesStr = string.Join(",", record.StaffNames.Select(sn => sn.Name).OrderBy(x => x));
            if (ReportColumnSelectionsEnum.Staff == FirstColumn) {
                StaffClientServiceInformationLineItem tempRecord = new StaffClientServiceInformationLineItem();
                StaffTracker currentTracker = null;
                foreach (var n in record.StaffNames) {
                    if (staffNamesDictionary.ContainsKey(n.Name)) {
                        currentTracker = staffNamesDictionary[n.Name];
                    }
                    else {
                        currentTracker = new StaffTracker(n.Name);
                        staffNamesDictionary[n.Name] = currentTracker;
                    }
                    currentTracker.Records.Add(record);
                }
            }

            double averagePercentFundedPerStaff = 1;
            if (_fundingSourceIds != null) {
                int staffCount = record.StaffAndFunding.Select(sf => sf.SvId).Distinct().Count();
                int percentFundedSum = record.StaffAndFunding.Where(sf => sf.FundingSourceId != null && _fundingSourceIds.Contains(sf.FundingSourceId) && (_svIds?.Contains(sf.SvId) ?? true)).Sum(sf => sf.PercentFund ?? 0);
                averagePercentFundedPerStaff = percentFundedSum / 100.0 / staffCount;
            }
            record.ServiceHours *= averagePercentFundedPerStaff;
        }
    }

    public class StaffClientServiceInformationLineItem {
        public int? ClientId { get; set; }
        public string ClientCode { get; set; }
        public IEnumerable<StaffName> StaffNames { get; set; }
        public string StaffNamesStr { get; set; }
        public string ServiceStr { get; set; }
        public DateTime? ServiceDate { get; set; }
        public double? ServiceHours { get; set; }
        public IEnumerable<StaffFunding> StaffAndFunding { get; set; }
        public class StaffName {
            public int SvId { get; set; }
            public string Name { get; set; }
        }
    }
}