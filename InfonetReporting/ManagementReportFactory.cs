using System;
using System.Linq;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using Infonet.Reporting.ManagementReports.Builders;
using Infonet.Reporting.Ordering.Cancellations;
using Infonet.Reporting.Ordering.EventDetailsStaff;
using Infonet.Reporting.Ordering.OrdersOfProtection;
using Infonet.Reporting.Ordering.OtherStaffActivities;
using Infonet.Reporting.Ordering.PhoneHotlines;
using Infonet.Reporting.Ordering.ProgramDetailsStaff;
using Infonet.Reporting.Ordering.PublicationsStaff;
using Infonet.Reporting.Ordering.ServiceDetailsOfClient;
using Infonet.Reporting.Ordering.TurnAwayServices;
using Infonet.Reporting.ViewModels;

namespace Infonet.Reporting {
	public class ManagementReportFactory {
		public ReportContainer RunReport(ManagementReportViewModel model) {
			var result = new ReportContainer("Management Report") {
				Provider = model.Provider,
				StartDate = model.StartDate,
				EndDate = model.EndDate,
				CenterIds = model.CenterIds
			};
			switch (model.ReportSelection) {
				case SubReportSelection.MngRptClientClientDetail:
					RunManagementClientClientDetailReport(model, result);
					break;
				case SubReportSelection.MngRptStaffServiceServiceInformation:
					RunManagementStaffClientServiceReport(model, result);
					break;
				case SubReportSelection.MngRptStaffServiceStaffReport:
					RunManagementStaffReport(model, result);
					break;
				case SubReportSelection.MngRptStaffServiceTurnAway:
					RunManagementStaffTurnAwayServiceReport(model, result);
					break;
				case SubReportSelection.MngRptClientChildBehavioral:
					RunManagementClientChildBehavioralReport(model, result);
					break;
				case SubReportSelection.MngRptClientIncomeSourceManagement:
					RunManagementIncomeSourceReport(model, result);
					break;
				case SubReportSelection.MngRptOtherOrderOfProtection:
					RunManagementOrderOfProtectionReport(model, result);
					break;
				case SubReportSelection.MngRptStaffServiceMediaPublication:
				case SubReportSelection.MngRptStaffServiceEventMediaPublication:
					RunManagementEventMediaPublicationReport(model, result);
					break;
				case SubReportSelection.MngRptStaffServiceCommunityGroup:
					RunManagementCommunityGroupReport(model, result);
					break;
				case SubReportSelection.MngRptStaffServiceCancellation:
					RunManagementCancellationNoshowReport(model, result);
					break;
				case SubReportSelection.MngRptStaffServiceOtherStaffActivity:
					RunManagementOtherStaffActivityReport(model, result);
					break;
				case SubReportSelection.MngRptStaffServiceCrisisIntervention:
					RunManagementCrisisInterventionReport(model, result);
					break;
				case SubReportSelection.MngRptStaffServiceHotline:
					RunManagementHotlineReport(model, result);
					break;
			}
			return result;
		}

		private void RunManagementClientClientDetailReport(ManagementReportViewModel model, ReportContainer container) {
			var serviceDetailReport = ReportQueries.ServiceDetailOfClient();
			serviceDetailReport.Filters.Add(new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
			serviceDetailReport.Filters.Add(new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
			if (model.ClientDetailClientCodeSelection != null)
				serviceDetailReport.Filters.Add(new ClientCodeFilter(model.ClientDetailClientCodeSelection));
			if (model.ClientDetailClientTypeSelections != null)
				serviceDetailReport.Filters.Add(new ClientTypeFilter(model.ClientDetailClientTypeSelections));
			if (model.ClientDetailGenderSelections != null)
				serviceDetailReport.Filters.Add(new ClientGenderIdentityFilter(model.ClientDetailGenderSelections));
			if (model.ClientDetailEthnicitySelections != null)
				serviceDetailReport.Filters.Add(new ClientEthnicityFilter(model.ClientDetailEthnicitySelections));
			if (model.ClientDetailMinimumAge != null || model.ClientDetailMaximumAge != null)
				serviceDetailReport.Filters.Add(new ClientCaseAgeFilter(model.ClientDetailMinimumAge, model.ClientDetailMaximumAge));
			if (model.ClientDetailRaceSelections != null)
				if (model.Provider == Provider.CAC)
					serviceDetailReport.Filters.Add(new ClientRaceFilter(model.ClientDetailRaceSelections));
				else
					serviceDetailReport.Filters.Add(new ClientRaceHudFilter(model.ClientDetailRaceSelections));
			if (model.ClientDetailCityOrTownSelections != null || model.ClientDetailTownshipSelections != null || model.ClientDetailCountySelections != null || model.ClientDetailStateSelections != null || model.ClientDetailZipCodeSelections != null)
				serviceDetailReport.Filters.Add(new ServiceDetailTwnTshipCountyFilter(model.ClientDetailCityOrTownSelections, model.ClientDetailTownshipSelections, model.ClientDetailCountySelections, model.ClientDetailStateSelections, model.ClientDetailZipCodeSelections));
			if (model.StaffClientServiceServiceNameSelections != null)
				serviceDetailReport.Filters.Add(new ServiceDetailServiceFilter(model.StaffClientServiceServiceNameSelections));

			// Create SubReport Builder
			var clientDetailSub = new ClientDetailInformationSubReport(SubReportSelection.MngRptClientClientDetail) { DisplayOrder = 1 };
			/*For testing */
			//start
			if (model.ClientDetailOrderSelection != 0)
				clientDetailSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = model.ClientDetailOrderSelection, Order = 1 });
			clientDetailSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Client, Order = 2 });
			//end

