using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "Police,Hospital,Medical,MedicalAdvocacyProgram,LegalSystem,Clergy,SocialServiceProgram,EducationSystem,Friend,Relative,Self,Other,WhatOther,PrivateAttorney,PublicHealth,Media,StateAttorney,CircuitClerk,DCFS,ToPolice,ToHospital,ToMedical,ToLegalSystem,ToClergy,ToSocialServiceProgram,ToEducationSystem,ToEducationSystem,ToOther,ToWhatOther,ToPrivateAttorney,ToPublicHealth,ToCircuitClerk,ToStateAttorney,ToCircuitClerk,AgencyName,AgencyID,ChildAdvocacyCenter,OtherRapeCrisisCenter,StatewideHelpLine,NationalHotline,OtherLocalHotline,HousingProgram,Hotline,SexualAssaultProgram,OtherDVProgram,ToHousingProgram,ToSexualAssaultProgram,ToOtherDVProgram,ToDCFS")]
	public class ClientReferralSource : IRevisable {
		public int ClientID { get; set; }
		public int CaseID { get; set; }

		[Display(Name = "Law Enforcement")]
		public bool Police { get; set; }

		[Display(Name = "Hospital")]
		public bool Hospital { get; set; }

		[Display(Name = "Medical")]
		public bool Medical { get; set; }

		[Display(Name = "Medical Advocacy Program")]
		public bool MedicalAdvocacyProgram { get; set; }

		[Display(Name = "Legal System")]
		public bool LegalSystem { get; set; }

		[Display(Name = "Clergy")]
		public bool Clergy { get; set; }

		[Display(Name = "Social Services Program")]
		public bool SocialServiceProgram { get; set; }

		[Display(Name = "Education System")]
		public bool EducationSystem { get; set; }

		[Display(Name = "Friend")]
		public bool Friend { get; set; }

		[Display(Name = "Relative")]
		public bool Relative { get; set; }

		[Display(Name = "Self")]
		public bool Self { get; set; }

		#region obsolete
		[Obsolete]
		public bool OtherProject { get; set; }

		[Obsolete]
		public bool Telephone { get; set; }
		#endregion

		[Display(Name = "Other")]
		public bool Other { get; set; }

		[Display(Name = "Other")]
        [MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string WhatOther { get; set; }

		[Display(Name = "Private Attorney")]
		public bool PrivateAttorney { get; set; }

		[Display(Name = "Public Health")]
		public bool PublicHealth { get; set; }

		[Display(Name = "Media")]
		public bool Media { get; set; }

		[Display(Name = "State's Attorney")]
		public bool StateAttorney { get; set; }

		[Display(Name = "Circuit Clerk")]
		public bool CircuitClerk { get; set; }

		[Display(Name = "DCFS")]
		public bool DCFS { get; set; }

		[Display(Name = "Law Enforcement")]
		public bool ToPolice { get; set; }

		[Display(Name = "Hospital")]
		public bool ToHospital { get; set; }

		[Display(Name = "Medical")]
		public bool ToMedical { get; set; }

		#region obsolete
		[Obsolete]
		public bool ToMedicalAdvocacyProgram { get; set; }
		#endregion

		[Display(Name = "Legal System")]
		public bool ToLegalSystem { get; set; }

		[Display(Name = "Clergy")]
		public bool ToClergy { get; set; }

		[Display(Name = "Social Services Program")]
		public bool ToSocialServiceProgram { get; set; }

		[Display(Name = "Education System")]
		public bool ToEducationSystem { get; set; }
	
		#region obsolete
		[Obsolete]
		public bool ToOtherProject { get; set; }
		#endregion

		[Display(Name = "Other")]
		public bool ToOther { get; set; }

		[Display(Name = "Other")]
        [MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string ToWhatOther { get; set; }

		[Display(Name = "Private Attorney")]
		public bool ToPrivateAttorney { get; set; }

		[Display(Name = "Public Health")]
		public bool ToPublicHealth { get; set; }

		[Display(Name = "State's Attorney")]
		public bool ToStateAttorney { get; set; }

		[Display(Name = "Circuit Clerk")]
		public bool ToCircuitClerk { get; set; }

		[Display(Name = "Center Hotline")]
		public bool Hotline { get; set; }

		[Display(Name = "Agency Name")]
		public string AgencyName { get; set; }

		public DateTime? RevisionStamp { get; set; }

		[Display(Name = "Agency Name")]
		public int? AgencyID { get; set; }

		[Display(Name = "Child Advocacy Center")]
		public bool ChildAdvocacyCenter { get; set; }

		[Display(Name = "Other Rape Crisis Center")]
		public bool OtherRapeCrisisCenter { get; set; }

		[Display(Name = "Illinois DV Helpline")]
		public bool StatewideHelpLine { get; set; }

		[Display(Name = "National DV Hotline")]
		public bool NationalHotline { get; set; }

		[Display(Name = "Other Local Hotline")]
		public bool OtherLocalHotline { get; set; }

		[Display(Name = "Housing Program")]
		public bool HousingProgram { get; set; }

		[Display(Name = "Sexual Assault Program")]
		public bool SexualAssaultProgram { get; set; }

		[Display(Name = "Other DV Program")]
		public bool OtherDVProgram { get; set; }

		[Display(Name = "Housing Program")]
		public bool ToHousingProgram { get; set; }

		[Display(Name = "Sexual Assault Program")]
		public bool ToSexualAssaultProgram { get; set; }

		[Display(Name = "Other DV Program")]
		public bool ToOtherDVProgram { get; set; }

		[Display(Name = "DCFS")]
		public bool ToDCFS { get; set; }

		public virtual Agency Agency { get; set; }
		public virtual ClientCase ClientCase { get; set; }
	}
}