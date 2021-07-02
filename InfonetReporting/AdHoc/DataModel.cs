using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;

namespace Infonet.Reporting.AdHoc {
	public static class DataModel {
		#region constants
		private static readonly Condition[] _NoConditions = Array.Empty<Condition>();
		private const string AGENCY_OPTIONS_SQL = "SELECT a.agencyId, a.agencyName, CASE WHEN c.centerId = 0 THEN NULL ELSE c.centerName END FROM T_Agency a JOIN LookupList_ItemAssignment i ON i.tableId = 48 AND i.codeId = a.agencyId JOIN T_Center c ON c.centerId = a.centerId WHERE i.isActive = 1 AND (c.centerId = 0 OR c.parentCenterId = @centerId) AND (i.providerId = 0 OR i.providerId = @providerId) ORDER BY 3, i.displayOrder, 2, 1";
		private const string AGENCY_SELECT_SQL = "(SELECT agencyName FROM T_Agency WHERE agencyId = {@expression})";
		private const string CONTACT_OPTIONS_SQL = "SELECT co.contactId, co.contactName, CASE WHEN c.centerId = 0 THEN NULL ELSE c.centerName END FROM T_Contact co JOIN LookupList_ItemAssignment i ON i.tableId = 49 AND i.codeId = co.contactId JOIN T_Center c ON c.centerId = co.centerId WHERE i.isActive = 1 AND (c.centerId = 0 OR c.parentCenterId = @centerId) AND (i.providerId = 0 OR i.providerId = @providerId) ORDER BY 3, i.displayOrder, 2, 1";
		private const string CONTACT_SELECT_SQL = "(SELECT contactName FROM T_Contact WHERE contactId = {@expression})";
		private const string FUNDINGDATE_OPTIONS_SQL = "SELECT fd.fundDateId, CONVERT(VARCHAR(10), fd.fundingDate, 101), c.centerName FROM T_FundingDates fd JOIN T_Center c ON c.centerId = fd.centerId WHERE c.parentCenterId = @centerId ORDER BY c.centerName, fd.fundingDate desc";
		private const string FUNDINGDATE_SELECT_SQL = "(SELECT fundingDate FROM T_FundingDates WHERE fundDateId = {@expression})";
		private const string FUNDINGSOURCE_OPTIONS_SQL = "SELECT fs.codeId, fs.description, CASE WHEN c.centerId = 0 THEN NULL ELSE c.centerName END FROM Tlu_Codes_FundingSource fs JOIN LookupList_ItemAssignment i ON i.tableId = 16 AND i.codeId = fs.codeId JOIN T_Center c on c.centerId = fs.centerId WHERE i.isActive = 1 AND (c.centerId = 0 OR c.parentCenterId = @centerId) AND (i.providerId = 0 OR i.providerId = @providerId) AND (fs.beginDate IS NULL OR fs.beginDate <= @effectiveDate) AND (fs.endDate IS NULL OR fs.endDate >= @effectiveDate) ORDER BY 3, i.displayOrder, 2, 1";
		private const string FUNDINGSOURCE_SELECT_SQL = "(SELECT description FROM Tlu_Codes_FundingSource WHERE codeId = {@expression})";
		private const string LOCATION_OPTIONS_SQL = "SELECT centerId, centerName FROM T_Center WHERE parentCenterId = @centerId ORDER BY 2, 1";
		private const string LOCATION_SELECT_SQL = "(SELECT centerName FROM T_Center WHERE centerId = {@expression})";
		private const string OTHERACTIVITY_OPTIONS_SQL = "SELECT osa.codeId, osa.description, CASE WHEN c.centerId = 0 THEN NULL ELSE c.centerName END FROM Tlu_Codes_OtherStaffActivity osa JOIN LookupList_ItemAssignment i ON i.tableId = 16 AND i.codeId = osa.codeId JOIN T_Center c on c.centerId = osa.centerId WHERE i.isActive = 1 AND (c.centerId = 0 OR c.parentCenterId = @centerId) AND (i.providerId = 0 OR i.providerId = @providerId) ORDER BY 3, i.displayOrder, 2, 1";
		private const string OTHERACTIVITY_SELECT_SQL = "(SELECT description FROM Tlu_Codes_OtherStaffActivity WHERE codeId = {@expression})";
		private const string RESPONDENT_SELECT_SQL = "CASE WHEN {@expression} = 1 THEN 'Non Offending Caretaker' WHEN {@expression} = 2 THEN 'Offender' ELSE CAST({@expression} AS VARCHAR) END";
		private static readonly Option[] _RespondentOptions = { new Option { Value = 1, Label = "Non Offending Caretaker" }, new Option { Value = 2, Label = "Offender" } };
		private const string STAFF_OPTIONS_SQL = "SELECT sv.svId, CONCAT(NULLIF(CONCAT(sv.FirstName, ' '), ' '), sv.LastName), c.centerName FROM T_StaffVolunteer sv JOIN T_Center c ON c.centerId = sv.centerId WHERE (sv.StartDate IS NULL OR sv.StartDate <= @effectiveDate) AND (sv.TerminationDate IS NULL OR sv.TerminationDate >= @effectiveDate) AND c.parentCenterId = @centerId ORDER BY 3, 2, 1";
		private const string STAFF_SELECT_SQL = "(SELECT CONCAT(NULLIF(CONCAT(FirstName, ' '), ' '), LastName) FROM T_StaffVolunteer WHERE svId = {@expression})";
		#endregion

		private static readonly ProviderMap<OrdinalEnumMap<Perspective, Model>> _ByProviderAndPerspective = new ProviderMap<OrdinalEnumMap<Perspective, Model>>(
			new Dictionary<Provider, OrdinalEnumMap<Perspective, Model>> {
				[Provider.DV] = new OrdinalEnumMap<Perspective, Model>(
					new Dictionary<Perspective, Model> {
						[Perspective.Center] = CreateCenterPerspective(Provider.DV),
						[Perspective.Client] = CreateClientPerspective(Provider.DV),
						[Perspective.Staff] = CreateStaffPerspective(Provider.DV)
					}
				),
				[Provider.SA] = new OrdinalEnumMap<Perspective, Model>(
					new Dictionary<Perspective, Model> {
						[Perspective.Center] = CreateCenterPerspective(Provider.SA),
						[Perspective.Client] = CreateClientPerspective(Provider.SA),
						[Perspective.Staff] = CreateStaffPerspective(Provider.SA)
					}
				),
				[Provider.CAC] = new OrdinalEnumMap<Perspective, Model>(
					new Dictionary<Perspective, Model> {
						[Perspective.Center] = CreateCenterPerspective(Provider.CAC),
						[Perspective.Client] = CreateClientPerspective(Provider.CAC),
						[Perspective.Staff] = CreateStaffPerspective(Provider.CAC)
					}
				)
			}
		);

		public static Model For(Provider provider, Perspective perspective) {
			return _ByProviderAndPerspective[provider][perspective];
		}

		public static IEnumerable<Perspective> PerspectivesFor(Provider provider) {
			return _ByProviderAndPerspective[provider].Where(kv => kv.Value != null).Select(kv => kv.Key);
		}

		private static Model CreateCenterPerspective(Provider provider) {
			var perspective = Perspective.Center;
			var model = new Model();

			#region center
			var center = AddCenter(model, provider);
			#endregion

			#region groupService +Staff +StaffFunding +Client +ClientTown
			var groupService = AddGroupService(model, provider, perspective);
			var groupServiceStaff = AddGroupServiceStaff(model, perspective);
			groupServiceStaff.RequiredEntityIds.Add("groupService");
			var groupServiceStaffFunding = AddFunding(model, "groupServiceStaffFunding", "Group Service > Staff > Funding");
			groupServiceStaffFunding.RequiredEntityIds.Add("groupService");
			var groupServiceClient = AddGroupServiceClient(model, provider, perspective);
			var groupServiceClientTown = AddTown(model, provider, "groupServiceClientTown", "Group Service > Attendee > Residence");
			groupServiceClientTown.RequiredEntityIds.Add("groupServiceClient");
			groupService.RelateOneTo(Cardinal.ZeroOrMany, groupServiceStaff, "{groupService}.ics_id = {groupServiceStaff}.ics_id");
			//KMS DO join below won't work if required entities aren't joined prior
			groupServiceStaff.RelateOneTo(Cardinal.ZeroOrMany, groupServiceStaffFunding, "groupService.fundDateId = {groupServiceStaffFunding}.fundDateId AND {groupServiceStaff}.svId = {groupServiceStaffFunding}.svId AND groupService.programId = {groupServiceStaffFunding}.serviceProgramId");
			groupService.RelateOneTo(Cardinal.ZeroOrMany, groupServiceClient, "{groupService}.ics_id = {groupServiceClient}.ics_id");
			groupServiceClient.RelateOneTo(Cardinal.ZeroOrOne, groupServiceClientTown, "{groupServiceClient}.cityTownTownshpId = {groupServiceClientTown}.locId");
			center.RelateOneTo(Cardinal.ZeroOrMany, groupService, "{center}.centerId = {groupService}.centerId");
			#endregion

			#region communityService +Staff +StaffFunding
			var communityService = AddCommunityService(model, provider, perspective);
			var communityServiceStaff = AddCommunityServiceStaff(model, perspective);
			communityServiceStaff.RequiredEntityIds.Add("communityService");
			var communityServiceStaffFunding = AddFunding(model, "communityServiceStaffFunding", "Community/Institutional Service > Staff > Funding");
			communityServiceStaffFunding.RequiredEntityIds.Add("communityService");
			communityService.RelateOneTo(Cardinal.ZeroOrMany, communityServiceStaff, "{communityService}.ics_id = {communityServiceStaff}.ics_id");
			//KMS DO join below won't work if required entities aren't joined prior
			communityServiceStaff.RelateOneTo(Cardinal.ZeroOrMany, communityServiceStaffFunding, "communityService.fundDateId = {communityServiceStaffFunding}.fundDateId AND {communityServiceStaff}.svId = {communityServiceStaffFunding}.svId AND communityService.programId = {communityServiceStaffFunding}.serviceProgramId");
			center.RelateOneTo(Cardinal.ZeroOrMany, communityService, "{center}.centerId = {communityService}.centerId");
			#endregion

			#region publication +Staff +StaffFunding
			var publication = AddPublication(model, provider, perspective);
			var publicationStaff = AddPublicationStaff(model, provider, perspective);
			publicationStaff.RequiredEntityIds.Add("publication");
			var publicationStaffFunding = AddFunding(model, "publicationStaffFunding", "Media/Publication > Staff > Funding");
			publicationStaffFunding.RequiredEntityIds.Add("publication");
			publication.RelateOneTo(Cardinal.ZeroOrMany, publicationStaff, "{publication}.ics_id = {publicationStaff}.ics_id");
			//KMS DO join below won't work if required entities aren't joined prior
			publicationStaff.RelateOneTo(Cardinal.ZeroOrMany, publicationStaffFunding, "publication.fundDateId = {publicationStaffFunding}.fundDateId AND {publicationStaff}.svId = {publicationStaffFunding}.svId AND publication.programId = {publicationStaffFunding}.serviceProgramId");
			center.RelateOneTo(Cardinal.ZeroOrMany, publication, "{center}.centerId = {publication}.centerId");
			#endregion

			if (provider == Provider.SA) {
				#region event +Staff +StaffFunding
				var @event = AddEvent(model, provider, perspective);
				var eventStaff = AddEventStaff(model, perspective);
				eventStaff.RequiredEntityIds.Add("event");
				var eventStaffFunding = AddFunding(model, "eventStaffFunding", "Event > Staff > Funding");
				eventStaffFunding.RequiredEntityIds.Add("event");
				@event.RelateOneTo(Cardinal.ZeroOrMany, eventStaff, "{event}.ics_id = {eventStaff}.ics_id");
				//KMS DO join below won't work if required entities aren't joined prior
				eventStaff.RelateOneTo(Cardinal.ZeroOrMany, eventStaffFunding, "event.fundDateId = {eventStaffFunding}.fundDateId AND {eventStaff}.svId = {eventStaffFunding}.svId AND event.programId = {eventStaffFunding}.serviceProgramId");
				center.RelateOneTo(Cardinal.ZeroOrMany, @event, "{center}.centerId = {event}.centerId");
				#endregion
			}

			if (provider != Provider.CAC) {
				#region hotline +Funding
				var hotline = AddHotline(model, provider, perspective);
				var hotlineFunding = AddFunding(model, "hotlineFunding", (provider == Provider.SA ? "Non-Client Crisis Intervention" : "Hotline Call") + " > Funding");
				hotlineFunding.RequiredEntityIds.Add("hotline");
				hotline.RelateOneTo(Cardinal.ZeroOrMany, hotlineFunding, "{hotline}.fundDateId = {hotlineFunding}.fundDateId AND {hotline}.svId = {hotlineFunding}.svId AND {hotlineFunding}.serviceProgramId = " + Lookups.HotlineService[provider].Single().CodeId);
				center.RelateOneTo(Cardinal.ZeroOrMany, hotline, "{center}.centerId = {hotline}.centerId");
				#endregion
			}

			if (provider == Provider.DV) {
				#region turnAway
				var turnAway = AddTurnAway(model, provider, perspective);
				center.RelateOneTo(Cardinal.ZeroOrMany, turnAway, "{center}.centerId = {turnAway}.locationId");
				#endregion

				#region outcome
				var outcome = AddOutcome(model, provider, perspective);
				center.RelateOneTo(Cardinal.ZeroOrMany, outcome, "{center}.centerId = {outcome}.locationId");
				#endregion
			}

			#region directService +Town +Funding
			var directService = AddDirectService(model, provider, perspective);
			var directServiceTown = AddTown(model, provider, "directServiceTown", "Direct Service > Residence");
			directServiceTown.RequiredEntityIds.Add("directService");
			var directServiceFunding = AddFunding(model, "directServiceFunding", "Direct Service > Funding");
			directServiceFunding.RequiredEntityIds.Add("directService");
			directService.RelateOneTo(Cardinal.ZeroOrOne, directServiceTown, "{directService}.cityTownTownshpId = {directServiceTown}.locId");
			directService.RelateOneTo(Cardinal.ZeroOrMany, directServiceFunding, "{directService}.fundDateId = {directServiceFunding}.fundDateId AND {directService}.svId = {directServiceFunding}.svId AND {directService}.serviceId = {directServiceFunding}.serviceProgramId");
			center.RelateOneTo(Cardinal.ZeroOrMany, directService, "{center}.centerId = {directService}.locationId");
			#endregion

			if (provider == Provider.DV) {
				#region housingService +Town
				var housingService = AddHousingService(model, provider, perspective);
				var housingServiceTown = AddTown(model, provider, "housingServiceTown", "Housing Service > Residence");
				housingServiceTown.RequiredEntityIds.Add("housingService");
				housingService.RelateOneTo(Cardinal.ZeroOrOne, housingServiceTown, "{housingService}.cityTownTownshpId = {housingServiceTown}.locId");
				center.RelateOneTo(Cardinal.ZeroOrMany, housingService, "{center}.centerId = {housingService}.locationId");
				#endregion
			}

			if (provider == Provider.CAC) {
				#region referralDetail +Town
				var referralDetail = AddReferralDetail(model, provider, perspective);
				var referralDetailTown = AddTown(model, provider, "referralDetailTown", "Referral > Residence");
				referralDetailTown.RequiredEntityIds.Add("referralDetail");
				referralDetail.RelateOneTo(Cardinal.ZeroOrOne, referralDetailTown, "{referralDetail}.cityTownTownshpId = {referralDetailTown}.locId");
				center.RelateOneTo(Cardinal.ZeroOrMany, referralDetail, "{center}.centerId = {referralDetail}.locationId");
				#endregion
			} else {
				#region cancellation
				var cancellation = AddCancellation(model, provider, perspective);
				center.RelateOneTo(Cardinal.ZeroOrMany, cancellation, "{center}.centerId = {cancellation}.locationId");
				#endregion
			}

			#region investigation +Client
			var investigation = AddInvestigation(model, provider);
			var investigationClient = AddInvestigationClient(model, provider, perspective);
			investigationClient.RequiredEntityIds.Add("investigation");
			investigation.RelateOneTo(Cardinal.ZeroOrMany, investigationClient, "{investigation}.id = {investigationClient}.t_cacInvestigations_fk");
			center.RelateOneTo(Cardinal.ZeroOrMany, investigation, "{center}.centerId = {investigation}.centerId");
			#endregion

			if (provider == Provider.DV) {
				#region hms
				var hms = AddHms(model, provider, perspective);
				center.RelateOneTo(Cardinal.ZeroOrMany, hms, "{center}.centerId = {hms}.locationId");
				#endregion
			}

			return model;
		}