			// Apply Ordering
			switch (model.ClientDetailOrderSelection) {
				case ReportOrderSelectionsEnum.Town:
					serviceDetailReport.Orders.Add(new ServiceDetailOfClientTownReportOrder());
					break;
				case ReportOrderSelectionsEnum.Township:
					serviceDetailReport.Orders.Add(new ServiceDetailOfClientTownshipReportOrder());
					break;
				case ReportOrderSelectionsEnum.County:
					serviceDetailReport.Orders.Add(new ServiceDetailOfClientCountyReportOrder());
					break;
				case ReportOrderSelectionsEnum.ZipCode:
					serviceDetailReport.Orders.Add(new ServiceDetailOfClientZipCodeReportOrder());
					break;
				case ReportOrderSelectionsEnum.State:
					serviceDetailReport.Orders.Add(new ServiceDetailOfClientStateReportOrder());
					break;
			}
			serviceDetailReport.Orders.Add(new ServiceDetailOfClientClientCodeReportOrder { DisplayOrder = 1, HideOrder = true });
			serviceDetailReport.Orders.Add(new ServiceDetailOfClientCaseReportOrder { DisplayOrder = 2, HideOrder = true });


            if (model.ClientDetailOrderSelection != ReportOrderSelectionsEnum.Town)
				serviceDetailReport.Orders.Add(new ServiceDetailOfClientTownReportOrder { DisplayOrder = 3, HideOrder = true });
			if (model.ClientDetailOrderSelection != ReportOrderSelectionsEnum.Township)
				serviceDetailReport.Orders.Add(new ServiceDetailOfClientTownshipReportOrder { DisplayOrder = 4, HideOrder = true });
			if (model.ClientDetailOrderSelection != ReportOrderSelectionsEnum.County)
				serviceDetailReport.Orders.Add(new ServiceDetailOfClientCountyReportOrder { DisplayOrder = 5, HideOrder = true });
			if (model.ClientDetailOrderSelection != ReportOrderSelectionsEnum.ZipCode)
				serviceDetailReport.Orders.Add(new ServiceDetailOfClientZipCodeReportOrder { DisplayOrder = 6, HideOrder = true });
			if (model.ClientDetailOrderSelection != ReportOrderSelectionsEnum.State)
				serviceDetailReport.Orders.Add(new ServiceDetailOfClientStateReportOrder { DisplayOrder = 7, HideOrder = true });
            serviceDetailReport.Orders.Add(new ServiceDetailOfClientServiceDateReportOrder { DisplayOrder = 9, HideOrder = true });
            serviceDetailReport.Orders.Add(new ServiceDetailOfClientServiceNameReportOrder { DisplayOrder = 8, HideOrder = true });			
			serviceDetailReport.Orders.Add(new ServiceDetailOfClientShelterBegDateReportOrder { DisplayOrder = 10, HideOrder = true });            

