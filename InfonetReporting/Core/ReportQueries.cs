using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Offenders;
using Infonet.Data.Models.Services;

namespace Infonet.Reporting.Core {
	public static class ReportQueries {
		public static ReportQuery<AbuseNeglectPetition> AbuseNeglectPetition() {
			return new ReportQuery<AbuseNeglectPetition>(
				rc => rc.InfonetContext.Ts_AbuseNeglectPetitions.AsNoTracking(),
				fc => fc.AbuseNeglectPetition);
		}

		public static ReportQuery<Cancellation> Cancellation() {
			return new ReportQuery<Cancellation>(
				rc => rc.InfonetContext.Tl_Cancellations.AsNoTracking(),
				fc => fc.Cancellation);
		}

		public static ReportQuery<ChildBehavioralIssues> ChildBehavioralIssues() {
			return new ReportQuery<ChildBehavioralIssues>(
				rc => rc.InfonetContext.Ts_ClientChildBehavioralIssues.AsNoTracking(),
				fc => fc.ChildBehavioralIssues);
		}

		public static ReportQuery<ClientCase> ClientCase() {
			return new ReportQuery<ClientCase>(
				rc => rc.InfonetContext.T_ClientCases.AsNoTracking(),
				fc => fc.ClientCase);
		}

		public static ReportQuery<ClientCJProcess> ClientCjProcess() {
			return new ReportQuery<ClientCJProcess>(
				rc => rc.InfonetContext.Ts_ClientCJProcess.AsNoTracking(),
				fc => fc.ClientCjProcess);
		}

		public static ReportQuery<ClientMDT> ClientMDT() {
			return new ReportQuery<ClientMDT>(
				rc => rc.InfonetContext.Tl_ClientMDT.AsNoTracking(),
				fc => fc.ClientMDT);
		}

		public static ReportQuery<ClientPoliceProsecution> ClientPoliceProsecution() {
			return new ReportQuery<ClientPoliceProsecution>(
				rc => rc.InfonetContext.Ts_ClientPoliceProsecution.AsNoTracking(),
				fc => fc.ClientPoliceProsecution);
		}

		public static ReportQuery<ClientReferralDetail> ClientReferralDetail() {
			return new ReportQuery<ClientReferralDetail>(
				rc => rc.InfonetContext.Ts_ClientReferralDetail.AsNoTracking(),
				fc => fc.ClientReferralDetail);
		}

		public static ReportQuery<DCFSAllegation> DCFSAllegation() {
			return new ReportQuery<DCFSAllegation>(
				rc => rc.InfonetContext.Ts_DCFSAllegations.AsNoTracking(),
				fc => fc.DCFSAllegation);
		}

		public static ReportQuery<EventDetail> EventDetail() {
			return new ReportQuery<EventDetail>(
				rc => rc.InfonetContext.Tl_EventDetail.AsNoTracking(),
				fc => fc.EventDetail);
		}

		public static ReportQuery<EventDetailStaff> EventDetailStaff() {
			return new ReportQuery<EventDetailStaff>(
				rc => rc.InfonetContext.Ts_EventDetail_Staffs.AsNoTracking(),
				fc => fc.EventDetailStaff);
		}

		public static ReportQuery<HivMentalSubstance> HivMentalSubstance() {
			return new ReportQuery<HivMentalSubstance>(
				rc => rc.InfonetContext.Ts_HivMentalSubstance.AsNoTracking(),
				fc => fc.HivMentalSubstance);
		}

		public static ReportQuery<Offender> Offender() {
			return new ReportQuery<Offender>(
				rc => rc.InfonetContext.T_Offender.AsNoTracking(),
				fc => fc.Offender);
		}

		public static ReportQuery<OrderOfProtection> OrderOfProtection() {
			return new ReportQuery<OrderOfProtection>(
				rc => rc.InfonetContext.Ts_OrderOfProtection.AsNoTracking(),
				fc => fc.OrderOfProtection);
		}

		public static ReportQuery<OtherStaffActivity> OtherStaffActivity() {
			return new ReportQuery<OtherStaffActivity>(
				rc => rc.InfonetContext.Ts_OtherStaffActivity.AsNoTracking(),
				fc => fc.OtherStaffActivity);
		}

		public static ReportQuery<PhoneHotline> PhoneHotline() {
			return new ReportQuery<PhoneHotline>(
				rc => rc.InfonetContext.T_PhoneHotline.AsNoTracking(),
				fc => fc.PhoneHotline);
		}

		public static ReportQuery<PoliceCharge> PoliceCharge() {
			return new ReportQuery<PoliceCharge>(
				rc => rc.InfonetContext.Ts_PoliceCharge.AsNoTracking(),
				fc => fc.PoliceCharge);
		}

		public static ReportQuery<ProgramDetail> ProgramDetail() {
			return new ReportQuery<ProgramDetail>(
				rc => rc.InfonetContext.Tl_ProgramDetail.AsNoTracking(),
				fc => fc.ProgramDetail);
		}

		public static ReportQuery<ProgramDetailStaff> ProgramDetailStaff() {
			return new ReportQuery<ProgramDetailStaff>(
				rc => rc.InfonetContext.Ts_ProgramDetail_Staffs.AsNoTracking(),
				fc => fc.ProgramDetailStaff);
		}

		public static ReportQuery<PublicationDetail> PublicationDetail() {
			return new ReportQuery<PublicationDetail>(
				rc => rc.InfonetContext.Tl_PublicationDetail.AsNoTracking(),
				fc => fc.PublicationDetail);
		}

		public static ReportQuery<PublicationDetailStaff> PublicationDetailStaff() {
			return new ReportQuery<PublicationDetailStaff>(
				rc => rc.InfonetContext.Ts_PublicationDetail_Staffs.AsNoTracking(),
				fc => fc.PublicationDetailStaff);
		}

		public static ReportQuery<ServiceDetailOfClient> ServiceDetailOfClient() {
			return new ReportQuery<ServiceDetailOfClient>(
				rc => rc.InfonetContext.Tl_ServiceDetailOfClient.AsNoTracking(),
				fc => fc.ServiceDetailOfClient);
		}

		public static ReportQuery<ServiceOutcome> ServiceOutcome() {
			return new ReportQuery<ServiceOutcome>(
				rc => rc.InfonetContext.Ts_ServiceOutcome.AsNoTracking(),
				fc => fc.ServiceOutcome);
		}

		public static ReportQuery<TrialCharge> TrialCharge() {
			return new ReportQuery<TrialCharge>(
				rc => rc.InfonetContext.Ts_TrialCharges.AsNoTracking(),
				fc => fc.TrialCharge);
		}

		public static ReportQuery<TurnAwayService> TurnAway() {
			return new ReportQuery<TurnAwayService>(
				rc => rc.InfonetContext.Ts_TurnAwayServices.AsNoTracking(),
				fc => fc.TurnAwayService);
		}

		public static ReportQuery<VictimSensitiveInterview> VictimSensitiveInterview() {
			return new ReportQuery<VictimSensitiveInterview>(
				rc => rc.InfonetContext.Ts_VictimSensitiveInterviews.AsNoTracking(),
				fc => fc.VictimSensitiveInterview);
		}
	}
}