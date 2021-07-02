using System.Linq;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ExceptionReports.Builders;
using Infonet.Reporting.ExceptionReports.Filters;
using Infonet.Reporting.Filters;
using Infonet.Reporting.ViewModels;

namespace Infonet.Reporting {
	public class ExceptionReportFactory {
		public ReportContainer RunReport(ExceptionReportViewModel model) {
			switch (model.ReportSelectionType) {
				case SubReportSelection.ExcRptClientsWithUNRUFields:
					model.StartDate = model.UNRUStartDate;
					model.EndDate = model.UNRUEndDate;
					break;
				case SubReportSelection.ExcRptClientsWithoutOffenderInformation:
					model.StartDate = model.OffenderStartDate;
					model.EndDate = model.OffenderEndDate;
					break;
			}

			var result = new ReportContainer("Exception Report") {
				Provider = model.Provider,
				StartDate = model.StartDate,
				EndDate = model.EndDate,
				CenterIds = model.CenterIds
			};
			switch (model.ReportSelectionType) {
				case SubReportSelection.ExcRptClientsWithoutServiceRecord:
					RunClientsWithoutServiceRecordReport(model, result);
					break;
				case SubReportSelection.ExcRptLengthyShelterUse:
					RunLengthyShelterReport(model, result);
					break;
				case SubReportSelection.ExcRptFirstContactDateLaterThanServiceDate:
					RunFirstContactDateLaterThanServiceDateReport(model, result);
					break;
				case SubReportSelection.ExcRptOrdersOfProtectionWithoutExpirationDate:
					RunOrdersOfProtectionWithoutExpirationReport(model, result);
					break;
				case SubReportSelection.ExcRptOpenClientCases:
					RunOpenCasesReport(model, result);
					break;
				case SubReportSelection.ExcRptClientsWithoutPresentingIssues:
					RunClientsWithoutPresentingIssuesReport(model, result);
					break;
				case SubReportSelection.ExcRptClientsWithUNRUFields:
					RunClientsWithUnknownNotReportedUnassignedFieldsReport(model, result);
					break;
				case SubReportSelection.ExcRptClientsWithoutOffenderInformation:
					RunClientsWithoutOffenderInformationReport(result);
					break;
			}
			return result;
		}

