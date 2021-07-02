using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Clients {
	public class PresentingIssuesMap : EntityTypeConfiguration<PresentingIssues> {
		public PresentingIssuesMap() {
			// Primary Key
			HasKey(t => new { t.ClientId, t.CaseId });

			// Properties
			Property(t => t.ClientId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CaseId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.Comment).HasMaxLength(256);

			Property(t => t.RapeOrSexualAssault)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.AdultSurvivor)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Stalking)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Harassment)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.PhysicalDomesticViolence)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.SexualDomesticViolence)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.EmotionalDomesticViolence)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.DomesticBattery)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.AggravatedDomesticBattery)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.ViolationOfOOP)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Assault)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Battery)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.AssaultAndOrBattery)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.HateCrime)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.UnknownOffense)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.ChildAbuse)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.ChildNeglect)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.ChildSexualAssault)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.ElderAbuse)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Homicide)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.AttemptedHomicide)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.HomeInvasion)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Robbery)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Burglary)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.OtherOffenseAgPerson)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.OtherOffense)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.WhatOther).HasMaxLength(255);

			Property(t => t.DwiDui)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.DateRape)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Drugged)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Exploitation)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.FondlingOverClothes)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.FondlingUnderClothes)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.IntercourseVaginal)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.IntercourseAnal)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Masturbation)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Oral)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.PenetrationDigital)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.PenetrationObjectile)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Solicitation)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.SexualOther)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.SexualComment).HasMaxLength(255);

			Property(t => t.BoneFractures)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.BrainDamage)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Burn)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Death)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.InternalInjuries)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Poison)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Sprains)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Shaken)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.SubduralHematoma)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Torture)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Wounds)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.PhysicalOther)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.PhysicalComment).HasMaxLength(255);

            Property(t => t.HumanLaborTrafficking)
               .IsRequired()
               .IsFixedLength()
               .HasMaxLength(1);

            Property(t => t.HumanSexTrafficking)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            Property(t => t.FinancialAbuse)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            Property(t => t.SpiritualAbuse)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            ToTable("Ts_ClientPresentingIssue");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.PrimaryPresentingIssueID).HasColumnName("PrimaryPresentingIssueID");
			Property(t => t.DateOfPrimOffense).HasColumnName("DateOfPrimOffense");
			Property(t => t.EndDateOfAbuse).HasColumnName("EndDateOfAbuse");
			Property(t => t.Comment).HasColumnName("Comment");
			Property(t => t.LocOfPrimOffenseID).HasColumnName("LocOfPrimOffenseID");
			Property(t => t.CountyID).HasColumnName("CountyID");
			Property(t => t.RapeOrSexualAssault).HasColumnName("RapeOrSexualAssault");
			Property(t => t.AdultSurvivor).HasColumnName("AdultSurvivor");
			Property(t => t.Stalking).HasColumnName("Stalking");
			Property(t => t.Harassment).HasColumnName("Harassment");
			Property(t => t.PhysicalDomesticViolence).HasColumnName("PhysicalDomesticViolence");
			Property(t => t.SexualDomesticViolence).HasColumnName("SexualDomesticViolence");
			Property(t => t.EmotionalDomesticViolence).HasColumnName("EmotionalDomesticViolence");
			Property(t => t.DomesticBattery).HasColumnName("DomesticBattery");
			Property(t => t.AggravatedDomesticBattery).HasColumnName("AggravatedDomesticBattery");
			Property(t => t.ViolationOfOOP).HasColumnName("ViolationOfOOP");
			Property(t => t.Assault).HasColumnName("Assault");
			Property(t => t.Battery).HasColumnName("Battery");
			Property(t => t.AssaultAndOrBattery).HasColumnName("AssaultAndOrBattery");
			Property(t => t.HateCrime).HasColumnName("HateCrime");
			Property(t => t.UnknownOffense).HasColumnName("UnknownOffense");
			Property(t => t.ChildAbuse).HasColumnName("ChildAbuse");
			Property(t => t.ChildNeglect).HasColumnName("ChildNeglect");
			Property(t => t.ChildSexualAssault).HasColumnName("ChildSexualAssault");
			Property(t => t.ElderAbuse).HasColumnName("ElderAbuse");
			Property(t => t.Homicide).HasColumnName("Homicide");
			Property(t => t.AttemptedHomicide).HasColumnName("AttemptedHomicide");
			Property(t => t.HomeInvasion).HasColumnName("HomeInvasion");
			Property(t => t.Robbery).HasColumnName("Robbery");
			Property(t => t.Burglary).HasColumnName("Burglary");
			Property(t => t.OtherOffenseAgPerson).HasColumnName("OtherOffenseAgPerson");
			Property(t => t.OtherOffense).HasColumnName("OtherOffense");
			Property(t => t.WhatOther).HasColumnName("WhatOther");
			Property(t => t.DwiDui).HasColumnName("DwiDui");
			Property(t => t.DateRape).HasColumnName("DateRape");
			Property(t => t.Drugged).HasColumnName("Drugged");
			Property(t => t.Exploitation).HasColumnName("Exploitation");
			Property(t => t.FondlingOverClothes).HasColumnName("FondingOverClothes");
			Property(t => t.FondlingUnderClothes).HasColumnName("FondingUnderClothes");
			Property(t => t.IntercourseVaginal).HasColumnName("IntercourseVaginal");
			Property(t => t.IntercourseAnal).HasColumnName("IntercourseAnal");
			Property(t => t.Masturbation).HasColumnName("Masturbation");
			Property(t => t.Oral).HasColumnName("Oral");
			Property(t => t.PenetrationDigital).HasColumnName("PenetrationDigital");
			Property(t => t.PenetrationObjectile).HasColumnName("PenetrationObjectile");
			Property(t => t.Solicitation).HasColumnName("Solicitation");
			Property(t => t.SexualOther).HasColumnName("SexualOther");
			Property(t => t.SexualComment).HasColumnName("SexualComment");
			Property(t => t.BoneFractures).HasColumnName("BoneFractures");
			Property(t => t.BrainDamage).HasColumnName("BrainDamage");
			Property(t => t.Burn).HasColumnName("Burn");
			Property(t => t.Death).HasColumnName("Death");
			Property(t => t.InternalInjuries).HasColumnName("InternalInjuries");
			Property(t => t.Poison).HasColumnName("Poison");
			Property(t => t.Sprains).HasColumnName("Sprains");
			Property(t => t.Shaken).HasColumnName("Shaken");
			Property(t => t.SubduralHematoma).HasColumnName("SubduralHematoma");
			Property(t => t.Torture).HasColumnName("Torture");
			Property(t => t.Wounds).HasColumnName("Wounds");
			Property(t => t.PhysicalOther).HasColumnName("PhysicalOther");
			Property(t => t.PhysicalComment).HasColumnName("PhysicalComment");
            Property(t => t.HumanLaborTrafficking).HasColumnName("HumanLaborTrafficking");
            Property(t => t.HumanSexTrafficking).HasColumnName("HumanSexTrafficking");
            Property(t => t.FinancialAbuse).HasColumnName("FinancialAbuse");
            Property(t => t.SpiritualAbuse).HasColumnName("SpiritualAbuse");
            Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.TownshipID).HasColumnName("TownshipID");
			Property(t => t.CityID).HasColumnName("CityID");
			Property(t => t.StateID).HasColumnName("StateID");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithOptional(t => t.PresentingIssues);
		}
	}
}