            // Apply Column Selections
            // Client ID and Case ID are required selections and disabled by default, so they will not postback. SA dont have CaseID.
            clientDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientCode, 0));
            if (model.Provider != Provider.SA) 
                clientDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.CaseID, 0));

			for (int i = 0; i < model.ClientDetailColumnSelections?.Length; i++)
				clientDetailSub.ColumnSelections.Add(new SubReportColumnSelection(model.ClientDetailColumnSelections[i], i));
			serviceDetailReport.SubReports.Add(clientDetailSub);

			container.Reports.Add(serviceDetailReport);
		}

		private void RunManagementStaffClientServiceReport(ManagementReportViewModel model, ReportContainer container) {
			var serviceDetailReport = ReportQueries.ServiceDetailOfClient();
			serviceDetailReport.Filters.Add(new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
			serviceDetailReport.Filters.Add(new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
			serviceDetailReport.Filters.Add(new PredicateFilter<ServiceDetailOfClient>(fc => fc.ServiceDetailOfClient, ServiceDetailOfClient.IsNotShelter()));
			if (model.StaffClientServiceStaffSelections != null)
				serviceDetailReport.Filters.Add(new ServiceDetailStaffFilter(model.StaffClientServiceStaffSelections));
			if (model.StaffClientServiceServiceNameSelections != null)
				serviceDetailReport.Filters.Add(new ServiceDetailServiceFilter(model.StaffClientServiceServiceNameSelections));
			if (model.StaffClientServiceFundingSourceSelections != null)
				serviceDetailReport.Filters.Add(new ServiceDetailStaffFundingSourceFilter(model.StaffClientServiceFundingSourceSelections));

			// Create SubReport Builder
			var staffClientServiceSub = new StaffClientServiceInformationSubReport(SubReportSelection.MngRptStaffServiceServiceInformation) {
				DisplayOrder = 1,
				DetailOrGroupSelection = model.StaffClientServiceRecordDetailOrderSelection
			};
			if (model.StaffClientServiceRecordDetailOrderSelection == RecordDetailOrderSelectionsEnum.GroupBy) {
				int displayOrder = 0;
				foreach (var column in model.StaffClientServiceColumnSelections) {
					switch (column) {
						case ReportColumnSelectionsEnum.ClientCode:
							staffClientServiceSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Client, Order = displayOrder });
							serviceDetailReport.Orders.Add(new ServiceDetailOfClientClientCodeReportOrder { DisplayOrder = displayOrder });
							break;
						case ReportColumnSelectionsEnum.Staff:
							staffClientServiceSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Staff, Order = displayOrder });
							serviceDetailReport.Orders.Add(new ServiceDetailOfClientStaffNameReportOrder { DisplayOrder = displayOrder });
							break;
						case ReportColumnSelectionsEnum.ServiceName:
							staffClientServiceSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Service, Order = displayOrder });
							serviceDetailReport.Orders.Add(new ServiceDetailOfClientServiceNameReportOrder { DisplayOrder = displayOrder });
							break;
					}
					displayOrder++;
				}
			} else {
				int displayOrder = 0;
				foreach (var column in model.StaffClientServiceColumnSelections) {
					switch (column) {
						case ReportColumnSelectionsEnum.ClientCode:
							staffClientServiceSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Client, Order = displayOrder });
							serviceDetailReport.Orders.Add(new ServiceDetailOfClientClientCodeReportOrder { DisplayOrder = displayOrder });
							break;
						case ReportColumnSelectionsEnum.Staff:
							staffClientServiceSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Staff, Order = displayOrder });
							serviceDetailReport.Orders.Add(new ServiceDetailOfClientStaffNameReportOrder { DisplayOrder = displayOrder });
							break;
						case ReportColumnSelectionsEnum.ServiceName:
							staffClientServiceSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Service, Order = displayOrder });
							serviceDetailReport.Orders.Add(new ServiceDetailOfClientServiceNameReportOrder { DisplayOrder = displayOrder });
							break;
					}
					displayOrder++;
				}
			}

			// Apply Column Selections
			int columnOrder = 0;
			foreach (var column in model.StaffClientServiceColumnSelections)
				staffClientServiceSub.ColumnSelections.Add(new SubReportColumnSelection(column, columnOrder++));
			serviceDetailReport.SubReports.Add(staffClientServiceSub);

			container.Reports.Add(serviceDetailReport);
		}

		private void RunManagementStaffTurnAwayServiceReport(ManagementReportViewModel model, ReportContainer container) {
			var turnAwayReportQuery = ReportQueries.TurnAway();
			turnAwayReportQuery.Filters.Add(new TurnAwayDateFilter(model.StartDate, model.EndDate) { Visible = false });
			turnAwayReportQuery.Filters.Add(new TurnAwayLocationFilter(model.CenterIds) { Visible = false });
			if (model.TurnAwayReferralSelections != null)
				turnAwayReportQuery.Filters.Add(new TurnAwayReferralMadeFilter(model.TurnAwayReferralSelections));

			turnAwayReportQuery.Orders.Add(new TurnAwayServiceReferralMadeReportOrder());
			turnAwayReportQuery.Orders.Add(new TurnAwayServiceDateReportOrder());

			var turnAwaySub = new TurnAwayInformationSubReport(SubReportSelection.MngRptStaffServiceTurnAway);
			turnAwaySub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.ReferralMade });
			turnAwaySub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Date, 1));
			turnAwaySub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.TurnAwayNumOfAdultVictims, 2));
			turnAwaySub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.TurnAwayNumOfChildren, 3));
			turnAwaySub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.TurnAwayNumOfFamily, 4));
			turnAwaySub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.TurnAwayReferralMade, 5));
			turnAwayReportQuery.SubReports.Add(turnAwaySub);
			container.Reports.Add(turnAwayReportQuery);
		}

		private void RunManagementStaffReport(ManagementReportViewModel model, ReportContainer container) {
			var clientCaseReport = ReportQueries.ClientCase();
			clientCaseReport.SubReports.Add(new StaffSubReport(SubReportSelection.MngRptStaffServiceStaffReport, model.StaffReportAgeSetSelection));
			if (model.Provider != Provider.CAC || model.StaffReportStaffSelections != null) {
				clientCaseReport.Filters.Add(new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
				clientCaseReport.Filters.Add(new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
				if (model.StaffReportStaffSelections != null)
					clientCaseReport.Filters.Add(new ServiceDetailStaffFilter(model.StaffReportStaffSelections));
			} else {
				var serviceDetailContext = new SubcontextFilter<ServiceDetailOfClient>(c => c.ServiceDetailOfClient,
					new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false },
					new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
				var referralDetailContext = new SubcontextFilter<ClientReferralDetail>(c => c.ClientReferralDetail,
					new ReferralDetailDateFilter(model.StartDate, model.EndDate) { Visible = false },
					new ReferralDetailLocationFilter(model.CenterIds) { Visible = false });
				clientCaseReport.Filters.Add(new OrFilter<ClientCase>(c => c.ClientCase, serviceDetailContext, referralDetailContext));
			}
			container.Reports.Add(clientCaseReport);

			var serviceDetailReport = ReportQueries.ServiceDetailOfClient();
			serviceDetailReport.SubReports.Add(new Staff_DirectClientServicesSubReport(SubReportSelection.MngRptStaffServiceStaffReport));
			serviceDetailReport.Filters.Add(new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
			serviceDetailReport.Filters.Add(new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
			if (model.StaffReportStaffSelections != null)
				serviceDetailReport.Filters.Add(new ServiceDetailStaffFilter(model.StaffReportStaffSelections));
			container.Reports.Add(serviceDetailReport);
		}

		private void RunManagementClientChildBehavioralReport(ManagementReportViewModel model, ReportContainer container) {
			var childBehaviorReport = ReportQueries.ChildBehavioralIssues();
			childBehaviorReport.SubReports.Add(new ClientChildBehavioralSubReport(SubReportSelection.MngRptClientChildBehavioral));
			childBehaviorReport.Filters.Add(new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
			childBehaviorReport.Filters.Add(new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
			container.Reports.Add(childBehaviorReport);
		}

		private void RunManagementIncomeSourceReport(ManagementReportViewModel model, ReportContainer container) {
			var clientIncome = ReportQueries.ClientCase();
			clientIncome.SubReports.Add(new ClientIncomeSourceSubReport(SubReportSelection.MngRptClientIncomeSourceManagement) {
				IncomeSourceIncomeRangeLowerBounds = model.IncomeSourceIncomeRanges.Select(range => range.LowerBound).Cast<decimal>().ToArray(),
				IncomeSourceIncomeRangeUpperBounds = model.IncomeSourceIncomeRanges.Select(range => range.UpperBound).ToArray()
			});
			clientIncome.Filters.Add(new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
			clientIncome.Filters.Add(new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
			clientIncome.Filters.Add(new ClientCaseAnnualIncomeRangeFilter(
				model.IncomeSourceIncomeRanges.Select(range => range.LowerBound).Cast<decimal>().ToArray(),
				model.IncomeSourceIncomeRanges.Select(range => range.UpperBound).ToArray()));
			if (model.IncomeSourceShelterTypeSelections != null)
				clientIncome.Filters.Add(new ClientCaseShelterTypeFilter(model.IncomeSourceShelterTypeSelections, model.StartDate, model.EndDate));
			container.Reports.Add(clientIncome);
		}

		private void RunManagementOrderOfProtectionReport(ManagementReportViewModel model, ReportContainer container) {
			var orderOfProtectionReport = ReportQueries.OrderOfProtection();
			orderOfProtectionReport.Filters.Add(new OrderOfProtectionLocationFilter(model.CenterIds) { Visible = false });

			//Apply Orders
			orderOfProtectionReport.Orders.Add(new OrderOfProtectionClientCodeReportOrder { HideOrder = true });

			// Create SubReport 
			var orderOfProtectionSub = new OtherOrderOfProtectionSubReport(SubReportSelection.MngRptOtherOrderOfProtection) {
				DateFilter = model.OrderOfProtectionDateSelection
			};

			// Apply Column Selections
			orderOfProtectionSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ClientCode, 1));
			orderOfProtectionSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.DateIssued, 2));
			orderOfProtectionSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.DateExpired, 3));
			orderOfProtectionSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.OriginalOpType, 4));

			orderOfProtectionReport.SubReports.Add(orderOfProtectionSub);

			if (model.OrderOfProtectionDateSelection == OrderOfProtectionIssuedOrExpiredSelectionsEnum.DateIssued)
				orderOfProtectionReport.Filters.Add(new OrderOfProtectionDateIssuedFilter(model.StartDate, model.EndDate) { Visible = false });
			else if (model.OrderOfProtectionDateSelection == OrderOfProtectionIssuedOrExpiredSelectionsEnum.DateExpired)
				orderOfProtectionReport.Filters.Add(new OrderOfProtectionDateExpiredFilter(model.StartDate, model.EndDate) { Visible = false });
			else
				throw new NotSupportedException($"OrderOfProtectionIssuedOrExpiredSelectionsEnum.{model.OrderOfProtectionDateSelection} not supported");
			container.Reports.Add(orderOfProtectionReport);
		}

		private void RunManagementEventMediaPublicationReport(ManagementReportViewModel model, ReportContainer container) {
			if (model.Provider == Provider.SA) {
				var eventStaffReport = ReportQueries.EventDetailStaff();
				eventStaffReport.Filters.Add(new EventDetailLocationFilter(model.CenterIds) { Visible = false });
				eventStaffReport.Filters.Add(new EventDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
				if (model.StaffMediaPublicationStaffSelections != null)
					eventStaffReport.Filters.Add(new EventDetailStaffFilter(model.StaffMediaPublicationStaffSelections));
				if (model.StaffMediaPublicationPublicationTypeSelections != null) //KMS DO this shouldn't use same selections as publications below!!!
					eventStaffReport.Filters.Add(new EventDetailProgramFilter(model.StaffMediaPublicationPublicationTypeSelections));
				if (model.StaffMediaPublicationLocationSelections != null)
					eventStaffReport.Filters.Add(new EventDetailAddressFilter(model.StaffMediaPublicationLocationSelections));

				//Apply Orders
				eventStaffReport.Orders.Add(new EventStaffNameReportOrder { HideOrder = true });
				eventStaffReport.Orders.Add(new EventTypeReportOrder { HideOrder = true });
				eventStaffReport.Orders.Add(new EventDateReportOrder { HideOrder = true });

				// Create SubReport 
				var eventSub = new StaffEventDetailInformationSubReport(SubReportSelection.MngRptStaffServiceEvent);

				//Grouping selection
				eventSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Staff });

				// Apply Column Selections
				eventSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Staff, 1));
				eventSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.EventType, 2));
				eventSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Date, 3));
				eventSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.EventName, 4));
				eventSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.EventHrs, 5));
				eventSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.PeopleReached, 6));
				eventSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.StaffConductHours, 7));
				eventSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.StaffPrepHours, 8));
				eventSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.StaffTravelHours, 9));
				eventSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Location, 10));

				eventStaffReport.SubReports.Add(eventSub);
				container.Reports.Add(eventStaffReport);
			}

			var publicationStaffReport = ReportQueries.PublicationDetailStaff();
			publicationStaffReport.Filters.Add(new PublicationDetailLocationFilter(model.CenterIds) { Visible = false });
			publicationStaffReport.Filters.Add(new PublicationDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
			if (model.StaffMediaPublicationStaffSelections != null)
				publicationStaffReport.Filters.Add(new PublicationDetailStaffFilter(model.StaffMediaPublicationStaffSelections));
			if (model.StaffMediaPublicationPublicationTypeSelections != null)
				publicationStaffReport.Filters.Add(new PublicationDetailProgramFilter(model.StaffMediaPublicationPublicationTypeSelections));

			//Apply Orders
			publicationStaffReport.Orders.Add(new PublicationStaffNameReportOrder { HideOrder = true });
			publicationStaffReport.Orders.Add(new PublicationMediaTypeReportOrder { HideOrder = true });
			publicationStaffReport.Orders.Add(new PublicationDateReportOrder { HideOrder = true });

			// Create SubReport 
			var mediaPublicationSub = new StaffMediaPublicationInformationSubReport(SubReportSelection.MngRptStaffServiceMediaPublication);

			//Grouping selection
			mediaPublicationSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Staff });

			// Apply Column Selections
			mediaPublicationSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Staff, 1));
			mediaPublicationSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.MediaPublicationType, 2));
			mediaPublicationSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Date, 3));
			mediaPublicationSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Title, 4));
			mediaPublicationSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.PrepareHours, 5));
			mediaPublicationSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.NumOfSegments, 6));
			mediaPublicationSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.StaffPrepHours, 7));

			publicationStaffReport.SubReports.Add(mediaPublicationSub);
			container.Reports.Add(publicationStaffReport);
		}

		private void RunManagementCommunityGroupReport(ManagementReportViewModel model, ReportContainer container) {
			var programDetailStaffReport = ReportQueries.ProgramDetailStaff();
			programDetailStaffReport.Filters.Add(new ProgramDetailLocationFilter(model.CenterIds) { Visible = false });
			programDetailStaffReport.Filters.Add(new ProgramDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
			if (model.StaffCommGroupServiceStaffSelections != null)
				programDetailStaffReport.Filters.Add(new ProgramDetailStaffFilter(model.StaffCommGroupServiceStaffSelections));
			if (model.StaffCommGroupServiceServiceNameSelections != null)
				programDetailStaffReport.Filters.Add(new ProgramDetailProgramFilter(model.StaffCommGroupServiceServiceNameSelections));
			if (model.StaffCommGroupServiceAgencySelections != null)
				programDetailStaffReport.Filters.Add(new ProgramDetailAgencyFilter(model.StaffCommGroupServiceAgencySelections));
			if (model.StaffCommGroupServiceLocationSelections != null)
				programDetailStaffReport.Filters.Add(new ProgramDetailAddressFilter(model.StaffCommGroupServiceLocationSelections));

			//Apply Orders
			programDetailStaffReport.Orders.Add(new ProgramDetailStaffNameReportOrder { HideOrder = true });
			programDetailStaffReport.Orders.Add(new ProgramDetailServiceReportOrder { HideOrder = true });
			programDetailStaffReport.Orders.Add(new ProgramDetailDateReportOrder { HideOrder = true });

			// Create SubReport 
			var programDetailSub = new StaffProgramDetailInformationSubReport(SubReportSelection.MngRptStaffServiceCommunityGroup);

			//Grouping selection
			programDetailSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Staff });

			// Apply Column Selections
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Staff, 1));
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ServiceName, 2));
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ServiceDate, 3));
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.NumOfPresentations, 4));
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.PresentationHrs, 5));
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.NumOfParticipants, 6));
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.StaffPresentationHrs, 7));
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.StaffPrepHours, 8));
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.StaffTravelHours, 9));
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Agency, 10));
			programDetailSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Location, 11));

			programDetailStaffReport.SubReports.Add(programDetailSub);
			container.Reports.Add(programDetailStaffReport);
		}

		private void RunManagementCrisisInterventionReport(ManagementReportViewModel model, ReportContainer container) {
			var crisisInterventionReport = ReportQueries.PhoneHotline();
			crisisInterventionReport.Filters.Add(new HotlineLocationFilter(model.CenterIds) { Visible = false });
			crisisInterventionReport.Filters.Add(new HotlineDateFilter(model.StartDate, model.EndDate) { Visible = false });
			if (model.StaffHotlineStaffSelections != null)
				crisisInterventionReport.Filters.Add(new HotlineStaffFilter(model.StaffHotlineStaffSelections));
			if (model.StaffHotlineHotlineTypeSelections != null)
				crisisInterventionReport.Filters.Add(new HotlineCallTypeFilter(model.StaffHotlineHotlineTypeSelections) { Label = "Type of Intervention" });

			//Apply Orders
			crisisInterventionReport.Orders.Add(new HotlineStaffNameReportOrder { HideOrder = true });
			crisisInterventionReport.Orders.Add(new HotlineDateReportOrder { HideOrder = true });

			// Create SubReport 
			var crisisInterventionSub = new HotlineCrisisInformationSubReport(SubReportSelection.MngRptStaffServiceCrisisIntervention);

			//Grouping selection
			crisisInterventionSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Staff });

			// Apply Column Selections
			crisisInterventionSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.Staff, 1));
			crisisInterventionSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ContactType, 2));
			crisisInterventionSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ContactDate, 3));
			crisisInterventionSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.NumOfContacts, 4));
			crisisInterventionSub.ColumnSelections.Add(new SubReportColumnSelection(ReportColumnSelectionsEnum.ContactTime, 5));

			crisisInterventionReport.SubReports.Add(crisisInterventionSub);
			container.Reports.Add(crisisInterventionReport);
		}

		private void RunManagementHotlineReport(ManagementReportViewModel model, ReportContainer container) {
			var hotlineReport = ReportQueries.PhoneHotline();
			hotlineReport.Filters.Add(new HotlineLocationFilter(model.CenterIds) { Visible = false });
			hotlineReport.Filters.Add(new HotlineDateFilter(model.StartDate, model.EndDate) { Visible = false });
			if (model.StaffHotlineStaffSelections != null)
				hotlineReport.Filters.Add(new HotlineStaffFilter(model.StaffHotlineStaffSelections));
			if (model.StaffHotlineHotlineTypeSelections != null)
				hotlineReport.Filters.Add(new HotlineCallTypeFilter(model.StaffHotlineHotlineTypeSelections));
			if (model.StaffHotlineCitySelections != null || model.StaffHotlineTownshipSelections != null || model.StaffHotlineCountySelections != null || model.StaffHotlineZipCodeSelections != null)
				hotlineReport.Filters.Add(new HotlineTwnTshipCountyFilter(model.StaffHotlineCitySelections, model.StaffHotlineTownshipSelections, model.StaffHotlineCountySelections, model.StaffHotlineZipCodeSelections));

			// Create SubReport 
			var hotlineSub = new HotlineCrisisInformationSubReport(SubReportSelection.MngRptStaffServiceHotline) { DisplayOrder = 1 };
			if (model.StaffHotlineOrderSelection != 0)
				hotlineSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = model.StaffHotlineOrderSelection, Order = 1 });
			hotlineSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Staff, Order = 2 });

			// Apply Ordering
			switch (model.StaffHotlineOrderSelection) {
				case ReportOrderSelectionsEnum.Town:
					hotlineReport.Orders.Add(new HotlineTownReportOrder());
					break;
				case ReportOrderSelectionsEnum.Township:
					hotlineReport.Orders.Add(new HotlineTownshipReportOrder());
					break;
				case ReportOrderSelectionsEnum.County:
					hotlineReport.Orders.Add(new HotlineCountyReportOrder());
					break;
				case ReportOrderSelectionsEnum.ZipCode:
					hotlineReport.Orders.Add(new HotlineZipcodeReportOrder());
					break;
			}

			hotlineReport.Orders.Add(new HotlineStaffNameReportOrder { DisplayOrder = 1, HideOrder = true });
			hotlineReport.Orders.Add(new HotlineDateReportOrder { DisplayOrder = 2, HideOrder = true });
			if (model.ClientDetailOrderSelection != ReportOrderSelectionsEnum.Town)
				hotlineReport.Orders.Add(new HotlineTownReportOrder { DisplayOrder = 3, HideOrder = true });
			if (model.ClientDetailOrderSelection != ReportOrderSelectionsEnum.Township)
				hotlineReport.Orders.Add(new HotlineTownshipReportOrder { DisplayOrder = 4, HideOrder = true });
			if (model.ClientDetailOrderSelection != ReportOrderSelectionsEnum.County)
				hotlineReport.Orders.Add(new HotlineCountyReportOrder { DisplayOrder = 5, HideOrder = true });
			if (model.ClientDetailOrderSelection != ReportOrderSelectionsEnum.ZipCode)
				hotlineReport.Orders.Add(new HotlineZipcodeReportOrder { DisplayOrder = 6, HideOrder = true });

			// Apply Column Selections
			int columnOrder = 0;
			foreach (var column in model.StaffHotlineColumnSelections)
				hotlineSub.ColumnSelections.Add(new SubReportColumnSelection(column, columnOrder++));

			hotlineReport.SubReports.Add(hotlineSub);
			container.Reports.Add(hotlineReport);
		}

		private void RunManagementCancellationNoshowReport(ManagementReportViewModel model, ReportContainer container) {
			var cancellationReport = ReportQueries.Cancellation();
			cancellationReport.Filters.Add(new CancellationLocationFilter(model.CenterIds) { Visible = false });
			cancellationReport.Filters.Add(new CancellationDateFilter(model.StartDate, model.EndDate) { Visible = false });
			if (model.CancellationStaffSelections != null)
				cancellationReport.Filters.Add(new CancellationStaffFilter(model.CancellationStaffSelections));
			if (model.CancellationServiceSelections != null)
				cancellationReport.Filters.Add(new CancellationServiceFilter(model.CancellationServiceSelections));
			if (model.CancellationReasonSelections != null)
				cancellationReport.Filters.Add(new CancellationReasonFilter(model.CancellationReasonSelections));

			// Create SubReport 
			var cancellationSub = new CancellationNoshowInformationSubReport(SubReportSelection.MngRptStaffServiceCancellation);

			//Grouping selection
			int displayOrder = 0;
			foreach (var column in model.CancellationColumnSelections) {
				switch (column) {
					case ReportColumnSelectionsEnum.ClientCode:
						cancellationSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Client, Order = displayOrder });
						cancellationReport.Orders.Add(new CancellationClientCodeReportOrder { DisplayOrder = displayOrder });
						displayOrder++;
						cancellationReport.Orders.Add(new CancellationCaseIdReportOrder { DisplayOrder = displayOrder, HideOrder = true });
						break;
					case ReportColumnSelectionsEnum.Staff:
						cancellationSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Staff, Order = displayOrder });
						cancellationReport.Orders.Add(new CancellationStaffNameReportOrder { DisplayOrder = displayOrder });
						break;
					case ReportColumnSelectionsEnum.ServiceName:
						cancellationSub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Service, Order = displayOrder });
						cancellationReport.Orders.Add(new CancellationServiceNameReportOrder { DisplayOrder = displayOrder });
						break;
					case ReportColumnSelectionsEnum.Date:
						cancellationReport.Orders.Add(new CancellationDateReportOrder { DisplayOrder = displayOrder, HideOrder = true });
						break;
					case ReportColumnSelectionsEnum.Reason:
						cancellationReport.Orders.Add(new CancellationReasonReportOrder { DisplayOrder = displayOrder, HideOrder = true });
						break;
				}
				displayOrder++;
			}

			// Apply Column Selections
			int columnOrder = 0;
			foreach (var column in model.CancellationColumnSelections)
				cancellationSub.ColumnSelections.Add(new SubReportColumnSelection(column, columnOrder++));
			cancellationReport.SubReports.Add(cancellationSub);
			container.Reports.Add(cancellationReport);
		}

		private void RunManagementOtherStaffActivityReport(ManagementReportViewModel model, ReportContainer container) {
			var otherStaffActivityReport = ReportQueries.OtherStaffActivity();
			otherStaffActivityReport.Filters.Add(new OtherStaffActivityLocationFilter(model.CenterIds) { Visible = false });
			otherStaffActivityReport.Filters.Add(new OtherStaffActivityDateFilter(model.StartDate, model.EndDate) { Visible = false });
			if (model.OtherStaffActivityStaffSelections != null)
				otherStaffActivityReport.Filters.Add(new OtherStaffActivityStaffFilter(model.OtherStaffActivityStaffSelections));
			if (model.OtherStaffActivityActivitySelections != null)
				otherStaffActivityReport.Filters.Add(new OtherStaffActivityTypeFilter(model.OtherStaffActivityActivitySelections));

			// Create SubReport
			var otherStaffActivitySub = new OtherStaffActivitySubReport(SubReportSelection.MngRptStaffServiceOtherStaffActivity);

			//Grouping selection
			int displayOrder = 0;
			otherStaffActivitySub.DetailOrGroupSelection = model.OtherStaffActivityRecordDetailOrderSelection;
			foreach (ReportColumnSelectionsEnum column in model.OtherStaffActivityColumnSelections) {
				switch (column) {
					case ReportColumnSelectionsEnum.Staff:
						otherStaffActivitySub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Staff, Order = displayOrder });
						otherStaffActivityReport.Orders.Add(new OSAStaffNameReportOrder { DisplayOrder = displayOrder });
						break;
					case ReportColumnSelectionsEnum.Activity:
						otherStaffActivitySub.GroupingSelections.Add(new SubReportTableGroupingSelection { GroupingSelection = ReportOrderSelectionsEnum.Activity, Order = displayOrder });
						otherStaffActivityReport.Orders.Add(new OSActivityNameReportOrder { DisplayOrder = displayOrder });
						break;
					case ReportColumnSelectionsEnum.Date:
						otherStaffActivityReport.Orders.Add(new OSADateReportOrder { DisplayOrder = displayOrder, HideOrder = true });
						break;
				}
				displayOrder++;
			}

			// Apply Column Selections
			int columnOrder = 0;
			foreach (var column in model.OtherStaffActivityColumnSelections)
				otherStaffActivitySub.ColumnSelections.Add(new SubReportColumnSelection(column, columnOrder++));
			otherStaffActivityReport.SubReports.Add(otherStaffActivitySub);
			container.Reports.Add(otherStaffActivityReport);
		}
	}
}