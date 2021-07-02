using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Infonet.Data.Helpers;
using Infonet.Data.Models.Reporting;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports;
using Infonet.Reporting.ViewModels;
using PagedList;

namespace Infonet.Web.ViewModels.Reporting {
	public class CompletedScheduledReportViewModel : StandardReportSpecification, IReportCenterSelectionBase, IDateRangeWithOutputType {
		public CompletedScheduledReportViewModel() {
			ReportRecords = new List<ReportRecord>();
			PageNumber = 1;
			PageSize = 10;
		}
		public IPagedList PagingMetaData { get; set; }

		public int PageSize { get; set; }

		public int PageNumber { get; set; }

		[DisplayName("Submit Date")]
		public DateTime? SubmittedDate { get; set; }

		public string ViewRole { get; set; }

		public string Range { get; set; }

		public ReportOutputType OutputType { get; set; }

		public PdfSize PdfSize { get; set; }

		public PdfOrientation Orientation { get; set; }

		public List<CenterInfo> Centers { get; set; }

		public List<ReportRecord> ReportRecords { get; set; }

		public List<ReportRecord> ReportRecordsDisplayed { get; set; }

		public List<SelectListItem> CenterActions { get; set; }

		public List<SelectListItem> CenterActionNoAction { get; set; }

		[DisplayName("Run Date")]
		public DateTime RunDate { get; set; }

		[DisplayName("Type")]
		public string ReportType { get; set; }

		[DisplayName("Submitter")]
		public string SubmitterCenter { get; set; }

		public IEnumerable<SelectListItem> FilterRunDate { get; set; }

		public string SelectedRunDate { get; set; }

		public IEnumerable<SelectListItem> FilterTitle { get; set; }

		public string SelectedTitle { get; set; }

		public IEnumerable<SelectListItem> FilterType { get; set; }

		public string SelectedType { get; set; }

		public IEnumerable<SelectListItem> FilterBeginDate { get; set; }

		public string SelectedBeginDate { get; set; }

		public IEnumerable<SelectListItem> FilterEndDate { get; set; }

		public string SelectedEndDate { get; set; }

		public IEnumerable<SelectListItem> FilterSubmittedDate { get; set; }

		public string SelectedSubmittedDate { get; set; }

		public IEnumerable<SelectListItem> FilterCenterApproval { get; set; }

		public string SelectedCenterApproval { get; set; }

		public IEnumerable<SelectListItem> FilterCenterApprovalRejectionDate { get; set; }

		public string SelectedCenterApprovalRejectionDate { get; set; }

		public IEnumerable<SelectListItem> FilterCenterName { get; set; }

		public int? SelectedCenterId { get; set; }

		public int? SelectedSubmitterCenterId { get; set; }

		public IEnumerable<SelectListItem> FilterFundingSource { get; set; }

		public IEnumerable<SelectListItem> FilterSubmitterCenter { get; set; }

		public string SelectedFundingSource { get; set; }

		public string ReportTitle { get; set; }

		public int? RptJobId { get; set; }

		public class ReportRecord {
			public string Title { get; set; }

			public DateTime? StartDate { get; set; }

			public DateTime? EndDate { get; set; }

			public int?[] CenterIds { get; set; }

			public ReportSelection ReportSelection { get; set; }

			public SubReportSelection[] SubReportSelection { get; set; }

			[Display(Name = "Funding")]
			public int?[] FundingSourceIds { get; set; }

			public string ReportTypeDescription { get; set; }

			public string SpecialCenterSelectionType { get; set; }

			public string SpecialFundingSelectionType { get; set; }

			public int SelectedCenterActionId { get; set; }

			[Display(Name = "Delete")]
			public bool FlagForDelete { get; set; }

            [Display(Name = "ID")]
            public int Id { get; set; }

			public DateTime? RunDate { get; set; }

			public bool CenterActionModified { get; set; }

			public DateTime? SubmittedDate { get; set; }

			public byte[] RowVersion { get; set; }

			public List<SelectListItem> CenterApprovalDescription { get; set; }

			public DateTime? CenterActionApprovalDate { get; set; }

			public int? ApprovalStatusId { get; set; }

			public int? StatusId { get; set; }

			public string CenterComment { get; set; }

			//Used for Filters
			public List<SelectListItem> Center { get; set; }

			public List<SelectListItem> Type { get; set; }

			public List<SelectListItem> FundingSourceDescription { get; set; }

			public List<SelectListItem> CenterAction { get; set; }

			public List<SelectListItem> SubmitterCenter { get; set; }
			//End Used for Filters
		}

		public class CompletedRecord {
			public string SpecificationJson { get; set; }

			public ReportJobApproval Approval { get; set; }

			public int? Id { get; set; }

			public DateTime? ScheduledForDate { get; set; }

			public DateTime? SubmittedDate { get; set; }

			public int CenterActionId { get; set; }

			public byte[] RowVersion { get; set; }

			public DateTime? CenterActionApprovalDate { get; set; }
			public int? StatusId { get; internal set; }
		}

		public enum ReportTypeDescription {
			[Display(Name = "Demographics")]
			rptDemo = 1,

			[Display(Name = "Medical CJ")]
			rptMed = 2,

			[Display(Name = "Services")]
			rptServ = 3
		}

		public enum CenterActionsEnum {
			[Display(Name = "No Action")]
			NoAction = 0,

			[Display(Name = "Review")]
			Review = 1,

			[Display(Name = "Approve")]
			Approve = 2
		}

		public enum ReportApprovalStatusDescription {
			[Display(Name = "Review")]
			ReviewOnly = ReportJobApproval.Status.ReviewOnly,

			[Display(Name = "View Report")]
			Pending = ReportJobApproval.Status.Pending,

			[Display(Name = "Approved")]
			Approved = ReportJobApproval.Status.Approved,

			[Display(Name = "Rejected")]
			Rejected = ReportJobApproval.Status.Rejected
		}
	}
}