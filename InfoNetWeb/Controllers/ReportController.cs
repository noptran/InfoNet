using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Helpers;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ViewModels;
using Infonet.Usps.Data;
using Infonet.Web.Mvc;
using Newtonsoft.Json;
using Rotativa;
using Infonet.Reporting;
using Infonet.Web.Models.Reporting;
using Infonet.Data.Models.Reporting;
using Microsoft.AspNet.Identity;
using static Infonet.Reporting.ViewModels.SubmitReportViewModel;
using System.Configuration;
using Infonet.Reporting.StandardReports;
using System.Data.Entity.Infrastructure;
using Infonet.Data.Models.Centers;
using System.Data.Entity;
using Infonet.Data;
using System.Net.Mail;
using Infonet.Core;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Reporting;
using Rotativa.Options;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "CACADMIN, CACDATAENTRY, DVADMIN, DVDATAENTRY, DVDATAREADER, DVDATAEXPORTER, SADATAENTRY, SAADMIN")]
	public class ReportController : InfonetControllerBase {
		#region constants
		private const string HTML = ".html";
		private const string PDF = ".pdf";
		private const string CSV = ".csv";
		private const string CSV_CONTENT_TYPE = "text/csv";
		#endregion

		#region static configuration
		// ReSharper disable once InconsistentNaming
		private static readonly string _WkHtmlToPdfOptions = ConfigurationManager.AppSettings["Reporting:WkHtmlToPdf:Options"];
		private static readonly string _ReportingService_OutputDirectory = ConfigurationManager.AppSettings["ReportingService:OutputDirectory"];
		private static readonly string _HelpDeskEmail = ConfigurationManager.AppSettings["HelpDeskEmail"];
		private static readonly string _HelpDeskPhone = ConfigurationManager.AppSettings["HelpDeskPhone"];
		private static readonly bool _EnableEmail = ConvertNull.ToBoolean(ConfigurationManager.AppSettings["EnableEmail"]) ?? false;
		#endregion

		private ReportFilterListGenerator _filterListGenerator;

		protected override void OnActionExecuting(ActionExecutingContext filterContext) {
			base.OnActionExecuting(filterContext);
			_filterListGenerator = new ReportFilterListGenerator(Session, db, EnsureDisposal(new UspsContext()));
		}

		//KMS DO replacing spaces might not be enough to create filenames below
		private ActionResult RunReport(ReportContainer container, ReportOutputType outputType, PdfSize pdfPageSize, PdfOrientation pdfOrientation) {
			switch (outputType) {
				case ReportOutputType.Csv:
					Response.ClearHeaders();
					Response.ContentType = CSV_CONTENT_TYPE;
					Response.AddHeader("Content-Disposition", $"attachment;filename={container.Title.Replace(" ", "")}{CSV}");
					using (var sw = new StreamWriter(Response.OutputStream, Encoding.UTF8, BufferHelper.DEFAULT_STREAMWRITER_BUFFER_SIZE, true))
						container.Write(null, sw);
					return new EmptyResult();
				case ReportOutputType.Html:
					return View("ReportContainer", container);
				case ReportOutputType.Pdf:
					ViewBag.IsPDF = true;
					var pdfView = new ViewAsPdf("ReportContainer", container) {
						ContentDisposition = ContentDisposition.Attachment,
						FileName = $"{container.Title.Replace(" ", "")}_{DateTime.Now:yyyy-MM-dd-HHmm}{PDF}",
						PageSize = (Size)pdfPageSize,
						PageOrientation = (Orientation)pdfOrientation
					};
					if (_WkHtmlToPdfOptions != null)
						pdfView.CustomSwitches = _WkHtmlToPdfOptions;
					return pdfView;
				default:
					throw new NotSupportedException($"{nameof(outputType)} {outputType} is not supported");
			}
		}

		#region Exception Reports
		public ActionResult ExceptionReport() {
			var model = new ExceptionReportViewModel();
			model.AvailableSelectionTypes = GetExceptionReportSelectionsForProvider();
			model.AvailableUNRUDataFieldsSelections = GetExceptionUnknownNotReportedUnassignedFields();
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ExceptionReportRun(ExceptionReportViewModel model) {
			if (!ModelState.IsValid)
				return View("ReportError");

			model.Provider = Session.Center().Provider;
			var container = EnsureDisposal(new ExceptionReportFactory().RunReport(model));
			return RunReport(container, model.OutputType, model.PdfSize, model.Orientation);
		}
		#endregion

		#region Management Reports
		public ActionResult ManagementReport() {
			var model = new ManagementReportViewModel();
			PopulateModel(model);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ManagementReportRun(ManagementReportViewModel model) {
			if (!ModelState.IsValid)
				return View("ReportError");

			model.Provider = Session.Center().Provider;
			var container = EnsureDisposal(new ManagementReportFactory().RunReport(model));
			return RunReport(container, model.OutputType, model.PdfSize, model.Orientation);
		}

		public ActionResult AddNewIncomeRange() {
			ManagementReportViewModel model = new ManagementReportViewModel();
			return PartialView("~/Views/Report/Management/ColumnSelections/_NewIncomeRangePartial.cshtml", model);
		}

		private void PopulateModel(ManagementReportViewModel model) {
			model.AvailableSelections = GetManagementReportSelectionsForProvider();
			model.CancellationAvailableColumnSelections = GetManagementReportCancellationColumnSelections();
			model.ClientDetailAvailableColumnSelections = GetManagementReportClientDetailColumnSelections();
			model.ClientDetailAvailableOrderSelections = GetManagementReportClientDetailOrderSelections();
			model.OtherStaffActivityAvailableColumnSelections = GetManagementReportOtherStaffActivityColumnSelections();
			model.StaffClientServiceAvailableColumnSelections = GetManagementReportStaffClientServiceColumnSelections();
			model.StaffHotlineAvailableColumnSelections = GetManagementReportStaffHotlineColumnSelections();
			model.StaffHotlineAvailableOrderSelections = GetManagementReportStaffHotlineOrderSelections();
		}

		private List<ReportOrderSelectionsEnum> GetManagementReportClientDetailOrderSelections() {
			List<ReportOrderSelectionsEnum> orders = new List<ReportOrderSelectionsEnum>();
			orders.Add(ReportOrderSelectionsEnum.Town);
			orders.Add(ReportOrderSelectionsEnum.Township);
			orders.Add(ReportOrderSelectionsEnum.County);
			orders.Add(ReportOrderSelectionsEnum.ZipCode);
			orders.Add(ReportOrderSelectionsEnum.State);
			return orders;
		}

		private List<ReportColumnSelectionsEnum> GetManagementReportClientDetailColumnSelections() {
			List<ReportColumnSelectionsEnum> columns = new List<ReportColumnSelectionsEnum>();
			columns.Add(ReportColumnSelectionsEnum.ClientCode);
			if (Session.Center().Provider != Provider.SA) {
				columns.Add(ReportColumnSelectionsEnum.CaseID);
			}
			columns.Add(ReportColumnSelectionsEnum.ClientType);
			columns.Add(ReportColumnSelectionsEnum.FirstContactDate);
			columns.Add(ReportColumnSelectionsEnum.Age);
			columns.Add(ReportColumnSelectionsEnum.Gender);
			columns.Add(ReportColumnSelectionsEnum.Race);
			if (Session.Center().Provider != Provider.CAC) {
				columns.Add(ReportColumnSelectionsEnum.Ethnicity);
                columns.Add(ReportColumnSelectionsEnum.SexualOrientation);
			}
			columns.Add(ReportColumnSelectionsEnum.Town);
			columns.Add(ReportColumnSelectionsEnum.Township);
			columns.Add(ReportColumnSelectionsEnum.County);
			columns.Add(ReportColumnSelectionsEnum.State);
			columns.Add(ReportColumnSelectionsEnum.ZipCode);
			columns.Add(ReportColumnSelectionsEnum.ServiceName);
			columns.Add(ReportColumnSelectionsEnum.Staff);
			columns.Add(ReportColumnSelectionsEnum.ServiceHours);
			columns.Add(ReportColumnSelectionsEnum.ServiceDate);
            if (Session.Center().Provider == Provider.DV) {
                columns.Add(ReportColumnSelectionsEnum.ShelterBeginDate);
                columns.Add(ReportColumnSelectionsEnum.ShelterEndDate);
            }           
            return columns;
		}

		private List<ReportColumnSelectionsEnum> GetManagementReportStaffClientServiceColumnSelections() {
			List<ReportColumnSelectionsEnum> columns = new List<ReportColumnSelectionsEnum>();
			columns.Add(ReportColumnSelectionsEnum.Staff);
			columns.Add(ReportColumnSelectionsEnum.ClientCode);
			columns.Add(ReportColumnSelectionsEnum.ServiceName);
			columns.Add(ReportColumnSelectionsEnum.ServiceDate);
			columns.Add(ReportColumnSelectionsEnum.ServiceHours);
			return columns;
		}

		private List<ReportColumnSelectionsEnum> GetManagementReportStaffHotlineColumnSelections() {
			List<ReportColumnSelectionsEnum> columns = new List<ReportColumnSelectionsEnum>();
			columns.Add(ReportColumnSelectionsEnum.Staff);
			columns.Add(ReportColumnSelectionsEnum.HotlineCallType);
			columns.Add(ReportColumnSelectionsEnum.HotlineCallDate);
			columns.Add(ReportColumnSelectionsEnum.HotlineCallContacts);
			columns.Add(ReportColumnSelectionsEnum.HotlineCallTime);
			columns.Add(ReportColumnSelectionsEnum.Town);
			columns.Add(ReportColumnSelectionsEnum.Township);
			columns.Add(ReportColumnSelectionsEnum.County);
			columns.Add(ReportColumnSelectionsEnum.ZipCode);
			return columns;
		}

		private List<ReportOrderSelectionsEnum> GetManagementReportStaffHotlineOrderSelections() {
			List<ReportOrderSelectionsEnum> orders = new List<ReportOrderSelectionsEnum>();
			orders.Add(ReportOrderSelectionsEnum.Town);
			orders.Add(ReportOrderSelectionsEnum.Township);
			orders.Add(ReportOrderSelectionsEnum.County);
			orders.Add(ReportOrderSelectionsEnum.ZipCode);
			return orders;
		}

		private List<ReportColumnSelectionsEnum> GetManagementReportOtherStaffActivityColumnSelections() {
			List<ReportColumnSelectionsEnum> columns = new List<ReportColumnSelectionsEnum>();
			columns.Add(ReportColumnSelectionsEnum.Staff);
			columns.Add(ReportColumnSelectionsEnum.Activity);
			columns.Add(ReportColumnSelectionsEnum.Date);
			columns.Add(ReportColumnSelectionsEnum.ConductHours);
			columns.Add(ReportColumnSelectionsEnum.TravelHours);
			columns.Add(ReportColumnSelectionsEnum.PrepareHours);
			return columns;
		}

		private List<ReportColumnSelectionsEnum> GetManagementReportCancellationColumnSelections() {
			List<ReportColumnSelectionsEnum> columns = new List<ReportColumnSelectionsEnum>();
			columns.Add(ReportColumnSelectionsEnum.ClientCode);
			columns.Add(ReportColumnSelectionsEnum.ServiceName);
			columns.Add(ReportColumnSelectionsEnum.Staff);
			columns.Add(ReportColumnSelectionsEnum.Date);
			columns.Add(ReportColumnSelectionsEnum.Reason);
			return columns;
		}
		#endregion

		#region Standard Reports

		public ActionResult StandardReport() {
			var model = new StandardReportViewModel();
			PopulateModel(model);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult StandardReportRun(StandardReportViewModel model) {
			ValidateStandardReport(model);
			if (!ModelState.IsValid)
				return View("ReportError");

			model.Provider = Session.Center().Provider;
			var container = EnsureDisposal(StandardReportFactory.RunStandardReport(model));
			return RunReport(container, model.OutputType, model.PdfSize, model.Orientation);
		}

		private void ValidateStandardReport(StandardReportViewModel model) {
			if (ModelState["StartDate"].Errors.Count == 0 && !model.StartDate.HasValue)
				ModelState.AddModelError("StartDate", "You must specify a Start date.");
			if (ModelState["EndDate"].Errors.Count == 0 && !model.EndDate.HasValue)
				ModelState.AddModelError("EndDate", "You must specify an End date.");
			if (model.SubReportSelections == null || model.SubReportSelections.Length == 0)
				ModelState.AddModelError("SubReportSelections", "You must specify at least one report to generate.");
			if (model.CenterIds == null || model.CenterIds.Length == 0)
				ModelState.AddModelError("CenterIds", "You must specify at least one center.");
		}

		private void PopulateModel(StandardReportViewModel model) {
			model.AvailableSelections = GetStandardReportSelectionsForProvider();
			model.OffenderRelationshipDefault = _filterListGenerator.GetLookupValues(Lookups.RelationshipToClient);
			model.GenderDefault = _filterListGenerator.GetLookupValues(Lookups.GenderIdentity);
			model.EthnicityDefault = _filterListGenerator.GetLookupValues(Lookups.Ethnicity);
			if (Session.Center().Provider == Provider.CAC) {
				model.RaceDefault = _filterListGenerator.GetLookupValues(Lookups.Race);
			} else {
				model.RaceDefault = _filterListGenerator.GetLookupValues(Lookups.RaceHud);
			}
			if (Session.Center().Provider == Provider.DV) {
				model.ClientTypeDefault = _filterListGenerator.GetShelterType();
			}
			// default PDF output settings
			model.PdfSize = PdfSize.Letter;
			model.Orientation = PdfOrientation.Portrait;
		}
		#endregion

		#region Submit Reports
		[OverrideAuthorization]
		[Authorize(Roles = "SACOALITIONADMIN, DVCOALITIONADMIN, CACCOALITIONADMIN, DHSCOALITIONADMIN,CDFSSCOALITIONADMIN")]
		public ActionResult SubmitReport() {
			var model = new SubmitReportViewModel();
			SubmitReportPopulateModel(model);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[OverrideAuthorization]
		[Authorize(Roles = "SACOALITIONADMIN, DVCOALITIONADMIN, CACCOALITIONADMIN, DHSCOALITIONADMIN,CDFSSCOALITIONADMIN")]
		public ActionResult SubmitReport(SubmitReportViewModel model) {
			SubmitReportValidate(model);
			if (ModelState.IsValid) {
				//if (model.Title == null) {
				//	model.Title = "";
				//}
				SubmitReportViewModel originalModel = (SubmitReportViewModel)model.Clone();
				SubmitReportAddRecords(model);
				model = originalModel;
                SubmitReportPopulateModelFilters(model);
            }

			model.AvailableSelections = GetSubmitReportSelectionsForProvider();
			return View(model);
		}

		private void SubmitReportAddRecords(SubmitReportViewModel model) {
			int cntCheckedCenters = model.Centers.Count(a => a.isChecked);
			int cntParentCenters = model.Centers.GroupBy(x => x.ParentCenterId).Select(y => y.First()).Distinct().Count();

			switch (model.CenterSelectionRadio) {
				case "aggregateall":
					if (model.Centers.All(c => c.isChecked)) {
						model.SpecialCenterSelectionType = "(All Centers and Satellites)";
					} else if (cntCheckedCenters > 1 && cntParentCenters == 1) {
						model.SpecialCenterSelectionType = "(Misc. Related Agencies)";
					} else if (cntCheckedCenters > 1 && cntParentCenters > 1) {
						model.SpecialCenterSelectionType = "(Misc. Unrelated Agencies)";
					} else {
						model.SpecialCenterSelectionType = model.Centers.Where(c => c.isChecked).Select(x => x.CenterName).First();
					}
					break;
				case "aggregatebycenter":
					model.SpecialCenterSelectionType = "(Misc. Related Agencies)";
					break;
				case "individual":
					model.SpecialCenterSelectionType = model.Centers.Where(c => c.isChecked).Select(x => x.CenterName).First();
					break;
			}

			var cntFundingFilters = model.FundingFilter?.Count(m => m.IsChecked);

			if (cntFundingFilters == null || cntFundingFilters == 0) {
				model.SpecialFundingSelectionType = "(No Filters)";
			} else {
				int cntVAWATotal = model.FundingFilter.Count(x => x.Description.Any(y => y.ToString().StartsWith("VAWA", StringComparison.Ordinal)));
				int cntVAWASelected = model.FundingFilter.Count(x => x.Description.Any(y => y.ToString().StartsWith("VAWA", StringComparison.Ordinal)) && x.IsChecked);

				int cntVOCATotal = model.FundingFilter.Count(x => x.Description.Any(y => y.ToString().StartsWith("VOCA", StringComparison.Ordinal)));
				int cntVOCASelected = model.FundingFilter.Count(x => x.Description.Any(y => y.ToString().StartsWith("VOCA", StringComparison.Ordinal)) && x.IsChecked);

				int cntFSSelected = model.FundingFilter.Count(x => x.IsChecked);

				if (AllEqual(cntFSSelected, cntVAWATotal, cntVAWASelected) && cntVOCATotal == cntVOCASelected) {
					model.SpecialFundingSelectionType = "(All VAWA and VOCA)";
				} else if (cntVOCATotal != cntVOCASelected && cntVAWATotal == cntVAWASelected) {
					model.SpecialFundingSelectionType = "(All VAWA)";
				} else {
					model.SpecialFundingSelectionType = "(Misc. Funding)";
				}
			}

			foreach (int reportType in model.ReportTypes) {
				model.ReportSelection = (ReportSelection)reportType;

				int[] subReports = new int[0];

				switch (model.ReportSelection) {
					case ReportSelection.StdRptClientInformation:
						subReports = model.SubReportGroup1.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
						break;
					case ReportSelection.StdRptMedicalCjProcess:
						subReports = model.SubReportGroup2.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
						break;
					case ReportSelection.StdRptServicePrograms:
						subReports = model.SubReportGroup3.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
						break;
				}

				model.SubReportSelections = subReports.Select(e => (SubReportSelection)e).ToArray();
				switch (model.CenterSelectionRadio) {
					case "aggregateall":
						model.CenterIds = model.Centers.Where(m => m.isChecked).Select(m => m.CenterId).Cast<int?>().ToArray();
						SubmitReportFundingFilters(model);
						break;
					case "aggregatebycenter":
						foreach (int parentCenter in model.Centers.Where(m => m.isChecked).GroupBy(m => m.ParentCenterId).Select(g => g.First().ParentCenterId)) {
							model.ApprovalCenterId = parentCenter;
							model.CenterIds = model.Centers.Where(m => m.isChecked && m.ParentCenterId == parentCenter).Select(m => m.CenterId).Cast<int?>().ToArray();
							SubmitReportFundingFilters(model);
						}

						break;
					case "individual":
						var individualCenters = model.Centers.Where(m => m.isChecked).Select(m => new { m.CenterId, m.CenterName }).ToList();
						foreach (var center in individualCenters) {
							model.ApprovalCenterId = center.CenterId;
							model.SpecialCenterSelectionType = center.CenterName;
							model.CenterIds = new[] { (int?)center.CenterId };
							SubmitReportFundingFilters(model);
						}

						break;
				}
			}

			AddSuccessMessage("Your changes have been successfully saved.");
			db.SaveChanges();

			if (model.RunDate > DateTime.Today && model.reportEmailList != null && model.reportEmailList.Any()) {
				foreach (var item in model.reportEmailList) {

					var specification = JsonConvert.DeserializeObject<StandardReportSpecification>(item.SpecificationJson, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
					var job = item.ReportJob;
					job.SubmitterCenter = db.T_Center.Find(item.ReportJob.SubmitterCenterId);

					var emailAddresses = GetDirectorEmailAddresses(db, item.CenterIds.Cast<int>());
					if (emailAddresses.Length > 0)
						using (var smtp = new SmtpClient())
						using (var email = new MailMessage { IsBodyHtml = true }) {
							foreach (string each in emailAddresses)
								email.To.Add(each);
							email.Subject = $"InfoNet Report Notification: {specification.Title}";
							email.Body = ComposeEmailBodySubmit(specification, job);

							job.Log($"Sending email to {emailAddresses.ToConjoinedString("and")}");

							if (_EnableEmail)
								try {
									smtp.Send(email);
								} catch (Exception e) {
									job.Log(e.ToString());
								}
						}
				}
			}
		}

		private void SubmitReportAddRecord(SubmitReportViewModel model) {
			var reportJob = new ReportJob();
			reportJob.ExpirationDate = DateTime.Now.AddDays(Convert.ToInt32(ConfigurationManager.AppSettings["ReportJobExpiratonDays"]));
			reportJob.ScheduledForDate = model.RunDate;
			reportJob.SpecificationJson = JsonConvert.SerializeObject(new StandardReportSpecification(model), new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
			reportJob.EnterStatus(ReportJob.Status.Ready);
			reportJob.SubmittedDate = reportJob.StatusDate;
			reportJob.SubmitterCenterId = Session.Center().Id;
			reportJob.SubmitterUserName = User.Identity.GetUserName();

			if (model.CenterAction == (int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve || model.CenterAction == (int)CompletedScheduledReportViewModel.CenterActionsEnum.Review) {
				//SubmitReportAddApprovalRecord(model, reportJob);
				reportJob.Approval = SubmitReportAddApprovalRecord(model, reportJob);
			}
			db.RPT_ReportJobs.Add(reportJob);

			if (model.CenterAction == (int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve && (model.CenterSelectionRadio == "aggregatebycenter" || model.CenterSelectionRadio == "individual")) {
				model.reportEmailList.Add(new ReportEmailList {
					SpecificationJson = reportJob.SpecificationJson,
					CenterIds = model.CenterIds,
					ReportJob = reportJob
				});
			}
		}

		private ReportJobApproval SubmitReportAddApprovalRecord(SubmitReportViewModel model, ReportJob reportJob) {
			var reportJobApproval = new ReportJobApproval();

			reportJobApproval.CenterIds = model.CenterIds.Cast<int>();
			reportJobApproval.StatusDate = DateTime.Now;
			reportJobApproval.StatusId = model.CenterAction;

			reportJobApproval = ReportJobApprovalSystemMessage(reportJobApproval, reportJob);

			return reportJobApproval;
		}

		private void SubmitReportFundingFilters(SubmitReportViewModel model) {
			if (model.FundingFilter == null || model.FundingFilter.All(m => m.IsChecked == false)) {
				model.FundingSourceIds = null;
				SubmitReportAddRecord(model);
			} else {

				var individualFundingFilters = model.FundingFilter.Where(m => m.IsChecked).Select(x => new { x.CodeId, x.Description }).ToList();

				if (model.FundingFilter.Any(m => m.IsChecked) && model.FundingFilterRadio == "indivdual") {
					foreach (var fundingFilter in individualFundingFilters) {
						model.SpecialFundingSelectionType = fundingFilter.Description;
						model.FundingSourceIds = new[] { fundingFilter.CodeId };
						SubmitReportAddRecord(model);
					}
				} else if (model.FundingFilterRadio == "aggregate") {
					model.FundingSourceIds = individualFundingFilters.Select(m => m.CodeId).ToArray();
					SubmitReportAddRecord(model);
				}
			}
		}

		private static bool AllEqual<T>(params T[] values) {
			if (values == null || values.Length == 0)
				return true;
			return values.All(v => v.Equals(values[0]));
		}

		private void SubmitReportValidate(SubmitReportViewModel model) {
			if (model.CenterSelectionRadio == "aggregateall" && (model.CenterAction == 1 || model.CenterAction == 2)) {
				ModelState.AddModelError("Centers", "When Aggregate All is chosen, Review is not allowed.");
			}

			if (model.EndDate < model.StartDate) {
				ModelState.AddModelError("", "Start Date must be before End date.");
			}
			if (!model.StartDate.HasValue || !model.EndDate.HasValue) {
				ModelState.AddModelError("", "You must specify a Start and End date.");
			}
            if (Session.Center().Provider == Provider.DV && User.IsInRole("CDFSSCOALITIONADMIN")) {
                //add server side validation for when report filter applied

            }
                if (model.SubReportSelections == null || model.SubReportSelections.Length == 0) {
				//ModelState.AddModelError("", "You must specify at least one report to generate.");
			}
			if (model.CenterIds == null || model.CenterIds.Length == 0) {
				ModelState.AddModelError("", "You must specify at least one center.");
			}
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("", "You must specify Batch report title.");
            }
        }

		private void SubmitReportPopulateModel(SubmitReportViewModel model) {
			model.AvailableSelections = GetSubmitReportSelectionsForProvider();
			model.CenterAction = (int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction;
			model.CenterSelectionRadio = "aggregateall";
			model.FundingFilterRadio = "aggregate";
			model.RunDate = DateTime.Now;

			int centerId = Session.Center().Top.Id;

			model.Centers = new List<CenterInfo>();

			if (centerId == -5) {
				model.Centers = db.T_Center
				.Where(c => c.ProviderID == 1 && c.IsRealCenter == true).Select(c =>
					new CenterInfo {
						CenterId = c.CenterID,
						CenterName = c.CenterName,
						ParentCenterId = c.ParentCenterID ?? c.CenterID,
						isReal = c.IsRealCenter ?? false,
						isSatellite = c.CenterID != c.ParentCenterID,
						isChecked = false
					})
			.OrderBy(m => m.CenterName).ToList();
			} else {
				model.Centers = db.Ts_CenterAdministrators
					.Where(ts => ts.CenterAdminId == centerId && ts.CenterAdminActive)
				.Join(db.T_Center, ts => ts.CenterId, t => t.CenterID, (ts, t) => new CenterInfo {
					CenterId = t.CenterID,
					CenterName = t.CenterName,
					ParentCenterId = t.ParentCenterID ?? t.CenterID,
					isReal = t.IsRealCenter ?? false,
					isSatellite = t.CenterID != t.ParentCenterID,
					isChecked = false
				})
				.OrderBy(m => m.CenterName).ToList();
			}

			var fundingFilter = new List<FundingFilterOptions> { new FundingFilterOptions() };
			if (Session.Center().Top.Id < 0) {
				var centerAdminFundingSourceCodeIds = db.Ts_CenterAdminFundingSources.Where(t => t.CenterAdminID == centerId).Select(t => t.CodeID).ToList();
				fundingFilter = db.TLU_Codes_FundingSource
					.Where(t => centerAdminFundingSourceCodeIds.Contains((int)t.CodeID)).
					OrderBy(m => m.Description).
					Select(m => new FundingFilterOptions { CodeId = m.CodeID, Description = m.Description, IsChecked = false }
					).ToList();
			}

			model.FundingFilter = fundingFilter;
            //Coaltion report filters for CDFSS  
            SubmitReportPopulateModelFilters(model);                   
        }
        private void SubmitReportPopulateModelFilters(SubmitReportViewModel model) {
            if (Session.Center().Provider == Provider.DV && User.IsInRole("CDFSSCOALITIONADMIN")) {
                model.ClientTypeDefault = _filterListGenerator.GetShelterType();
                //Only Hispanic is allowed for Chicago DFSS
                model.RaceDefault = _filterListGenerator.GetLookupValues(Lookups.RaceHud).Where(r => r.CodeId == "7").Select(m => new FilterSelection { CodeId = m.CodeId, Description = m.Description }).ToList();
                //Only Chicago is allowed for Chicago DFSS
                model.CityOrTownsDefault = new List<FilterSelection> { new FilterSelection { CodeId = "Chicago", Description = "Chicago" } };
                model.IsFilterCollapsed = (model.CityOrTowns == null && model.ClientTypeIds == null && model.RaceIds == null);                
            }
        }

        #endregion

            #region Scheduled Reports
        [OverrideAuthorization]
		[Authorize(Roles = "CACADMIN, CACDATAENTRY, DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY, SACOALITIONADMIN, DVCOALITIONADMIN, CACCOALITIONADMIN, DHSCOALITIONADMIN,CDFSSCOALITIONADMIN")]
		public ActionResult ScheduledReport() {
			var model = new CompletedScheduledReportViewModel();
			model.ReportTitle = "Scheduled";
			ViewRole(model);

			var records = ScheduledReportResults(model);

			ScheduledReportPopulateModel(model, records);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[OverrideAuthorization]
		[Authorize(Roles = "CACADMIN, CACDATAENTRY, DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY, SACOALITIONADMIN, DVCOALITIONADMIN, CACCOALITIONADMIN, DHSCOALITIONADMIN,CDFSSCOALITIONADMIN")]
		public ActionResult ScheduledReport(CompletedScheduledReportViewModel model) {
			RemoveSubReportSelectionError(ModelState);

			if (ModelState.IsValid) {
				var modifiedRecords = model.ReportRecordsDisplayed.Where(x => x.CenterActionModified).ToList();
				var deletedRecords = model.ReportRecordsDisplayed.Where(r => r.FlagForDelete).ToList();

				try {
					using (var transaction = db.Database.BeginTransaction())
						try {
							foreach (var modified in modifiedRecords) { ScheduledReportUpdate(modified); }
							foreach (var deleted in deletedRecords) { ScheduledReportDelete(deleted); }

							SaveCommitSuccessMessage(transaction);
							return RedirectToAction("ScheduledReport");
						} catch {
							transaction.Rollback();
							throw;
						}
				} catch (RetryLimitExceededException) {
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
					CompletedScheduledActions(model);
					return View(model);
				}
			}

			CompletedScheduledActions(model);
			return View(model);
		}

		private void ScheduledReportDelete(CompletedScheduledReportViewModel.ReportRecord deleteRecord) {
			var reportJobApprovalRecord = db.RPT_ReportJobApprovals.SingleOrDefault(r => r.ReportJobId == deleteRecord.Id);
			var reportJobRecord = db.RPT_ReportJobs.SingleOrDefault(r => r.Id == deleteRecord.Id);
           
            if (reportJobRecord.RowVersion.SequenceEqual(deleteRecord.RowVersion)) {
				if (reportJobApprovalRecord != null) {
                    var systemMessageRecord = db.T_SystemMessages.SingleOrDefault(r => r.Id == reportJobApprovalRecord.SystemMessageId);
                    db.Entry(reportJobApprovalRecord).State = EntityState.Deleted;                    
                    if (systemMessageRecord != null) 
                        db.Entry(systemMessageRecord).State = EntityState.Deleted;
                }
					
				reportJobRecord.ExpirationDate = DateTime.Now.AddTicks(-1);
				db.RPT_ReportJobs.Attach(reportJobRecord);
				db.Entry(reportJobRecord).Property(m => m.ExpirationDate).IsModified = true;
			}

		}

		[OverrideAuthorization]
		[Authorize(Roles = "CACADMIN, CACDATAENTRY, DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY, SACOALITIONADMIN, DVCOALITIONADMIN, CACCOALITIONADMIN, DHSCOALITIONADMIN,CDFSSCOALITIONADMIN")]
		public PartialViewResult ScheduledReportFilter(CompletedScheduledReportViewModel model) {
			var records = new List<CompletedScheduledReportViewModel.ReportRecord>();

			var results = ScheduledReportResults(model);
			for (int x = 0; x < results.Count; x = x + 1) {
				var recordJson = JsonConvert.DeserializeObject<StandardReportSpecification>(results[x].SpecificationJson);
				records.Add(new CompletedScheduledReportViewModel.ReportRecord {
					Id = (int)results[x].Id,
					CenterIds = recordJson.CenterIds,
					EndDate = recordJson.EndDate,
					StartDate = recordJson.StartDate,
					FundingSourceIds = recordJson.FundingSourceIds,
					RunDate = results[x].ScheduledForDate,
					SubReportSelection = recordJson.SubReportSelections,
					Title = recordJson.Title,
				});
			}

			if (model.SelectedBeginDate != null)
				records = records.Where(x => Convert.ToDateTime(x.StartDate) == Convert.ToDateTime(model.SelectedBeginDate)).ToList();

			if (model.SelectedEndDate != null)
				records = records.Where(x => Convert.ToDateTime(x.EndDate) == Convert.ToDateTime(model.SelectedEndDate)).ToList();

			if (model.SelectedCenterId != null)
				records = records.Where(x => x.CenterIds.Contains(model.SelectedCenterId)).ToList();

			if (model.SelectedRunDate != null)
				records = records.Where(x => Convert.ToDateTime(x.RunDate) == Convert.ToDateTime(model.SelectedRunDate)).ToList();

			if (model.SelectedTitle != null)
				records = records.Where(x => x.Title == model.SelectedTitle).ToList();

			if (model.SelectedType != null)
				records = records.Where(x => x.SubReportSelection.Cast<int>().ToArray().Contains(Convert.ToInt32(model.SelectedType))).ToList();

			if (model.SelectedFundingSource != null)
				records = records.Where(r => r.FundingSourceIds != null && r.FundingSourceIds.Contains(Convert.ToInt32(model.SelectedFundingSource))).ToList();

			var recordIds = records.Select(r => r.Id).ToList();
			var recordz = db.RPT_ReportJobs.Where(r => recordIds.Contains((int)r.Id)).ToList();

			ScheduledReportPopulateModel(model, recordz);
			return PartialView("_ScheduledReportTable", model);
		}

		private void ScheduledReportPopulateModel(CompletedScheduledReportViewModel model, List<ReportJob> records) {
			model.ReportTitle = "Scheduled";

			var centerQuery = db.T_Center.ToArray();
			var fundingSourceQuery = db.TLU_Codes_FundingSource.ToArray();

			for (int x = 0; x < records.Count; x = x + 1) {
				var recordJson = JsonConvert.DeserializeObject<StandardReportSpecification>(records[x].SpecificationJson);

				var center = new List<SelectListItem>();
				var fundingDescription = new List<SelectListItem>();
				var type = new List<SelectListItem>();
				var submitterCenter = new List<SelectListItem> {
					new SelectListItem {
						Value = records[x].SubmitterCenterId.Value.ToString(),
						Text = records[x].SubmitterCenter.CenterName
					}
				};

				if (recordJson.CenterIds != null)
					center = centerQuery.Where(t => recordJson.CenterIds.Contains(t.CenterID)).
							   OrderBy(m => m.CenterName).Select(m => new SelectListItem { Value = m.CenterID.ToString(), Text = m.CenterName }).ToList();

				if (recordJson.FundingSourceIds != null)
					fundingDescription = fundingSourceQuery.Where(t => recordJson.FundingSourceIds.Contains(t.CodeID)).
							   OrderBy(m => m.Description).Select(m => new SelectListItem { Value = m.CodeID.ToString(), Text = m.Description }).ToList();

				foreach (int item in recordJson.SubReportSelections) {
					type.Add(new SelectListItem {
						Value = item.ToString(),
						Text = ((SubReportSelection)item).GetDisplayName()
					});
				}

				int currentRecordId = (int)records[x].Id;
				int centerActionId = (int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction;
				var centerAction = new List<SelectListItem>();

				var reportStatus = db.RPT_ReportJobApprovals.Where(m => m.ReportJobId == currentRecordId).Select(s => s.StatusId).FirstOrDefault();
				if (reportStatus != null) {
					centerActionId = (int)reportStatus;
					centerAction.Add(new SelectListItem {
						Value = ((int)reportStatus).ToString(),
						Text = ((CompletedScheduledReportViewModel.ReportApprovalStatusDescription)centerActionId).GetDisplayName()
					});
				}

				model.ReportRecords.Add(new CompletedScheduledReportViewModel.ReportRecord {
					StatusId = records[x].StatusId,
					ApprovalStatusId = records[x].Approval?.StatusId ?? 9999,
					Id = currentRecordId,
					ReportTypeDescription = ((CompletedScheduledReportViewModel.ReportTypeDescription)(int)recordJson.ReportSelection).GetDisplayName(),
					CenterIds = recordJson.CenterIds,
					EndDate = recordJson.EndDate,
					StartDate = recordJson.StartDate,
					FundingSourceIds = recordJson.FundingSourceIds,
					ReportSelection = recordJson.ReportSelection,
					SubReportSelection = recordJson.SubReportSelections,
					Title = recordJson.Title,
					RunDate = records[x].ScheduledForDate,
					FundingSourceDescription = fundingDescription,
					SpecialCenterSelectionType = recordJson.SpecialCenterSelectionType,
					SpecialFundingSelectionType = recordJson.SpecialFundingSelectionType,
					Type = type,
					SelectedCenterActionId = centerActionId,
					FlagForDelete = false,
					Center = center,
					RowVersion = records[x].RowVersion,
					CenterAction = centerAction,
					SubmitterCenter = submitterCenter
				});
			}
			if (model.ReportRecords.Count > 0) {
				model.PagingMetaData = new StaticPagedList<CompletedScheduledReportViewModel.ReportRecord>(model.ReportRecords, model.PageNumber, model.PageSize, model.ReportRecords.Count).GetMetaData();
				model.ReportRecordsDisplayed = model.ReportRecords.GetRange(model.PagingMetaData.FirstItemOnPage - 1, model.PagingMetaData.IsLastPage ? model.PagingMetaData.LastItemOnPage - model.PagingMetaData.FirstItemOnPage + 1 : model.PagingMetaData.PageSize);
			}

			CompletedScheduledActions(model);
		}

		public List<ReportJob> ScheduledReportResults(CompletedScheduledReportViewModel model) {
			int sessionCenterId = Session.Center().Id;
			string sessionCenterIdPiped = $"|{sessionCenterId}|";

			List<ReportJob> results;
			var statusToSelect = new List<int?> { 2, 3, 4, 5, 7 };

			if (model.ViewRole == "funder") {
				results = db.RPT_ReportJobs.Where(m =>
					m.SubmitterCenterId == sessionCenterId &&
                    m.SubmitterUserName == User.Identity.Name &&    //to prevent ICJIA ICADVADMIN account seeing report from ICADV/ICASA
                    statusToSelect.Contains(m.StatusId) &&
					(m.ExpirationDate == null || m.ExpirationDate > DateTime.Now)
				).OrderByDescending(r => r.ScheduledForDate).ToList();
			} else {
				results = db.RPT_ReportJobs.Where(m => statusToSelect.Contains(m.StatusId) &&
					m.Approval != null &&
					m.Approval.CenterIdsString != null &&
					m.Approval.CenterIdsString.Contains(sessionCenterIdPiped) &&
					(m.ExpirationDate == null || m.ExpirationDate > DateTime.Now)
				 ).OrderByDescending(r => r.ScheduledForDate).ToList();
			}
			return results;
		}

		private void ScheduledReportUpdate(CompletedScheduledReportViewModel.ReportRecord modifiedRecord) {
			var reportJobApprovalRecord = db.RPT_ReportJobApprovals.SingleOrDefault(r => r.ReportJobId == modifiedRecord.Id);
			var reportJobRecord = db.RPT_ReportJobs.SingleOrDefault(r => r.Id == modifiedRecord.Id);
			var reportJobJson = JsonConvert.DeserializeObject<StandardReportSpecification>(reportJobRecord.SpecificationJson);
            var systemMessageRecord = db.T_SystemMessages.SingleOrDefault(r => r.Id == reportJobApprovalRecord.SystemMessageId);

            if (reportJobRecord.RowVersion.SequenceEqual(modifiedRecord.RowVersion)) {
				if (modifiedRecord.SelectedCenterActionId == (int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction) {
					if (reportJobApprovalRecord != null) { 
						db.Entry(reportJobApprovalRecord).State = EntityState.Deleted;
                        if (systemMessageRecord != null)
                            db.Entry(systemMessageRecord).State = EntityState.Deleted;
                    }
                }

				if (modifiedRecord.SelectedCenterActionId == (int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve || modifiedRecord.SelectedCenterActionId == (int)CompletedScheduledReportViewModel.CenterActionsEnum.Review) {
					if (reportJobApprovalRecord != null) {
						reportJobApprovalRecord.StatusId = modifiedRecord.SelectedCenterActionId;
						db.RPT_ReportJobApprovals.Attach(reportJobApprovalRecord);
						db.Entry(reportJobApprovalRecord).Property(m => m.StatusId).IsModified = true;
					} else {
						var reportJobApprovalNew = new ReportJobApproval();

						reportJobApprovalNew.CenterIds = reportJobJson.CenterIds.Cast<int>();
						reportJobApprovalNew.StatusDate = DateTime.Now;
						reportJobApprovalNew.StatusId = modifiedRecord.SelectedCenterActionId;

						if (modifiedRecord.SelectedCenterActionId == (int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve) {
							reportJobApprovalNew = ReportJobApprovalSystemMessage(reportJobApprovalNew, reportJobRecord);
						}

						if (reportJobRecord.Approval == null) {
							reportJobRecord.Approval = new ReportJobApproval();
						}

						reportJobRecord.Approval.CenterIds = reportJobApprovalNew.CenterIds;
						reportJobRecord.Approval.StatusDate = reportJobApprovalNew.StatusDate;
						reportJobRecord.Approval.StatusId = reportJobApprovalNew.StatusId;
						reportJobRecord.Approval.SystemMessage = reportJobApprovalNew.SystemMessage;
					}
				}
			}
		}
		#endregion

		#region Completed Reports
		[OverrideAuthorization]
		[Authorize(Roles = "CACADMIN, CACDATAENTRY, DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY, SACOALITIONADMIN, DVCOALITIONADMIN, CACCOALITIONADMIN, DHSCOALITIONADMIN,CDFSSCOALITIONADMIN")]
		public ActionResult CompletedReport(int? rptId) {
			var model = new CompletedScheduledReportViewModel { ReportTitle = "Completed" };
			ViewRole(model);

			if (rptId != null && db.RPT_ReportJobApprovals.FirstOrDefault(m => m.ReportJobId == rptId) != null)
				model.RptJobId = rptId;

			var records = CompletedReportResults(model);
			CompletedReportPopulateModel(model, records);
			return View("ScheduledReport", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[OverrideAuthorization]
		[Authorize(Roles = "CACADMIN, CACDATAENTRY, DVADMIN, DVDATAENTRY, SAADMIN, SACOALITIONADMIN, SADATAENTRY, DVCOALITIONADMIN, CACCOALITIONADMIN, DHSCOALITIONADMIN,CDFSSCOALITIONADMIN")]
		public ActionResult CompletedReport(CompletedScheduledReportViewModel model) {
			RemoveSubReportSelectionError(ModelState);

			if (ModelState.IsValid) {
				var deletedRecords = model.ReportRecordsDisplayed.Where(r => r.FlagForDelete).ToList();

				try {
					using (var transaction = db.Database.BeginTransaction())
						try {
							foreach (var deleted in deletedRecords) { ScheduledReportDelete(deleted); }

							SaveCommitSuccessMessage(transaction);
							return RedirectToAction("CompletedReport");
						} catch {
							transaction.Rollback();
							throw;
						}
				} catch (RetryLimitExceededException) {
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
					CompletedScheduledActions(model);
					return View("ScheduledReport", model);
				}
			}

			CompletedScheduledActions(model);
			return View("ScheduledReport", model);
		}

		public ActionResult CompletedReportApproveReject(int approvalId, int approvalStatusId, string approvalApproverComment) {
			try {
				using (var transaction = db.Database.BeginTransaction())
					try {
						var reportJobApprovalRecord = db.RPT_ReportJobApprovals.Where(r => r.ReportJobId == approvalId).SingleOrDefault();
                        var reportJobRecord = db.RPT_ReportJobs.SingleOrDefault(r => r.Id == approvalId);
                        var reportJobJson = JsonConvert.DeserializeObject<StandardReportSpecification>(reportJobRecord.SpecificationJson);
                        string reportApproveRejected = Enum.GetName(typeof(CompletedScheduledReportViewModel.ReportApprovalStatusDescription), approvalStatusId); 

                        //if (reportJobRecord.RowVersion.SequenceEqual(modifiedRecord.RowVersion)) {
                        reportJobApprovalRecord.StatusId = approvalStatusId;
						reportJobApprovalRecord.ApproverComment = approvalApproverComment;
                        reportJobApprovalRecord.StatusDate = DateTime.Now;

                        //Update system message  to [Report Name] generated by [Admin User] on [Run Date] was [approved or rejected] on [Approve/Reject Date].                 
                        if (reportJobApprovalRecord.SystemMessage != null) {
                            reportJobApprovalRecord.SystemMessage.ExpirationDate = DateTime.Now.AddDays(7);
                            reportJobApprovalRecord.SystemMessage.Title = $"Report {reportApproveRejected}";
                            reportJobApprovalRecord.SystemMessage.Message = $"<strong>{reportJobJson.Title}</strong> generated by <i>{reportJobRecord.SubmitterCenter.CenterName}</i> on {reportJobRecord.ScheduledForDate.Value.ToShortDateString()} was <strong>{reportApproveRejected}</strong> on {DateTime.Now:M/d/yyyy}";
                            reportJobApprovalRecord.SystemMessage.LinkText = "View report";
                        }
                        
                        db.RPT_ReportJobApprovals.Attach(reportJobApprovalRecord);
						db.Entry(reportJobApprovalRecord).Property(m => m.StatusId).IsModified = true;
						db.Entry(reportJobApprovalRecord).Property(m => m.ApproverComment).IsModified = true;
                        db.Entry(reportJobApprovalRecord).Property(m => m.StatusDate).IsModified = true;
                     

                        SaveCommitSuccessMessage(transaction);
						return RedirectToAction("CompletedReport");
					} catch {
						transaction.Rollback();
						throw;
					}
			} catch (RetryLimitExceededException) {
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return RedirectToAction("CompletedReport");
			}
		}

		[OverrideAuthorization]
		[Authorize(Roles = "CACADMIN, CACDATAENTRY, DVADMIN, SAADMIN, SACOALITIONADMIN, SADATAENTRY, DVCOALITIONADMIN, CACCOALITIONADMIN, DHSCOALITIONADMIN,CDFSSCOALITIONADMIN")]
		public PartialViewResult CompletedReportFilter(CompletedScheduledReportViewModel model) {
			RemoveSubReportSelectionError(ModelState);

			var records = new List<CompletedScheduledReportViewModel.ReportRecord>();
			var results = CompletedReportResults(model);

			for (int x = 0; x < results.Count; x = x + 1) {
				var recordJson = JsonConvert.DeserializeObject<StandardReportSpecification>(results[x].SpecificationJson);
				records.Add(new CompletedScheduledReportViewModel.ReportRecord {
					Id = (int)results[x].Id,
					CenterIds = recordJson.CenterIds,
					EndDate = recordJson.EndDate,
					StartDate = recordJson.StartDate,
					FundingSourceIds = recordJson.FundingSourceIds,
					RunDate = results[x].ScheduledForDate,
					SubReportSelection = recordJson.SubReportSelections,
					Title = recordJson.Title,
					SubmittedDate = results[x].SubmittedDate,
					CenterActionApprovalDate = results[x].CenterActionApprovalDate,
					CenterApprovalDescription = new List<SelectListItem>()
				});
				int centerActionId = (int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction;
				int reportId = records[x].Id;

				var reportJob = db.RPT_ReportJobs.FirstOrDefault(m => m.Id == reportId);

				records[x].SubmitterCenter = new List<SelectListItem> {
					new SelectListItem {
						Value = reportJob.SubmitterCenter.CenterID.ToString(),
						Text = reportJob.SubmitterCenter.CenterName
					}
				};

				if (results[x].Approval != null) {
					centerActionId = (int)results[x].Approval.StatusId;

					switch (centerActionId) {
						case (int)ReportJobApproval.Status.ReviewOnly:
							records[x].CenterApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.ReviewOnly).ToString(),
								Text = CompletedScheduledReportViewModel.ReportApprovalStatusDescription.ReviewOnly.GetDisplayName()
							});
							break;
						case (int)ReportJobApproval.Status.Pending:
							records[x].CenterApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Pending).ToString(),
								Text = CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Pending.GetDisplayName()
							});
							break;
						case (int)ReportJobApproval.Status.Approved:
							records[x].CenterApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Approved).ToString(),
								Text = CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Approved.GetDisplayName()
							});
							break;
						case (int)ReportJobApproval.Status.Rejected:
							records[x].CenterApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Rejected).ToString(),
								Text = CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Rejected.GetDisplayName()
							});
							break;
					}
				} else {
					switch (centerActionId) {
						case (int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction:
							records[x].CenterApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction).ToString(),
								Text = CompletedScheduledReportViewModel.CenterActionsEnum.NoAction.GetDisplayName()
							});
							break;
						case (int)CompletedScheduledReportViewModel.CenterActionsEnum.Review:
							records[x].CenterApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.CenterActionsEnum.Review).ToString(),
								Text = CompletedScheduledReportViewModel.CenterActionsEnum.Review.GetDisplayName()
							});
							break;
						case (int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve:
							records[x].CenterApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve).ToString(),
								Text = CompletedScheduledReportViewModel.CenterActionsEnum.Approve.GetDisplayName()
							});
							break;
					}
				}
			}

			if (model.SelectedBeginDate != null)
				records = records.Where(x => Convert.ToDateTime(x.StartDate) == Convert.ToDateTime(model.SelectedBeginDate)).ToList();

			if (model.SelectedEndDate != null)
				records = records.Where(x => Convert.ToDateTime(x.EndDate) == Convert.ToDateTime(model.SelectedEndDate)).ToList();

			if (model.SelectedSubmittedDate != null)
				records = records.Where(x => x.SubmittedDate.Value.ToShortDateString() == model.SelectedSubmittedDate).ToList();

			if (model.SelectedCenterId != null)
				records = records.Where(x => x.CenterIds.Contains(model.SelectedCenterId)).ToList();

			if (model.SelectedRunDate != null)
				records = records.Where(x => Convert.ToDateTime(x.RunDate) == Convert.ToDateTime(model.SelectedRunDate)).ToList();

			if (model.SelectedTitle != null)
				records = records.Where(x => x.Title == model.SelectedTitle).ToList();

			if (model.SelectedType != null)
				records = records.Where(x => x.SubReportSelection.Cast<int>().ToArray().Contains(Convert.ToInt32(model.SelectedType))).ToList();

			if (model.SelectedFundingSource != null)
				records = records.Where(r => r.FundingSourceIds != null && r.FundingSourceIds.Contains(Convert.ToInt32(model.SelectedFundingSource))).ToList();

			if (model.SelectedCenterApprovalRejectionDate != null)
				records = (from r in records
						   where r.CenterActionApprovalDate != null
						   && r.CenterActionApprovalDate.Value.ToShortDateString() == model.SelectedCenterApprovalRejectionDate
						   && (r.CenterApprovalDescription.Select(i => Convert.ToInt32(i.Value)).SingleOrDefault() == (int)ReportJobApproval.Status.Rejected
						   || r.CenterApprovalDescription.Select(i => Convert.ToInt32(i.Value)).SingleOrDefault() == (int)ReportJobApproval.Status.Approved)
						   select r).ToList();

			if (model.SelectedCenterApproval != null) {
				records = records.Where(x => x.CenterApprovalDescription.Select(p => p.Value).Contains(model.SelectedCenterApproval)).ToList();
			}

			if (model.SelectedSubmitterCenterId != null) {
				records = records.Where(x => x.SubmitterCenter.Select(p => p.Value).Contains(model.SelectedSubmitterCenterId.ToString())).ToList();
			}

			var recordIds = records.Select(r => r.Id).ToList();
			var recordz = CompletedReportResults(model);

			recordz = recordz.Where(r => recordIds.Contains((int)r.Id)).ToList();

			CompletedReportPopulateModel(model, recordz);

			return PartialView("_ScheduledReportTable", model);
		}

		private void CompletedReportPopulateModel(CompletedScheduledReportViewModel model, List<CompletedScheduledReportViewModel.CompletedRecord> records) {
			model.ReportTitle = "Completed";
			ViewRole(model);

			var center = new List<SelectListItem>();

			for (int x = 0; x < records.Count; x = x + 1) {
				var recordJson = JsonConvert.DeserializeObject<StandardReportSpecification>(records[x].SpecificationJson);

				var fundingDescription = new List<SelectListItem>();
				var type = new List<SelectListItem>();

				if (recordJson.CenterIds != null) {
					center = db.T_Center.Where(t => recordJson.CenterIds.Contains(t.CenterID)).
							   OrderBy(m => m.CenterName).Select(m => new SelectListItem { Value = m.CenterID.ToString(), Text = m.CenterName }).ToList();
				}

				if (recordJson.FundingSourceIds != null) {
					fundingDescription = db.TLU_Codes_FundingSource.Where(t => recordJson.FundingSourceIds.Contains(t.CodeID)).
							   OrderBy(m => m.Description).Select(m => new SelectListItem { Value = m.CodeID.ToString(), Text = m.Description }).ToList();
				}

				foreach (int item in recordJson.SubReportSelections) {
					type.Add(new SelectListItem {
						Value = item.ToString(),
						Text = ((SubReportSelection)item).GetDisplayName()
					});
				}

				int currentRecordId = (int)records[x].Id;
				int centerActionId = (int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction;
				var centerApprovalDescription = new List<SelectListItem>();
				string centerComment = "";
				var centerActionApprovalDate = records[x].CenterActionApprovalDate;
				var submitterCenter = new List<SelectListItem>();

				var reportJob = db.RPT_ReportJobs.FirstOrDefault(m => m.Id == currentRecordId);

				submitterCenter.Add(new SelectListItem {
					Value = reportJob.SubmitterCenter.CenterID.ToString(),
					Text = reportJob.SubmitterCenter.CenterName
				});

				var reportApproval = db.RPT_ReportJobApprovals.SingleOrDefault(m => m.ReportJobId == currentRecordId);

				if (reportApproval != null) {
					centerActionId = (int)reportApproval.StatusId;
				}

				if (reportApproval != null) {
					centerComment = reportApproval.ApproverComment;
					centerActionApprovalDate = reportApproval.StatusDate;
					switch (centerActionId) {
						case (int)ReportJobApproval.Status.ReviewOnly:
							centerApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.ReviewOnly).ToString(),
								Text = ((CompletedScheduledReportViewModel.ReportApprovalStatusDescription)(int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.ReviewOnly).GetDisplayName()
							});
							break;
						case (int)ReportJobApproval.Status.Pending:
							centerApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Pending).ToString(),
								Text = ((CompletedScheduledReportViewModel.ReportApprovalStatusDescription)(int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Pending).GetDisplayName()
							});
							break;
						case (int)ReportJobApproval.Status.Approved:
							centerApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Approved).ToString(),
								Text = ((CompletedScheduledReportViewModel.ReportApprovalStatusDescription)(int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Approved).GetDisplayName()
							});
							break;
						case (int)ReportJobApproval.Status.Rejected:
							centerApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Rejected).ToString(),
								Text = ((CompletedScheduledReportViewModel.ReportApprovalStatusDescription)(int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Rejected).GetDisplayName()
							});
							break;
					}
				} else {
					switch (centerActionId) {
						case (int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction:
							centerApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction).ToString(),
								Text = ((CompletedScheduledReportViewModel.CenterActionsEnum)(int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction).GetDisplayName()
							});
							break;
						case (int)CompletedScheduledReportViewModel.CenterActionsEnum.Review:
							centerApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.CenterActionsEnum.Review).ToString(),
								Text = ((CompletedScheduledReportViewModel.CenterActionsEnum)(int)CompletedScheduledReportViewModel.CenterActionsEnum.Review).GetDisplayName()
							});
							break;
						case (int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve:
							centerApprovalDescription.Add(new SelectListItem {
								Value = ((int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve).ToString(),
								Text = ((CompletedScheduledReportViewModel.CenterActionsEnum)(int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve).GetDisplayName()
							});
							break;
					}
				}

				model.ReportRecords.Add(new CompletedScheduledReportViewModel.ReportRecord {
					Id = currentRecordId,
					ReportTypeDescription = ((CompletedScheduledReportViewModel.ReportTypeDescription)(int)recordJson.ReportSelection).GetDisplayName(),
					CenterIds = recordJson.CenterIds,
					EndDate = recordJson.EndDate,
					StartDate = recordJson.StartDate,
					FundingSourceIds = recordJson.FundingSourceIds,
					ReportSelection = recordJson.ReportSelection,
					SubReportSelection = recordJson.SubReportSelections,
					Title = recordJson.Title,
					RunDate = records[x].ScheduledForDate,
					FundingSourceDescription = fundingDescription,
					SpecialCenterSelectionType = recordJson.SpecialCenterSelectionType,
					SpecialFundingSelectionType = recordJson.SpecialFundingSelectionType,
					Type = type,
					SelectedCenterActionId = records[x].CenterActionId,
					FlagForDelete = false,
					Center = center,
					SubmittedDate = records[x].SubmittedDate,
					CenterApprovalDescription = centerApprovalDescription,
					CenterActionApprovalDate = centerActionApprovalDate,
					CenterComment = centerComment,
					ApprovalStatusId = reportApproval?.StatusId ?? 9999,
					StatusId = records[x].StatusId,
					RowVersion = records[x].RowVersion,
					CenterAction = center,
					SubmitterCenter = submitterCenter
				});
			}

			if (model.ReportRecords.Count > 0) {
				model.PagingMetaData = new StaticPagedList<CompletedScheduledReportViewModel.ReportRecord>(model.ReportRecords, model.PageNumber, model.PageSize, model.ReportRecords.Count).GetMetaData();
				model.ReportRecordsDisplayed = model.ReportRecords.GetRange(model.PagingMetaData.FirstItemOnPage - 1, model.PagingMetaData.IsLastPage ? model.PagingMetaData.LastItemOnPage - model.PagingMetaData.FirstItemOnPage + 1 : model.PagingMetaData.PageSize);
			}
			CompletedScheduledReportFilters(model);
		}

		public List<CompletedScheduledReportViewModel.CompletedRecord> CompletedReportResults(CompletedScheduledReportViewModel model) {
			int sessionCenterId = Session.Center().Id;
			string sessionCenterIdPiped = $"|{sessionCenterId}|";
			List<CompletedScheduledReportViewModel.CompletedRecord> results;

			if (model.ViewRole == "funder") {
				results = (from rj in db.RPT_ReportJobs
						   where rj.StatusId == 6 &&
								 rj.SubmitterCenterId == sessionCenterId &&
                                 rj.SubmitterUserName == User.Identity.Name &&   //to prevent ICJIA ICADVADMIN account seeing report from ICADV/ICASA
                                 (rj.ExpirationDate == null || rj.ExpirationDate > DateTime.Now)
						   select new CompletedScheduledReportViewModel.CompletedRecord {
							   SpecificationJson = rj.SpecificationJson,
							   Approval = rj.Approval,
							   Id = rj.Id,
							   ScheduledForDate = rj.ScheduledForDate,
							   CenterActionId = (int)rj.StatusId,
							   SubmittedDate = rj.SubmittedDate,
							   RowVersion = rj.RowVersion,
							   CenterActionApprovalDate = rj.StatusDate,
							   StatusId = rj.StatusId
						   }).OrderByDescending(r => r.ScheduledForDate).ToList();
			} else {
				results = (from rj in db.RPT_ReportJobs
						   where rj.StatusId == 6 &&
								 rj.Approval != null &&
								 rj.Approval.CenterIdsString != null &&
								 rj.Approval.CenterIdsString.Contains(sessionCenterIdPiped) &&
								 (rj.ExpirationDate == null || rj.ExpirationDate > DateTime.Now)
						   select new CompletedScheduledReportViewModel.CompletedRecord {
							   SpecificationJson = rj.SpecificationJson,
							   Approval = rj.Approval,
							   Id = rj.Id,
							   ScheduledForDate = rj.ScheduledForDate,
							   CenterActionId = (int)rj.StatusId,
							   SubmittedDate = rj.SubmittedDate,
							   RowVersion = rj.RowVersion,
							   CenterActionApprovalDate = rj.StatusDate,
							   StatusId = rj.StatusId
						   }).OrderByDescending(r => r.ScheduledForDate).ToList();
			}
			return results;
		}

		//http://localhost:59774/Report/ViewCompleted/27
		//http://localhost:59774/Report/ViewCompleted/27?format=html
		//http://localhost:59774/Report/ViewCompleted/27?format=csv
		//http://localhost:59774/Report/ViewCompleted/27?format=pdf
		[OverrideAuthorization]
		[Authorize(Roles = "CACADMIN, CACDATAENTRY, DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY, SACOALITIONADMIN, DVCOALITIONADMIN, CACCOALITIONADMIN, DHSCOALITIONADMIN,CDFSSCOALITIONADMIN")]
		public ActionResult ViewCompleted(int id, ReportOutputType format = ReportOutputType.Html) {
			int sessionCenterId = Session.Center().Id;
			string sessionCenterIdPiped = $"|{sessionCenterId}|";
			var job = db.RPT_ReportJobs.SingleOrDefault(j => j.Id == id && j.StatusId == (int)ReportJob.Status.Succeeded && (j.SubmitterCenterId == sessionCenterId || j.Approval != null && j.Approval.CenterIdsString != null && j.Approval.CenterIdsString.Contains(sessionCenterIdPiped)));
			if (job == null)
				return HttpNotFound();

			if (format == ReportOutputType.Csv && (job.Approval == null || !job.Approval.CenterIds.Contains(sessionCenterId)))
				return HttpNotFound();

			string filePath = Path.Combine(_ReportingService_OutputDirectory, job.Id.ToString(), job.Id + (format == ReportOutputType.Csv ? CSV : HTML));
			if (!System.IO.File.Exists(filePath))
				return HttpNotFound();

			var specification = JsonConvert.DeserializeObject<StandardReportSpecification>(job.SpecificationJson, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

			switch (format) {
				case ReportOutputType.Html:
					ViewBag.Title = specification.Title;
					ViewBag.FilePath = filePath;
					return View("ReportFile");
				case ReportOutputType.Csv:
					return File(filePath, CSV_CONTENT_TYPE, specification.Title.Replace(" ", "") + CSV); //KMS DO do a better job stripping chars (elsewhere also)
				case ReportOutputType.Pdf:
					ViewBag.Title = specification.Title;
					ViewBag.FilePath = filePath;
					ViewBag.IsPDF = true;
					return new ViewAsPdf("ReportFile");
				default:
					throw new NotSupportedException($"{nameof(ReportOutputType)} {format} is not supported");
			}
		}
		#endregion

		#region CompletedScheduled
		private void CompletedScheduledActions(CompletedScheduledReportViewModel model) {
			CompletedScheduledReportFilters(model);
			CompletedScheduledCenterActions(model);
			ViewRole(model);
		}

		private CompletedScheduledReportViewModel CompletedScheduledCenterActions(CompletedScheduledReportViewModel model) {
			model.CenterActions = new List<SelectListItem>();
			model.CenterActions.Add(new SelectListItem {
				Value = ((int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction).ToString(),
				Text = ((CompletedScheduledReportViewModel.CenterActionsEnum)(int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction).GetDisplayName()
			});
			model.CenterActions.Add(new SelectListItem {
				Value = ((int)CompletedScheduledReportViewModel.CenterActionsEnum.Review).ToString(),
				Text = ((CompletedScheduledReportViewModel.CenterActionsEnum)(int)CompletedScheduledReportViewModel.CenterActionsEnum.Review).GetDisplayName()
			});
			model.CenterActions.Add(new SelectListItem {
				Value = ((int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve).ToString(),
				Text = ((CompletedScheduledReportViewModel.CenterActionsEnum)(int)CompletedScheduledReportViewModel.CenterActionsEnum.Approve).GetDisplayName()
			});

			model.CenterActionNoAction = new List<SelectListItem>();
			model.CenterActionNoAction.Add(new SelectListItem {
				Value = ((int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction).ToString(),
				Text = ((CompletedScheduledReportViewModel.CenterActionsEnum)(int)CompletedScheduledReportViewModel.CenterActionsEnum.NoAction).GetDisplayName()
			});
			return model;
		}

		private CompletedScheduledReportViewModel CompletedScheduledReportFilters(CompletedScheduledReportViewModel model) {
			if (model.ReportTitle == "Scheduled" || model.ReportTitle == "Completed") {
				model.FilterRunDate = model.ReportRecords.GroupBy(m => m.RunDate).Select(g => g.First())
				.Select(m => new SelectListItem { Value = m.RunDate.Value.ToShortDateString(), Text = m.RunDate.Value.ToShortDateString() }).OrderByDescending(o => Convert.ToDateTime(o.Text));

				model.FilterTitle = model.ReportRecords.GroupBy(m => m.Title).Select(g => g.First())
					.Select(m => new SelectListItem { Value = m.Title, Text = m.Title }).OrderBy(o => o.Text);

				model.FilterType = model.ReportRecords.SelectMany(s => s.Type).GroupBy(g => g.Text).Select(s => s.First())
					.Select(m => new SelectListItem { Value = m.Value, Text = m.Text }).OrderBy(o => o.Text);

				model.FilterBeginDate = model.ReportRecords.GroupBy(m => m.StartDate).Select(g => g.First())
					.Select(m => new SelectListItem { Value = m.StartDate.Value.ToShortDateString(), Text = m.StartDate.Value.ToShortDateString() }).OrderByDescending(o => Convert.ToDateTime(o.Text));

				model.FilterEndDate = model.ReportRecords.GroupBy(m => m.EndDate).Select(g => g.First())
					.Select(m => new SelectListItem { Value = m.EndDate.Value.ToShortDateString(), Text = m.EndDate.Value.ToShortDateString() }).OrderByDescending(o => Convert.ToDateTime(o.Text));

				model.FilterCenterName = model.ReportRecords.SelectMany(s => s.Center).GroupBy(g => g.Value).Select(s => s.First())
					.Select(m => new SelectListItem { Value = m.Value, Text = m.Text }).OrderBy(o => o.Text);

				model.FilterFundingSource = model.ReportRecords.Where(x => x.FundingSourceDescription != null).SelectMany(s => s.FundingSourceDescription).GroupBy(g => g.Value).Select(s => s.First())
					.Select(m => new SelectListItem { Value = m.Value, Text = m.Text }).OrderBy(o => o.Text);
			}

			model.FilterSubmitterCenter = model.ReportRecords.Where(x => x.SubmitterCenter != null).SelectMany(s => s.SubmitterCenter).GroupBy(g => g.Value).Select(s => s.First())
				.Select(m => new SelectListItem { Value = m.Value, Text = m.Text }).OrderBy(o => o.Text);

			if (model.ReportTitle == "Scheduled" && model.ViewRole != "funder") {
				model.FilterCenterApproval = model.ReportRecords.Where(x => x.CenterAction != null).SelectMany(s => s.CenterAction).GroupBy(g => g.Value).Select(s => s.First())
					.Select(m => new SelectListItem { Value = m.Value, Text = m.Text }).OrderBy(o => o.Text);
			}

			if (model.ReportTitle == "Completed") {
				model.FilterCenterApproval = model.ReportRecords.Where(x => x.CenterApprovalDescription != null).SelectMany(s => s.CenterApprovalDescription).GroupBy(g => g.Value).Select(s => s.First())
					.Select(m => new SelectListItem { Value = m.Value, Text = m.Text }).OrderBy(o => o.Text);

				model.FilterCenterApprovalRejectionDate = model.ReportRecords.Where(m => m.ApprovalStatusId == (int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Approved || m.ApprovalStatusId == (int)CompletedScheduledReportViewModel.ReportApprovalStatusDescription.Rejected)
					.GroupBy(m => m.CenterActionApprovalDate.Value.ToShortDateString()).Select(g => g.First())
					.Select(m => new SelectListItem { Value = m.CenterActionApprovalDate.Value.ToShortDateString(), Text = m.CenterActionApprovalDate.Value.ToShortDateString() }).OrderByDescending(o => Convert.ToDateTime(o.Text));

				model.FilterSubmittedDate = model.ReportRecords.GroupBy(m => m.SubmittedDate.Value.ToShortDateString()).Select(g => g.First())
					.Select(m => new SelectListItem { Value = m.SubmittedDate.Value.ToShortDateString(), Text = m.SubmittedDate.Value.ToShortDateString() }).OrderByDescending(o => Convert.ToDateTime(o.Text));
			}

			return model;
		}
		#endregion

		#region AJAX

		public ActionResult StaffNames(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null) {
				return Json(new List<Staff>(), JsonRequestBehavior.AllowGet);
			}
			return Json(_filterListGenerator.GetStaffNames(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
		}

		public ActionResult ServiceNames(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetServices(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
		}
        public ActionResult ProgramNames(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null)
        {
            return Json(_filterListGenerator.GetPrograms(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
        }

        public ActionResult OffenderRelationships(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetLookupValues(Lookups.RelationshipToClient), JsonRequestBehavior.AllowGet);
		}

		public ActionResult FundingSources(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetFundingSources(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Races(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (Session.Center().Provider == Provider.CAC) {
				return Json(_filterListGenerator.GetLookupValues(Lookups.Race), JsonRequestBehavior.AllowGet);
			}
			return Json(_filterListGenerator.GetLookupValues(Lookups.RaceHud), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Genders(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetLookupValues(Lookups.GenderIdentity), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Ethnicities(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetLookupValues(Lookups.Ethnicity), JsonRequestBehavior.AllowGet);
		}

		public ActionResult ShelterTypes(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetShelterType(), JsonRequestBehavior.AllowGet);
		}

		public ActionResult ClientTypes(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetLookupValues(Lookups.ClientType), JsonRequestBehavior.AllowGet);
		}

		public ActionResult HotlineCallTypes() {
			return Json(_filterListGenerator.GetLookupValues(Lookups.HotlineCallType), JsonRequestBehavior.AllowGet);
		}

		public ActionResult ReferralMade() {
			return Json(_filterListGenerator.GetLookupValues(Lookups.YesNo2), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Activities(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetOtherStaffActivities(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
		}

		public ActionResult CancellationReasons() {
			return Json(_filterListGenerator.GetLookupValues(Lookups.CancellationReason), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Agencies(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetAgencies(centerIds), JsonRequestBehavior.AllowGet);
		}

		public ActionResult ServiceLocations(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetServiceLocations(centerIds), JsonRequestBehavior.AllowGet);
		}

		public ActionResult EventLocations(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetEventLocations(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Publications() {
			return Json(_filterListGenerator.GetPublications(), JsonRequestBehavior.AllowGet);
		}

		public ActionResult CityOrTowns(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetCityOrTowns(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Townships(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetTownships(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Counties(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetCounties(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
		}

		public ActionResult States(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetStates(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Zipcodes(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			return Json(_filterListGenerator.GetZipcodes(centerIds, startDate, endDate), JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Report UI Setup

		private Dictionary<ReportSelection, List<SubReportSelection>> GetStandardReportSelectionsForProvider() {
			var selections = new Dictionary<ReportSelection, List<SubReportSelection>>();

			// Client Information
			var clientInfoSelections = new List<SubReportSelection>();
			clientInfoSelections.Add(SubReportSelection.StdRptClientInformationBasicDemographics);
			clientInfoSelections.Add(SubReportSelection.StdRptClientInformationReferralSource);
			clientInfoSelections.Add(SubReportSelection.StdRptClientInformationSpecialNeeds);
			clientInfoSelections.Add(SubReportSelection.StdRptClientInformationPresentingIssues);
			if (Session.Center().Provider == Provider.DV) {
				clientInfoSelections.Add(SubReportSelection.StdRptClientInformationAggregateClientInformation);
				clientInfoSelections.Add(SubReportSelection.StdRptClientInformationResidenceDestinationInformation);
			}
			selections.Add(ReportSelection.StdRptClientInformation, clientInfoSelections);

			// Service/Programs
			var serviceSelections = new List<SubReportSelection>();
			serviceSelections.Add(SubReportSelection.StdRptServiceProgramsDirectClientServices);
			serviceSelections.Add(SubReportSelection.StdRptServiceProgramsCommunityInstitutionalGroupServices);

			if (Session.Center().Provider == Provider.SA) {
				serviceSelections.Add(SubReportSelection.StdRptServiceProgramsNonClientCrisisIntervention);
				serviceSelections.Add(SubReportSelection.StdRptServiceProgramsNonClientCrisisInterventionDemographics);
			}

			serviceSelections.Add(SubReportSelection.StdRptServiceProgramsVolunteerServiceInformation);

			if (Session.Center().Provider == Provider.DV) {
				serviceSelections.Add(SubReportSelection.StdRptServiceProgramsHotlineInformationReferral);
                //Turn-away report is in both regular service report and HUD report
                serviceSelections.Add(SubReportSelection.StdRptServiceProgramsHudHmisTurnAway);
                serviceSelections.Add(SubReportSelection.StdRptServiceProgramsServiceOutcomesServiceReport);                
                serviceSelections.Add(SubReportSelection.StdRptServiceProgramsHudHmisServiceReport);
			}
			selections.Add(ReportSelection.StdRptServicePrograms, serviceSelections);

			// MedicalCJ
			var medicalSelections = new List<SubReportSelection>();
			medicalSelections.Add(SubReportSelection.StdRptMedicalCJOffenders);
			if (Session.Center().Provider != Provider.CAC) {
				medicalSelections.Add(SubReportSelection.StdRptMedicalCJMedicalSystemInvolvement);
			}
			medicalSelections.Add(SubReportSelection.StdRptMedicalCJPoliceInvolvement);
			medicalSelections.Add(SubReportSelection.StdRptMedicalCJProsecutionInvolvement);
			if (Session.Center().Provider == Provider.DV) {
				medicalSelections.Add(SubReportSelection.StdRptMedicalCJOrderOfProtection);
			}
			selections.Add(ReportSelection.StdRptMedicalCjProcess, medicalSelections);

			// Investigation
			if (Session.Center().Provider == Provider.CAC)
				selections.Add(ReportSelection.StdRptInvestigationInformation, new List<SubReportSelection> {
					SubReportSelection.StdRptInvestigationDCFSAllegations,
					SubReportSelection.StdRptInvestigationAbuseNeglectPetitions,
					SubReportSelection.StdRptInvestigationMultiDisciplinaryTeam,
					SubReportSelection.StdRptInvestigationVictimSensitiveInterview,
					SubReportSelection.StdRptInvestigationMedical
				});

			return selections;
		}

		private Dictionary<ReportSelection, List<SubReportSelection>> GetSubmitReportSelectionsForProvider() {
			var selections = new Dictionary<ReportSelection, List<SubReportSelection>>();

			// Client Information
			var clientInfoSelections = new List<SubReportSelection>();
			clientInfoSelections.Add(SubReportSelection.StdRptClientInformationBasicDemographics);
			clientInfoSelections.Add(SubReportSelection.StdRptClientInformationReferralSource);
			clientInfoSelections.Add(SubReportSelection.StdRptClientInformationSpecialNeeds);
			clientInfoSelections.Add(SubReportSelection.StdRptClientInformationPresentingIssues);
			if (Session.Center().Provider == Provider.DV) {
				clientInfoSelections.Add(SubReportSelection.StdRptClientInformationAggregateClientInformation);
				clientInfoSelections.Add(SubReportSelection.StdRptClientInformationResidenceDestinationInformation);
			}
			selections.Add(ReportSelection.StdRptClientInformation, clientInfoSelections);

			// Service/Programs
			var serviceSelections = new List<SubReportSelection>();
			serviceSelections.Add(SubReportSelection.StdRptServiceProgramsDirectClientServices);
			serviceSelections.Add(SubReportSelection.StdRptServiceProgramsCommunityInstitutionalGroupServices);
			if (Session.Center().Provider == Provider.DV)
				serviceSelections.Add(SubReportSelection.StdRptServiceProgramsHotlineInformationReferral);
			if (Session.Center().Provider == Provider.SA)
				serviceSelections.Add(SubReportSelection.StdRptServiceProgramsNonClientCrisisIntervention);
			serviceSelections.Add(SubReportSelection.StdRptServiceProgramsVolunteerServiceInformation);
			if (Session.Center().Provider == Provider.DV) {
                //Turn-away report is in both regular service report and HUD report
                serviceSelections.Add(SubReportSelection.StdRptServiceProgramsHudHmisTurnAway);				
				serviceSelections.Add(SubReportSelection.StdRptServiceProgramsServiceOutcomesServiceReport);
                serviceSelections.Add(SubReportSelection.StdRptServiceProgramsHudHmisServiceReport);
			}
			selections.Add(ReportSelection.StdRptServicePrograms, serviceSelections);

			// MedicalCJ
			var medicalSelections = new List<SubReportSelection>();
			medicalSelections.Add(SubReportSelection.StdRptMedicalCJOffenders);
			if (Session.Center().Provider != Provider.CAC) {
				medicalSelections.Add(SubReportSelection.StdRptMedicalCJMedicalSystemInvolvement);
			}
			medicalSelections.Add(SubReportSelection.StdRptMedicalCJPoliceInvolvement);
			medicalSelections.Add(SubReportSelection.StdRptMedicalCJProsecutionInvolvement);
			if (Session.Center().Provider == Provider.DV) {
				medicalSelections.Add(SubReportSelection.StdRptMedicalCJOrderOfProtection);
			}
			selections.Add(ReportSelection.StdRptMedicalCjProcess, medicalSelections);

			return selections;
		}

		private Dictionary<ReportSelection, List<SubReportSelection>> GetManagementReportSelectionsForProvider() {
			var selections = new Dictionary<ReportSelection, List<SubReportSelection>>();

			// Client
			var clientSelections = new List<SubReportSelection>();
			clientSelections.Add(SubReportSelection.MngRptClientClientDetail);
			if (Session.Center().Provider == Provider.DV) {
				clientSelections.Add(SubReportSelection.MngRptClientChildBehavioral);
				clientSelections.Add(SubReportSelection.MngRptClientIncomeSourceManagement);
			}
			selections.Add(ReportSelection.MngRptClient, clientSelections);

			// Staff/Service
			var serviceSelections = new List<SubReportSelection>();
			serviceSelections.Add(SubReportSelection.MngRptStaffServiceServiceInformation);
			serviceSelections.Add(SubReportSelection.MngRptStaffServiceCommunityGroup);
			if (Session.Center().Provider != Provider.SA) {
				serviceSelections.Add(SubReportSelection.MngRptStaffServiceMediaPublication);
			}
			if (Session.Center().Provider == Provider.SA) {
				serviceSelections.Add(SubReportSelection.MngRptStaffServiceEventMediaPublication);
			}
			if (Session.Center().Provider == Provider.DV) {
				serviceSelections.Add(SubReportSelection.MngRptStaffServiceHotline);
			}
			if (Session.Center().Provider == Provider.SA) {
				serviceSelections.Add(SubReportSelection.MngRptStaffServiceCrisisIntervention);
			}
			serviceSelections.Add(SubReportSelection.MngRptStaffServiceStaffReport);
			serviceSelections.Add(SubReportSelection.MngRptStaffServiceOtherStaffActivity);
			if (Session.Center().Provider == Provider.DV) {
				serviceSelections.Add(SubReportSelection.MngRptStaffServiceTurnAway);
			}
			if (Session.Center().Provider != Provider.CAC) {
				serviceSelections.Add(SubReportSelection.MngRptStaffServiceCancellation);
			}
			selections.Add(ReportSelection.MngRptStaffService, serviceSelections);

			// Other
			var otherSelections = new List<SubReportSelection>();
			if (Session.Center().Provider == Provider.DV) {
				otherSelections.Add(SubReportSelection.MngRptOtherOrderOfProtection);
			}
			selections.Add(ReportSelection.MngRptOther, otherSelections);

			return selections;
		}

		private List<SubReportSelection> GetExceptionReportSelectionsForProvider() {
			var selections = new List<SubReportSelection>();
			selections.Add(SubReportSelection.ExcRptClientsWithoutServiceRecord);
			if (Session.Center().IsDV)
				selections.Add(SubReportSelection.ExcRptLengthyShelterUse);
			selections.Add(SubReportSelection.ExcRptFirstContactDateLaterThanServiceDate);
			if (Session.Center().IsDV)
				selections.Add(SubReportSelection.ExcRptOrdersOfProtectionWithoutExpirationDate);
			selections.Add(SubReportSelection.ExcRptOpenClientCases);
			selections.Add(SubReportSelection.ExcRptClientsWithoutPresentingIssues);
			selections.Add(SubReportSelection.ExcRptClientsWithUNRUFields);
			selections.Add(SubReportSelection.ExcRptClientsWithoutOffenderInformation);
			return selections;
		}

		private List<UNRUDataFieldsEnum> GetExceptionUnknownNotReportedUnassignedFields() {
			var selections = new List<UNRUDataFieldsEnum>();
			switch (Session.Center().Provider) {
				case Provider.DV:
					selections.Add(UNRUDataFieldsEnum.Race);
					selections.Add(UNRUDataFieldsEnum.Age);
					selections.Add(UNRUDataFieldsEnum.Employment);
					selections.Add(UNRUDataFieldsEnum.Education);
					selections.Add(UNRUDataFieldsEnum.HealthInsurance);
					selections.Add(UNRUDataFieldsEnum.MaritalStatus);
					selections.Add(UNRUDataFieldsEnum.Pregnant);
                    selections.Add(UNRUDataFieldsEnum.PrimaryIncome);
                    selections.Add(UNRUDataFieldsEnum.NumberOfChildren);
                    selections.Add(UNRUDataFieldsEnum.School);
                    selections.Add(UNRUDataFieldsEnum.Custody);
                    selections.Add(UNRUDataFieldsEnum.LivesWith);
                    selections.Add(UNRUDataFieldsEnum.ReferralSourceDV);
                    selections.Add(UNRUDataFieldsEnum.PrimaryPresentingIssue);
					selections.Add(UNRUDataFieldsEnum.LocationOfPrimaryOffense);
					selections.Add(UNRUDataFieldsEnum.ChildBehaviors);
					selections.Add(UNRUDataFieldsEnum.Residence);
					break;
				case Provider.SA:
                    selections.Add(UNRUDataFieldsEnum.Gender);
                    selections.Add(UNRUDataFieldsEnum.Race);					
					selections.Add(UNRUDataFieldsEnum.Age);
                    selections.Add(UNRUDataFieldsEnum.Employment);
                    selections.Add(UNRUDataFieldsEnum.Education);
                    selections.Add(UNRUDataFieldsEnum.HealthInsurance);	
					selections.Add(UNRUDataFieldsEnum.MaritalStatus);
					selections.Add(UNRUDataFieldsEnum.Pregnant);
                    selections.Add(UNRUDataFieldsEnum.PrimaryIncome);
                    selections.Add(UNRUDataFieldsEnum.ReferralSourceSA);
                    selections.Add(UNRUDataFieldsEnum.PrimaryPresentingIssue);
                    selections.Add(UNRUDataFieldsEnum.LocationOfPrimaryOffense);
                    selections.Add(UNRUDataFieldsEnum.Residence);
                    selections.Add(UNRUDataFieldsEnum.RelationshipToVictim);
					selections.Add(UNRUDataFieldsEnum.SignifigantOtherOf);
					break;
				case Provider.CAC:
                    selections.Add(UNRUDataFieldsEnum.Gender);
                    selections.Add(UNRUDataFieldsEnum.Race);					
					selections.Add(UNRUDataFieldsEnum.Age);
					selections.Add(UNRUDataFieldsEnum.HealthInsurance);
                    selections.Add(UNRUDataFieldsEnum.Pregnant);
                    selections.Add(UNRUDataFieldsEnum.Custody);
                    selections.Add(UNRUDataFieldsEnum.Employment);
					selections.Add(UNRUDataFieldsEnum.MaritalStatus);
                    selections.Add(UNRUDataFieldsEnum.NumberOfChildren);
                    selections.Add(UNRUDataFieldsEnum.RelationshipToVictim);
                    selections.Add(UNRUDataFieldsEnum.ReferredFrom);
                    selections.Add(UNRUDataFieldsEnum.PrimaryPresentingIssue);
                    selections.Add(UNRUDataFieldsEnum.LocationOfPrimaryOffense);
                    selections.Add(UNRUDataFieldsEnum.Residence);
                    selections.Add(UNRUDataFieldsEnum.InvestigationType);
					break;
			}

			return selections;
		}
		#endregion


		private void RemoveSubReportSelectionError(ModelStateDictionary m) {
			if (m.ContainsKey("SubReportSelections"))
				m["SubReportSelections"].Errors.Clear();
		}

		private void SaveCommitSuccessMessage(DbContextTransaction transaction, string successMsg = "Your changes have been successfully saved.") {
			db.SaveChanges();
			transaction.Commit();
			AddSuccessMessage(successMsg);
		}

		private CompletedScheduledReportViewModel ViewRole(CompletedScheduledReportViewModel model) {
			if (model.ReportTitle == "Scheduled" || model.ReportTitle == "Completed") {
				if (User.IsInRole("DVCOALITIONADMIN") || User.IsInRole("CACCOALITIONADMIN") || User.IsInRole("DHSCOALITIONADMIN") || User.IsInRole("SACOALITIONADMIN") || User.IsInRole("CDFSSCOALITIONADMIN")) {
					model.ViewRole = "funder";
				} else if (User.IsInRole("DVADMIN") || User.IsInRole("SAADMIN")) {
					model.ViewRole = "";
				} else {
					model.ViewRole = "";
				}
			}
			if (model.ReportTitle == "Submit") {
				if (User.IsInRole("DVCOALITIONADMIN") || User.IsInRole("CACCOALITIONADMIN") || User.IsInRole("DHSCOALITIONADMIN") || User.IsInRole("SACOALITIONADMIN") || User.IsInRole("CDFSSCOALITIONADMIN")) {
					model.ViewRole = "funder";
				} else if (User.IsInRole("DVADMIN") || User.IsInRole("SAADMIN")) {
					model.ViewRole = "";
				} else {
					model.ViewRole = "";
				}
			}
			return model;
		}

		private static string[] GetDirectorEmailAddresses(InfonetServerContext db, IEnumerable<int> approvalCenterIds) {
			var emailAddressQuery = db.T_Center.Where(c => approvalCenterIds.Contains(c.CenterID) && c.DirectorEmail != null && c.DirectorEmail.Trim() != "").Select(c => c.DirectorEmail);
			var emailAddresses = new HashSet<string>(emailAddressQuery, StringComparer.CurrentCultureIgnoreCase).ToArray();
			Array.Sort(emailAddresses);
			return emailAddresses;
		}

		public ReportJobApproval ReportJobApprovalSystemMessage(ReportJobApproval reportJobApproval, ReportJob reportJob) {
			var reportJobJson = JsonConvert.DeserializeObject<StandardReportSpecification>(reportJob.SpecificationJson);
			var submitterCenter = db.T_Center.Find(reportJob.SubmitterCenterId);

			reportJobApproval.SystemMessage = new SystemMessage {
				Title = reportJobJson.Title,
				Message = $"The following report has been submitted by <i>{ submitterCenter.CenterName}</i> on { reportJob.SubmittedDate:M/d/yyyy}: <strong>{ reportJobJson.Title}</strong>. Please make sure data entry for {reportJobJson.StartDate.Value.ToShortDateString()} - {reportJobJson.EndDate.Value.ToShortDateString()} is complete by 12:00 am on {reportJob.ScheduledForDate.Value.ToShortDateString()}. <br/>",
				PostedDate = reportJobApproval.StatusDate,
				LocationIdsString = reportJobApproval.CenterIdsString,
				IsHot = true
			};
			return reportJobApproval;
		}

		private static string ComposeEmailBodySubmit(StandardReportSpecification specification, ReportJob job) {
			using (var sb = new StringWriter()) {
				sb.Write($"This email is to notify you that the following report has been submitted by <i>{job.SubmitterCenter.CenterName}</i> on {job.SubmittedDate:M/d/yyyy}: <strong>{specification.Title}</strong>.<br/>");
                sb.Write($"Please make sure data entry for {specification.StartDate.Value.ToShortDateString()} - {specification.EndDate.Value.ToShortDateString()} is completed by 12:00 am on {job.ScheduledForDate.Value.ToShortDateString()}. You will receive another notification after this report has generated and it is ready for approval.<br/>");
				sb.Write($"<br/><strong>Scheduled to Run</strong>: {job.ScheduledForDate.Value.ToShortDateString()}");
				sb.Write($"<br/><strong>Date Range</strong>: {specification.StartDate.Value.ToShortDateString()} - {specification.EndDate.Value.ToShortDateString()}");
				sb.Write("<dl>");
				sb.Write("<dt><strong>Report Types</strong>:</dt>");
				foreach (var sr in specification.SubReportSelections) {
					sb.Write($"<dd>{sr.GetDisplayName()}</dd>");
				}
				sb.Write("</dl>");
				if (_HelpDeskEmail != null || _HelpDeskPhone != null) {
					sb.Write("<br/><br/>If you have any questions or need technical assistance with InfoNet, please contact the help desk at ");
					if (_HelpDeskEmail != null) {
						sb.Write($"<a href=\"mailto:{_HelpDeskEmail}\">{_HelpDeskEmail}</a>");
						if (_HelpDeskPhone != null)
							sb.Write(" or ");
					}
					if (_HelpDeskPhone != null)
						sb.Write($"<a href=\"tel:+1-{_HelpDeskPhone}\">{_HelpDeskPhone}</a>");
					sb.Write(".");
				}
				return sb.ToString();
			}
		}
	}
}