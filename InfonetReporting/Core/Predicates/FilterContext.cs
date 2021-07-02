using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Offenders;
using Infonet.Data.Models.Services;

namespace Infonet.Reporting.Core.Predicates {
	public class FilterContext {
		public FilterContext() {
			/* VERTICES */
			AbuseNeglectPetition = new Vertex<AbuseNeglectPetition>();
			Cancellation = new Vertex<Cancellation>();
			ChildBehavioralIssues = new Vertex<ChildBehavioralIssues>();
			Client = new Vertex<Client>();
			ClientCase = new Vertex<ClientCase>();
			ClientMDT = new Vertex<ClientMDT>();
			ClientCjProcess = new Vertex<ClientCJProcess>();
			ClientPoliceProsecution = new Vertex<ClientPoliceProsecution>();
			ClientRace = new Vertex<ClientRace>();
			ClientReferralDetail = new ClientReferralDetailVertex();
			DCFSAllegation = new Vertex<DCFSAllegation>();
			EventDetail = new Vertex<EventDetail>();
			EventDetailStaff = new EventDetailStaffVertex();
			HivMentalSubstance = new Vertex<HivMentalSubstance>();
			Offender = new Vertex<Offender>();
			OrderOfProtection = new Vertex<OrderOfProtection>();
			OtherStaffActivity = new Vertex<OtherStaffActivity>();
			PhoneHotline = new PhoneHotlineVertex();
			PoliceCharge = new Vertex<PoliceCharge>();
			ProgramDetail = new Vertex<ProgramDetail>();
			ProgramDetailStaff = new ProgramDetailStaffVertex();
			PublicationDetail = new Vertex<PublicationDetail>();
			PublicationDetailStaff = new PublicationDetailStaffVertex();
			ServiceOutcome = new Vertex<ServiceOutcome>();
			ServiceDetailOfClient = new ServiceDetailOfClientVertex();
			TrialCharge = new Vertex<TrialCharge>();
			TurnAwayService = new Vertex<TurnAwayService>();
			VictimSensitiveInterview = new Vertex<VictimSensitiveInterview>();

			/* REVERSIBLE EDGES */
			Client.AddEdge(ClientRace, c => c.ClientRaces, r => r.Client);
			ClientCase.AddEdge(Cancellation, cc => cc.Cancellations, c => c.ClientCase);
			ClientCase.AddEdge(Client, cc => cc.Client, c => c.ClientCases);
			ClientCase.AddEdge(Offender, cc => cc.Offenders, o => o.ClientCase);
			ClientCase.AddEdge(ClientPoliceProsecution, cc => cc.ClientPoliceProsecutions, cpp => cpp.ClientCase);
			ClientCase.AddEdge(ClientReferralDetail, cc => cc.ClientReferralDetail, crd => crd.ClientCase);
			ClientCase.AddEdge(ChildBehavioralIssues, cc => cc.ChildBehavioralIssues, cbi => cbi.ClientCase);
			ClientCase.AddEdge(DCFSAllegation, cc => cc.DCFSAllegations, da => da.ClientCase);
			ClientCase.AddEdge(AbuseNeglectPetition, cc => cc.AbuseNeglectPetitions, anp => anp.ClientCase);
			ClientCase.AddEdge(OrderOfProtection, cc => cc.OrdersOfProtection, oop => oop.ClientCase);
			ClientCase.AddEdge(VictimSensitiveInterview, cc => cc.VictimSensitiveInterviews, vsi => vsi.ClientCase);
			ClientCase.AddEdge(ClientMDT, cc => cc.ClientMDT, mdt => mdt.ClientCase);
			ClientCjProcess.AddEdge(ClientCase, cjp => cjp.ClientCase, cc => cc.ClientCJProcesses);
			ClientReferralDetail.AddEdge(ClientReferralDetail.TwnTshipCounty, r => r.TwnTshipCounty, t => t.ClientReferralDetails);
			EventDetail.AddEdge(EventDetailStaff, ed => ed.EventDetailStaff, eds => eds.EventDetail);
			Offender.AddEdge(PoliceCharge, o => o.PoliceCharges, pc => pc.Offender);
			Offender.AddEdge(TrialCharge, o => o.TrialCharges, tc => tc.Offender);
			ProgramDetail.AddEdge(ProgramDetailStaff, pd => pd.ProgramDetailStaff, pds => pds.ProgramDetail);
			PublicationDetail.AddEdge(PublicationDetailStaff, pd => pd.PublicationDetailStaff, pds => pds.PublicationDetail);
			ServiceDetailOfClient.AddEdge(ClientCase, s => s.ClientCase, cc => cc.ServiceDetailsOfClient);
			ServiceDetailOfClient.AddEdge(ServiceDetailOfClient.TwnTshipCounty, s => s.TwnTshipCounty, t => t.ServiceDetailsOfClient);
			ServiceDetailOfClient.AddEdge(ProgramDetail, sdc => sdc.Tl_ProgramDetail, pd => pd.ServiceDetailsOfClient);

			/* ONE-WAY EDGES */
			EventDetailStaff.AddEdge(EventDetailStaff.FundServiceProgramOfStaff, s => s.EventDetail.FundingDate.FundServiceProgramsOfStaff.Where(fs => fs.SVID == s.SVID && fs.ServiceProgramID == s.EventDetail.ProgramID));
			PhoneHotline.AddEdge(PhoneHotline.FundServiceProgramOfStaff, s => s.FundingDate.FundServiceProgramsOfStaff.Where(fs => fs.SVID == s.SVID && StaffFunding.HotlineServiceIds.Contains(fs.ServiceProgramID)));
			ProgramDetailStaff.AddEdge(ProgramDetailStaff.FundServiceProgramOfStaff, s => s.ProgramDetail.FundingDate.FundServiceProgramsOfStaff.Where(fs => fs.SVID == s.SVID && fs.ServiceProgramID == s.ProgramDetail.ProgramID));
			PublicationDetailStaff.AddEdge(PublicationDetailStaff.FundServiceProgramOfStaff, s => s.PublicationDetail.FundingDate.FundServiceProgramsOfStaff.Where(fs => fs.SVID == s.SVID && fs.ServiceProgramID == s.PublicationDetail.ProgramID));
			ServiceDetailOfClient.AddEdge(ServiceDetailOfClient.StaffFunding, StaffFunding.ServiceDetail);
		}