		private static Model CreateClientPerspective(Provider provider) {
			var perspective = Perspective.Client;
			var model = new Model();

			#region client
			var client = AddClient(model, provider);
			#endregion

			if (provider != Provider.CAC) {
				#region clientRace
				var clientRace = AddClientRace(model, provider);
				clientRace.RequiredEntityIds.Add("client");
				client.RelateOneTo(Cardinal.Many, clientRace, "{client}.clientId = {clientRace}.clientId");
				#endregion
			}

			#region town
			var town = AddTown(model, provider, "town", "Residence"); //KMS DO restrict this to only current?
			town.RequiredEntityIds.Add("client");
			client.RelateOneTo(Cardinal.ZeroOrMany, town, "{client}.clientId = {town}.clientId");
			#endregion

			#region clientCase
			var clientCase = AddClientCase(model, provider);
			clientCase.RequiredEntityIds.Add("client");
			client.RelateOneTo(provider == Provider.SA ? Cardinal.One : Cardinal.Many, clientCase, "{client}.clientId = {clientCase}.clientId");
			#endregion

			#region presentingIssue
			var presentingIssue = AddPresentingIssue(model, provider);
			presentingIssue.RequiredEntityIds.Add("client");
			clientCase.RelateOneTo(Cardinal.ZeroOrOne, presentingIssue, "{clientCase}.clientId = {presentingIssue}.clientId AND {clientCase}.caseId = {presentingIssue}.caseId");
			#endregion

			if (provider == Provider.DV) {
				#region financialResource
				var financialResource = AddFinancialResource(model, provider);
				financialResource.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrMany, financialResource, "{clientCase}.clientId = {financialResource}.clientId AND {clientCase}.caseId = {financialResource}.caseId");
				#endregion
			}

			if (provider == Provider.SA) {
				#region income
				var income = AddIncome(model, provider);
				income.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrOne, income, "{clientCase}.clientId = {income}.clientId AND {clientCase}.caseId = {income}.caseId");
				#endregion
			}

