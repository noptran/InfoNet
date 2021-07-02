using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "Shelter,Housing,Medical,Transportation,Emotional,ChildCare,Financial,Legal,Employment,Education,Referral,LegalAdvocacy,MedicalAdvocacy,CrisisIntervention,LockUp,Therapy,IndividualSupportChild,GroupActivity,SchoolAdvocacyChild,ParentChildSupport,CommunityAdvocacyChild")]
	public class ServiceGot : IRevisable {
		public int ClientID { get; set; }

		public int CaseID { get; set; }

		[Display(Name = "Shelter")]
		public bool Shelter { get; set; }

		[Display(Name = "Housing")]
		public bool Housing { get; set; }

		[Display(Name = "Medical Services")]
		public bool Medical { get; set; }

		[Display(Name = "Transportation")]
		public bool Transportation { get; set; }

		[Display(Name = "Emotional/Counseling")]
		public bool Emotional { get; set; }

		[Display(Name = "Child Care")]
		public bool ChildCare { get; set; }

		[Display(Name = "Financial")]
		public bool Financial { get; set; }

		[Display(Name = "Legal Services")]
		public bool Legal { get; set; }

		[Display(Name = "Employment")]
		public bool Employment { get; set; }

		[Display(Name = "Education")]
		public bool Education { get; set; }

		[Display(Name = "Referral")]
		public bool Referral { get; set; }

		[Display(Name = "Legal Advocacy")]
		public bool LegalAdvocacy { get; set; }

		[Display(Name = "Medical Advocacy")]
		public bool MedicalAdvocacy { get; set; }

		[Display(Name = "Crisis Intervention")]
		public bool CrisisIntervention { get; set; }

		[Display(Name = "Lock Up/Board Up")]
		public bool LockUp { get; set; }

		[Display(Name = "Therapy")]
		public bool Therapy { get; set; }

		[Display(Name = "Individual Support (Child)")]
		public bool IndividualSupportChild { get; set; }

		[Display(Name = "Group Activity (Child)")]
		public bool GroupActivity { get; set; }

		[Display(Name = "School Advocacy (Child)")]
		public bool SchoolAdvocacyChild { get; set; }

		[Display(Name = "Parent/Child Support (Child)")]
		public bool ParentChildSupport { get; set; }

		[Display(Name = "Community Advocacy (Child)")]
		public bool CommunityAdvocacyChild { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual ClientCase ClientCase { get; set; }
	}
}