		public Vertex<AbuseNeglectPetition> AbuseNeglectPetition { get; }

		public Vertex<Cancellation> Cancellation { get; }

		public Vertex<ChildBehavioralIssues> ChildBehavioralIssues { get; }

		public Vertex<Client> Client { get; }

		public Vertex<ClientCase> ClientCase { get; }

		public Vertex<ClientCJProcess> ClientCjProcess { get; }

		public Vertex<ClientMDT> ClientMDT { get; }

		public Vertex<ClientPoliceProsecution> ClientPoliceProsecution { get; }

		public Vertex<ClientRace> ClientRace { get; }

		public ClientReferralDetailVertex ClientReferralDetail { get; }

		public Vertex<DCFSAllegation> DCFSAllegation { get; }

		public Vertex<EventDetail> EventDetail { get; }

		public EventDetailStaffVertex EventDetailStaff { get; }

		public Vertex<HivMentalSubstance> HivMentalSubstance { get; }

		public Vertex<Offender> Offender { get; }

		public Vertex<OrderOfProtection> OrderOfProtection { get; }

		public Vertex<OtherStaffActivity> OtherStaffActivity { get; }

		public PhoneHotlineVertex PhoneHotline { get; }

		public Vertex<PoliceCharge> PoliceCharge { get; }

		public Vertex<ProgramDetail> ProgramDetail { get; }

		public ProgramDetailStaffVertex ProgramDetailStaff { get; }

		public Vertex<PublicationDetail> PublicationDetail { get; }

		public PublicationDetailStaffVertex PublicationDetailStaff { get; }

		public ServiceDetailOfClientVertex ServiceDetailOfClient { get; }

		public Vertex<ServiceOutcome> ServiceOutcome { get; }

		public Vertex<TrialCharge> TrialCharge { get; }

		public Vertex<TurnAwayService> TurnAwayService { get; }

		public Vertex<VictimSensitiveInterview> VictimSensitiveInterview { get; }

		/** Returns 'this' to allow cascaded calls. **/
		public FilterContext Apply(ReportContainer container, params ReportFilter[] filters) {
			if (filters == null)
				throw new ArgumentException(nameof(filters));
			foreach (var each in filters)
				each.ApplyTo(this, container);
			return this;
		}

		#region vertices with aliased children
		public class ClientReferralDetailVertex : Vertex<ClientReferralDetail> {
			internal ClientReferralDetailVertex() {
				TwnTshipCounty = new Vertex<TwnTshipCounty>();
			}

			public Vertex<TwnTshipCounty> TwnTshipCounty { get; }
		}

		public class EventDetailStaffVertex : Vertex<EventDetailStaff> {
			internal EventDetailStaffVertex() {
				FundServiceProgramOfStaff = new Vertex<FundServiceProgramOfStaff>();
			}

			public Vertex<FundServiceProgramOfStaff> FundServiceProgramOfStaff { get; }
		}

		public class PhoneHotlineVertex : Vertex<PhoneHotline> {
			internal PhoneHotlineVertex() {
				FundServiceProgramOfStaff = new Vertex<FundServiceProgramOfStaff>();
			}

			public Vertex<FundServiceProgramOfStaff> FundServiceProgramOfStaff { get; }
		}

		public class ProgramDetailStaffVertex : Vertex<ProgramDetailStaff> {
			internal ProgramDetailStaffVertex() {
				FundServiceProgramOfStaff = new Vertex<FundServiceProgramOfStaff>();
			}

			public Vertex<FundServiceProgramOfStaff> FundServiceProgramOfStaff { get; }
		}

		public class PublicationDetailStaffVertex : Vertex<PublicationDetailStaff> {
			internal PublicationDetailStaffVertex() {
				FundServiceProgramOfStaff = new Vertex<FundServiceProgramOfStaff>();
			}

			public Vertex<FundServiceProgramOfStaff> FundServiceProgramOfStaff { get; }
		}

		public class ServiceDetailOfClientVertex : Vertex<ServiceDetailOfClient> {
			internal ServiceDetailOfClientVertex() {
				StaffFunding = new Vertex<StaffFunding>();
				TwnTshipCounty = new Vertex<TwnTshipCounty>();
			}

			[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
			public Vertex<StaffFunding> StaffFunding { get; }

			public Vertex<TwnTshipCounty> TwnTshipCounty { get; }
		}
		#endregion
	}
}