			if (provider == Provider.DV) {
				#region benefit
				var benefit = AddBenefit(model);
				benefit.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrOne, benefit, "{clientCase}.clientId = {benefit}.clientId AND {clientCase}.caseId = {benefit}.caseId");
				#endregion
			}

			#region referredBy
			var referredBy = AddReferredBy(model, provider);
			referredBy.RequiredEntityIds.Add("client");
			clientCase.RelateOneTo(Cardinal.ZeroOrOne, referredBy, "{clientCase}.clientId = {referredBy}.clientId AND {clientCase}.caseId = {referredBy}.caseId");
			#endregion

			if (provider == Provider.DV) {
				#region referredTo
				var referredTo = AddReferredTo(model);
				referredTo.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrOne, referredTo, "{clientCase}.clientId = {referredTo}.clientId AND {clientCase}.caseId = {referredTo}.caseId");
				#endregion
			}

			#region disability
			var disability = AddDisability(model, provider);
			disability.RequiredEntityIds.Add("client");
			clientCase.RelateOneTo(Cardinal.ZeroOrOne, disability, "{clientCase}.clientId = {disability}.clientId AND {clientCase}.caseId = {disability}.caseId");
			#endregion

			if (provider == Provider.DV) {
				#region serviceNeed
				var serviceNeed = AddServiceNeed(model);
				serviceNeed.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrOne, serviceNeed, "{clientCase}.clientId = {serviceNeed}.clientId AND {clientCase}.caseId = {serviceNeed}.caseId");
				#endregion

				#region serviceGot
				var serviceGot = AddServiceGot(model);
				serviceGot.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrOne, serviceGot, "{clientCase}.clientId = {serviceGot}.clientId AND {clientCase}.caseId = {serviceGot}.caseId");
				#endregion

				#region behaviorIn
				var behaviorIn = AddBehaviorIn(model);
				behaviorIn.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrOne, behaviorIn, "{clientCase}.clientId = {behaviorIn}.clientId AND {clientCase}.caseId = {behaviorIn}.caseId");
				#endregion

				#region behaviorOut
				var behaviorOut = AddBehaviorOut(model);
				behaviorOut.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrOne, behaviorOut, "{clientCase}.clientId = {behaviorOut}.clientId AND {clientCase}.caseId = {behaviorOut}.caseId");
				#endregion

				#region previousService
				var previousService = AddPreviousService(model, provider);
				previousService.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrOne, previousService, "{clientCase}.clientId = {previousService}.clientId AND {clientCase}.caseId = {previousService}.caseId");
				#endregion
			}

			if (provider != Provider.CAC) {
				#region policeProsecution
				var policeProsecution = AddPoliceProsecution(model, provider);
				policeProsecution.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrOne, policeProsecution, "{clientCase}.clientId = {policeProsecution}.clientId AND {clientCase}.caseId = {policeProsecution}.caseId");
				#endregion
			}

			if (provider == Provider.DV) {
				#region conflictScale
				var conflictScale = AddConflictScale(model);
				conflictScale.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrOne, conflictScale, "{clientCase}.clientId = {conflictScale}.clientId AND {clientCase}.caseId = {conflictScale}.caseId");
				#endregion
			}

			if (provider != Provider.CAC) {
				#region courtAppearance
				var courtAppearance = AddCourtAppearance(model, provider);
				courtAppearance.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrMany, courtAppearance, "{clientCase}.clientId = {courtAppearance}.clientId AND {clientCase}.caseId = {courtAppearance}.caseId");
				#endregion
			}

			#region medicalVisit
			var medicalVisit = AddMedicalVisit(model, provider);
			medicalVisit.RequiredEntityIds.Add("client");
			clientCase.RelateOneTo(provider == Provider.CAC ? Cardinal.ZeroOrMany : Cardinal.ZeroOrOne, medicalVisit, "{clientCase}.clientId = {medicalVisit}.clientId AND {clientCase}.caseId = {medicalVisit}.caseId");
			#endregion

			if (provider != Provider.CAC) {
				#region protection
				var protection = AddProtection(model, provider);
				protection.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(provider == Provider.DV ? Cardinal.ZeroOrMany : Cardinal.ZeroOrOne, protection, "{clientCase}.clientId = {protection}.clientId AND {clientCase}.caseId = {protection}.caseId");
				#endregion

				if (provider == Provider.DV) {
					#region protectionActivity
					var protectionActivity = AddProtectionActivity(model, provider);
					protectionActivity.RequiredEntityIds.Add("client");
					protection.RelateOneTo(Cardinal.ZeroOrMany, protectionActivity, "{protection}.op_id = {protectionActivity}.op_id");
					#endregion
				}
			}

			if (provider == Provider.CAC) {
				#region mdt
				var mdt = AddMdt(model, provider);
				mdt.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrMany, mdt, "{clientCase}.clientId = {mdt}.clientId AND {clientCase}.caseId = {mdt}.caseId");
				#endregion
			}

			if (provider == Provider.CAC) {
				#region allegation +Respondent
				var allegation = AddAllegation(model, provider);
				allegation.RequiredEntityIds.Add("client");
				var allegationRespondent = AddAllegationRespondent(model);
				allegationRespondent.RequiredEntityIds.Add("client");
				allegation.RelateOneTo(Cardinal.Many, allegationRespondent, "{allegation}.id = {allegationRespondent}.dcfsAllegations_fk");
				clientCase.RelateOneTo(Cardinal.ZeroOrMany, allegation, "{clientCase}.clientId = {allegation}.clientId AND {clientCase}.caseId = {allegation}.caseId");
				#endregion

				#region petition +Respondent
				var petition = AddPetition(model, provider);
				petition.RequiredEntityIds.Add("client");
				var petitionRespondent = AddPetitionRespondent(model);
				petitionRespondent.RequiredEntityIds.Add("client");
				petition.RelateOneTo(Cardinal.Many, petitionRespondent, "{petition}.id = {petitionRespondent}.abuseNeglectPetitions_fk");
				clientCase.RelateOneTo(Cardinal.ZeroOrMany, petition, "{clientCase}.clientId = {petition}.clientId AND {clientCase}.caseId = {petition}.caseId");
				#endregion

				#region interview +Observer
				var interview = AddInterview(model, provider);
				var interviewObserver = AddInterviewObserver(model, provider);
				interviewObserver.RequiredEntityIds.Add("interview");
				interview.RelateOneTo(Cardinal.ZeroOrMany, interviewObserver, "{interview}.vsi_id = {interviewObserver}.vsi_id");
				clientCase.RelateOneTo(Cardinal.ZeroOrMany, interview, "{clientCase}.clientId = {interview}.clientId AND {clientCase}.caseId = {interview}.caseId");
				#endregion
			}

			#region offender
			var offender = AddOffender(model, provider);
			var policeCharge = AddPoliceCharge(model, provider);
			policeCharge.RequiredEntityIds.Add("offender");
			var trialCharge = AddTrialCharge(model, provider);
			trialCharge.RequiredEntityIds.Add("offender");
			var sentence = AddSentence(model, provider);
			sentence.RequiredEntityIds.Add("offender");
			trialCharge.RelateOneTo(Cardinal.ZeroOrMany, sentence, "{trialCharge}.tc_id = {sentence}.tc_id");
			offender.RelateOneTo(Cardinal.ZeroOrMany, policeCharge, "{offender}.offenderRecordId = {policeCharge}.offenderRecordId");
			offender.RelateOneTo(Cardinal.ZeroOrMany, trialCharge, "{offender}.offenderRecordId = {trialCharge}.offenderRecordId");
			clientCase.RelateOneTo(Cardinal.ZeroOrMany, offender, "{clientCase}.clientId = {offender}.clientId AND {clientCase}.caseId = {offender}.caseId");
			#endregion

			#region directService +Town +Funding
			var directService = AddDirectService(model, provider, perspective);
			var directServiceTown = AddTown(model, provider, "directServiceTown", "Direct Service > Residence");
			directServiceTown.RequiredEntityIds.Add("directService");
			var directServiceFunding = AddFunding(model, "directServiceFunding", "Direct Service > Funding");
			directServiceFunding.RequiredEntityIds.Add("directService");
			directService.RelateOneTo(Cardinal.ZeroOrOne, directServiceTown, "{directService}.cityTownTownshpId = {directServiceTown}.locId");
			directService.RelateOneTo(Cardinal.ZeroOrMany, directServiceFunding, "{directService}.fundDateId = {directServiceFunding}.fundDateId AND {directService}.svId = {directServiceFunding}.svId AND {directService}.serviceId = {directServiceFunding}.serviceProgramId");
			clientCase.RelateOneTo(Cardinal.ZeroOrMany, directService, "{clientCase}.clientId = {directService}.clientId AND {clientCase}.caseId = {directService}.caseId");
			#endregion

			if (provider == Provider.DV) {
				#region housingService +Town
				var housingService = AddHousingService(model, provider, perspective);
				var housingServiceTown = AddTown(model, provider, "housingServiceTown", "Housing Service > Residence");
				housingServiceTown.RequiredEntityIds.Add("housingService");
				housingService.RelateOneTo(Cardinal.ZeroOrOne, housingServiceTown, "{housingService}.cityTownTownshpId = {housingServiceTown}.locId");
				clientCase.RelateOneTo(Cardinal.ZeroOrMany, housingService, "{clientCase}.clientId = {housingService}.clientId AND {clientCase}.caseId = {housingService}.caseId");
				#endregion
			}

			if (provider == Provider.CAC) {
				#region referralDetail +Town
				var referralDetail = AddReferralDetail(model, provider, perspective);
				var referralDetailTown = AddTown(model, provider, "referralDetailTown", "Referral > Residence");
				referralDetailTown.RequiredEntityIds.Add("referralDetail");
				referralDetail.RelateOneTo(Cardinal.ZeroOrOne, referralDetailTown, "{referralDetail}.cityTownTownshpId = {referralDetailTown}.locId");
				clientCase.RelateOneTo(Cardinal.ZeroOrMany, referralDetail, "{clientCase}.clientId = {referralDetail}.clientId AND {clientCase}.caseId = {referralDetail}.caseId");
				#endregion
			}

			if (provider == Provider.DV) {
				#region departure
				var departure = AddDepature(model, provider);
				departure.RequiredEntityIds.Add("client");
				clientCase.RelateOneTo(Cardinal.ZeroOrMany, departure, "{clientCase}.clientId = {departure}.clientId AND {clientCase}.caseId = {departure}.caseId");
				#endregion
			}

			if (provider != Provider.CAC) {
				#region cancellation
				var cancellation = AddCancellation(model, provider, perspective);
				clientCase.RelateOneTo(Cardinal.ZeroOrMany, cancellation, "{clientCase}.clientId = {cancellation}.clientId AND {clientCase}.caseId = {cancellation}.caseId");
				#endregion
			}

			#region investigation +Client
			var investigation = AddInvestigation(model, provider);
			var investigationClient = AddInvestigationClient(model, provider, perspective);
			investigationClient.RequiredEntityIds.Add("investigation");
			investigation.RelateOneTo(Cardinal.Many /*intentional*/, investigationClient, "{investigation}.id = {investigationClient}.t_cacInvestigations_fk");
			clientCase.RelateOneTo(Cardinal.ZeroOrMany, investigationClient, "{clientCase}.clientId = {investigationClient}.clientId AND {clientCase}.caseId = {investigationClient}.caseId");
			#endregion

			#region groupService +Staff +StaffFunding +Client +ClientTown
			var groupService = AddGroupService(model, provider, perspective);
			var groupServiceStaff = AddGroupServiceStaff(model, perspective);
			groupServiceStaff.RequiredEntityIds.Add("groupService");
			var groupServiceStaffFunding = AddFunding(model, "groupServiceStaffFunding", "Group Service > Staff > Funding");
			groupServiceStaffFunding.RequiredEntityIds.Add("groupService");
			var groupServiceClient = AddGroupServiceClient(model, provider, perspective);
			var groupServiceClientTown = AddTown(model, provider, "groupServiceClientTown", "Group Service > Attendee > Residence");
			groupServiceClientTown.RequiredEntityIds.Add("groupServiceClient");
			groupService.RelateOneTo(Cardinal.ZeroOrMany, groupServiceStaff, "{groupService}.ics_id = {groupServiceStaff}.ics_id");
			//KMS DO join below won't work if required entities aren't joined prior
			groupServiceStaff.RelateOneTo(Cardinal.ZeroOrMany, groupServiceStaffFunding, "groupService.fundDateId = {groupServiceStaffFunding}.fundDateId AND {groupServiceStaff}.svId = {groupServiceStaffFunding}.svId AND groupService.programId = {groupServiceStaffFunding}.serviceProgramId");
			groupService.RelateOneTo(Cardinal.Many /*intentional*/, groupServiceClient, "{groupService}.ics_id = {groupServiceClient}.ics_id");
			groupServiceClient.RelateOneTo(Cardinal.ZeroOrOne, groupServiceClientTown, "{groupServiceClient}.cityTownTownshpId = {groupServiceClientTown}.locId");
			clientCase.RelateOneTo(Cardinal.ZeroOrMany, groupServiceClient, "{clientCase}.clientId = {groupServiceClient}.clientId AND {clientCase}.caseId = {groupServiceClient}.caseId");
			#endregion

			return model;
		}

		private static Model CreateStaffPerspective(Provider provider) {
			var perspective = Perspective.Staff;
			var model = new Model();

			#region staff
			var staff = AddStaff(model, provider, perspective);
			#endregion

			#region center
			var center = AddCenter(model, provider);
			center.RelateOneTo(Cardinal.Many, staff, "{center}.centerId = {staff}.centerId");
			#endregion

			#region groupService +Staff +StaffFunding +Client +ClientTown
			var groupService = AddGroupService(model, provider, perspective);
			var groupServiceStaff = AddGroupServiceStaff(model, perspective);
			groupServiceStaff.RequiredEntityIds.Add("groupService");
			var groupServiceStaffFunding = AddFunding(model, "groupServiceStaffFunding", "Group Service > Staff > Funding");
			groupServiceStaffFunding.RequiredEntityIds.Add("groupService");
			var groupServiceClient = AddGroupServiceClient(model, provider, perspective);
			var groupServiceClientTown = AddTown(model, provider, "groupServiceClientTown", "Group Service > Attendee > Residence");
			groupServiceClientTown.RequiredEntityIds.Add("groupServiceClient");
			groupService.RelateOneTo(Cardinal.Many /*intentional*/, groupServiceStaff, "{groupService}.ics_id = {groupServiceStaff}.ics_id");
			//KMS DO join below won't work if required entities aren't joined prior
			groupServiceStaff.RelateOneTo(Cardinal.ZeroOrMany, groupServiceStaffFunding, "groupService.fundDateId = {groupServiceStaffFunding}.fundDateId AND {groupServiceStaff}.svId = {groupServiceStaffFunding}.svId AND groupService.programId = {groupServiceStaffFunding}.serviceProgramId");
			groupService.RelateOneTo(Cardinal.ZeroOrMany, groupServiceClient, "{groupService}.ics_id = {groupServiceClient}.ics_id");
			groupServiceClient.RelateOneTo(Cardinal.ZeroOrOne, groupServiceClientTown, "{groupServiceClient}.cityTownTownshpId = {groupServiceClientTown}.locId");
			staff.RelateOneTo(Cardinal.ZeroOrMany, groupServiceStaff, "{staff}.svId = {groupServiceStaff}.svId");
			#endregion

			#region communityService +Staff +StaffFunding
			var communityService = AddCommunityService(model, provider, perspective);
			var communityServiceStaff = AddCommunityServiceStaff(model, perspective);
			communityServiceStaff.RequiredEntityIds.Add("communityService");
			var communityServiceStaffFunding = AddFunding(model, "communityServiceStaffFunding", "Community/Institutional Service > Staff > Funding");
			communityServiceStaffFunding.RequiredEntityIds.Add("communityService");
			communityService.RelateOneTo(Cardinal.Many /*intentional*/, communityServiceStaff, "{communityService}.ics_id = {communityServiceStaff}.ics_id");
			//KMS DO join below won't work if required entities aren't joined prior
			communityServiceStaff.RelateOneTo(Cardinal.ZeroOrMany, communityServiceStaffFunding, "communityService.fundDateId = {communityServiceStaffFunding}.fundDateId AND {communityServiceStaff}.svId = {communityServiceStaffFunding}.svId AND communityService.programId = {communityServiceStaffFunding}.serviceProgramId");
			staff.RelateOneTo(Cardinal.ZeroOrMany, communityServiceStaff, "{staff}.svId = {communityServiceStaff}.svId");
			#endregion

			#region publication +Staff +StaffFunding
			var publication = AddPublication(model, provider, perspective);
			var publicationStaff = AddPublicationStaff(model, provider, perspective);
			publicationStaff.RequiredEntityIds.Add("publication");
			var publicationStaffFunding = AddFunding(model, "publicationStaffFunding", "Media/Publication > Staff > Funding");
			publicationStaffFunding.RequiredEntityIds.Add("publication");
			publication.RelateOneTo(Cardinal.Many /*intentional*/, publicationStaff, "{publication}.ics_id = {publicationStaff}.ics_id");
			//KMS DO join below won't work if required entities aren't joined prior
			publicationStaff.RelateOneTo(Cardinal.ZeroOrMany, publicationStaffFunding, "publication.fundDateId = {publicationStaffFunding}.fundDateId AND {publicationStaff}.svId = {publicationStaffFunding}.svId AND publication.programId = {publicationStaffFunding}.serviceProgramId");
			staff.RelateOneTo(Cardinal.ZeroOrMany, publicationStaff, "{staff}.svId = {publicationStaff}.svId");
			#endregion

			if (provider == Provider.SA) {
				#region event +Staff +StaffFunding
				var @event = AddEvent(model, provider, perspective);
				var eventStaff = AddEventStaff(model, perspective);
				eventStaff.RequiredEntityIds.Add("event");
				var eventStaffFunding = AddFunding(model, "eventStaffFunding", "Event > Staff > Funding");
				eventStaffFunding.RequiredEntityIds.Add("event");
				@event.RelateOneTo(Cardinal.Many /*intentional*/, eventStaff, "{event}.ics_id = {eventStaff}.ics_id");
				//KMS DO join below won't work if required entities aren't joined prior
				eventStaff.RelateOneTo(Cardinal.ZeroOrMany, eventStaffFunding, "event.fundDateId = {eventStaffFunding}.fundDateId AND {eventStaff}.svId = {eventStaffFunding}.svId AND event.programId = {eventStaffFunding}.serviceProgramId");
				staff.RelateOneTo(Cardinal.ZeroOrMany, eventStaff, "{staff}.svId = {eventStaff}.svId");
				#endregion
			}

			if (provider != Provider.CAC) {
				#region hotline +Funding
				var hotline = AddHotline(model, provider, perspective);
				var hotlineFunding = AddFunding(model, "hotlineFunding", (provider == Provider.SA ? "Non-Client Crisis Intervention" : "Hotline Call") + " > Funding");
				hotlineFunding.RequiredEntityIds.Add("hotline");
				hotline.RelateOneTo(Cardinal.ZeroOrMany, hotlineFunding, "{hotline}.fundDateId = {hotlineFunding}.fundDateId AND {hotline}.svId = {hotlineFunding}.svId AND {hotlineFunding}.serviceProgramId = " + Lookups.HotlineService[provider].Single().CodeId);
				staff.RelateOneTo(Cardinal.ZeroOrMany, hotline, "{staff}.svId = {hotline}.svId");
				#endregion
			}

			#region directService +Town +Funding
			var directService = AddDirectService(model, provider, perspective);
			var directServiceTown = AddTown(model, provider, "directServiceTown", "Direct Service > Residence");
			directServiceTown.RequiredEntityIds.Add("directService");
			var directServiceFunding = AddFunding(model, "directServiceFunding", "Direct Service > Funding");
			directServiceFunding.RequiredEntityIds.Add("directService");
			directService.RelateOneTo(Cardinal.ZeroOrOne, directServiceTown, "{directService}.cityTownTownshpId = {directServiceTown}.locId");
			directService.RelateOneTo(Cardinal.ZeroOrMany, directServiceFunding, "{directService}.fundDateId = {directServiceFunding}.fundDateId AND {directService}.svId = {directServiceFunding}.svId AND {directService}.serviceId = {directServiceFunding}.serviceProgramId");
			staff.RelateOneTo(Cardinal.ZeroOrMany, directService, "{staff}.svId = {directService}.svId");
			#endregion

			#region otherActivity
			var otherActivity = AddOtherActivity(model, perspective);
			otherActivity.RequiredEntityIds.Add("staff");
			staff.RelateOneTo(Cardinal.ZeroOrMany, otherActivity, "{staff}.svId = {otherActivity}.svId");
			#endregion

			if (provider != Provider.CAC) {
				#region cancellation
				var cancellation = AddCancellation(model, provider, perspective);
				staff.RelateOneTo(Cardinal.ZeroOrMany, cancellation, "{staff}.svId = {cancellation}.svId");
				#endregion
			}

			return model;
		}

		#region center entities
		private static Entity AddCenter(Model model, Provider provider) {
			var result = model.Add(new Entity("(SELECT * FROM T_{#Center} WHERE parentCenterId = @centerId)"));
			result.Key = new[] {
				result.Add(new Field("{center}.center{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new Field("{center}.center{#Name}", FieldType.NVarChar) { NotEmpty = true });
			result.Add(new Field("{center}.{#address}", FieldType.NVarChar));
			result.Add(new Field("{center}.{#zipCode}", FieldType.NVarChar));
			result.Add(new Field("(SELECT countyName FROM USPSData.dbo.Counties WHERE id = {center}.{#county}Id)", FieldType.NVarChar));
			result.Add(new Field("{center}.{#empNumber}", FieldType.Quantity) { Label = "Number of Employees (FTEs)" });
			result.Add(new Field("{center}.{#boardMemberNum}", FieldType.Quantity) { Label = "Number of Board Members" });
			result.Add(new Field("{center}.{#legisDistrict}", FieldType.NVarChar) { Label = "Legislative District" });
			result.Add(new Field("{center}.{#judicialDistrict}", FieldType.NVarChar) { Label = "Judicial District" });
			result.Add(new Field("{center}.{#serviceArea}", FieldType.NVarChar));
			result.Add(new Field("CAST({center}.{#population} AS BIGINT)", FieldType.BigQuantity));
			result.Add(new Field("{center}.{#fax}", FieldType.NVarChar) { Label = "Fax Number" });
			result.Add(new Field("{center}.{#federalEmployerId}", FieldType.NVarChar) { Label = "Federal Employer ID" });
			result.Add(new Field("{center}.{#email}", FieldType.NVarChar) { Label = "Email Address" });
			result.Add(new Field("{center}.{#telephone}", FieldType.NVarChar) { Label = "Phone Number" });
			result.Add(new Field("{center}.{#directorEmail}", FieldType.NVarChar) { Label = "Director Email Address" });
			new DateField("{center}.{#creationDate}") { NotEmpty = true }.AddTo(result, true);
			new DateField("{center}.{#terminationDate}").AddTo(result, true);
			result.Add(new LookupField("{center}.{#state}Id", Lookups.StateNames[provider]));
			result.Add(new Field("(SELECT cityName FROM USPSData.dbo.Cities WHERE id = {center}.{#city}Id)", FieldType.NVarChar));
			result.Add(new Field("{center}.{#shelter}Status", FieldType.Checkbox) { NotEmpty = true, Label = "Is Shelter?" });
			result.Add(new Field("satellite", "CASE WHEN {center}.centerId = {center}.parentCenterId THEN 0 ELSE 1 END", FieldType.Checkbox) { NotEmpty = true, Label = "Is Satellite?" });
			return result;
		}

		private static Entity AddStaff(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("(SELECT * FROM T_{#Staff}Volunteer WHERE centerId IN (SELECT centerId FROM T_Center WHERE parentCenterId = @centerId))") { Label = "Staff/Volunteer" });
			result.Key = new[] {
				result.Add(new Field("{staff}.sv{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{staff}.{#center}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			result.Add(new Field("{staff}.{#lastName}", FieldType.NVarChar) { NotEmpty = true });
			result.Add(new Field("{staff}.{#firstName}", FieldType.NVarChar) { NotEmpty = provider == Provider.CAC });
			result.Add(new LookupField("{staff}.{#sex}Id", Lookups.Sex[provider]) { Label = "Gender Identity" });
			result.Add(new LookupField("{staff}.{#race}Id", Lookups.Race[provider]) { Label = "Race/Ethnicity" });
			result.Add(new LookupField("{staff}.{#personnelType}Id", Lookups.PersonnelType[provider]));
			result.Add(new Field("{staff}.{#title}", FieldType.NVarChar));
			result.Add(new Field("{staff}.collegeUniv{#Student}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{staff}.{#department}", FieldType.NVarChar));
			result.Add(new Field("{staff}.{#workPhone}", FieldType.NVarChar));
			result.Add(new Field("{staff}.{#email}", FieldType.NVarChar));
			new DateField("{staff}.{#startDate}").AddTo(result, true);
			new DateField("{staff}.{#terminationDate}").AddTo(result, true);
			result.Add(new Field("{staff}.{#supervisor}Id", FieldType.Id, STAFF_SELECT_SQL) { OptionsSql = "SELECT DISTINCT v.supervisorId, (SELECT CONCAT(NULLIF(CONCAT(s.FirstName, ' '), ' '), s.LastName) FROM T_StaffVolunteer s WHERE s.svId = v.supervisorId), c.centerName FROM T_StaffVolunteer v JOIN T_Center c ON c.centerId = v.centerId WHERE (v.StartDate IS NULL OR v.StartDate <= @effectiveDate) AND (v.TerminationDate IS NULL OR v.TerminationDate >= @effectiveDate) AND c.parentCenterId = @centerId ORDER BY 3, 2, 1" });
			result.Add(new Field("{staff}.{#type}Id", FieldType.Id) { NotEmpty = true, Label = "Staff or Volunteer?", Formatter = o => ((StaffVolunteer.StaffType)(int)o).GetDisplayName(), Options = Enums.GetValues<StaffVolunteer.StaffType>().Select(v => new Option { Value = v.ToInt32(), Label = v.GetDisplayName() }) });
			return result;
		}

		private static Entity AddFunding(Model model, string id, string label) {
			var result = model.Add(new Entity(id, "Tl_FundServiceProgramOfStaffs") { Label = label });
			result.Key = new[] {
				new Field("{" + id + "}.{#fundDateId}", FieldType.Id) { NotEmpty = true },
				new Field("{" + id + "}.{#svId}", FieldType.Id) { NotEmpty = true },
				new Field("{" + id + "}.{#serviceProgram}Id", FieldType.Id) { NotEmpty = true },
				result.Add(new Field("{" + id + "}.{#fundingSource}Id", FieldType.Id, FUNDINGSOURCE_SELECT_SQL) { NotEmpty = true, OptionsSql = FUNDINGSOURCE_OPTIONS_SQL })
			};
			result.Add(new Field("{" + id + "}.{#percentFund}", FieldType.Percent) { NotEmpty = true, Label = "Percent" });
			return result;
		}
		#endregion

		#region client entities
		private static Entity AddClient(Model model, Provider provider) {
			var result = model.Add(new Entity("(SELECT * FROM T_{#Client} WHERE centerId = @centerId)"));
			result.Key = new[] {
				result.Add(new Field("{client}.client{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new Field("{client}.{#clientCode}", FieldType.NVarChar) { NotEmpty = true, Label = "Client ID" });
			result.Add(new LookupField("{client}.{#sex}Id", Lookups.GenderIdentity[provider]) { NotEmpty = true, Label = "Gender Identity" });
			if (provider == Provider.CAC)
				result.Add(new LookupField("{client}.{#race}Id", Lookups.Race[provider]) { NotEmpty = true, Label = "Race/Ethnicity" });
			result.Add(new LookupField("{client}.{#type}Id", Lookups.ClientType[provider]) { NotEmpty = true, Label = "Client Type" });
			if (provider != Provider.CAC)
				result.Add(new LookupField("{client}.{#ethnicity}Id", Lookups.Ethnicity[provider]) { NotEmpty = true });
			return result;
		}

		public static Entity AddClientRace(Model model, Provider provider) {
			var result = model.AddEntity("clientRace", "Ts_ClientRace", "Race/Ethnicity");
			result.Key = new[] {
				new Field("{clientRace}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				result.Add(new LookupField("{clientRace}.{#raceId}", Lookups.RaceHud[provider]) { NotEmpty = true, Label = "Race/Ethnicity" })
			};
			return result;
		}

		private static Entity AddTown(Model model, Provider provider, string id, string label) {
			var result = model.Add(new Entity(id, "Ts_TwnTshipCounty") { Label = label });
			result.Key = new[] {
				result.Add(new Field("{" + id + "}.loc{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new Field("{" + id + "}.{#cityOrTown}", FieldType.NVarChar));
			result.Add(new Field("{" + id + "}.{#township}", FieldType.NVarChar));
			result.Add(new Field("(SELECT countyName FROM USPSData.dbo.Counties WHERE id = {" + id + "}.{#county}Id)", FieldType.NVarChar));
			result.Add(new Field("{" + id + "}.{#zipCode}", FieldType.NVarChar));
			new DateField("{" + id + "}.{#moveDate}") { NotEmpty = true, Label = "Effective Date" }.AddTo(result, true);
			result.Add(new LookupField("{" + id + "}.{#state}Id", Lookups.StateNames[provider]) { NotEmpty = provider == Provider.CAC });
			if (provider == Provider.DV) {
				result.Add(new LookupField("{" + id + "}.{#residenceType}Id", Lookups.ResidenceType[provider]));
				result.Add(new LookupField("{" + id + "}.{#lengthOfStay}InResidenceId", Lookups.LengthOfStay[provider]) { Label = "Length of Stay" });
			}
			return result;
		}
		#endregion

		#region client intake entities
		private static Entity AddClientCase(Model model, Provider provider) {
			var result = model.AddEntity("clientCase", "T_ClientCases", "Client Case");
			result.Key = new[] {
				new Field("{clientCase}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{clientCase}.{#caseId}", FieldType.Sequential) { NotEmpty = true, Label = "Case ID" }
			};
			if (provider == Provider.SA) {
				result.Key[0].Label = "ID";
				result.Add(result.Key[0]);
			} else {
				result.Add(new Field("NULLIF(CONCAT({clientCase}.client{#Id}, '-', {clientCase}.caseId), '-')", FieldType.NVarCharId) { NotEmpty = true, AvailableConditions = _NoConditions });
				result.Add(result.Key[1]);
			}
			result.Add(new Field("{clientCase}.{#age}", FieldType.Quantity) { NotEmpty = true, Label = "Age at First Contact" });
			if (provider == Provider.SA)
				result.Add(new Field("{clientCase}.collegeUniv{#Student}", FieldType.Checkbox) { Label = "College/University Student" });
			result.Add(new LookupField("{clientCase}.{#employment}Id", Lookups.EmploymentType[provider]));
			if (provider != Provider.DV)
				result.Add(new LookupField("{clientCase}.{#healthInsurance}Id", Lookups.HealthInsurance[provider]));
			if (provider != Provider.CAC)
				result.Add(new LookupField("{clientCase}.{#education}Id", Lookups.Education[provider]));
			result.Add(new LookupField("{clientCase}.{#maritalStatus}Id", Lookups.MaritalStatus[provider]));
			result.Add(new LookupField("{clientCase}.{#pregnant}Id", Lookups.Pregnant[provider]));
			if (provider != Provider.SA)
				result.Add(new Field("{clientCase}.num{#Children}", FieldType.Quantity) { Label = "Number of Children" });
			new DateField("{clientCase}.{#firstContactDate}") { NotEmpty = true, Label = provider == Provider.CAC ? "CAC Case Open Date" : "First Contact Date" }.AddTo(result, true);
			result.Add(new LookupField("{clientCase}.{#caseClosedReason}Id", Lookups.CaseClosedReason[provider]) { Label = "Reason Case Closed" });
			result.Add(new Field("{clientCase}.{#caseClosed}", FieldType.Checkbox));
			new DateField("{clientCase}.{#caseClosedDate}") { Label = "Date Closed" }.AddTo(result, true);
			if (provider == Provider.SA)
				result.Add(new LookupField("{clientCase}.{#significantOtherOf}Id", Lookups.SignificantOtherOf[provider]));
			else
				result.Add(new LookupField("{clientCase}.{#custody}Id", Lookups.ChildCustody[provider]));
			if (provider == Provider.DV) {
				result.Add(new LookupField("{clientCase}.{#livesWith}Id", Lookups.ChildLivesWith[provider]));
				result.Add(new Field("{clientCase}.{#dcfsInvestigation}", FieldType.Checkbox) { Label = "DCFS Investigation" });
				result.Add(new Field("{clientCase}.{#dcfsOpen}", FieldType.Checkbox) { Label = "DCFS Open" });
				result.Add(new LookupField("{clientCase}.{#school}Id", Lookups.School[provider]));
			} else {
				result.Add(new LookupField("{clientCase}.{#relationSOToClient}Id", Lookups.RelationshipToClient[provider]) { Label = "Relationship to Victim" });
			}
			if (provider == Provider.CAC) {
				new DateField("{clientCase}.{#dcfsHotlineDate}") { Label = "DCFS Hotline Date" }.AddTo(result, true);
				new DateField("{clientCase}.{#dcfsServiceDate}") { Label = "DCFS Service Date" }.AddTo(result, true);
				result.Add(new LookupField("{clientCase}.{#investigationType}Id", Lookups.InvestigationType[provider]));
				new DateField("{clientCase}.{#policeReportDate}").AddTo(result, true);
				result.Add(new Field("{clientCase}.{#informationOnlyCase}", FieldType.Checkbox));
			}
			if (provider == Provider.DV)
				result.Add(new LookupField("{clientCase}.{#vetStatus}Id", Lookups.YesNo[provider]) { Label = "Veteran's Status" });
			if (provider != Provider.CAC)
				result.Add(new LookupField("{clientCase}.{#sexualOrientation}Id", Lookups.SexualOrientation[provider]));
			return result;
		}

		private static Entity AddPresentingIssue(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_Client{#PresentingIssue}") { Label = "Presenting Issues" });
			result.Key = new[] {
				new Field("{presentingIssue}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{presentingIssue}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new LookupField("{presentingIssue}.{#primaryPresentingIssue}Id", Lookups.PrimaryPresentingIssue[provider]));
			new DateField("{presentingIssue}.{#dateOfPrimOffense}") { Label = provider == Provider.DV ? "Primary Offense Date" : "Approximate Abuse/Offense Date (or start of abuse)" }.AddTo(result, true);
			if (provider != Provider.DV) {
				new DateField("{presentingIssue}.{#endDateOfAbuse}") { Label = "End of Abuse/Offense Date (if applicable)" }.AddTo(result, true);
				result.Add(new Field("{presentingIssue}.{#comment}", FieldType.NVarChar));
			}
			result.Add(new LookupField("{presentingIssue}.{#locOfPrimOffense}Id", Lookups.PresentingIssueLocation[provider]) { Label = "Primary Offense Location" });
			if (provider != Provider.DV)
				result.Add(new Field("(SELECT countyName FROM USPSData.dbo.Counties WHERE id = {presentingIssue}.{#county}Id)", FieldType.NVarChar));
            if (provider != Provider.CAC) {
                result.Add(new Field("CAST({presentingIssue}.{#adultSurvivor} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Adult Survivor of Incest or Sexual Assault" });
                result.Add(new Field("CAST({presentingIssue}.{#aggravatedDomesticBattery} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#assaultAndOrBattery} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Assault and/or Battery" });
                result.Add(new Field("CAST({presentingIssue}.{#attemptedHomicide} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#battery} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#burglary} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#childAbuse} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#childNeglect} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#childSexualAssault} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#dateRape} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#domesticBattery} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                if (provider == Provider.DV) { 
                    result.Add(new Field("CAST({presentingIssue}.{#drugged} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                    result.Add(new Field("CAST({presentingIssue}.{#dwiDui} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "DUI / DWI" });
                }          
                result.Add(new Field("CAST({presentingIssue}.{#elderAbuse} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#emotionalDomesticViolence} AS BIT)", FieldType.Checkbox) { NotEmpty = true });            
                if (provider == Provider.DV)
                    result.Add(new Field("CAST({presentingIssue}.{#financialAbuse} AS BIT)", FieldType.Checkbox) { NotEmpty = true });                    
            
                if (provider == Provider.DV)
                    result.Add(new Field("CAST({presentingIssue}.{#harassment} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Harassment" });
                result.Add(new Field("CAST({presentingIssue}.{#hateCrime} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#homeInvasion} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#homicide} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#humanLaborTrafficking} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#humanSexTrafficking} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#physicalDomesticViolence} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                if (provider == Provider.DV)
                    result.Add(new Field("CAST({presentingIssue}.{#rapeOrSexualAssault} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Rape or Sexual Assault" });
                result.Add(new Field("CAST({presentingIssue}.{#robbery} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                if (provider == Provider.SA)
                    result.Add(new Field("CAST({presentingIssue}.{#rapeOrSexualAssault} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Sexual Assault or Abuse" });
                result.Add(new Field("CAST({presentingIssue}.{#sexualDomesticViolence} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                if (provider == Provider.SA)
                    result.Add(new Field("CAST({presentingIssue}.{#harassment} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Sexual Harassment" });
                if (provider == Provider.DV)
                    result.Add(new Field("CAST({presentingIssue}.{#spiritualAbuse} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#stalking} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("CAST({presentingIssue}.{#violationOfOop} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Violation of Order of Protection" });
				result.Add(new Field("CAST({presentingIssue}.{#assault} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Other Assault" });
				result.Add(new Field("CAST({presentingIssue}.{#otherOffenseAgPerson} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Other Offense Against Person" });
                result.Add(new Field("CAST({presentingIssue}.{#otherOffense} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
                result.Add(new Field("CAST({presentingIssue}.{#unknownOffense} AS BIT)", FieldType.Checkbox) { NotEmpty = true });              
            }
         
			if (provider == Provider.CAC) {
				result.Add(new Field("CAST(({presentingIssue}.{#exploitation} & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Exploitation (Passive)" });
				result.Add(new Field("fondlingOverClothesPassive", "CAST(({presentingIssue}.fondingOverClothes & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Fondling - Over Clothes (Passive)" });
				result.Add(new Field("fondlingOverClothesActive", "CAST(({presentingIssue}.fondingOverClothes & 2) / 2 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Fondling - Over Clothes (Active)" });
				result.Add(new Field("fondlingUnderClothesPassive", "CAST(({presentingIssue}.fondingUnderClothes & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Fondling - Under Clothes (Passive)" });
				result.Add(new Field("fondlingUnderClothesActive", "CAST(({presentingIssue}.fondingUnderClothes & 2) / 2 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Fondling - Under Clothes (Active)" });
				result.Add(new Field("intercourseVaginalPassive", "CAST(({presentingIssue}.intercourseVaginal & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Intercourse - Vaginal (Passive)" });
				result.Add(new Field("intercourseVaginalActive", "CAST(({presentingIssue}.intercourseVaginal & 2) / 2 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Intercourse - Vaginal (Active)" });
				result.Add(new Field("intercourseAnalPassive", "CAST(({presentingIssue}.intercourseAnal & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Intercourse - Anal (Passive)" });
				result.Add(new Field("intercourseAnalActive", "CAST(({presentingIssue}.intercourseAnal & 2) / 2 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Intercourse - Anal (Active)" });
				result.Add(new Field("masturbationPassive", "CAST(({presentingIssue}.masturbation & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Masturbation (Passive)" });
				result.Add(new Field("masturbationActive", "CAST(({presentingIssue}.masturbation & 2) / 2 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Masturbation (Active)" });
				result.Add(new Field("oralPassive", "CAST(({presentingIssue}.oral & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Oral (Passive)" });
				result.Add(new Field("oralActive", "CAST(({presentingIssue}.oral & 2) / 2 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Oral (Active)" });
				result.Add(new Field("penetrationDigitalPassive", "CAST(({presentingIssue}.penetrationDigital & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Penetration - Digital (Passive)" });
				result.Add(new Field("penetrationDigitalActive", "CAST(({presentingIssue}.penetrationDigital & 2) / 2 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Penetration - Digital (Active)" });
				result.Add(new Field("penetrationObjectilePassive", "CAST(({presentingIssue}.penetrationObjectile & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Penetration - Objectile (Passive)" });
				result.Add(new Field("penetrationObjectileActive", "CAST(({presentingIssue}.penetrationObjectile & 2) / 2 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Penetration - Objectile (Active)" });
				result.Add(new Field("CAST(({presentingIssue}.{#solicitation} & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Solicitation (Passive)" });
				result.Add(new Field("sexualOtherPassive", "CAST(({presentingIssue}.sexualOther & 4) / 4 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Other Sexual Abuse (Passive)" });
				result.Add(new Field("sexualOtherActive", "CAST(({presentingIssue}.sexualOther & 2) / 2 AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Other Sexual Abuse (Active)" });
				result.Add(new Field("{presentingIssue}.{#sexualComment}", FieldType.NVarChar) { Label = "Other Sexual Abuse Text" });
				result.Add(new Field("CAST({presentingIssue}.{#boneFractures} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("CAST({presentingIssue}.{#brainDamage} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Brain Damage/Skull Fractures" });
				result.Add(new Field("CAST({presentingIssue}.{#burn} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Burn/Scalding" });
				result.Add(new Field("CAST({presentingIssue}.{#death} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("CAST({presentingIssue}.{#internalInjuries} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("CAST({presentingIssue}.{#poison} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Poison/Noxious" });
				result.Add(new Field("CAST({presentingIssue}.{#sprains} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Sprains/Dislocations" });
				result.Add(new Field("CAST({presentingIssue}.{#shaken} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("CAST({presentingIssue}.{#subduralHematoma} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("CAST({presentingIssue}.{#torture} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("CAST({presentingIssue}.{#wounds} AS BIT)", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("CAST({presentingIssue}.{#physicalOther} AS BIT)", FieldType.Checkbox) { NotEmpty = true, Label = "Other Physical Abuse" });
				result.Add(new Field("{presentingIssue}.{#physicalComment}", FieldType.NVarChar) { Label = "Other Physical Abuse Text" });
			}
			if (provider == Provider.CAC) {
				result.Add(new Field("(SELECT townshipName FROM USPSData.dbo.Townships WHERE id = {presentingIssue}.{#township}Id)", FieldType.NVarChar));
				result.Add(new Field("(SELECT cityName FROM USPSData.dbo.Cities WHERE id = {presentingIssue}.{#city}Id)", FieldType.NVarChar));
			}
			if (provider != Provider.DV)
				result.Add(new LookupField("{presentingIssue}.{#state}Id", Lookups.StateNames[provider]));
			return result;
		}

		private static Entity AddFinancialResource(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_Client{#FinancialResource}s") { Label = "Income" });
			result.Key = new[] {
				result.Add(new Field("{financialResource}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new LookupField("{financialResource}.{#income}Id", Lookups.IncomeSource2[provider]) { NotEmpty = true, Label = "Income Source" });
			result.Add(new Field("{financialResource}.{#amount}", FieldType.Dollars) { NotEmpty = true });
			return result;
		}

		private static Entity AddIncome(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_Client{#Income}"));
			result.Key = new[] {
				new Field("{income}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{income}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new LookupField("{income}.{#primaryIncome}Id", Lookups.IncomeSource[provider]));
			result.Add(new Field("{income}.{#afdc}", FieldType.Checkbox) { NotEmpty = true, Label = "TANF/AFDC" });
			result.Add(new Field("{income}.{#unknown}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{income}.{#generalAssistance}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{income}.{#socialSecurity}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{income}.{#ssi}", FieldType.Checkbox) { NotEmpty = true, Label = "SSI" });
			result.Add(new Field("{income}.{#alimonyChildSupport}", FieldType.Checkbox) { NotEmpty = true, Label = "Alimony/Child Support" });
			result.Add(new Field("{income}.{#employment}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{income}.{#otherIncome}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{income}.{#whatOther}", FieldType.NVarChar) { Label = "Other Income Text" });
			return result;
		}

		private static Entity AddBenefit(Model model) {
			var result = model.Add(new Entity("Ts_ClientNonCash{#Benefit}s") { Label = "Health Insurance and Non-Cash Benefits" });
			result.Key = new[] {
				new Field("{benefit}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{benefit}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new Field("{benefit}.{#foodBenefit}", FieldType.Checkbox) { NotEmpty = true, Label = "Food stamps/food benefit card (Link Card)" });
			result.Add(new Field("{benefit}.{#specSuppNutr}", FieldType.Checkbox) { NotEmpty = true, Label = "Special supplemental nutrition (WIC)" });
			result.Add(new Field("{benefit}.{#tanfChildCare}", FieldType.Checkbox) { NotEmpty = true, Label = "TANF child care services" });
			result.Add(new Field("{benefit}.{#tanfTrans}", FieldType.Checkbox) { NotEmpty = true, Label = "TANF transportation" });
			result.Add(new Field("{benefit}.{#tanfOther}", FieldType.Checkbox) { NotEmpty = true, Label = "Other TANF funded services" });
			result.Add(new Field("{benefit}.{#publicHousing}", FieldType.Checkbox) { NotEmpty = true, Label = "Section 8, public housing, rent assistance" });
			result.Add(new Field("{benefit}.{#otherSource}", FieldType.Checkbox) { NotEmpty = true, Label = "Other non-cash benefit" });
			result.Add(new Field("{benefit}.{#medicaid}", FieldType.Checkbox) { NotEmpty = true, Label = "Medicaid health insurance (Client 18 or older only)" });
			result.Add(new Field("{benefit}.{#medicare}", FieldType.Checkbox) { Label = "Medicare health insurance" });
			result.Add(new Field("{benefit}.{#stateChildHealth}", FieldType.Checkbox) { NotEmpty = true, Label = "State children's health insurance (Illinois Medicaid)" });
			result.Add(new Field("{benefit}.{#vetAdminMed}", FieldType.Checkbox) { NotEmpty = true, Label = "Veteran's Administration Med Services" });
			result.Add(new Field("{benefit}.{#privateIns}", FieldType.Checkbox) { Label = "Private Health Insurance" });
			result.Add(new Field("{benefit}.{#noHealthIns}", FieldType.Checkbox) { Label = "No Health Insurance" });
			result.Add(new Field("{benefit}.{#unknownHealthIns}", FieldType.Checkbox) { NotEmpty = true, Label = "Unknown Health Insurance" });
			result.Add(new Field("{benefit}.{#noBenefit}", FieldType.Checkbox) { NotEmpty = true, Label = "Client receives no non-cash benefits" });
			result.Add(new Field("{benefit}.{#unknownBenefit}", FieldType.Checkbox) { Label = "Unknown non-cash benefit" });
			return result;
		}

		private static Entity AddReferredBy(Model model, Provider provider) {
			var result = model.Add(new Entity("referredBy", "Ts_ClientReferralSource") { Label = provider == Provider.CAC ? "Referred From" : "Referred By" });
			result.Key = new[] {
				new Field("{referredBy}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{referredBy}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new Field("{referredBy}.{#police}", FieldType.Checkbox) { NotEmpty = true, Label = provider == Provider.SA ? "Police" : "Law Enforcement" });
			if (provider != Provider.CAC)
				result.Add(new Field("{referredBy}.{#hospital}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredBy}.{#medical}", FieldType.Checkbox) { NotEmpty = true, Label = (provider == Provider.SA ? "Other " : "") + "Medical" + (provider == Provider.CAC ? " Provider" : "") });
			if (provider == Provider.DV) {
				result.Add(new Field("{referredBy}.{#medicalAdvocacyProgram}", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{referredBy}.{#legalSystem}", FieldType.Checkbox) { NotEmpty = true });
			}
			if (provider != Provider.CAC) {
				result.Add(new Field("{referredBy}.{#clergy}", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{referredBy}.{#socialServiceProgram}", FieldType.Checkbox) { NotEmpty = true, Label = "Social Services Program" });
				result.Add(new Field("{referredBy}.{#educationSystem}", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{referredBy}.{#friend}", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{referredBy}.{#relative}", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{referredBy}.{#self}", FieldType.Checkbox) { NotEmpty = true });
			}
			result.Add(new Field("{referredBy}.{#other}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredBy}.{#whatOther}", FieldType.NVarChar) { Label = "Other Text" });
			if (provider != Provider.CAC) {
				result.Add(new Field("{referredBy}.{#privateAttorney}", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{referredBy}.{#publicHealth}", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{referredBy}.{#media}", FieldType.Checkbox) { NotEmpty = true });
			}
			result.Add(new Field("{referredBy}.{#stateAttorney}", FieldType.Checkbox) { NotEmpty = true, Label = (provider == Provider.SA ? "Legal System, " : "") + "State's Attorney" });
			if (provider == Provider.DV)
				result.Add(new Field("{referredBy}.{#circuitClerk}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredBy}.{#dcfs}", FieldType.Checkbox) { NotEmpty = true, Label = "DCFS" });
			if (provider == Provider.SA)
				result.Add(new Field("{referredBy}.{#hotline}", FieldType.Checkbox) { NotEmpty = true, Label = "Center Hotline" });
			result.Add(new Field("{referredBy}.{#agency}Id", FieldType.Id, AGENCY_SELECT_SQL) { Label = "Agency Name", OptionsSql = AGENCY_OPTIONS_SQL });
			if (provider != Provider.CAC)
				result.Add(new Field("{referredBy}.{#childAdvocacyCenter}", FieldType.Checkbox) { NotEmpty = true });
			if (provider == Provider.SA)
				result.Add(new Field("{referredBy}.{#otherRapeCrisisCenter}", FieldType.Checkbox) { NotEmpty = true });
			if (provider == Provider.DV) {
				result.Add(new Field("{referredBy}.{#statewideHelpLine}", FieldType.Checkbox) { NotEmpty = true, Label = "Illinois DV Helpline" });
				result.Add(new Field("{referredBy}.{#nationalHotline}", FieldType.Checkbox) { NotEmpty = true, Label = "National DV Hotline" });
				result.Add(new Field("{referredBy}.{#otherLocalHotline}", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{referredBy}.{#housingProgram}", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{referredBy}.{#sexualAssaultProgram}", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{referredBy}.{#otherDvProgram}", FieldType.Checkbox) { NotEmpty = true, Label = "Other DV Program" });
			}
			return result;
		}

		private static Entity AddReferredTo(Model model) {
			var result = model.Add(new Entity("referredTo", "Ts_ClientReferralSource"));
			result.Key = new[] {
				new Field("{referredTo}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{referredTo}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new Field("{referredTo}.to{#Police}", FieldType.Checkbox) { NotEmpty = true, Label = "Law Enforcement" });
			result.Add(new Field("{referredTo}.to{#Medical}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#Hospital}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#LegalSystem}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#Clergy}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#SocialServiceProgram}", FieldType.Checkbox) { NotEmpty = true, Label = "Social Services Program" });
			result.Add(new Field("{referredTo}.to{#EducationSystem}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#Other}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#WhatOther}", FieldType.NVarChar) { Label = "Other Text" });
			result.Add(new Field("{referredTo}.to{#PrivateAttorney}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#PublicHealth}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#StateAttorney}", FieldType.Checkbox) { NotEmpty = true, Label = "State's Attorney" });
			result.Add(new Field("{referredTo}.to{#CircuitClerk}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#HousingProgram}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#SexualAssaultProgram}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{referredTo}.to{#OtherDvProgram}", FieldType.Checkbox) { NotEmpty = true, Label = "Other DV Program" });
			result.Add(new Field("{referredTo}.to{#Dcfs}", FieldType.Checkbox) { NotEmpty = true, Label = "DCFS" });
			return result;
		}

		private static Entity AddDisability(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_Client{#Disability}") { Label = provider == Provider.SA ? "Language & Disability Needs" : "Special Needs" });
			result.Key = new[] {
				new Field("{disability}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{disability}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new Field("{disability}.{#developmentallyDisabled}", FieldType.Checkbox) { NotEmpty = true, Label = provider == Provider.SA ? "Developmental disability" : "Has developmental disability, requires assistance" });
			if (provider == Provider.DV) {
				result.Add(new Field("{disability}.{#specialDiet}", FieldType.Checkbox) { NotEmpty = true, Label = "Requires special diet" });
				result.Add(new Field("{disability}.{#immobil}", FieldType.Checkbox) { NotEmpty = true, Label = "Has immobility, requires assistance" });
			}
			result.Add(new Field("{disability}.{#wheelChair}", FieldType.Checkbox) { NotEmpty = true, Label = "Requires wheelchair accessibility" });
			if (provider == Provider.DV)
				result.Add(new Field("{disability}.{#medsAdministered}", FieldType.Checkbox) { NotEmpty = true, Label = "Must have medication administered" });
			result.Add(new Field("{disability}.{#deaf}", FieldType.Checkbox) { NotEmpty = true, Label = provider == Provider.SA ? "Hard of hearing/deaf" : "Has hearing impairment, requires assistance" });
			result.Add(new Field("{disability}.{#visualProblem}", FieldType.Checkbox) { NotEmpty = true, Label = provider == Provider.SA ? "Low vision/blind" : "Has a visual impairment, requires assistance" });
			result.Add(new Field("{disability}.{#limitedEnglish}", FieldType.Checkbox) { NotEmpty = true, Label = provider == Provider.SA ? "Requires Non-English language services" : "Has limited English proficiency, requires interpreter" });
			if (provider == Provider.DV)
				result.Add(new Field("{disability}.{#adlProblem}", FieldType.Checkbox) { NotEmpty = true, Label = "Requires assistance in feeding, dressing, toileting, or other ADL" });

			var otherDisability = result.Add(new Field("{disability}.{#otherDisability}", FieldType.Checkbox) { NotEmpty = true });
			var otherDisabilityText = result.Add(new Field("{disability}.{#whatOther}", FieldType.NVarChar) { Label = "Other Disability Text" });
			if (provider == Provider.DV) {
				otherDisability.Label = "Other special needs";
				otherDisabilityText.Label = "Other special needs text";
			} else if (provider == Provider.CAC) {
				otherDisability.Label = "Has other physical disability, requires assistance";
				otherDisabilityText.Label = "Other physical disability text";
			}

			if (provider != Provider.DV)
				result.Add(new Field("{disability}.{#mentalDisability}", FieldType.Checkbox) { NotEmpty = true, Label = provider == Provider.SA ? "Mental/emotional disability" : "Has mental health disability, requires assistance" });
			result.Add(new LookupField("{disability}.{#primaryLanguage}Id", Lookups.Language[provider]));
			if (provider != Provider.CAC)
				result.Add(new Field("{disability}.{#noSpecialNeeds}", FieldType.Checkbox) { NotEmpty = true, Label = provider == Provider.SA ? "None Identified" : "No Special Needs Indicated" });
			if (provider == Provider.DV) {
				result.Add(new Field("{disability}.{#unknown}SpecialNeeds", FieldType.Checkbox) { NotEmpty = true });
				result.Add(new Field("{disability}.{#notReported}", FieldType.Checkbox) { NotEmpty = true });
			}
			return result;
		}

		private static Entity AddServiceNeed(Model model) {
			var result = model.Add(new Entity("Ts_Client{#ServiceNeed}s") { Label = "Services Needed" });
			result.Key = new[] {
				new Field("{serviceNeed}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{serviceNeed}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new Field("{serviceNeed}.{#shelter}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#housing}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#medical}", FieldType.Checkbox) { NotEmpty = true, Label = "Medical Services" });
			result.Add(new Field("{serviceNeed}.{#transportation}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#emotional}", FieldType.Checkbox) { NotEmpty = true, Label = "Emotional/Counseling" });
			result.Add(new Field("{serviceNeed}.{#childCare}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#financial}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#legal}", FieldType.Checkbox) { NotEmpty = true, Label = "Legal Services" });
			result.Add(new Field("{serviceNeed}.{#employment}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#education}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#referral}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#legalAdvocacy}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#medicalAdvocacy}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#crisisIntervention}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#lockUp}", FieldType.Checkbox) { NotEmpty = true, Label = "Lock Up/Board Up" });
			result.Add(new Field("{serviceNeed}.{#therapy}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceNeed}.{#individualSupportChild}", FieldType.Checkbox) { NotEmpty = true, Label = "Individual Support (Child)" });
			result.Add(new Field("{serviceNeed}.{#groupActivity}", FieldType.Checkbox) { NotEmpty = true, Label = "Group Activity (Child)" });
			result.Add(new Field("{serviceNeed}.{#schoolAdvocacyChild}", FieldType.Checkbox) { NotEmpty = true, Label = "School Advocacy (Child)" });
			result.Add(new Field("{serviceNeed}.{#parentChildSupport}", FieldType.Checkbox) { NotEmpty = true, Label = "Parent/Child Support (Child)" });
			result.Add(new Field("{serviceNeed}.{#communityAdvocacyChild}", FieldType.Checkbox) { NotEmpty = true, Label = "Community Advocacy (Child)" });
			return result;
		}

		private static Entity AddServiceGot(Model model) {
			var result = model.Add(new Entity("Ts_Client{#ServiceGot}") { Label = "Services Received" });
			result.Key = new[] {
				new Field("{serviceGot}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{serviceGot}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new Field("{serviceGot}.{#shelter}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#housing}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#medical}", FieldType.Checkbox) { NotEmpty = true, Label = "Medical Services" });
			result.Add(new Field("{serviceGot}.{#transportation}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#emotional}", FieldType.Checkbox) { NotEmpty = true, Label = "Emotional/Counseling" });
			result.Add(new Field("{serviceGot}.{#childCare}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#financial}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#legal}", FieldType.Checkbox) { NotEmpty = true, Label = "Legal Services" });
			result.Add(new Field("{serviceGot}.{#employment}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#education}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#referral}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#legalAdvocacy}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#medicalAdvocacy}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#crisisIntervention}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#lockUp}", FieldType.Checkbox) { NotEmpty = true, Label = "Lock Up/Board Up" });
			result.Add(new Field("{serviceGot}.{#therapy}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{serviceGot}.{#individualSupportChild}", FieldType.Checkbox) { NotEmpty = true, Label = "Individual Support (Child)" });
			result.Add(new Field("{serviceGot}.{#groupActivity}", FieldType.Checkbox) { NotEmpty = true, Label = "Group Activity (Child)" });
			result.Add(new Field("{serviceGot}.{#schoolAdvocacyChild}", FieldType.Checkbox) { NotEmpty = true, Label = "School Advocacy (Child)" });
			result.Add(new Field("{serviceGot}.{#parentChildSupport}", FieldType.Checkbox) { NotEmpty = true, Label = "Parent/Child Support (Child)" });
			result.Add(new Field("{serviceGot}.{#communityAdvocacyChild}", FieldType.Checkbox) { NotEmpty = true, Label = "Community Advocacy (Child)" });
			return result;
		}

		private static Entity AddBehaviorIn(Model model) {
			var result = model.Add(new Entity("behaviorIn", "Ts_ClientChildBehavioralIssues") { Label = "Behaviors at Intake" });
			result.Key = new[] {
				new Field("{behaviorIn}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{behaviorIn}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new Field("{behaviorIn}.{#afraid}", FieldType.Checkbox) { NotEmpty = true, Label = "Is often afraid" });
			result.Add(new Field("{behaviorIn}.{#cantLeave}", FieldType.Checkbox) { NotEmpty = true, Label = "Can't leave parent" });
			result.Add(new Field("{behaviorIn}.{#accepts}", FieldType.Checkbox) { NotEmpty = true, Label = "Accepts w/o question" });
			result.Add(new Field("{behaviorIn}.{#cries}", FieldType.Checkbox) { NotEmpty = true, Label = "Cries often" });
			result.Add(new Field("{behaviorIn}.{#mood}", FieldType.Checkbox) { NotEmpty = true, Label = "Mood swings" });
			result.Add(new Field("{behaviorIn}.{#noInteract}", FieldType.Checkbox) { NotEmpty = true, Label = "Little interaction" });
			result.Add(new Field("{behaviorIn}.{#nightmares}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{behaviorIn}.{#hurtsSelf}", FieldType.Checkbox) { NotEmpty = true, Label = "Hurts self" });
			result.Add(new Field("{behaviorIn}.{#suicidal}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{behaviorIn}.{#bedWet}", FieldType.Checkbox) { NotEmpty = true, Label = "Bed wets" });
			result.Add(new Field("{behaviorIn}.{#illnesses}", FieldType.Checkbox) { NotEmpty = true, Label = "Illness often" });
			result.Add(new Field("{behaviorIn}.{#weight}", FieldType.Checkbox) { NotEmpty = true, Label = "Weight problem" });
			result.Add(new Field("{behaviorIn}.{#moreActive}", FieldType.Checkbox) { NotEmpty = true, Label = "More active" });
			result.Add(new Field("{behaviorIn}.{#specialClassActive}", FieldType.Checkbox) { NotEmpty = true, Label = "Special class active" });
			result.Add(new Field("{behaviorIn}.{#abuseDrugs}", FieldType.Checkbox) { NotEmpty = true, Label = "Abuses drugs" });
			result.Add(new Field("{behaviorIn}.{#abuseAlcohol}", FieldType.Checkbox) { NotEmpty = true, Label = "Abuses alcohol" });
			result.Add(new Field("{behaviorIn}.{#fire}", FieldType.Checkbox) { NotEmpty = true, Label = "Plays with fire" });
			result.Add(new Field("{behaviorIn}.{#roleReversal}", FieldType.Checkbox) { NotEmpty = true, Label = "Role reversal" });
			result.Add(new Field("{behaviorIn}.{#protective}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{behaviorIn}.{#resists}", FieldType.Checkbox) { NotEmpty = true, Label = "Resists guidance" });
			result.Add(new Field("{behaviorIn}.{#possessive}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{behaviorIn}.{#hitsKicksBites}", FieldType.Checkbox) { NotEmpty = true, Label = "Hits, kicks, bites" });
			result.Add(new Field("{behaviorIn}.{#behavesYoung}", FieldType.Checkbox) { NotEmpty = true, Label = "Behaves young" });
			result.Add(new Field("{behaviorIn}.{#harmsAnimals}", FieldType.Checkbox) { NotEmpty = true, Label = "Harms animals" });
			result.Add(new Field("{behaviorIn}.{#missSchool}", FieldType.Checkbox) { NotEmpty = true, Label = "Misses school" });
			result.Add(new Field("{behaviorIn}.{#dropOut}", FieldType.Checkbox) { NotEmpty = true, Label = "Drop out" });
			result.Add(new Field("{behaviorIn}.{#schoolRules}", FieldType.Checkbox) { NotEmpty = true, Label = "Disobeys the rules" });
			result.Add(new Field("{behaviorIn}.{#behaviorProblems}", FieldType.Checkbox) { NotEmpty = true, Label = "Behavior problems" });
			result.Add(new Field("{behaviorIn}.{#specClassBeh}", FieldType.Checkbox) { NotEmpty = true, Label = "Special class behavioral problems" });
			result.Add(new Field("{behaviorIn}.{#learningProblems}", FieldType.Checkbox) { NotEmpty = true, Label = "Learning problems" });
			result.Add(new Field("{behaviorIn}.{#specClassLearn}", FieldType.Checkbox) { NotEmpty = true, Label = "Special class learning problems" });
			result.Add(new Field("{behaviorIn}.{#noneObserved}", FieldType.Checkbox) { Label = "None observed" });
			return result;
		}

		private static Entity AddBehaviorOut(Model model) {
			var result = model.Add(new Entity("behaviorOut", "Ts_ClientChildBehavioralIssues") { Label = "Behaviors at Outtake" });
			result.Key = new[] {
				new Field("{behaviorOut}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{behaviorOut}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new Field("{behaviorOut}.{#afraid}_depart", FieldType.Checkbox) { Label = "Is often afraid" });
			result.Add(new Field("{behaviorOut}.{#cantLeave}_depart", FieldType.Checkbox) { Label = "Can't leave parent" });
			result.Add(new Field("{behaviorOut}.{#accepts}_depart", FieldType.Checkbox) { Label = "Accepts w/o question" });
			result.Add(new Field("{behaviorOut}.{#cries}_depart", FieldType.Checkbox) { Label = "Cries often" });
			result.Add(new Field("{behaviorOut}.{#mood}_depart", FieldType.Checkbox) { Label = "Mood swings" });
			result.Add(new Field("{behaviorOut}.{#noInteract}_depart", FieldType.Checkbox) { Label = "Little interaction" });
			result.Add(new Field("{behaviorOut}.{#nightmares}_depart", FieldType.Checkbox));
			result.Add(new Field("{behaviorOut}.{#hurtsSelf}_depart", FieldType.Checkbox) { Label = "Hurts self" });
			result.Add(new Field("{behaviorOut}.{#suicidal}_depart", FieldType.Checkbox));
			result.Add(new Field("{behaviorOut}.{#bedWet}_depart", FieldType.Checkbox) { Label = "Bed wets" });
			result.Add(new Field("{behaviorOut}.{#illnesses}_depart", FieldType.Checkbox) { Label = "Illness often" });
			result.Add(new Field("{behaviorOut}.{#weight}_depart", FieldType.Checkbox) { Label = "Weight problem" });
			result.Add(new Field("{behaviorOut}.{#moreActive}_depart", FieldType.Checkbox) { Label = "More active" });
			result.Add(new Field("{behaviorOut}.{#specialClassActive}_depart", FieldType.Checkbox) { Label = "Special class active" });
			result.Add(new Field("{behaviorOut}.{#abuseDrugs}_depart", FieldType.Checkbox) { Label = "Abuses drugs" });
			result.Add(new Field("{behaviorOut}.{#abuseAlcohol}_depart", FieldType.Checkbox) { Label = "Abuses alcohol" });
			result.Add(new Field("{behaviorOut}.{#fire}_depart", FieldType.Checkbox) { Label = "Plays with fire" });
			result.Add(new Field("{behaviorOut}.{#roleReversal}_depart", FieldType.Checkbox) { Label = "Role reversal" });
			result.Add(new Field("{behaviorOut}.{#protective}_depart", FieldType.Checkbox));
			result.Add(new Field("{behaviorOut}.{#resists}_depart", FieldType.Checkbox) { Label = "Resists guidance" });
			result.Add(new Field("{behaviorOut}.{#possessive}_depart", FieldType.Checkbox));
			result.Add(new Field("{behaviorOut}.{#hitsKicksBites}_depart", FieldType.Checkbox) { Label = "Hits, kicks, bites" });
			result.Add(new Field("{behaviorOut}.{#behavesYoung}_depart", FieldType.Checkbox) { Label = "Behaves young" });
			result.Add(new Field("{behaviorOut}.{#harmsAnimals}_depart", FieldType.Checkbox) { Label = "Harms animals" });
			result.Add(new Field("{behaviorOut}.{#missSchool}_depart", FieldType.Checkbox) { Label = "Misses school" });
			result.Add(new Field("{behaviorOut}.{#dropOut}_depart", FieldType.Checkbox) { Label = "Drop out" });
			result.Add(new Field("{behaviorOut}.{#schoolRules}_depart", FieldType.Checkbox) { Label = "Disobeys the rules" });
			result.Add(new Field("{behaviorOut}.{#behaviorProblems}_depart", FieldType.Checkbox) { Label = "Behavior problems" });
			result.Add(new Field("{behaviorOut}.{#specClassBeh}_depart", FieldType.Checkbox) { Label = "Special class behavioral problems" });
			result.Add(new Field("{behaviorOut}.{#learningProblems}_depart", FieldType.Checkbox) { Label = "Learning problems" });
			result.Add(new Field("{behaviorOut}.{#specClassLearn}_depart", FieldType.Checkbox) { Label = "Special class learning problems" });
			result.Add(new Field("{behaviorOut}.{#noneObserved}_depart", FieldType.Checkbox) { Label = "None observed" });
			return result;
		}

		private static Entity AddPreviousService(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_{#PreviousService}Use") { Label = "Previous Service Use" });
			result.Key = new[] {
				new Field("{previousService}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{previousService}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new LookupField("{previousService}.{#prevShelterUse}", Lookups.YesNo2[provider]) { Label = "Have you used another DV shelter in the service area in the last year?" });
			new DateField("{previousService}.{#prevShelterDate}") { Label = "Date used another DV shelter in the service area in the last year" }.AddTo(result, true);
			result.Add(new LookupField("{previousService}.{#prevServiceUse}", Lookups.YesNo2[provider]) { Label = "Have you used another homeless service in the service area in the last year?" });
			new DateField("{previousService}.{#prevServiceDate}") { Label = "Date used another homeless service in the service area in the last year" }.AddTo(result, true);
			return result;
		}
		#endregion

		#region client medical/criminal justice entities
		private static Entity AddPoliceProsecution(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_Client{#PoliceProsecution}") { Label = "Police/Prosecution" });
			result.Key = new[] {
				result.Add(new Field("{policeProsecution}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (provider == Provider.SA)
				result.Add(new LookupField("{policeProsecution}.{#appealStatus}Id", Lookups.AppealStatus[provider]));
			new DateField("{policeProsecution}.{#dateReportPolice}") { Label = "Date Reported to Police" }.AddTo(result, true);
			result.Add(new Field("{policeProsecution}.{#detectiveInterview}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{policeProsecution}.{#patrolInterview}", FieldType.Checkbox) { NotEmpty = true });
			result.Add(new Field("{policeProsecution}.{#saInterview}", FieldType.Checkbox) { NotEmpty = true, Label = "State's Attorney Interview" });
			result.Add(new LookupField("{policeProsecution}.{#trialType}Id", Lookups.TrialType[provider]));
			result.Add(new Field("{policeProsecution}.{#trialScheduled}", FieldType.Checkbox) { NotEmpty = true, Label = "Trial Scheduled?" });
			result.Add(new LookupField("{policeProsecution}.{#vwParticipate}Id", Lookups.VictimWitnessParticipation[provider]) { Label = "Victim/Witness Participate?" });
			if (provider == Provider.DV)
				result.Add(new Field("{policeProsecution}.{#vwProgram}", FieldType.Checkbox) { NotEmpty = true, Label = "Victim/Witness Program" });
			return result;
		}

		private static Entity AddConflictScale(Model model) {
			var result = model.Add(new Entity("Ts_Client{#ConflictScale}") { Label = "Severity of Abuse" });
			result.Key = new[] {
				new Field("{conflictScale}.{#clientId}", FieldType.Sequential) { NotEmpty = true },
				new Field("{conflictScale}.{#caseId}", FieldType.Sequential) { NotEmpty = true }
			};
			result.Add(new Field("{conflictScale}.{#threw}", FieldType.Checkbox) { NotEmpty = true, Label = "Threw something at your victim" });
			result.Add(new Field("{conflictScale}.{#pushed}", FieldType.Checkbox) { NotEmpty = true, Label = "Pushed, grabbed or shoved your victim" });
			result.Add(new Field("{conflictScale}.{#slapped}", FieldType.Checkbox) { NotEmpty = true, Label = "Slapped your victim" });
			result.Add(new Field("{conflictScale}.{#kicked}", FieldType.Checkbox) { NotEmpty = true, Label = "Kicked, bit or hit your victim with a fist" });
			result.Add(new Field("{conflictScale}.{#hit}", FieldType.Checkbox) { NotEmpty = true, Label = "Hit or tried to hit your victim with something" });
			result.Add(new Field("{conflictScale}.{#beatUp}", FieldType.Checkbox) { NotEmpty = true, Label = "Beat up your victim" });
			result.Add(new Field("{conflictScale}.{#choked}", FieldType.Checkbox) { NotEmpty = true, Label = "Strangled your victim" });
			result.Add(new Field("{conflictScale}.{#threatened}", FieldType.Checkbox) { NotEmpty = true, Label = "Threatened your victim with a knife or gun" });
			result.Add(new Field("{conflictScale}.{#used}", FieldType.Checkbox) { NotEmpty = true, Label = "Used a knife or fired a gun" });
			return result;
		}

		private static Entity AddCourtAppearance(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_Client{#CourtAppearance}") { Label = "Police/Prosecution > Court Appearance" });
			result.Key = new[] {
				result.Add(new Field("{courtAppearance}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new LookupField("{courtAppearance}.court{#Continuance}Id", Lookups.CourtContinuance[provider]) { Label = "Court Progress" });
			new DateField("{courtAppearance}.{#courtDate}") { Label = "Date of Court Appearance" }.AddTo(result, true);
			return result;
		}

		private static Entity AddMedicalVisit(Model model, Provider provider) {
			var result = model.Add(new Entity("medicalVisit", "Ts_ClientCjProcess") { Label = provider == Provider.CAC ? "Mecical Visit" : "Medical" });
			result.Key = new[] {
				result.Add(new Field("{medicalVisit}.med_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new LookupField("{medicalVisit}.{#medicalVisit}", Lookups.YesNo[provider]) { NotEmpty = provider == Provider.CAC, Label = "Visited Medical Facility?" });
			result.Add(new LookupField("{medicalVisit}.{#medicalTreatment}", Lookups.YesNo[provider]) { Label = "Treated for Injuries?" });
			if (provider != Provider.CAC)
				result.Add(new LookupField("{medicalVisit}.{#injury}Id", Lookups.InjurySeverity[provider]) { Label = "Seriousness of Injury" });
			result.Add(new LookupField("{medicalVisit}.{#evidKit}", Lookups.YesNo[provider]) { Label = "Evidence Kit Used?" });
			if (provider != Provider.CAC)
				result.Add(new LookupField("{medicalVisit}.{#photos}Id", Lookups.PhotosTaken[provider]) { Label = "Photos Taken?" });
			result.Add(new LookupField("{medicalVisit}.{#medWhere}Id", Lookups.MedicalTreatmentLocation[provider]) { Label = provider == Provider.CAC ? "Facility Type" : "Type of Medical Facility" });
			if (provider == Provider.DV) {
				result.Add(new Field("{medicalVisit}.{#otherFamilyProblem}", FieldType.NVarChar) { Label = "Other Problem" });
				result.Add(new Field("{medicalVisit}.{#wherePhotos}", FieldType.NVarChar) { Label = "Where are the photos?" });
			}
			if (provider == Provider.SA) {
                result.Add(new Field("{medicalVisit}.{#hospitalName}", FieldType.NVarChar) { Label = "Hospital/Medical Facility Visited" });
                result.Add(new LookupField("{medicalVisit}.{#SANETreated}Id", Lookups.YesNo[provider]) { Label = "Treated by SANE?" });
            }

				
			if (provider == Provider.CAC) {
				result.Add(new LookupField("{medicalVisit}.{#beforeAfter}Id", Lookups.BeforeAfter[provider]) { Label = "Before or After VSI" });
				result.Add(new LookupField("{medicalVisit}.{#colposcopeUsed}Id", Lookups.YesNo[provider]));
				result.Add(new LookupField("{medicalVisit}.{#examCompleted}Id", Lookups.YesNo[provider]));
				new DateField("{medicalVisit}.{#examDate}").AddTo(result, true);
				result.Add(new LookupField("{medicalVisit}.{#examType}Id", Lookups.MedicalExamType[provider]) { Label = "Type of Exam" });
				result.Add(new LookupField("{medicalVisit}.{#finding}Id", Lookups.MedicalExamFinding[provider]));
				result.Add(new LookupField("{medicalVisit}.site{#Location}Id", Lookups.SiteLocation[provider]));
				result.Add(new Field("{medicalVisit}.{#agency}Id", FieldType.Id, AGENCY_SELECT_SQL) { Label = "Facility Name", OptionsSql = AGENCY_OPTIONS_SQL });
			}
			return result;
		}

		private static Entity AddProtection(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_OrderOf{#Protection}") { Label = provider == Provider.SA ? "Police/Prosecution > Orders" : "Order of Protection" });
			result.Key = new[] {
				result.Add(new Field("{protection}.op_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (provider == Provider.DV) {
				result.Add(new LookupField("{protection}.{#status}Id", Lookups.OrderOfProtectionStatus[provider]) { Label = "Originally Sought Order" });
				new DateField("{protection}.{#dateFiled}").AddTo(result, true);
				result.Add(new Field("(SELECT countyName FROM USPSData.dbo.Counties WHERE id = {protection}.{#county}Id)", FieldType.NVarChar));
				new DateField("{protection}.{#dateIssued}") { Label = "Issue Date" }.AddTo(result, true);
				new DateField("{protection}.{#dateVacated}") { Label = "Vacate Date" }.AddTo(result, true);
				new DateField("{protection}.{#originalExpirationDate}").AddTo(result, true);
			}
			result.Add(new LookupField("{protection}.{#type}OfOpId", Lookups.OrderOfProtectionType[provider]) { NotEmpty = provider == Provider.DV, Label = provider == Provider.SA ? "Order of Protection Type" : "Type" });
			result.Add(new LookupField("{protection}.{#forum}Id", Lookups.OrderOfProtectionForum[provider]) { Label = provider == Provider.SA ? "Order of Protection" : "Forum" });
			if (provider == Provider.DV) {
				result.Add(new Field("{protection}.{#comments}", FieldType.NVarChar));
				result.Add(new Field("{protection}.{#location}Id", FieldType.Id, LOCATION_SELECT_SQL) { OptionsSql = LOCATION_OPTIONS_SQL });
			}
			if (provider == Provider.SA) {
				result.Add(new LookupField("{protection}.{#civilNoContactOrder}Id", Lookups.OrderOfProtectionForum[provider]));
				result.Add(new LookupField("{protection}.{#civilNoContactOrderType}Id", Lookups.OrderOfProtectionType[provider]));
				result.Add(new LookupField("{protection}.{#civilNoContactOrderRequest}Id", Lookups.OrderOfProtectionStatus[provider]));
			}
			return result;
		}

		private static Entity AddProtectionActivity(Model model, Provider provider) {
			var result = model.Add(new Entity("protectionActivity", "Ts_OpActivity") { Label = "Order of Protection > Activity" });
			result.Key = new[] {
				result.Add(new Field("{protectionActivity}.opActivity{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new LookupField("{protectionActivity}.op{#Activity}CodeId", Lookups.OrderOfProtectionActivity[provider]) { NotEmpty = true });
			new DateField("{protectionActivity}.op{#ActivityDate}").AddTo(result, true);
			new DateField("{protectionActivity}.{#newExpirationDate}").AddTo(result, true);
			return result;
		}
		#endregion

		#region client investigation entities
		private static Entity AddMdt(Model model, Provider provider) {
			var result = model.Add(new Entity("Tl_Client{#Mdt}") { Label = "Multidisciplinary Team" });
			result.Key = new[] {
				result.Add(new Field("{mdt}.mdt_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new Field("{mdt}.{#contact}Id", FieldType.Id, CONTACT_SELECT_SQL) { NotEmpty = true, OptionsSql = CONTACT_OPTIONS_SQL });
			result.Add(new Field("{mdt}.{#agency}Id", FieldType.Id, AGENCY_SELECT_SQL) { NotEmpty = true, OptionsSql = AGENCY_OPTIONS_SQL });
			result.Add(new LookupField("{mdt}.{#position}Id", Lookups.TeamMemberPosition[provider]));
			return result;
		}

		private static Entity AddAllegation(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_Dcfs{#Allegation}s") { Label = "DCFS Allegation" });
			result.Key = new[] {
				result.Add(new Field("{allegation}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new LookupField("{allegation}.{#abuseAllegation}Id", Lookups.AbuseAllegation[provider]) { Label = "DCFS Allegation" });
			result.Add(new LookupField("{allegation}.{#finding}Id", Lookups.AbuseAllegationFinding[provider]));
			new DateField("{allegation}.{#findingDate}") { Label = "Date of Finding" }.AddTo(result, true);
			return result;
		}

		private static Entity AddAllegationRespondent(Model model) {
			var result = model.Add(new Entity("allegationRespondent", "Ts_DcfsAllegationsRespondents") { Label = "DCFS Allegation > Respondent" });
			result.Key = new[] {
				result.Add(new Field("{allegationRespondent}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new Field("CASE WHEN {allegationRespondent}.respondentType = 1 THEN (SELECT clientCode FROM T_Client WHERE clientId = {allegationRespondent}.{#respondentId}) WHEN {allegationRespondent}.respondentType = 2 THEN (SELECT offenderCode FROM T_OffenderList WHERE offenderId = {allegationRespondent}.respondentId) ELSE NULL END", FieldType.NVarChar) { NotEmpty = true });
			result.Add(new Field("{allegationRespondent}.{#respondentType}", FieldType.Id, RESPONDENT_SELECT_SQL) { NotEmpty = true, Options = _RespondentOptions });
			return result;
		}

		private static Entity AddPetition(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_AbuseNeglect{#Petition}s") { Label = "Abuse/Neglect Petition" });
			result.Key = new[] {
				result.Add(new Field("{petition}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new LookupField("{petition}.abuseNeglect{#Petition}Id", Lookups.AbuseNeglectPetition[provider]) { NotEmpty = true });
			new DateField("{petition}.abuseNeglect{#PetitionDate}") { Label = "Date of Petition" }.AddTo(result, true);
			result.Add(new LookupField("{petition}.{#adjudicated}Id", Lookups.PetitionAdjudication[provider]));
			new DateField("{petition}.{#adjudicatedDate}") { Label = "Date Adjudicated" }.AddTo(result, true);
			return result;
		}

		private static Entity AddPetitionRespondent(Model model) {
			var result = model.Add(new Entity("petitionRespondent", "Ts_AbuseNeglectPetitionsRespondents") { Label = "Abuse/Neglect Petition > Respondent" });
			result.Key = new[] {
				result.Add(new Field("{petitionRespondent}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new Field("CASE WHEN {petitionRespondent}.respondentType = 1 THEN (SELECT clientCode FROM T_Client WHERE clientId = {petitionRespondent}.{#respondentId}) WHEN {petitionRespondent}.respondentType = 2 THEN (SELECT offenderCode FROM T_OffenderList WHERE offenderId = {petitionRespondent}.respondentId) ELSE NULL END", FieldType.NVarChar) { NotEmpty = true });
			result.Add(new Field("{petitionRespondent}.{#respondentType}", FieldType.Id, RESPONDENT_SELECT_SQL) { NotEmpty = true, Options = _RespondentOptions });
			return result;
		}

		private static Entity AddInterview(Model model, Provider provider) {
			var result = model.Add(new Entity("(SELECT * FROM Ts_VictimSensitive{#Interview}s WHERE locationId IN (SELECT centerId FROM T_Center WHERE parentCenterid = @centerId))") { Label = "Victim Sensitive Interview" });
			result.Key = new[] {
				result.Add(new Field("{interview}.vsi_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			new DateField("{interview}.{#interviewDate}") { NotEmpty = true }.AddTo(result, true);
			result.Add(new Field("{interview}.{#interviewer}Id", FieldType.Id, CONTACT_SELECT_SQL) { OptionsSql = CONTACT_OPTIONS_SQL });
			result.Add(new Field("{interview}.{#location}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			result.Add(new LookupField("{interview}.{#siteLocation}Id", Lookups.SiteLocation[provider]) { Label = "(Site) Location" });
			result.Add(new LookupField("{interview}.{#recordType}Id", Lookups.RecordingType[provider]) { Label = "Recorded" });
			result.Add(new Field("{interview}.{#courtesyInterview}", FieldType.Checkbox));
			return result;
		}

		private static Entity AddInterviewObserver(Model model, Provider provider) {
			var result = model.Add(new Entity("interviewObserver", "Ts_VsiObservers") { Label = "Victim Sensitive Interview > Observer" });
			result.Key = new[] {
				result.Add(new Field("{interviewObserver}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new Field("{interviewObserver}.{#agency}Id", FieldType.Id, AGENCY_SELECT_SQL) { OptionsSql = AGENCY_OPTIONS_SQL });
			result.Add(new Field("{interviewObserver}.{#contact}Id", FieldType.Id, CONTACT_SELECT_SQL) { OptionsSql = CONTACT_OPTIONS_SQL });
			result.Add(new LookupField("{interviewObserver}.{#observer}Id", Lookups.ObserverPosition[provider]) { NotEmpty = true, Label = "Position" });
			return result;
		}
		#endregion

		#region offender entities
		private static Entity AddOffender(Model model, Provider provider) {
			var result = model.Add(new Entity("(SELECT o.*, ol.offenderCode FROM T_{#Offender} o JOIN T_OffenderList ol ON o.offenderId = ol.offenderId WHERE ol.parentCenterId = @centerId)"));
			result.Key = new[] {
				result.Add(new Field("{offender}.offenderRecord{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new LookupField("{offender}.{#sex}Id", Lookups.GenderIdentity[provider]) { NotEmpty = provider == Provider.CAC, Label = "Gender Identity" });
			result.Add(new LookupField("{offender}.{#race}Id", Lookups.Race[provider]) { NotEmpty = provider == Provider.CAC, Label = "Race/Ethnicity" });
			result.Add(new Field("(SELECT countyName FROM USPSData.dbo.Counties WHERE id = {offender}.{#county}Id)", FieldType.NVarChar) { Label = "County of Residence" });
			result.Add(new LookupField("{offender}.{#relationshipToClient}Id", Lookups.RelationshipToClient[provider]) { Label = "Relationship to Victim" });
			result.Add(new Field("{offender}.{#age}", FieldType.Quantity) { NotEmpty = provider == Provider.CAC, Label = "Age at Victim Intake" });
			if (provider == Provider.DV)
				result.Add(new LookupField("{offender}.{#visitation}Id", Lookups.Visitation[provider]));
			if (provider == Provider.CAC)
				result.Add(new Field("{offender}.{#offenderCode}", FieldType.NVarChar) { NotEmpty = true, Label = "Offender ID" });
			result.Add(new LookupField("{offender}.{#state}Id", Lookups.StateNames[provider]) { NotEmpty = true, Label = "State of Residence" });
			if (provider != Provider.DV)
				result.Add(new LookupField("{offender}.{#registered}Id", Lookups.YesNo[provider]) { Label = "Registered Offender" });
			return result;
		}

		private static Entity AddPoliceCharge(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_{#PoliceCharge}s"));
			result.Key = new[] {
				result.Add(new Field("{policeCharge}.pc_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			new DateField("{policeCharge}.{#arrestDate}") { Label = "Date of Arrest" }.AddTo(result, true);
			result.Add(new LookupField("{policeCharge}.{#policeCharge}Id", Lookups.Statute[provider]));
			new DateField("{policeCharge}.{#chargeDate}") { Label = "Date of Charge" }.AddTo(result, true);
			result.Add(new LookupField("{policeCharge}.{#chargeType}Id", Lookups.CrimeClass[provider]) { Label = provider == Provider.CAC ? "Class" : "Charge Type" });
			if (provider == Provider.CAC)
				result.Add(new Field("{policeCharge}.{#chargeCounts}", FieldType.Quantity));
			result.Add(new LookupField("{policeCharge}.{#arrestMade}Id", Lookups.ArrestMade[provider]) { NotEmpty = provider == Provider.CAC, Label = "Arrest Made?" });
			return result;
		}

		private static Entity AddTrialCharge(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_{#TrialCharge}s"));
			result.Key = new[] {
				result.Add(new Field("{trialCharge}.tc_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new LookupField("{trialCharge}.{#trialCharge}Id", Lookups.Statute[provider]) { Label = "State's Attorney Charge" });
			new DateField("{trialCharge}.{#chargeDate}").AddTo(result, true);
			result.Add(new LookupField("{trialCharge}.{#disposition}Id", Lookups.Disposition[provider]));
			new DateField("{trialCharge}.{#dispositionDate}").AddTo(result, true);
			if (provider == Provider.CAC)
				result.Add(new Field("{trialCharge}.charge{#Counts}", FieldType.Quantity));
			result.Add(new LookupField("{trialCharge}.{#chargesFiled}Id", Lookups.TrialChargeFiled[provider]) { NotEmpty = provider == Provider.CAC, Label = "Charges Filed?" });
			if (provider == Provider.CAC)
				result.Add(new LookupField("{trialCharge}.{#chargeType}Id", Lookups.CrimeClass[provider]) { Label = "Class" });
			return result;
		}

		private static Entity AddSentence(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_{#Sentence}s"));
			result.Key = new[] {
				result.Add(new Field("{sentence}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new LookupField("{sentence}.{#sentence}Id", Lookups.Sentence[provider]));
			new DateField("{sentence}.{#sentenceDate}") { NotEmpty = provider == Provider.CAC }.AddTo(result, true);
			result.Add(new Field("{sentence}.{#years}Sentenced", FieldType.Quantity));
			result.Add(new Field("{sentence}.{#months}Sentenced", FieldType.Quantity));
			result.Add(new Field("{sentence}.{#days}Sentenced", FieldType.Quantity));
			return result;
		}
		#endregion

		#region client service entities
		private static Entity AddDirectService(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("directService", "(SELECT * FROM Tl_ServiceDetailOfClient WHERE ics_id IS NULL AND serviceId NOT IN (SELECT codeId FROM Tlu_Codes_ProgramsAndServices WHERE isShelter = 1) AND locationId IN (SELECT centerId FROM T_Center WHERE parentCenterid = @centerId))"));
			result.Key = new[] {
				result.Add(new Field("{directService}.serviceDetail{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Client) {
				result.Add(new Field("(SELECT clientCode FROM T_Client WHERE clientId = {directService}.{#clientId})", FieldType.NVarChar) { NotEmpty = true, Label = "Client ID" });
				if (provider != Provider.SA) {
					result.Add(new Field("{directService}.{#caseId}", FieldType.Sequential) { NotEmpty = true, Label = "Case ID" });
					result.Add(new Field("clientCaseId", "NULLIF(CONCAT({directService}.clientId, '-', {directService}.caseId), '-')", FieldType.NVarCharId) { NotEmpty = true, AvailableConditions = _NoConditions, Label = "Client Case ID" });
				}
			}
			result.Add(new LookupField("{directService}.{#service}Id", Lookups.DirectServices[provider]) { NotEmpty = true });
			if (perspective != Perspective.Staff)
				result.Add(new Field("{directService}.{#svId}", FieldType.Id, STAFF_SELECT_SQL) { NotEmpty = provider == Provider.CAC, Label = "Staff/Volunteer", OptionsSql = STAFF_OPTIONS_SQL });
			new DateField("{directService}.service{#Date}") { NotEmpty = provider != Provider.DV }.AddTo(result, true);
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{directService}.{#location}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			result.Add(new Field("{directService}.received{#Hours}", FieldType.Hours) { NotEmpty = true });
			result.Add(new Field("{directService}.{#fundDate}Id", FieldType.Date, FUNDINGDATE_SELECT_SQL) { Label = "Date Funding Issued", OptionsSql = FUNDINGDATE_OPTIONS_SQL });
			result.Add(new Field("{directService}.{#agencyRecId}", FieldType.Sequential) { Label = "Agency Record ID" });
			return result;
		}

		private static Entity AddHousingService(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("housingService", "(SELECT * FROM Tl_ServiceDetailOfClient WHERE ics_id IS NULL AND serviceId IN (SELECT codeId FROM Tlu_Codes_ProgramsAndServices WHERE isShelter = 1) AND locationId IN (SELECT centerId FROM T_Center WHERE parentCenterid = @centerId))"));
			result.Key = new[] {
				result.Add(new Field("{housingService}.serviceDetail{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Client) {
				result.Add(new Field("(SELECT clientCode FROM T_Client WHERE clientId = {housingService}.{#clientId})", FieldType.NVarChar) { NotEmpty = true, Label = "Client ID" });
				if (provider != Provider.SA) {
					result.Add(new Field("{housingService}.{#caseId}", FieldType.Sequential) { NotEmpty = true, Label = "Case ID" });
					result.Add(new Field("clientCaseId", "NULLIF(CONCAT({housingService}.clientId, '-', {housingService}.caseId), '-')", FieldType.NVarCharId) { NotEmpty = true, AvailableConditions = _NoConditions, Label = "Client Case ID" });
				}
			}
			result.Add(new LookupField("{housingService}.{#service}Id", Lookups.HousingServices[provider]) { NotEmpty = true });
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{housingService}.{#location}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			new DateField("shelterBeginDate", "{housingService}.shelterBegDate") { Label = "Shelter/Transitional Housing Begin" }.AddTo(result, true);
			new DateField("{housingService}.{#shelterEndDate}") { Label = "Shelter/Transitional Housing End" }.AddTo(result, true);
			result.Add(new Field("{housingService}.{#agencyRecId}", FieldType.Sequential) { Label = "Agency Record ID" });
			return result;
		}

		private static Entity AddReferralDetail(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("(SELECT * FROM Ts_Client{#ReferralDetail} WHERE locationId IN (SELECT centerId FROM T_Center WHERE parentCenterid = @centerId))") { Label = "Referral" });
			result.Key = new[] {
				result.Add(new Field("{referralDetail}.referralDetail{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Client) {
				result.Add(new Field("(SELECT clientCode FROM T_Client WHERE clientId = {referralDetail}.{#clientId})", FieldType.NVarChar) { NotEmpty = true, Label = "Client ID" });
				if (provider != Provider.SA) {
					result.Add(new Field("{referralDetail}.{#caseId}", FieldType.Sequential) { NotEmpty = true, Label = "Case ID" });
					result.Add(new Field("clientCaseId", "NULLIF(CONCAT({referralDetail}.clientId, '-', {referralDetail}.caseId), '-')", FieldType.NVarCharId) { NotEmpty = true, AvailableConditions = _NoConditions, Label = "Client Case ID" });
				}
			}
			new DateField("{referralDetail}.{#referralDate}") { NotEmpty = true }.AddTo(result, true);
			result.Add(new LookupField("{referralDetail}.{#referralType}Id", Lookups.ReferralType[provider]) { NotEmpty = true });
			result.Add(new Field("{referralDetail}.{#agency}Id", FieldType.Id, AGENCY_SELECT_SQL) { NotEmpty = true, OptionsSql = AGENCY_OPTIONS_SQL });
			result.Add(new LookupField("{referralDetail}.{#response}Id", Lookups.ReferralResponse[provider]) { NotEmpty = true });
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{referralDetail}.{#location}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			return result;
		}

		private static Entity AddDepature(Model model, Provider provider) {
			var result = model.Add(new Entity("Ts_Client{#Departure}"));
			result.Key = new[] {
				result.Add(new Field("{departure}.departure{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new LookupField("{departure}.{#destination}Id", Lookups.Destination[provider]) { NotEmpty = true });
			result.Add(new LookupField("{departure}.{#destinationTenure}Id", Lookups.DestinationTenure[provider]));
			result.Add(new LookupField("{departure}.{#destinationSubsidy}Id", Lookups.DestinationSubsidy[provider]));
			result.Add(new LookupField("{departure}.{#reasonForLeaving}Id", Lookups.ReasonForLeaving[provider]) { Label = "Reason for Leaving" });
			new DateField("{departure}.{#departureDate}") { NotEmpty = true }.AddTo(result, true);
			return result;
		}

		private static Entity AddCancellation(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("(SELECT * FROM Tl_{#Cancellation}s WHERE locationId IN (SELECT centerId FROM T_Center WHERE parentCenterid = @centerId))") { Label = "Cancellation/No Show" });
			result.Key = new[] {
				result.Add(new Field("{cancellation}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Client) {
				result.Add(new Field("(SELECT clientCode FROM T_Client WHERE clientId = {cancellation}.{#clientId})", FieldType.NVarChar) { NotEmpty = true, Label = "Client ID" });
				if (provider != Provider.SA) {
					result.Add(new Field("{cancellation}.{#caseId}", FieldType.Sequential) { NotEmpty = true, Label = "Case ID" });
					result.Add(new Field("clientCaseId", "NULLIF(CONCAT({cancellation}.clientId, '-', {cancellation}.caseId), '-')", FieldType.NVarCharId) { NotEmpty = true, AvailableConditions = _NoConditions, Label = "Client Case ID" });
				}
			}
			result.Add(new LookupField("{cancellation}.{#service}Id", Lookups.DirectOrGroupServices[provider]) { NotEmpty = true });
			new DateField("{cancellation}.{#date}") { NotEmpty = true }.AddTo(result, true);
			if (perspective != Perspective.Staff) {
				result.Add(new Field("{cancellation}.{#svId}", FieldType.Id, STAFF_SELECT_SQL) { NotEmpty = true, Label = "Staff/Volunteer", OptionsSql = STAFF_OPTIONS_SQL });
				if (perspective != Perspective.Center)
					result.Add(new Field("{cancellation}.{#location}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			}
			result.Add(new LookupField("{cancellation}.{#reason}Id", Lookups.CancellationReason[provider]) { NotEmpty = true });
			return result;
		}
		#endregion

		#region investigation entities
		private static Entity AddInvestigation(Model model, Provider provider) {
			var result = model.Add(new Entity("(SELECT * FROM T_{#Investigation}s WHERE centerId = @centerId)") { Label = provider == Provider.CAC ? "Relationship" : "Household" });
			result.Key = new[] {
				result.Add(new Field("{investigation}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new Field("{investigation}.{#investigationId}", FieldType.NVarChar) { NotEmpty = true, Label = provider == Provider.CAC ? "Relationship ID" : "Household ID" });

			new DateField("{investigation}.{#creationDate}") { NotEmpty = true }.AddTo(result, true);
			return result;
		}

		private static Entity AddInvestigationClient(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("Ts_{#InvestigationClient}s") { Label = provider == Provider.CAC ? "Relationship > Client" : "Household > Member" });
			result.Key = new[] {
				result.Add(new Field("{investigationClient}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Client) {
				result.Add(new Field("(SELECT clientCode FROM T_Client WHERE clientId = {investigationClient}.{#clientId})", FieldType.NVarChar) { NotEmpty = true, Label = "Client ID" });
				if (provider != Provider.SA) {
					result.Add(new Field("{investigationClient}.{#caseId}", FieldType.Sequential) { NotEmpty = true, Label = "Case ID" });
					result.Add(new Field("clientCaseId", "NULLIF(CONCAT({investigationClient}.clientId, '-', {investigationClient}.caseId), '-')", FieldType.NVarCharId) { NotEmpty = true, AvailableConditions = _NoConditions, Label = "Client Case ID" });
				}
			}
			if (provider == Provider.CAC)
				result.Add(new Field("(SELECT {#householdId} FROM Ts_InvestigationHouseholds WHERE ts_cacInvestigationClients_fk = {investigationClient}.id)", FieldType.Sequential) { NotEmpty = true, Label = "Household ID" });
			return result;
		}
		#endregion

		#region group service entities
		private static Entity AddGroupService(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("groupService", "(SELECT * FROM Tl_ProgramDetail WHERE programId IN (SELECT codeId FROM Tlu_Codes_ProgramsAndServices WHERE IsGroupService = 1) AND centerId IN (SELECT centerId FROM T_Center WHERE parentCenterId = @centerId))"));
			result.Key = new[] {
				result.Add(new Field("{groupService}.ics_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{groupService}.{#center}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			result.Add(new LookupField("{groupService}.{#program}Id", Lookups.GroupServices[provider]) { NotEmpty = true, Label = "Group Service" });
			result.Add(new Field("{groupService}.{#numOfSession}", FieldType.Quantity) { Label = "Number of Sessions" });
			result.Add(new Field("{groupService}.{#hours}", FieldType.Hours) { NotEmpty = true, Label = "Hours in Session" });
			result.Add(new Field("{groupService}.{#participantsNum}", FieldType.Quantity) { Label = "Number of Attendees" });
			new DateField("{groupService}.p{#Date}") { NotEmpty = provider == Provider.CAC }.AddTo(result, true);
			result.Add(new Field("{groupService}.{#fundDate}Id", FieldType.Date, FUNDINGDATE_SELECT_SQL) { Label = "Date Funding Issued", OptionsSql = FUNDINGDATE_OPTIONS_SQL });
			result.Add(new Field("agencyIcsId", "{groupService}.agency_ics_id", FieldType.Sequential) { Label = "Agency Record ID" });
			return result;
		}

		private static Entity AddGroupServiceStaff(Model model, Perspective perspective) {
			var result = model.AddEntity("groupServiceStaff", "Ts_ProgramDetail_Staffs", "Group Service > Staff");
			result.Key = new[] {
				result.Add(new Field("{groupServiceStaff}.ics_staff_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Staff)
				result.Add(new Field("{groupServiceStaff}.{#svId}", FieldType.Id, STAFF_SELECT_SQL) { NotEmpty = true, Label = "Staff/Volunteer", OptionsSql = STAFF_OPTIONS_SQL });
			result.Add(new Field("{groupServiceStaff}.{#conductHours}", FieldType.Hours) { NotEmpty = true });
			result.Add(new Field("{groupServiceStaff}.{#hoursPrep}", FieldType.Hours) { NotEmpty = true, Label = "Prepare Hours" });
			result.Add(new Field("{groupServiceStaff}.{#hoursTravel}", FieldType.Hours) { NotEmpty = true, Label = "Travel Hours" });
			return result;
		}

		private static Entity AddGroupServiceClient(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("groupServiceClient", "(SELECT * FROM Tl_ServiceDetailOfClient WHERE ics_id IS NOT NULL AND locationId IN (SELECT centerId FROM T_Center WHERE parentCenterid = @centerId))") { Label = "Group Service > Attendee" });
			result.Key = new[] {
				result.Add(new Field("{groupServiceClient}.serviceDetail{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Client) {
				result.Add(new Field("(SELECT clientCode FROM T_Client WHERE clientId = {groupServiceClient}.{#clientId})", FieldType.NVarChar) { NotEmpty = true, Label = "Client ID" });
				if (provider != Provider.SA) {
					result.Add(new Field("{groupServiceClient}.{#caseId}", FieldType.Sequential) { NotEmpty = true, Label = "Case ID" });
					result.Add(new Field("clientCaseId", "NULLIF(CONCAT({groupServiceClient}.clientId, '-', {groupServiceClient}.caseId), '-')", FieldType.NVarCharId) { NotEmpty = true, AvailableConditions = _NoConditions, Label = "Client Case ID" });
				}
			}
			result.Add(new Field("{groupServiceClient}.{#receivedHours}", FieldType.Hours) { NotEmpty = true });
			result.Add(new Field("{groupServiceClient}.{#agencyRecId}", FieldType.Sequential) { Label = "Agency Record ID" });
			return result;
		}
		#endregion

		#region community service entities
		private static Entity AddCommunityService(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("communityService", "(SELECT * FROM Tl_ProgramDetail WHERE programId IN (SELECT codeId FROM Tlu_Codes_ProgramsAndServices WHERE IsCommInst = 1) AND centerId IN (SELECT centerId FROM T_Center WHERE parentCenterId = @centerId))") { Label = "Community/Institutional Service" });
			result.Key = new[] {
				result.Add(new Field("{communityService}.ics_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{communityService}.{#center}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			result.Add(new LookupField("{communityService}.{#program}Id", Lookups.CommunityServices[provider]) { NotEmpty = true, Label = "Community/Institutional Service" });
			result.Add(new Field("{communityService}.{#numOfSession}", FieldType.Quantity) { Label = "Number of Presentations/Contacts" });
			result.Add(new Field("{communityService}.{#hours}", FieldType.Hours) { NotEmpty = true, Label = "Total Hours" });
			result.Add(new Field("{communityService}.{#participantsNum}", FieldType.Quantity) { Label = "Number of Participants" });
			new DateField("{communityService}.p{#Date}") { NotEmpty = provider == Provider.CAC }.AddTo(result, true);
			result.Add(new Field("{communityService}.{#comment}_act", FieldType.NVarChar) { Label = "Comments" });
			result.Add(new Field("{communityService}.{#fundDate}Id", FieldType.Date, FUNDINGDATE_SELECT_SQL) { Label = "Date Funding Issued", OptionsSql = FUNDINGDATE_OPTIONS_SQL });
			result.Add(new Field("{communityService}.{#location}", FieldType.NVarChar));
			result.Add(new Field("{communityService}.{#agency}Id", FieldType.Id, AGENCY_SELECT_SQL) { OptionsSql = AGENCY_OPTIONS_SQL });
			result.Add(new Field("agencyIcsId", "{communityService}.agency_ics_id", FieldType.Sequential) { Label = "Agency Record ID" });
			result.Add(new Field("(SELECT countyName FROM USPSData.dbo.Counties WHERE id = {communityService}.{#county}Id)", FieldType.NVarChar));
			result.Add(new LookupField("{communityService}.{#state}Id", Lookups.StateNames[provider]));
			return result;
		}

		private static Entity AddCommunityServiceStaff(Model model, Perspective perspective) {
			var result = model.AddEntity("communityServiceStaff", "Ts_ProgramDetail_Staffs", "Community/Institutional Service > Staff");
			result.Key = new[] {
				result.Add(new Field("{communityServiceStaff}.ics_staff_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Staff)
				result.Add(new Field("{communityServiceStaff}.{#svId}", FieldType.Id, STAFF_SELECT_SQL) { NotEmpty = true, Label = "Staff/Volunteer", OptionsSql = STAFF_OPTIONS_SQL });
			result.Add(new Field("{communityServiceStaff}.{#conductHours}", FieldType.Hours) { NotEmpty = true });
			result.Add(new Field("{communityServiceStaff}.{#hoursPrep}", FieldType.Hours) { NotEmpty = true, Label = "Prepare Hours" });
			result.Add(new Field("{communityServiceStaff}.{#hoursTravel}", FieldType.Hours) { NotEmpty = true, Label = "Travel Hours" });
			return result;
		}
		#endregion

		#region publication entities
		private static Entity AddPublication(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("(SELECT * FROM Tl_{#Publication}Detail WHERE centerId IN (SELECT centerId FROM T_Center WHERE parentCenterId = @centerId))") { Label = "Media/Publication" });
			result.Key = new[] {
				result.Add(new Field("{publication}.ics_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{publication}.{#center}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			result.Add(new LookupField("{publication}.{#program}Id", Lookups.PublicationTypes[provider]) { NotEmpty = true, Label = "Media/Publication Type" });
			new DateField("{publication}.p{#Date}") { NotEmpty = provider == Provider.CAC, Label = "Publication Date" }.AddTo(result, true);
			result.Add(new Field("{publication}.{#fundDate}Id", FieldType.Date, FUNDINGDATE_SELECT_SQL) { Label = "Date Funding Issued", OptionsSql = FUNDINGDATE_OPTIONS_SQL });
			result.Add(new Field("{publication}.{#title}", FieldType.NVarChar) { Label = "Publication Title" });
			result.Add(new Field("{publication}.{#prepareHours}", FieldType.Hours));
			result.Add(new Field("{publication}.{#numOfBrochure}", FieldType.Quantity) { Label = "Number of Publications or Media Segments" });
			result.Add(new Field("{publication}.{#comment}_pub", FieldType.NVarChar) { Label = "Comments" });
			return result;
		}

		private static Entity AddPublicationStaff(Model model, Provider provider, Perspective perspective) {
			var result = model.AddEntity("publicationStaff", "Ts_PublicationDetail_Staffs", "Media/Publication > Staff");
			result.Key = new[] {
				result.Add(new Field("{publicationStaff}.ics_staff_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Staff)
				result.Add(new Field("{publicationStaff}.{#svId}", FieldType.Id, STAFF_SELECT_SQL) { NotEmpty = true, Label = "Staff/Volunteer", OptionsSql = STAFF_OPTIONS_SQL });
			result.Add(new Field("{publicationStaff}.{#hoursPrep}", FieldType.Hours) { NotEmpty = provider == Provider.CAC, Label = "Preparing Hours" });
			return result;
		}
		#endregion

		#region event entities
		private static Entity AddEvent(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("(SELECT * FROM Tl_{#Event}Detail WHERE centerId IN (SELECT centerId FROM T_Center WHERE parentCenterId = @centerId))"));
			result.Key = new[] {
				result.Add(new Field("{event}.ics_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{event}.{#center}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			result.Add(new LookupField("{event}.{#program}Id", Lookups.EventTypes[provider]) { NotEmpty = true, Label = "Event Type" });
			result.Add(new Field("{event}.{#eventName}", FieldType.NVarChar) { NotEmpty = true });
			result.Add(new Field("{event}.{#eventHours}", FieldType.Hours) { NotEmpty = true });
			result.Add(new Field("{event}.num{#PeopleReached}", FieldType.Quantity) { NotEmpty = true, Label = "Number of People Reached" });
			new DateField("{event}.{#eventDate}") { NotEmpty = true }.AddTo(result, true);
			result.Add(new Field("{event}.{#comment}", FieldType.NVarChar) { Label = "Comments" });
			result.Add(new Field("{event}.{#location}", FieldType.NVarChar));
			result.Add(new Field("{event}.{#fundDate}Id", FieldType.Date, FUNDINGDATE_SELECT_SQL) { Label = "Date Funding Issued", OptionsSql = FUNDINGDATE_OPTIONS_SQL });
			result.Add(new Field("(SELECT countyName FROM USPSData.dbo.Counties WHERE id = {event}.{#county}Id)", FieldType.NVarChar));
			result.Add(new LookupField("{event}.{#state}Id", Lookups.StateNames[provider]));
			return result;
		}

		private static Entity AddEventStaff(Model model, Perspective perspective) {
			var result = model.AddEntity("eventStaff", "Ts_EventDetail_Staffs", "Event > Staff");
			result.Key = new[] {
				result.Add(new Field("{eventStaff}.ics_staff_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Staff)
				result.Add(new Field("{eventStaff}.{#svId}", FieldType.Id, STAFF_SELECT_SQL) { NotEmpty = true, Label = "Staff/Volunteer", OptionsSql = STAFF_OPTIONS_SQL });
			result.Add(new Field("{eventStaff}.{#hoursConduct}", FieldType.Hours) { NotEmpty = true, Label = "Conduct Hours" });
			result.Add(new Field("{eventStaff}.{#hoursPrep}", FieldType.Hours) { NotEmpty = true, Label = "Prepare Hours" });
			result.Add(new Field("{eventStaff}.{#hoursTravel}", FieldType.Hours) { NotEmpty = true, Label = "Travel Hours" });
			return result;
		}
		#endregion

		#region hotline entity
		private static Entity AddHotline(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("(SELECT * FROM T_Phone{#Hotline} WHERE centerId IN (SELECT centerId FROM T_Center WHERE parentCenterId = @centerId))") { Label = provider == Provider.SA ? "Non-Client Crisis Intervention" : "Hotline Call" });
			result.Key = new[] {
				result.Add(new Field("{hotline}.ph_{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Staff) {
				result.Add(new Field("{hotline}.{#svId}", FieldType.Id, STAFF_SELECT_SQL) { Label = "Staff/Volunteer", OptionsSql = STAFF_OPTIONS_SQL });
				if (perspective != Perspective.Center)
					result.Add(new Field("{hotline}.{#center}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			}
			new DateField("{hotline}.{#date}") { NotEmpty = true }.AddTo(result, true);
			result.Add(new LookupField("{hotline}.{#callType}Id", Lookups.HotlineCallType[provider]) { Label = provider == Provider.SA ? "Type of Intervention" : "Call Type" });
			result.Add(new Field("{hotline}.{#numberOfContacts}", FieldType.Quantity) { Label = "Number of Contacts" });
			result.Add(new Field("{hotline}.{#fundDate}Id", FieldType.Date, FUNDINGDATE_SELECT_SQL) { Label = "Date Funding Issued", OptionsSql = FUNDINGDATE_OPTIONS_SQL });
			result.Add(new Field("{hotline}.{#town}", FieldType.NVarChar));
			result.Add(new Field("{hotline}.{#township}", FieldType.NVarChar));
			result.Add(new Field("{hotline}.{#zipCode}", FieldType.NVarChar));
			result.Add(new Field("(SELECT countyName FROM USPSData.dbo.Counties WHERE id = {hotline}.{#county}Id)", FieldType.NVarChar));
			if (provider == Provider.DV)
				result.Add(new Field("{hotline}.{#timeOfDay}", FieldType.Time) { Label = "Time of Day" });
			result.Add(new Field("{hotline}.{#totalTime}", FieldType.Minutes) { Label = "Total Time (minutes)" });
			result.Add(new LookupField("{hotline}.{#referralFrom}Id", Lookups.ReferralSource[provider]) { Label = "Referred From" });
			result.Add(new LookupField("{hotline}.{#referralTo}Id", Lookups.ReferralSource[provider]) { Label = "Referred To" });
			if (provider == Provider.SA) {
				result.Add(new Field("{hotline}.{#age}", FieldType.Quantity));
				result.Add(new LookupField("{hotline}.{#sex}Id", Lookups.Sex[provider]) { Label = "Gender Identity" });
				result.Add(new LookupField("{hotline}.{#race}Id", Lookups.Race[provider]) { Label = "Race/Ethnicity" });
				result.Add(new LookupField("{hotline}.{#clientType}Id", Lookups.ClientType[provider]) { Label = "Non-Client Type" });
			}
			return result;
		}
		#endregion

		#region other activity entity
		private static Entity AddOtherActivity(Model model, Perspective perspective) {
			var result = model.Add(new Entity("otherActivity", "Ts_OtherStaffActivity") { Label = "Other Staff Activity" });
			result.Key = new[] {
				result.Add(new Field("{otherActivity}.osa{#Id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Staff)
				result.Add(new Field("{otherActivity}.{#svId}", FieldType.Id, STAFF_SELECT_SQL) { NotEmpty = true, Label = "Staff/Volunteer", OptionsSql = STAFF_OPTIONS_SQL });
			result.Add(new Field("{otherActivity}.{#otherStaffActivity}Id", FieldType.Id, OTHERACTIVITY_SELECT_SQL) { OptionsSql = OTHERACTIVITY_OPTIONS_SQL });
			result.Add(new Field("{otherActivity}.{#conductingHours}", FieldType.Hours) { NotEmpty = true, Label = "Conduct Hours" });
			result.Add(new Field("{otherActivity}.{#travelHours}", FieldType.Hours) { NotEmpty = true });
			result.Add(new Field("{otherActivity}.{#prepareHours}", FieldType.Hours) { NotEmpty = true });
			new DateField("{otherActivity}.osa{#Date}") { NotEmpty = true }.AddTo(result, true);
			return result;
		}
		#endregion

		#region turn away entity
		private static Entity AddTurnAway(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("(SELECT * FROM Ts_{#TurnAway}Services WHERE locationId IN (SELECT centerId FROM T_Center WHERE parentCenterId = @centerId))"));
			result.Key = new[] {
				result.Add(new Field("{turnAway}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			result.Add(new Field("{turnAway}.{#adults}No", FieldType.Quantity) { NotEmpty = true, Label = "Number of Adults" });
			result.Add(new Field("{turnAway}.{#children}No", FieldType.Quantity) { NotEmpty = true, Label = "Number of Children" });
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{turnAway}.{#location}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			result.Add(new LookupField("{turnAway}.{#referralMade}Id", Lookups.YesNo[provider]) { NotEmpty = true, Label = "Referral Made to Another Shelter?" });
			new DateField("{turnAway}.turnAway{#Date}") { NotEmpty = true }.AddTo(result, true);
			return result;
		}
		#endregion

		#region outcome entity
		private static Entity AddOutcome(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("(SELECT * FROM Ts_Service{#Outcome} WHERE locationId IN (SELECT centerId FROM T_Center WHERE parentCenterId = @centerId))") { Label = "Service Outcomes" });
			result.Key = new[] {
				result.Add(new Field("{outcome}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{outcome}.{#location}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			new DateField("{outcome}.outcome{#Date}") { NotEmpty = true }.AddTo(result, true);
			result.Add(new LookupField("{outcome}.{#service}Id", Lookups.ServiceCategory[provider]) { NotEmpty = true, Label = "Client Service Group" });
			result.Add(new LookupField("{outcome}.{#outcome}Id", Lookups.ServiceOutcome[provider]) { NotEmpty = true, Label = "Survey Question" });
			result.Add(new Field("{outcome}.{#responseYes}", FieldType.Quantity) { NotEmpty = true, Label = "Number of YES Responses" });
			result.Add(new Field("{outcome}.{#responseNo}", FieldType.Quantity) { NotEmpty = true, Label = "Number of NO Responses" });
			return result;
		}
		#endregion

		#region hms entity
		private static Entity AddHms(Model model, Provider provider, Perspective perspective) {
			var result = model.Add(new Entity("hms", "(SELECT * FROM Ts_HivMentalSubstance WHERE locationId IN (SELECT centerId FROM T_Center WHERE parentCenterId = @centerId))") { Label = "Aggregate Client Information" });
			result.Key = new[] {
				result.Add(new Field("{hms}.{#id}", FieldType.Sequential) { NotEmpty = true })
			};
			if (perspective != Perspective.Center && perspective != Perspective.Staff)
				result.Add(new Field("{hms}.{#location}Id", FieldType.Id, LOCATION_SELECT_SQL) { NotEmpty = true, OptionsSql = LOCATION_OPTIONS_SQL });
			result.Add(new LookupField("{hms}.{#type}Id", Lookups.HivMentalSubstance[provider]) { NotEmpty = true, Label = "Type of Information" });
			new DateField("{hms}.hms{#Date}") { NotEmpty = true }.AddTo(result, true);
			result.Add(new Field("{hms}.{#adults}No", FieldType.Quantity) { NotEmpty = true, Label = "Number of Adults" });
			result.Add(new Field("{hms}.{#children}No", FieldType.Quantity) { NotEmpty = true, Label = "Number of Children" });
			return result;
		}
		#endregion
	}
}