		private void RunClientsWithoutServiceRecordReport(ExceptionReportViewModel model, ReportContainer container) {
			var clientCaseQuery = ReportQueries.ClientCase();
			clientCaseQuery.Filters.Add(new ClientCenterFilter(model.CenterIds) { Visible = false });
			clientCaseQuery.Filters.Add(new PredicateFilter<ClientCase>(
				fc => fc.ClientCase,
				cc => !cc.ServiceDetailsOfClient.Any()));

			var clientSub = new ExceptionClientsWithoutServiceRecordSubReportBuilder(SubReportSelection.ExcRptClientsWithoutServiceRecord); // PRC DO add management reports to selection enums																																												 // Declare Columns
			clientSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientCode, 1));
			if (container.Provider != Provider.SA)
				clientSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.CaseID, 2));
			clientSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.FirstContactDate, 3));
			clientCaseQuery.SubReports.Add(clientSub);
			container.Reports.Add(clientCaseQuery);
		}

		private void RunLengthyShelterReport(ExceptionReportViewModel model, ReportContainer container) {
			var serviceDetailQuery = ReportQueries.ServiceDetailOfClient();
			serviceDetailQuery.Filters.Add(new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
			serviceDetailQuery.Filters.Add(new OpenAndLengthyShelterStaysFilter(model.ShelterDaysExceed ?? 60));

			// SubReportBuilder
			var lengthyShelterSub = new OpenAndLengthyShelterStaysSubReportBuilder(SubReportSelection.ExcRptLengthyShelterUse) { NumberOfDays = model.ShelterDaysExceed ?? 0 };
			// PRC DO add management reports to selection enums
			// Declare Columns
			lengthyShelterSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientCode, 1));
			lengthyShelterSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Comment, 2));
			lengthyShelterSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ShelterBeginDate, 3));
			lengthyShelterSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ShelterEndDate, 4));
			lengthyShelterSub.NumberOfDays = model.ShelterDaysExceed ?? 60;
			serviceDetailQuery.SubReports.Add(lengthyShelterSub);
			// Add to reportContainer
			container.Reports.Add(serviceDetailQuery);
		}

		private void RunFirstContactDateLaterThanServiceDateReport(ExceptionReportViewModel model, ReportContainer container) {
			var serviceDetailQuery = ReportQueries.ServiceDetailOfClient();
			serviceDetailQuery.Filters.Add(new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
			serviceDetailQuery.Filters.Add(new PredicateFilter<ServiceDetailOfClient>(
				fc => fc.ServiceDetailOfClient,
				sd => sd.ServiceDate < sd.ClientCase.FirstContactDate));

			// SubReportBuilder
			var fcdSub = new ExceptionFirstContactDateLaterThanServiceDateSubReportBuilder(SubReportSelection.ExcRptFirstContactDateLaterThanServiceDate);
			// Declare Columns
			fcdSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientCode, 1));

			if (container.Provider != Provider.SA)
				fcdSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.CaseID, 2));

			fcdSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.FirstContactDate, 3));
			fcdSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ServiceDate, 4));
			serviceDetailQuery.SubReports.Add(fcdSub);
			// Add to reportContainer
			container.Reports.Add(serviceDetailQuery);
		}

		private void RunOrdersOfProtectionWithoutExpirationReport(ExceptionReportViewModel model, ReportContainer container) {
			var orderOfProtectionQuery = ReportQueries.OrderOfProtection();
			orderOfProtectionQuery.Filters.Add(new OrderOfProtectionLocationFilter(model.CenterIds) { Visible = false });
			orderOfProtectionQuery.Filters.Add(new PredicateFilter<OrderOfProtection>(
				fc => fc.OrderOfProtection,
				q => q.DateIssued != null && q.OriginalExpirationDate == null));

			var opSub = new ExceptionOrdersOfProtectionWithoutExpirationDateSubReportBuilder(SubReportSelection.ExcRptOrdersOfProtectionWithoutExpirationDate) { DisplayOrder = 2 };
			opSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientCode, 1));
			opSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.CaseID, 2));
			opSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.DateIssued, 3));
			opSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.DateExpired, 4));
			orderOfProtectionQuery.SubReports.Add(opSub);

			container.Reports.Add(orderOfProtectionQuery);
		}

		private void RunOpenCasesReport(ExceptionReportViewModel model, ReportContainer container) {
			var clientCaseQuery = ReportQueries.ClientCase();
			clientCaseQuery.Filters.Add(new ClientCenterFilter(model.CenterIds) { Visible = false });
			clientCaseQuery.Filters.Add(new OpenButInactiveCasesFilter(model.OpenCases ?? 60));

			var openCasesSub = new ExceptionOpenCasesSubReportBuilder(SubReportSelection.ExcRptOpenClientCases);
			openCasesSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientCode, 1));
			if (container.Provider != Provider.SA)
				openCasesSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.CaseID, 2));
			openCasesSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.DateOfLastService, 3));
			openCasesSub.DaysSinceLastService = model.OpenCases ?? 60;
			clientCaseQuery.SubReports.Add(openCasesSub);
			container.Reports.Add(clientCaseQuery);
		}

		private void RunClientsWithoutPresentingIssuesReport(ExceptionReportViewModel model, ReportContainer container) {
			var clientCaseQuery = ReportQueries.ClientCase();
			clientCaseQuery.Filters.Add(new ClientCenterFilter(model.CenterIds) { Visible = false });
			clientCaseQuery.Filters.Add(new ClientTypeFilter(new int?[] { 1, 3, 7 }) { Visible = false });
			clientCaseQuery.Filters.Add(new PredicateFilter<ClientCase>(
				fc => fc.ClientCase,
				cc => cc.PresentingIssues == null || cc.PresentingIssues.PrimaryPresentingIssueID == null));

			var noPresSub = new ExceptionClientsWithoutPresentingIssueSubReportBuilder(SubReportSelection.ExcRptClientsWithoutPresentingIssues);
			noPresSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientCode, 1));
			if (container.Provider != Provider.SA)
				noPresSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.CaseID, 2));
			noPresSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.FirstContactDate, 3));
			clientCaseQuery.SubReports.Add(noPresSub);
			container.Reports.Add(clientCaseQuery);
		}

		private void RunClientsWithUnknownNotReportedUnassignedFieldsReport(ExceptionReportViewModel model, ReportContainer container) {
			var clientCaseQuery = ReportQueries.ClientCase();
			var unnruSub = new ExceptionClientsWithUNRUFieldsSubReportBuilder(SubReportSelection.ExcRptClientsWithUNRUFields);
			unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientInfo, 1));
			unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientCode, 2));
			if (container.Provider != Provider.SA)
				unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.CaseID, 3));
			unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientType, 4));
			unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.FirstContactDate, 5));
			unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientStatus, 6));
			foreach (var field in model.UNRUDataFieldsSelections)
				unnruSub.DataFieldSelections.Add(field);
			clientCaseQuery.SubReports.Add(unnruSub);
			container.Reports.Add(clientCaseQuery);
		}

		private void RunClientsWithoutOffenderInformationReport(ReportContainer container) {
			var clientCaseQuery = ReportQueries.ClientCase();
			var unnruSub = new ClientsWithoutOffendersDataFieldsSubReportBuilder(SubReportSelection.ExcRptClientsWithoutOffenderInformation);
			unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientCode, 1));
			if (container.Provider != Provider.SA)
				unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.CaseID, 2));
			unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientType, 3));
			unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.FirstContactDate, 4));
			unnruSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientStatus, 5));
			clientCaseQuery.SubReports.Add(unnruSub);
			container.Reports.Add(clientCaseQuery);
		}
	}
}