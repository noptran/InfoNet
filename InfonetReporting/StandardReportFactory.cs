using System;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using Infonet.Reporting.StandardReports;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;
using Infonet.Reporting.StandardReports.Builders.Investigation;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting {
	public static class StandardReportFactory {
		public static ReportContainer RunStandardReport(StandardReportSpecification model) {
			switch (model.ReportSelection) {
				case ReportSelection.StdRptClientInformation:
					return RunClientInformationReport(model);
				case ReportSelection.StdRptServicePrograms:
					return RunServiceProgramsReport(model);
				case ReportSelection.StdRptMedicalCjProcess:
					return RunMedicalCjProcessReport(model);
				case ReportSelection.StdRptInvestigationInformation:
					return RunInvestigationReport(model);
				default:
					throw new InvalidOperationException("Unrecognized " + nameof(model.ReportSelection));
			}
		}

		private static void SetReportContainerValues(StandardReportSpecification model, ReportContainer container) {
			container.Provider = model.Provider;
			container.StartDate = model.StartDate;
			container.EndDate = model.EndDate;
			container.CenterIds = model.CenterIds;
		}

		private static ReportContainer RunClientInformationReport(StandardReportSpecification model) {
			var result = new ReportContainer(model.Title ?? "Client Information Report");
			SetReportContainerValues(model, result);
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptClientInformationBasicDemographics)) {
				#region basic demographics
				var q = ReportQueries.ClientCase();
				q.SubReports.Add(new ClientInformationDemographicsSubReport(SubReportSelection.StdRptClientInformationBasicDemographics) {
					DisplayOrder = SubReportSelection.StdRptClientInformationBasicDemographics.ToInt32()
				});
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptClientInformationReferralSource)) {
				#region referral source
				var q = ReportQueries.ClientCase();
				q.SubReports.Add(new ClientInformationReferralSourcesSubReport(SubReportSelection.StdRptClientInformationReferralSource) {
					DisplayOrder = SubReportSelection.StdRptClientInformationReferralSource.ToInt32()
				});
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptClientInformationSpecialNeeds)) {
				#region special needs
				var q = ReportQueries.ClientCase();
				q.SubReports.Add(new ClientInformationSpecialNeedsSubReport(SubReportSelection.StdRptClientInformationSpecialNeeds) {
					DisplayOrder = SubReportSelection.StdRptClientInformationSpecialNeeds.ToInt32()
				});
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptClientInformationPresentingIssues)) {
				#region presenting issues
				var q = ReportQueries.ClientCase();
				q.SubReports.Add(new ClientInformationPresentingIssuesSubReport(SubReportSelection.StdRptClientInformationPresentingIssues) {
					DisplayOrder = SubReportSelection.StdRptClientInformationPresentingIssues.ToInt32()
				});
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptClientInformationAggregateClientInformation)) {
				#region aggregate client information
				var q = ReportQueries.HivMentalSubstance();
				q.SubReports.Add(new ClientInformationAggregateSubReport(SubReportSelection.StdRptClientInformationAggregateClientInformation) {
					DisplayOrder = SubReportSelection.StdRptClientInformationAggregateClientInformation.ToInt32()
				});
				q.Filters.Add(new HivMentalSubstanceDateFilter(model.StartDate, model.EndDate) { Visible = false });
				q.Filters.Add(new HivMentalSubstanceLocationFilter(model.CenterIds) { Visible = false });
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptClientInformationResidenceDestinationInformation)) {
				#region residence/destination information
				//KMS DO are displayed residences filtered properly with standard ClientCase filters
				var q = ReportQueries.ClientCase();
				q.SubReports.Add(new ClientInformationResidenceDestinationSubReport(SubReportSelection.StdRptClientInformationResidenceDestinationInformation) {
					DisplayOrder = SubReportSelection.StdRptClientInformationResidenceDestinationInformation.ToInt32()
				});
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			return result;
		}

		private static ReportContainer RunServiceProgramsReport(StandardReportSpecification model) {
			var result = new ReportContainer(model.Title ?? (model.SubReportSelections.Contains(SubReportSelection.StdRptServiceProgramsHudHmisServiceReport) ? "HUD/HMIS Service Report" : "Programs and Services Report"));
            
			SetReportContainerValues(model, result);
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptServiceProgramsDirectClientServices)) {
				#region direct client services
				var q1 = ReportQueries.ServiceDetailOfClient();
				q1.SubReports.Add(new DirectClientServicesSubReport(SubReportSelection.StdRptServiceProgramsDirectClientServices) { IsInGroup = model.Provider == Provider.CAC });
				AddServiceDetailFiltersTo(q1, model);
				result.Reports.Add(q1);

				if (model.Provider == Provider.CAC) {
					var q2 = ReportQueries.ClientReferralDetail();
					q2.SubReports.Add(new DirectClientReferralsSubReport(SubReportSelection.StdRptServiceProgramsDirectClientServices) { IsInGroup = true, IsEndOfGroup = true });
					q2.Filters.Add(new ReferralDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
					q2.Filters.Add(new ReferralDetailLocationFilter(model.CenterIds) { Visible = false });
					if (model.CityOrTowns != null || model.Townships != null || model.CountyIds != null || model.StateIds != null || model.Zipcodes != null)
						q2.Filters.Add(new ReferralDetailTwnTshipCountyFilter(model.CityOrTowns, model.Townships, model.CountyIds, model.StateIds, model.Zipcodes));
					if (model.GenderIds != null)
						q2.Filters.Add(new ClientGenderIdentityFilter(model.GenderIds));
					if (model.EthnicityIds != null)
						q2.Filters.Add(new ClientEthnicityFilter(model.EthnicityIds));
					if (model.MinimumAge != null || model.MaximumAge != null)
						q2.Filters.Add(new ClientCaseAgeFilter(model.MinimumAge, model.MaximumAge));
					if (model.RaceIds != null)
						if (model.Provider == Provider.CAC)
							q2.Filters.Add(new ClientRaceFilter(model.RaceIds));
						else
							q2.Filters.Add(new ClientRaceHudFilter(model.RaceIds));
					if (model.OffenderRelationshipIds != null)
						q2.Filters.Add(new OffenderRelationshipFilter(model.OffenderRelationshipIds));
					if (model.ClientTypeIds != null)
						q2.Filters.Add(new ClientCaseShelterTypeFilter(model.ClientTypeIds, model.StartDate, model.EndDate));

					if (model.SvIds != null || model.ServiceIds != null || model.FundingSourceIds != null) {
						var serviceDetailContext = new SubcontextFilter<ServiceDetailOfClient>(c => c.ServiceDetailOfClient,
							new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false },
							new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
						if (model.CityOrTowns != null || model.Townships != null || model.CountyIds != null || model.StateIds != null || model.Zipcodes != null)
							serviceDetailContext.Add(new ServiceDetailTwnTshipCountyFilter(model.CityOrTowns, model.Townships, model.CountyIds, model.StateIds, model.Zipcodes) { Visible = false });
						if (model.SvIds != null)
							serviceDetailContext.Add(new ServiceDetailStaffFilter(model.SvIds));
						if (model.ServiceIds != null)
							serviceDetailContext.Add(new ServiceDetailServiceFilter(model.ServiceIds));
						if (model.FundingSourceIds != null)
							serviceDetailContext.Add(new ServiceDetailStaffFundingSourceFilter(model.FundingSourceIds));
						q2.Filters.Add(serviceDetailContext);
					}

					result.Reports.Add(q2);
				}
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptServiceProgramsCommunityInstitutionalGroupServices)) {
				#region community/institutional and group services
				var q1 = ReportQueries.ProgramDetail();
				q1.SubReports.Add(new CommunityGroupSubReport(SubReportSelection.StdRptServiceProgramsCommunityInstitutionalGroupServices));
				AddProgramDetailFiltersTo(q1, model);
				result.Reports.Add(q1);

				if (model.Provider == Provider.SA) {
					var q2 = ReportQueries.EventDetail();
					q2.SubReports.Add(new EventDetailSubReport(SubReportSelection.StdRptServiceProgramsCommunityInstitutionalGroupServices));
					q2.Filters.Add(new EventDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
					q2.Filters.Add(new EventDetailLocationFilter(model.CenterIds) { Visible = false });
					if (model.SvIds != null)
						q2.Filters.Add(new EventDetailStaffFilter(model.SvIds));
					if (model.FundingSourceIds != null)
						q2.Filters.Add(new EventDetailFundingSourceFilter(model.FundingSourceIds));
					if (model.CountyIds != null || model.StateIds != null)
						q2.Filters.Add(new EventDetailCountyAndStateFilter(model.CountyIds, model.StateIds));
					result.Reports.Add(q2);
				}

				var q3 = ReportQueries.PublicationDetail();
				q3.SubReports.Add(new PublicationSubReport(SubReportSelection.StdRptServiceProgramsCommunityInstitutionalGroupServices));
				q3.Filters.Add(new PublicationDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
				q3.Filters.Add(new PublicationDetailLocationFilter(model.CenterIds) { Visible = false });
				if (model.SvIds != null)
					q3.Filters.Add(new PublicationDetailStaffFilter(model.SvIds));
				if (model.FundingSourceIds != null)
					q3.Filters.Add(new PublicationDetailFundingSourceFilter(model.FundingSourceIds));
				result.Reports.Add(q3);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptServiceProgramsNonClientCrisisIntervention)) {
				#region nonclient crisis intervention
				var q = ReportQueries.PhoneHotline();
				q.SubReports.Add(new NonClientCrisisInterventionSubReport(SubReportSelection.StdRptServiceProgramsNonClientCrisisIntervention) {
					ShowDemographics = model.SubReportSelections.Contains(SubReportSelection.StdRptServiceProgramsNonClientCrisisInterventionDemographics)
				});
				AddHotlineFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptServiceProgramsVolunteerServiceInformation) ||
				model.SubReportSelections.Contains(SubReportSelection.StdRptServiceProgramsHudHmisServiceReport)) {
				#region volunteer service information
				var q1 = ReportQueries.ServiceDetailOfClient();
				q1.SubReports.Add(new VolunteerClientServicesSubReport(SubReportSelection.StdRptServiceProgramsVolunteerServiceInformation));
				var db = result.InfonetContext;
				q1.Filters.Add(new PredicateFilter<StaffFunding>(fc => fc.ServiceDetailOfClient.StaffFunding, sf => db.T_StaffVolunteer.Any(sv => sv.SvId == sf.SvId && sv.TypeId == 2)));
				q1.Filters.Add(new PredicateFilter<ServiceDetailOfClient>(fc => fc.ServiceDetailOfClient, sd => sd.ICS_ID == null));
                AddServiceDetailFiltersTo(q1, model);
				result.Reports.Add(q1);

				if (model.Provider != Provider.CAC) {
					var q2 = ReportQueries.PhoneHotline();
					q2.SubReports.Add(new VolunteerHotlineCallsSubReport(SubReportSelection.StdRptServiceProgramsVolunteerServiceInformation));
					q2.Filters.Add(new PredicateFilter<PhoneHotline>(fc => fc.PhoneHotline, h => h.StaffVolunteer.TypeId == 2));
					q2.Filters.Add(new HotlineDateFilter(model.StartDate, model.EndDate) { Visible = false });
					q2.Filters.Add(new HotlineLocationFilter(model.CenterIds) { Visible = false });
					if (model.SvIds != null)
						q2.Filters.Add(new HotlineStaffFilter(model.SvIds));
					if (model.ServiceIds != null)
						q2.Filters.Add(new PredicateFilter<PhoneHotline>(fc => fc.PhoneHotline, h => false)); /* no available service used with PhoneHotline */
					if (model.FundingSourceIds != null)
						q2.Filters.Add(new HotlineFundingSourceFilter(model.FundingSourceIds));
					result.Reports.Add(q2);
				}

				var q3 = ReportQueries.ProgramDetailStaff();
				q3.SubReports.Add(new VolunteerGroupServicesSubReport(SubReportSelection.StdRptServiceProgramsVolunteerServiceInformation));
				q3.Filters.Add(new PredicateFilter<ProgramDetailStaff>(fc => fc.ProgramDetailStaff, pds => pds.StaffVolunteer.TypeId == 2));
				AddProgramDetailFiltersTo(q3, model);
				result.Reports.Add(q3);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptServiceProgramsHotlineInformationReferral)) {
				#region hotline/information referral
				var q = ReportQueries.PhoneHotline();
				q.SubReports.Add(new HotlineSubReport(SubReportSelection.StdRptServiceProgramsHotlineInformationReferral));
				AddHotlineFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Intersect(new[] { SubReportSelection.StdRptServiceProgramsServiceOutcomesServiceReport }).Any()) {
				#region service outcomes service report
				var q = ReportQueries.ServiceOutcome();
				q.SubReports.Add(new ServiceOutcomeSubReport(SubReportSelection.StdRptServiceProgramsServiceOutcomesServiceReport));
				q.Filters.Add(new ServiceOutcomeDateFilter(model.StartDate, model.EndDate) { Visible = false });
				q.Filters.Add(new ServiceOutcomeLocationFilter(model.CenterIds) { Visible = false });
				result.Reports.Add(q);
				#endregion
			}
            if (model.SubReportSelections.Contains(SubReportSelection.StdRptServiceProgramsHudHmisTurnAway)) {
                #region Turn Away report
                var q = ReportQueries.TurnAway();
                q.SubReports.Add(new HudTurnAwaysSubReport(SubReportSelection.StdRptServiceProgramsHudHmisTurnAway));
                q.Filters.Add(new TurnAwayDateFilter(model.StartDate, model.EndDate) { Visible = false });
                q.Filters.Add(new TurnAwayLocationFilter(model.CenterIds) { Visible = false });
                result.Reports.Add(q);
                #endregion
            }
            if (model.SubReportSelections.Contains(SubReportSelection.StdRptServiceProgramsHudHmisServiceReport)) {
                #region hud/hmis service report
                result.TitleNote = " <b>NOTE</b>:This report is for informational purposes only and does not fully comply with all HUD reporting requirements.";
                var q1 = ReportQueries.ServiceDetailOfClient();
				q1.SubReports.Add(new HudClientServicesSubReport(SubReportSelection.StdRptServiceProgramsHudHmisDirectServices));
				AddServiceDetailFiltersTo(q1, model);
				result.Reports.Add(q1);

				var q2 = ReportQueries.ProgramDetail();
				q2.SubReports.Add(new HudGroupServicesSubReport(SubReportSelection.StdRptServiceProgramsHudHmisGroupServices));
				q2.Filters.Add(new PredicateFilter<ProgramDetail>(fc => fc.ProgramDetail, h => h.TLU_Codes_ProgramsAndServices.IsGroupService ?? false));
				AddProgramDetailFiltersTo(q2, model);
				result.Reports.Add(q2);

				var q3 = ReportQueries.TurnAway();
				q3.SubReports.Add(new HudTurnAwaysSubReport(SubReportSelection.StdRptServiceProgramsHudHmisTurnAway));
				q3.Filters.Add(new TurnAwayDateFilter(model.StartDate, model.EndDate) { Visible = false });
				q3.Filters.Add(new TurnAwayLocationFilter(model.CenterIds) { Visible = false });
				result.Reports.Add(q3);
				#endregion
			}
			return result;
		}

		private static ReportContainer RunMedicalCjProcessReport(StandardReportSpecification model) {
			var result = new ReportContainer(model.Title ?? "Medical CJ Process Report");
			SetReportContainerValues(model, result);
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptMedicalCJOffenders)) {
				#region offenders
				var q = ReportQueries.Offender();
				q.SubReports.Add(new OffendersSubReport(SubReportSelection.StdRptMedicalCJOffenders));
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptMedicalCJMedicalSystemInvolvement)) {
				#region medical system involvement
				var q1 = ReportQueries.ClientCjProcess();               
                q1.SubReports.Add(new MedicalSystemInvolvementSubReport(SubReportSelection.StdRptMedicalCJMedicalSystemInvolvement) {IsInGroup = true, IsEndOfGroup = model.Provider == Provider.SA });
				AddClientFiltersTo(q1, model);
				result.Reports.Add(q1);

				if (model.Provider == Provider.DV) {
					var q2 = ReportQueries.ClientCase();
					q2.SubReports.Add(new MedicalSystemInvolvementConflictScaleSubReport(SubReportSelection.StdRptMedicalCJMedicalSystemInvolvement));
					AddClientFiltersTo(q2, model);
					result.Reports.Add(q2);
				}
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptMedicalCJPoliceInvolvement)) {
				#region police involvement
				var q1 = ReportQueries.ClientPoliceProsecution();
				q1.SubReports.Add(new MedicalCJPoliceInvolvementMedicalCJSubReport(SubReportSelection.StdRptMedicalCJPoliceInvolvement));
				AddClientFiltersTo(q1, model);
				result.Reports.Add(q1);

				var q2 = ReportQueries.PoliceCharge();
				q2.SubReports.Add(new MedicalCJPoliceInvolvementPoliceChargeSubReport(SubReportSelection.StdRptMedicalCJPoliceInvolvement));
				AddClientFiltersTo(q2, model);
				result.Reports.Add(q2);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptMedicalCJProsecutionInvolvement)) {
				#region prosecution involvement
				var q1 = ReportQueries.ClientPoliceProsecution();
				q1.SubReports.Add(new MedicalCJProsecutionInvolvementPoliceProsecutionSubReport(SubReportSelection.StdRptMedicalCJProsecutionInvolvement));
				AddClientFiltersTo(q1, model);
				result.Reports.Add(q1);

				var q2 = ReportQueries.TrialCharge();
				q2.SubReports.Add(new MedicalCJProsecutionInvolvementTrialChargesSubReport(SubReportSelection.StdRptMedicalCJProsecutionInvolvement));
				AddClientFiltersTo(q2, model);
				result.Reports.Add(q2);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptMedicalCJOrderOfProtection)) {
				#region order of protection
				var q = ReportQueries.OrderOfProtection();
				q.SubReports.Add(new MedicalCJOrdersOfProtectionSubReport(SubReportSelection.StdRptMedicalCJOrderOfProtection));
                q.Filters.Add(new OrderOfProtectionLocationFilter(model.CenterIds) { Visible = false });
                AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			return result;
		}

		private static ReportContainer RunInvestigationReport(StandardReportSpecification model) {
			var result = new ReportContainer(model.Title ?? "Investigation Report");
			SetReportContainerValues(model, result);
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptInvestigationDCFSAllegations)) {
				#region dcfs allegations
				var q = ReportQueries.DCFSAllegation();
				q.SubReports.Add(new DCFSAllegationSubReportBuilder(SubReportSelection.StdRptInvestigationDCFSAllegations));
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptInvestigationAbuseNeglectPetitions)) {
				#region abuse neglect petitions
				var q = ReportQueries.AbuseNeglectPetition();
				q.SubReports.Add(new AbuseNeglectPetitonSubReportBuilder(SubReportSelection.StdRptInvestigationAbuseNeglectPetitions));
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptInvestigationMultiDisciplinaryTeam)) {
				#region multi-disciplinary team
				var q = ReportQueries.ClientMDT();
				q.SubReports.Add(new ClientMDTSubReportBuilder(SubReportSelection.StdRptInvestigationMultiDisciplinaryTeam));
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptInvestigationVictimSensitiveInterview)) {
				#region victim sensitive interview
				var q = ReportQueries.VictimSensitiveInterview();
				q.SubReports.Add(new VictimSensitiveInterviewSubReportBuilder(SubReportSelection.StdRptInvestigationVictimSensitiveInterview));
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			if (model.SubReportSelections.Contains(SubReportSelection.StdRptInvestigationMedical)) {
				#region medical
				var q = ReportQueries.ClientCjProcess();
				q.SubReports.Add(new MedicalSubReportBuilder(SubReportSelection.StdRptInvestigationMedical));
				AddClientFiltersTo(q, model);
				result.Reports.Add(q);
				#endregion
			}
			return result;
		}

		#region shared filters
		private static void AddClientFiltersTo(IReportQuery query, StandardReportSpecification model) {
			if (model.GenderIds != null)
				query.Filters.Add(new ClientGenderIdentityFilter(model.GenderIds));
			if (model.EthnicityIds != null)
				query.Filters.Add(new ClientEthnicityFilter(model.EthnicityIds));
			if (model.MinimumAge != null || model.MaximumAge != null)
				query.Filters.Add(new ClientCaseAgeFilter(model.MinimumAge, model.MaximumAge));
			if (model.RaceIds != null)
				if (model.Provider == Provider.CAC)
					query.Filters.Add(new ClientRaceFilter(model.RaceIds));
				else
					query.Filters.Add(new ClientRaceHudFilter(model.RaceIds));
			if (model.SvIds != null)
				query.Filters.Add(new ServiceDetailStaffFilter(model.SvIds));
			if (model.ServiceIds != null)
				query.Filters.Add(new ServiceDetailServiceFilter(model.ServiceIds));
			if (model.OffenderRelationshipIds != null)
				query.Filters.Add(new OffenderRelationshipFilter(model.OffenderRelationshipIds));
			if (model.FundingSourceIds != null)
				query.Filters.Add(new ServiceDetailStaffFundingSourceFilter(model.FundingSourceIds));
			if (model.ClientTypeIds != null)
				query.Filters.Add(new ClientCaseShelterTypeFilter(model.ClientTypeIds, model.StartDate, model.EndDate));

			if (model.Provider != Provider.CAC || query.Filters.Any(f => f is ServiceDetailStaffFilter || f is ServiceDetailServiceFilter || f is ServiceDetailStaffFundingSourceFilter)) {
				query.Filters.Add(new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
				query.Filters.Add(new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
				if (model.CityOrTowns != null || model.Townships != null || model.CountyIds != null || model.StateIds != null || model.Zipcodes != null)
					query.Filters.Add(new ServiceDetailTwnTshipCountyFilter(model.CityOrTowns, model.Townships, model.CountyIds, model.StateIds, model.Zipcodes));
			} else {
				//KMS DO can we get rid of this duplicate code?
				var serviceDetailContext = new SubcontextFilter<ServiceDetailOfClient>(c => c.ServiceDetailOfClient,
					new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false },
					new ServiceDetailLocationFilter(model.CenterIds) { Visible = false });
				var referralDetailContext = new SubcontextFilter<ClientReferralDetail>(c => c.ClientReferralDetail,
					new ReferralDetailDateFilter(model.StartDate, model.EndDate) { Visible = false },
					new ReferralDetailLocationFilter(model.CenterIds) { Visible = false });
				if (model.CityOrTowns != null || model.Townships != null || model.CountyIds != null || model.StateIds != null || model.Zipcodes != null) {
					serviceDetailContext.Add(new ServiceDetailTwnTshipCountyFilter(model.CityOrTowns, model.Townships, model.CountyIds, model.StateIds, model.Zipcodes));
					referralDetailContext.Add(new ReferralDetailTwnTshipCountyFilter(model.CityOrTowns, model.Townships, model.CountyIds, model.StateIds, model.Zipcodes) { Visible = false });
				}
				query.Filters.Add(new OrFilter<ClientCase>(c => c.ClientCase, serviceDetailContext, referralDetailContext));
			}
		}

		private static void AddServiceDetailFiltersTo(IReportQuery query, StandardReportSpecification model) {
			var serviceNameSubcontext = new SubcontextFilter<ClientCase>(c => c.ClientCase);
			query.Filters.Add(serviceNameSubcontext.Add(new ServiceDetailDateFilter(model.StartDate, model.EndDate) { Visible = false }));
			query.Filters.Add(serviceNameSubcontext.Add(new ServiceDetailLocationFilter(model.CenterIds) { Visible = false }));
			if (model.GenderIds != null)
				query.Filters.Add(new ClientGenderIdentityFilter(model.GenderIds));
			if (model.EthnicityIds != null)
				query.Filters.Add(new ClientEthnicityFilter(model.EthnicityIds));
			if (model.MinimumAge != null || model.MaximumAge != null)
				query.Filters.Add(new ClientCaseAgeFilter(model.MinimumAge, model.MaximumAge));
			if (model.RaceIds != null)
				if (model.Provider == Provider.CAC)
					query.Filters.Add(new ClientRaceFilter(model.RaceIds));
				else
					query.Filters.Add(new ClientRaceHudFilter(model.RaceIds));
			if (model.CityOrTowns != null || model.Townships != null || model.CountyIds != null || model.StateIds != null || model.Zipcodes != null)
				query.Filters.Add(serviceNameSubcontext.Add(new ServiceDetailTwnTshipCountyFilter(model.CityOrTowns, model.Townships, model.CountyIds, model.StateIds, model.Zipcodes)));
			if (model.SvIds != null)
				query.Filters.Add(serviceNameSubcontext.Add(new ServiceDetailStaffFilter(model.SvIds)));
			if (model.OffenderRelationshipIds != null)
				query.Filters.Add(new OffenderRelationshipFilter(model.OffenderRelationshipIds));
			if (model.FundingSourceIds != null)
				query.Filters.Add(serviceNameSubcontext.Add(new ServiceDetailStaffFundingSourceFilter(model.FundingSourceIds)));
			if (model.ClientTypeIds != null)
				query.Filters.Add(new ClientCaseShelterTypeFilter(model.ClientTypeIds, model.StartDate, model.EndDate));
			if (model.ServiceIds != null) {
				serviceNameSubcontext.Add(new ServiceDetailServiceFilter(model.ServiceIds));
				query.Filters.Add(serviceNameSubcontext);
			}
		}

		private static void AddProgramDetailFiltersTo(IReportQuery report, StandardReportSpecification model) {
			report.Filters.Add(new ProgramDetailDateFilter(model.StartDate, model.EndDate) { Visible = false });
			report.Filters.Add(new ProgramDetailLocationFilter(model.CenterIds) { Visible = false });
			if (model.SvIds != null)
				report.Filters.Add(new ProgramDetailStaffFilter(model.SvIds));
			if (model.ServiceIds != null)
				report.Filters.Add(new ProgramDetailProgramFilter(model.ServiceIds));
			if (model.FundingSourceIds != null)
				report.Filters.Add(new ProgramDetailFundingSourceFilter(model.FundingSourceIds));
		}

		private static void AddHotlineFiltersTo(IReportQuery report, StandardReportSpecification model) {
			report.Filters.Add(new HotlineDateFilter(model.StartDate, model.EndDate) { Visible = false });
			report.Filters.Add(new HotlineLocationFilter(model.CenterIds) { Visible = false });
			if (model.Provider == Provider.SA) {
				if (model.GenderIds != null)
					report.Filters.Add(new HotlineSexFilter(model.GenderIds));
				if (model.MinimumAge != null || model.MaximumAge != null)
					report.Filters.Add(new HotlineAgeFilter(model.MinimumAge, model.MaximumAge));
				/* Removed because Lookups.RaceHUD in UI cannot be passed to Lookups.Race here. */
				//if (model.RaceIds != null)
				//	report.Filters2.Add(new HotlineRaceFilter(model.RaceIds));
			}
			if (model.CityOrTowns != null || model.Townships != null || model.CountyIds != null || model.Zipcodes != null)
				report.Filters.Add(new HotlineTwnTshipCountyFilter(model.CityOrTowns, model.Townships, model.CountyIds, model.Zipcodes));
			if (model.SvIds != null)
				report.Filters.Add(new HotlineStaffFilter(model.SvIds));
			if (model.FundingSourceIds != null)
				report.Filters.Add(new HotlineFundingSourceFilter(model.FundingSourceIds));
		}
		#endregion
	}
}