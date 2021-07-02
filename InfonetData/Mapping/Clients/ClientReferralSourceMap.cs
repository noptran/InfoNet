using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Clients {
	public class ClientReferralSourceMap : EntityTypeConfiguration<ClientReferralSource> {
		public ClientReferralSourceMap() {
			// Primary Key
			HasKey(t => new { t.ClientID, t.CaseID });

			// Properties
			Property(t => t.ClientID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CaseID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.WhatOther)
				.HasMaxLength(50);

			Property(t => t.ToWhatOther)
				.HasMaxLength(100);

			Property(t => t.AgencyName)
				.HasMaxLength(50);

			// Table & Column Mappings
			ToTable("Ts_ClientReferralSource");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.Police).HasColumnName("Police");
			Property(t => t.Hospital).HasColumnName("Hospital");
			Property(t => t.Medical).HasColumnName("Medical");
			Property(t => t.MedicalAdvocacyProgram).HasColumnName("MedicalAdvocacyProgram");
			Property(t => t.LegalSystem).HasColumnName("LegalSystem");
			Property(t => t.Clergy).HasColumnName("Clergy");
			Property(t => t.SocialServiceProgram).HasColumnName("SocialServiceProgram");
			Property(t => t.EducationSystem).HasColumnName("EducationSystem");
			Property(t => t.Friend).HasColumnName("Friend");
			Property(t => t.Relative).HasColumnName("Relative");
			Property(t => t.Self).HasColumnName("Self");
			Property(t => t.OtherProject).HasColumnName("OtherProject");
			Property(t => t.Telephone).HasColumnName("Telephone");
			Property(t => t.Other).HasColumnName("Other");
			Property(t => t.WhatOther).HasColumnName("WhatOther");
			Property(t => t.PrivateAttorney).HasColumnName("PrivateAttorney");
			Property(t => t.PublicHealth).HasColumnName("PublicHealth");
			Property(t => t.Media).HasColumnName("Media");
			Property(t => t.StateAttorney).HasColumnName("StateAttorney");
			Property(t => t.CircuitClerk).HasColumnName("CircuitClerk");
			Property(t => t.DCFS).HasColumnName("DCFS");
			Property(t => t.ToPolice).HasColumnName("ToPolice");
			Property(t => t.ToHospital).HasColumnName("ToHospital");
			Property(t => t.ToMedical).HasColumnName("ToMedical");
			Property(t => t.ToMedicalAdvocacyProgram).HasColumnName("ToMedicalAdvocacyProgram");
			Property(t => t.ToLegalSystem).HasColumnName("ToLegalSystem");
			Property(t => t.ToClergy).HasColumnName("ToClergy");
			Property(t => t.ToSocialServiceProgram).HasColumnName("ToSocialServiceProgram");
			Property(t => t.ToEducationSystem).HasColumnName("ToEducationSystem");
			Property(t => t.ToOtherProject).HasColumnName("ToOtherProject");
			Property(t => t.ToOther).HasColumnName("ToOther");
			Property(t => t.ToWhatOther).HasColumnName("ToWhatOther");
			Property(t => t.ToPrivateAttorney).HasColumnName("ToPrivateAttorney");
			Property(t => t.ToPublicHealth).HasColumnName("ToPublicHealth");
			Property(t => t.ToStateAttorney).HasColumnName("ToStateAttorney");
			Property(t => t.ToCircuitClerk).HasColumnName("ToCircuitClerk");
			Property(t => t.Hotline).HasColumnName("Hotline");
			Property(t => t.AgencyName).HasColumnName("AgencyName");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.AgencyID).HasColumnName("AgencyID");
			Property(t => t.ChildAdvocacyCenter).HasColumnName("ChildAdvocacyCenter");
			Property(t => t.OtherRapeCrisisCenter).HasColumnName("OtherRapeCrisisCenter");
			Property(t => t.StatewideHelpLine).HasColumnName("StatewideHelpLine");
			Property(t => t.NationalHotline).HasColumnName("NationalHotline");
			Property(t => t.OtherLocalHotline).HasColumnName("OtherLocalHotline");
			Property(t => t.HousingProgram).HasColumnName("HousingProgram");
			Property(t => t.SexualAssaultProgram).HasColumnName("SexualAssaultProgram");
			Property(t => t.OtherDVProgram).HasColumnName("OtherDVProgram");
			Property(t => t.ToHousingProgram).HasColumnName("ToHousingProgram");
			Property(t => t.ToSexualAssaultProgram).HasColumnName("ToSexualAssaultProgram");
			Property(t => t.ToOtherDVProgram).HasColumnName("ToOtherDVProgram");
			Property(t => t.ToDCFS).HasColumnName("ToDCFS");

			// Relationships
			HasOptional(t => t.Agency)
				.WithMany(t => t.ClientReferralSources)
				.HasForeignKey(d => d.AgencyID);
			HasRequired(t => t.ClientCase)
				.WithOptional(t => t.ClientReferralSource);
		}
	}
}