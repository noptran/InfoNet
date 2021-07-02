using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Entity;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "PrimaryPresentingIssueID,LocOfPrimOffenseID,DateOfPrimOffense,StartDateOfAbuse,CityID,TownshipID,IsRapeOrSexualAssault,IsAdultSurvivor,IsStalking,IsHarassment,IsChildSexualAssault,IsChildAbuse,IsPhysicalDomesticViolence,IsSexualDomesticViolence,IsEmotionalDomesticViolence,IsDomesticBattery,IsAggravatedDomesticBattery,IsViolationOfOOP,IsAssault,IsBattery,IsAssaultAndOrBattery,IsHateCrime,IsUnknownOffense,IsChildNeglect,IsElderAbuse,IsHomicide,IsAttemptedHomicide,IsHomeInvasion,IsRobbery,IsBurglary,IsOtherOffenseAgPerson,IsOtherOffense,IsDwiDui,IsDateRape,IsDrugged,EndDateOfAbuse,StateID,CountyID,Comment,IsFondlingOverClothesActive,IsFondlingOverClothesPassive,IsFondlingUnderClothesActive,IsFondlingUnderClothesPassive,IsExploitationPassive,IsIntercourseVaginalActive,IsIntercourseVaginalPassive,IsIntercourseAnalActive,IsIntercourseAnalPassive,IsMasturbationActive,IsMasturbationPassive,IsOralActive,IsOralPassive,IsPenetrationDigitalActive,IsPenetrationDigitalPassive,IsPenetrationObjectileActive,IsPenetrationObjectilePassive,IsSolicitationPassive,IsSexualOtherActive,IsSexualOtherPassive,SexualComment,IsBoneFractures,IsBrainDamage,IsBurn,IsDeath,IsInternalInjuries,IsPoison,IsSprains,IsShaken,IsSubduralHematoma,IsTorture,IsWounds,IsPhysicalOther,PhysicalComment,IsHumanLaborTrafficking,IsHumanSexTrafficking,IsFinancialAbuse,IsSpiritualAbuse")]
	public class PresentingIssues : IRevisable, IValidatableObject {
		public PresentingIssues() {
			RapeOrSexualAssault = new byte[1];
			AdultSurvivor = new byte[1];
			Stalking = new byte[1];
			Harassment = new byte[1];
			PhysicalDomesticViolence = new byte[1];
			SexualDomesticViolence = new byte[1];
			EmotionalDomesticViolence = new byte[1];
			DomesticBattery = new byte[1];
			AggravatedDomesticBattery = new byte[1];
			ViolationOfOOP = new byte[1];
			Assault = new byte[1];
			Battery = new byte[1];
			AssaultAndOrBattery = new byte[1];
			HateCrime = new byte[1];
			UnknownOffense = new byte[1];
			ChildAbuse = new byte[1];
			ChildNeglect = new byte[1];
			ChildSexualAssault = new byte[1];
			ElderAbuse = new byte[1];
			Homicide = new byte[1];
			AttemptedHomicide = new byte[1];
			HomeInvasion = new byte[1];
			Robbery = new byte[1];
			Burglary = new byte[1];
			OtherOffenseAgPerson = new byte[1];
			OtherOffense = new byte[1];
			DwiDui = new byte[1];
			DateRape = new byte[1];
			Drugged = new byte[1];
			Exploitation = new byte[1];
			FondlingOverClothes = new byte[1];
			FondlingUnderClothes = new byte[1];
			IntercourseVaginal = new byte[1];
			IntercourseAnal = new byte[1];
			Masturbation = new byte[1];
			Oral = new byte[1];
			PenetrationDigital = new byte[1];
			PenetrationObjectile = new byte[1];
			Solicitation = new byte[1];
			SexualOther = new byte[1];
			BoneFractures = new byte[1];
			BrainDamage = new byte[1];
			Burn = new byte[1];
			Death = new byte[1];
			InternalInjuries = new byte[1];
			Poison = new byte[1];
			Sprains = new byte[1];
			Shaken = new byte[1];
			SubduralHematoma = new byte[1];
			Torture = new byte[1];
			Wounds = new byte[1];
			PhysicalOther = new byte[1];
            HumanLaborTrafficking = new byte[1];
            HumanSexTrafficking = new byte[1];
            FinancialAbuse = new byte[1];
            SpiritualAbuse = new byte[1];

            RapeOrSexualAssault[0] = 0;
			AdultSurvivor[0] = 0;
			Stalking[0] = 0;
			Harassment[0] = 0;
			PhysicalDomesticViolence[0] = 0;
			SexualDomesticViolence[0] = 0;
			EmotionalDomesticViolence[0] = 0;
			DomesticBattery[0] = 0;
			AggravatedDomesticBattery[0] = 0;
			ViolationOfOOP[0] = 0;
			Assault[0] = 0;
			Battery[0] = 0;
			AssaultAndOrBattery[0] = 0;
			HateCrime[0] = 0;
			UnknownOffense[0] = 0;
			ChildAbuse[0] = 0;
			ChildNeglect[0] = 0;
			ChildSexualAssault[0] = 0;
			ElderAbuse[0] = 0;
			Homicide[0] = 0;
			AttemptedHomicide[0] = 0;
			HomeInvasion[0] = 0;
			Robbery[0] = 0;
			Burglary[0] = 0;
			OtherOffenseAgPerson[0] = 0;
			OtherOffense[0] = 0;
			DwiDui[0] = 0;
			DateRape[0] = 0;
			Drugged[0] = 0;
			Exploitation[0] = 0;
			FondlingOverClothes[0] = 0;
			FondlingUnderClothes[0] = 0;
			IntercourseVaginal[0] = 0;
			IntercourseAnal[0] = 0;
			Masturbation[0] = 0;
			Oral[0] = 0;
			PenetrationDigital[0] = 0;
			PenetrationObjectile[0] = 0;
			Solicitation[0] = 0;
			SexualOther[0] = 0;
			BoneFractures[0] = 0;
			BrainDamage[0] = 0;
			Burn[0] = 0;
			Death[0] = 0;
			InternalInjuries[0] = 0;
			Poison[0] = 0;
			Sprains[0] = 0;
			Shaken[0] = 0;
			SubduralHematoma[0] = 0;
			Torture[0] = 0;
			Wounds[0] = 0;
			PhysicalOther[0] = 0;
            HumanLaborTrafficking[0] = 0;
            HumanSexTrafficking[0] = 0;
            FinancialAbuse[0] = 0;
            SpiritualAbuse[0] = 0;
        }

		public int ClientId { get; set; }

		public int CaseId { get; set; }

		[Required]
		[Help("Select the primary presenting issue resulting in the client's decision to seek services.")]
		[Help(Provider.DV, "Select the most prominent issue for which the client is seeking services.  Use the options below to indicate any additional issues the client is facing.")]
		[Help(Provider.CAC, "Select the most prominent issue for which the client is seeking services.  Use the options below to provide additional detail about the abuse.")]
		[Lookup("PrimaryPresentingIssue")]
		[Display(Name = "Primary Presenting Issue")]
		public int? PrimaryPresentingIssueID { get; set; }

		[Help("Enter the date the primary offense occurred.")]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Primary Offense Date")]
		public DateTime? DateOfPrimOffense { get; set; }

		[NotMapped] /* alias for DateOfPrimOffense useful for SA and CAC */
		[Help("Enter the approximate date of the abuse/offense OR when the abuse first started.")]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Approximate Abuse/Offense Date (or start of abuse)")]
		public DateTime? StartDateOfAbuse {
			get { return DateOfPrimOffense; }
			set { DateOfPrimOffense = value; }
		}

		[Help("Thinking of the primary offense/incident that prompted the client to seek services, select the location where this occurred. If the location isn't listed, or was not provided, select Other.")]
		[Help(Provider.SA, "For the abuse/offense referenced above, select the location where the incident occurred.")]
		[Lookup("PresentingIssueLocation")]
		[Display(Name = "Primary Offense Location")]
		public int? LocOfPrimOffenseID { get; set; }

		public byte[] RapeOrSexualAssault { get; set; }

		[Display(Name = "Rape or Sexual Assault")]
		[NotMapped]
		public bool IsRapeOrSexualAssault {
			get { return RapeOrSexualAssault != null && RapeOrSexualAssault[0] == 1; }
			set { RapeOrSexualAssault = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] AdultSurvivor { get; set; }

		[Display(Name = "Adult Survivor of Incest or Sexual Assault")]
		[NotMapped]
		public bool IsAdultSurvivor {
			get { return AdultSurvivor != null && AdultSurvivor[0] == 1; }
			set { AdultSurvivor = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Stalking { get; set; }

		[Display(Name = "Stalking")]
		[NotMapped]
		public bool IsStalking {
			get { return Stalking != null && Stalking[0] == 1; }
			set { Stalking = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Harassment { get; set; }

		[Display(Name = "Harassment")]
		[NotMapped]
		public bool IsHarassment {
			get { return Harassment != null && Harassment[0] == 1; }
			set { Harassment = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] PhysicalDomesticViolence { get; set; }

		[Display(Name = "Physical Domestic Violence")]
		[NotMapped]
		public bool IsPhysicalDomesticViolence {
			get { return PhysicalDomesticViolence != null && PhysicalDomesticViolence[0] == 1; }
			set { PhysicalDomesticViolence = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] SexualDomesticViolence { get; set; }

		[Display(Name = "Sexual Domestic Violence")]
		[NotMapped]
		public bool IsSexualDomesticViolence {
			get { return SexualDomesticViolence != null && SexualDomesticViolence[0] == 1; }
			set { SexualDomesticViolence = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] EmotionalDomesticViolence { get; set; }

		[Display(Name = "Emotional Domestic Violence")]
		[NotMapped]
		public bool IsEmotionalDomesticViolence {
			get { return EmotionalDomesticViolence != null && EmotionalDomesticViolence[0] == 1; }
			set { EmotionalDomesticViolence = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] DomesticBattery { get; set; }

		[Display(Name = "Domestic Battery")]
		[NotMapped]
		public bool IsDomesticBattery {
			get { return DomesticBattery != null && DomesticBattery[0] == 1; }
			set { DomesticBattery = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] AggravatedDomesticBattery { get; set; }

		[Display(Name = "Aggravated Domestic Battery")]
		[NotMapped]
		public bool IsAggravatedDomesticBattery {
			get { return AggravatedDomesticBattery != null && AggravatedDomesticBattery[0] == 1; }
			set { AggravatedDomesticBattery = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] ViolationOfOOP { get; set; }

		[Display(Name = "Violation of Order of Protection")]
		[NotMapped]
		public bool IsViolationOfOOP {
			get { return ViolationOfOOP != null && ViolationOfOOP[0] == 1; }
			set { ViolationOfOOP = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Assault { get; set; }

		[Display(Name = "Other Assault")]
		[NotMapped]
		public bool IsAssault {
			get { return Assault != null && Assault[0] == 1; }
			set { Assault = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Battery { get; set; }

		[Display(Name = "Battery")]
		[NotMapped]
		public bool IsBattery {
			get { return Battery != null && Battery[0] == 1; }
			set { Battery = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] AssaultAndOrBattery { get; set; }

		[Display(Name = "Assault and/or Battery")]
		[NotMapped]
		public bool IsAssaultAndOrBattery {
			get { return AssaultAndOrBattery != null && AssaultAndOrBattery[0] == 1; }
			set { AssaultAndOrBattery = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] HateCrime { get; set; }

		[Display(Name = "Hate Crime")]
		[NotMapped]
		public bool IsHateCrime {
			get { return HateCrime != null && HateCrime[0] == 1; }
			set { HateCrime = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] UnknownOffense { get; set; }

		[Display(Name = "Unknown Offense")]
		[NotMapped]
		public bool IsUnknownOffense {
			get { return UnknownOffense != null && UnknownOffense[0] == 1; }
			set { UnknownOffense = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] ChildAbuse { get; set; }

		[Display(Name = "Child Abuse")]
		[NotMapped]
		public bool IsChildAbuse {
			get { return ChildAbuse != null && ChildAbuse[0] == 1; }
			set { ChildAbuse = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] ChildNeglect { get; set; }

		[Display(Name = "Child Neglect")]
		[NotMapped]
		public bool IsChildNeglect {
			get { return ChildNeglect != null && ChildNeglect[0] == 1; }
			set { ChildNeglect = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] ChildSexualAssault { get; set; }

		[Display(Name = "Child Sexual Assault")]
		[NotMapped]
		public bool IsChildSexualAssault {
			get { return ChildSexualAssault != null && ChildSexualAssault[0] == 1; }
			set { ChildSexualAssault = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] ElderAbuse { get; set; }

		[Display(Name = "Elder Abuse")]
		[NotMapped]
		public bool IsElderAbuse {
			get { return ElderAbuse != null && ElderAbuse[0] == 1; }
			set { ElderAbuse = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Homicide { get; set; }

		[Display(Name = "Homicide")]
		[NotMapped]
		public bool IsHomicide {
			get { return Homicide != null && Homicide[0] == 1; }
			set { Homicide = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] AttemptedHomicide { get; set; }

		[Display(Name = "Attempted Homicide")]
		[NotMapped]
		public bool IsAttemptedHomicide {
			get { return AttemptedHomicide != null && AttemptedHomicide[0] == 1; }
			set { AttemptedHomicide = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] HomeInvasion { get; set; }

		[Display(Name = "Home Invasion")]
		[NotMapped]
		public bool IsHomeInvasion {
			get { return HomeInvasion != null && HomeInvasion[0] == 1; }
			set { HomeInvasion = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Robbery { get; set; }

		[Display(Name = "Robbery")]
		[NotMapped]
		public bool IsRobbery {
			get { return Robbery != null && Robbery[0] == 1; }
			set { Robbery = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Burglary { get; set; }

		[Display(Name = "Burglary")]
		[NotMapped]
		public bool IsBurglary {
			get { return Burglary != null && Burglary[0] == 1; }
			set { Burglary = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] OtherOffenseAgPerson { get; set; }

		[Display(Name = "Other Offense Against Person")]
		[NotMapped]
		public bool IsOtherOffenseAgPerson {
			get { return OtherOffenseAgPerson != null && OtherOffenseAgPerson[0] == 1; }
			set { OtherOffenseAgPerson = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] OtherOffense { get; set; }

		[Display(Name = "Other Offense")]
		[NotMapped]
		public bool IsOtherOffense {
			get { return OtherOffense != null && OtherOffense[0] == 1; }
			set { OtherOffense = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] DwiDui { get; set; }

		[Display(Name = "DUI / DWI")]
		[NotMapped]
		public bool IsDwiDui {
			get { return DwiDui != null && DwiDui[0] == 1; }
			set { DwiDui = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] DateRape { get; set; }

		[Display(Name = "Date Rape")]
		[NotMapped]
		public bool IsDateRape {
			get { return DateRape != null && DateRape[0] == 1; }
			set { DateRape = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Drugged { get; set; }

		[Display(Name = "Drugged")]
		[NotMapped]
		public bool IsDrugged {
			get { return Drugged != null && Drugged[0] == 1; }
			set { Drugged = new[] { value ? (byte)1 : (byte)0 }; }
		}

        public byte[] HumanLaborTrafficking { get; set; }

        [Display(Name = "Human Labor Trafficking")]
        [NotMapped]
        public bool IsHumanLaborTrafficking {
            get { return HumanLaborTrafficking != null && HumanLaborTrafficking[0] == 1; }
            set { HumanLaborTrafficking = new[] { value ? (byte)1 : (byte)0 }; }
        }

        public byte[] HumanSexTrafficking { get; set; }
        [Display(Name = "Human Sex Trafficking")]
        [NotMapped]
        public bool IsHumanSexTrafficking {
            get { return HumanSexTrafficking != null && HumanSexTrafficking[0] == 1; }
            set { HumanSexTrafficking = new[] { value ? (byte)1 : (byte)0 }; }
        }

        public byte[] FinancialAbuse { get; set; }
        [Display(Name = "Financial Abuse")]
        [NotMapped]
        public bool IsFinancialAbuse {
            get { return FinancialAbuse != null && FinancialAbuse[0] == 1; }
            set { FinancialAbuse = new[] { value ? (byte)1 : (byte)0 }; }
        }

        public byte[] SpiritualAbuse { get; set; }
        [Display(Name = "Spiritual Abuse")]
        [NotMapped]
        public bool IsSpiritualAbuse {
            get { return SpiritualAbuse != null && SpiritualAbuse[0] == 1; }
            set { SpiritualAbuse = new[] { value ? (byte)1 : (byte)0 }; }
        }

        [Display(Name = "End of Abuse/Offense Date (if applicable)")]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? EndDateOfAbuse { get; set; }

        [MaxLength(256, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string Comment { get; set; }

		#region obsolete
		[Obsolete]
		public string WhatOther { get; set; }
		#endregion

		public DateTime? RevisionStamp { get; set; }

		[Display(Name = "Township")]
		public int? TownshipID { get; set; }

		[Display(Name = "City")]
		public int? CityID { get; set; }

		[Display(Name = "County")]
		public int? CountyID { get; set; }

		[Display(Name = "State")]
		public int? StateID { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		//CAC Physical Abuse 
		public byte[] BoneFractures { get; set; }

		[Display(Name = "Bone Fractures")]
		[NotMapped]
		public bool IsBoneFractures {
			get { return BoneFractures != null && BoneFractures[0] == 1; }
			set { BoneFractures = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] BrainDamage { get; set; }

		[Display(Name = "Brain Damage/Skull Fractures")]
		[NotMapped]
		public bool IsBrainDamage {
			get { return BrainDamage != null && BrainDamage[0] == 1; }
			set { BrainDamage = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Burn { get; set; }

		[Display(Name = "Burn/Scalding")]
		[NotMapped]
		public bool IsBurn {
			get { return Burn != null && Burn[0] == 1; }
			set { Burn = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Death { get; set; }

		[Display(Name = "Death")]
		[NotMapped]
		public bool IsDeath {
			get { return Death != null && Death[0] == 1; }
			set { Death = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] InternalInjuries { get; set; }

		[Display(Name = "Internal Injuries")]
		[NotMapped]
		public bool IsInternalInjuries {
			get { return InternalInjuries != null && InternalInjuries[0] == 1; }
			set { InternalInjuries = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Poison { get; set; }

		[Display(Name = "Poison/Noxious")]
		[NotMapped]
		public bool IsPoison {
			get { return Poison != null && Poison[0] == 1; }
			set { Poison = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Sprains { get; set; }

		[Display(Name = "Sprains/Dislocations")]
		[NotMapped]
		public bool IsSprains {
			get { return Sprains != null && Sprains[0] == 1; }
			set { Sprains = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Shaken { get; set; }

		[Display(Name = "Shaken")]
		[NotMapped]
		public bool IsShaken {
			get { return Shaken != null && Shaken[0] == 1; }
			set { Shaken = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] SubduralHematoma { get; set; }

		[Display(Name = "Subdural Hematoma")]
		[NotMapped]
		public bool IsSubduralHematoma {
			get { return SubduralHematoma != null && SubduralHematoma[0] == 1; }
			set { SubduralHematoma = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Torture { get; set; }

		[Display(Name = "Torture")]
		[NotMapped]
		public bool IsTorture {
			get { return Torture != null && Torture[0] == 1; }
			set { Torture = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] Wounds { get; set; } //iswounds or haswounds

		[Display(Name = "Wounds")]
		[NotMapped]
		public bool IsWounds {
			get { return Wounds != null && Wounds[0] == 1; }
			set { Wounds = new[] { value ? (byte)1 : (byte)0 }; }
		}

		public byte[] PhysicalOther { get; set; }

		[Display(Name = "Other")]
		[NotMapped]
		public bool IsPhysicalOther {
			get { return PhysicalOther != null && PhysicalOther[0] == 1; }
			set { PhysicalOther = new[] { value ? (byte)1 : (byte)0 }; }
		}

        [MaxLength(256, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string PhysicalComment { get; set; }

		public byte[] Exploitation { get; set; }

		[NotMapped]
		[Display(Name = "Exploitation")]
		public bool IsExploitationPassive {
			get { return GetElementState(AbuseType.PASSIVE, Exploitation[0]); }
			set { Exploitation = new[] { SetElementState(AbuseType.PASSIVE, value, Exploitation[0]) }; }
		}

		public byte[] FondlingOverClothes { get; set; }

		[NotMapped]
		public bool IsFondlingOverClothesActive {
			get { return GetElementState(AbuseType.ACTIVE, FondlingOverClothes[0]); }
			set { FondlingOverClothes = new[] { SetElementState(AbuseType.ACTIVE, value, FondlingOverClothes[0]) }; }
		}

		[NotMapped]
		[Display(Name = "Fondling - Over Clothes")]
		public bool IsFondlingOverClothesPassive {
			get { return GetElementState(AbuseType.PASSIVE, FondlingOverClothes[0]); }
			set { FondlingOverClothes = new[] { SetElementState(AbuseType.PASSIVE, value, FondlingOverClothes[0]) }; }
		}

		public byte[] FondlingUnderClothes { get; set; }

		[NotMapped]
		public bool IsFondlingUnderClothesActive {
			get { return GetElementState(AbuseType.ACTIVE, FondlingUnderClothes[0]); }
			set { FondlingUnderClothes = new[] { SetElementState(AbuseType.ACTIVE, value, FondlingUnderClothes[0]) }; }
		}

		[NotMapped]
		[Display(Name = "Fondling - Under Clothes")]
		public bool IsFondlingUnderClothesPassive {
			get { return GetElementState(AbuseType.PASSIVE, FondlingUnderClothes[0]); }
			set { FondlingUnderClothes = new[] { SetElementState(AbuseType.PASSIVE, value, FondlingUnderClothes[0]) }; }
		}

		public byte[] IntercourseVaginal { get; set; }

		[NotMapped]
		public bool IsIntercourseVaginalActive {
			get { return GetElementState(AbuseType.ACTIVE, IntercourseVaginal[0]); }
			set { IntercourseVaginal = new[] { SetElementState(AbuseType.ACTIVE, value, IntercourseVaginal[0]) }; }
		}

		[NotMapped]
		[Display(Name = "Intercourse - Vaginal")]
		public bool IsIntercourseVaginalPassive {
			get { return GetElementState(AbuseType.PASSIVE, IntercourseVaginal[0]); }
			set { IntercourseVaginal = new[] { SetElementState(AbuseType.PASSIVE, value, IntercourseVaginal[0]) }; }
		}

		public byte[] IntercourseAnal { get; set; }

		[NotMapped]
		public bool IsIntercourseAnalActive {
			get { return GetElementState(AbuseType.ACTIVE, IntercourseAnal[0]); }
			set { IntercourseAnal = new[] { SetElementState(AbuseType.ACTIVE, value, IntercourseAnal[0]) }; }
		}

		[NotMapped]
		[Display(Name = "Intercourse - Anal")]
		public bool IsIntercourseAnalPassive {
			get { return GetElementState(AbuseType.PASSIVE, IntercourseAnal[0]); }
			set { IntercourseAnal = new[] { SetElementState(AbuseType.PASSIVE, value, IntercourseAnal[0]) }; }
		}

		public byte[] Masturbation { get; set; }

		[NotMapped]
		public bool IsMasturbationActive {
			get { return GetElementState(AbuseType.ACTIVE, Masturbation[0]); }
			set { Masturbation = new[] { SetElementState(AbuseType.ACTIVE, value, Masturbation[0]) }; }
		}

		[NotMapped]
		[Display(Name = "Masturbation")]
		public bool IsMasturbationPassive {
			get { return GetElementState(AbuseType.PASSIVE, Masturbation[0]); }
			set { Masturbation = new[] { SetElementState(AbuseType.PASSIVE, value, Masturbation[0]) }; }
		}

		public byte[] Oral { get; set; }

		[NotMapped]
		public bool IsOralActive {
			get { return GetElementState(AbuseType.ACTIVE, Oral[0]); }
			set { Oral = new[] { SetElementState(AbuseType.ACTIVE, value, Oral[0]) }; }
		}

		[NotMapped]
		[Display(Name = "Oral")]
		public bool IsOralPassive {
			get { return GetElementState(AbuseType.PASSIVE, Oral[0]); }
			set { Oral = new[] { SetElementState(AbuseType.PASSIVE, value, Oral[0]) }; }
		}

		public byte[] PenetrationDigital { get; set; }

		[NotMapped]
		public bool IsPenetrationDigitalActive {
			get { return GetElementState(AbuseType.ACTIVE, PenetrationDigital[0]); }
			set { PenetrationDigital = new[] { SetElementState(AbuseType.ACTIVE, value, PenetrationDigital[0]) }; }
		}

		[NotMapped]
		[Display(Name = "Penetration - Digital")]
		public bool IsPenetrationDigitalPassive {
			get { return GetElementState(AbuseType.PASSIVE, PenetrationDigital[0]); }
			set { PenetrationDigital = new[] { SetElementState(AbuseType.PASSIVE, value, PenetrationDigital[0]) }; }
		}

		public byte[] PenetrationObjectile { get; set; }

		[NotMapped]
		public bool IsPenetrationObjectileActive {
			get { return GetElementState(AbuseType.ACTIVE, PenetrationObjectile[0]); }
			set { PenetrationObjectile = new[] { SetElementState(AbuseType.ACTIVE, value, PenetrationObjectile[0]) }; }
		}

		[NotMapped]
		[Display(Name = "Penetration - Objectile")]
		public bool IsPenetrationObjectilePassive {
			get { return GetElementState(AbuseType.PASSIVE, PenetrationObjectile[0]); }
			set { PenetrationObjectile = new[] { SetElementState(AbuseType.PASSIVE, value, PenetrationObjectile[0]) }; }
		}

		public byte[] Solicitation { get; set; }

		[NotMapped]
		[Display(Name = "Solicitation")]
		public bool IsSolicitationPassive {
			get { return GetElementState(AbuseType.PASSIVE, Solicitation[0]); }
			set { Solicitation = new[] { SetElementState(AbuseType.PASSIVE, value, Solicitation[0]) }; }
		}

		public byte[] SexualOther { get; set; }

		[NotMapped]
		public bool IsSexualOtherActive {
			get { return GetElementState(AbuseType.ACTIVE, SexualOther[0]); }
			set { SexualOther = new[] { SetElementState(AbuseType.ACTIVE, value, SexualOther[0]) }; }
		}

		[NotMapped]
		[Display(Name = "Other")]
		public bool IsSexualOtherPassive {
			get { return GetElementState(AbuseType.PASSIVE, SexualOther[0]); }
			set { SexualOther = new[] { SetElementState(AbuseType.PASSIVE, value, SexualOther[0]) }; }
		}

        [MaxLength(256, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string SexualComment { get; set; }

		private static bool GetElementState(AbuseType state, byte value) {
			bool result;
			if (state == AbuseType.ACTIVE)
				result = (value & (byte)AbuseType.ACTIVE) != (byte)AbuseType.NEITHER;
			else
				result = (value & (byte)AbuseType.PASSIVE) != (byte)AbuseType.NEITHER;
			return result;
		}

		private static byte SetElementState(AbuseType state, bool value, byte dbvalue) {
			byte result;
			if (value)
				result = (byte)(dbvalue | (byte)state);
			else
				result = (byte)(dbvalue & ((byte)AbuseType.BOTH ^ (byte)state));
			return result;
		}

		[Flags]
		public enum AbuseType {
			NEITHER = 0,
			ACTIVE = 2,
			PASSIVE = 4,
			BOTH = 6
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
			var results = new List<ValidationResult>();
			if (StartDateOfAbuse != null && EndDateOfAbuse != null && EndDateOfAbuse < StartDateOfAbuse)
				results.Add(new ValidationResult("End of Abuse/Offense Date (if applicable) cannot be before Approximate Abuse/Offense Date (or start of abuse).", new[] { "EndDateOfAbuse" }));
			return results;
		}

		public bool IsEqualTo(PresentingIssues obj) {
			return
				obj != null &&
				AdultSurvivor.SequenceEqual(obj.AdultSurvivor) &&
				AggravatedDomesticBattery.SequenceEqual(obj.AggravatedDomesticBattery) &&
				Assault.SequenceEqual(obj.Assault) &&
				AssaultAndOrBattery.SequenceEqual(obj.AssaultAndOrBattery) &&
				AttemptedHomicide.SequenceEqual(obj.AttemptedHomicide) &&
				Battery.SequenceEqual(obj.Battery) &&
				BoneFractures.SequenceEqual(obj.BoneFractures) &&
				BrainDamage.SequenceEqual(obj.BrainDamage) &&
				Burglary.SequenceEqual(obj.Burglary) &&
				Burn.SequenceEqual(obj.Burn) &&
				ChildAbuse.SequenceEqual(obj.ChildAbuse) &&
				ChildNeglect.SequenceEqual(obj.ChildNeglect) &&
				ChildSexualAssault.SequenceEqual(obj.ChildSexualAssault) &&
				DateRape.SequenceEqual(obj.DateRape) &&
				Death.SequenceEqual(obj.Death) &&
				DomesticBattery.SequenceEqual(obj.DomesticBattery) &&
				Drugged.SequenceEqual(obj.Drugged) &&
				DwiDui.SequenceEqual(obj.DwiDui) &&
				ElderAbuse.SequenceEqual(obj.ElderAbuse) &&
				EmotionalDomesticViolence.SequenceEqual(obj.EmotionalDomesticViolence) &&
				Exploitation.SequenceEqual(obj.Exploitation) &&
				FondlingOverClothes.SequenceEqual(obj.FondlingOverClothes) &&
				FondlingUnderClothes.SequenceEqual(obj.FondlingUnderClothes) &&
				Harassment.SequenceEqual(obj.Harassment) &&
				HateCrime.SequenceEqual(obj.HateCrime) &&
				HomeInvasion.SequenceEqual(obj.HomeInvasion) &&
				Homicide.SequenceEqual(obj.Homicide) &&
				IntercourseAnal.SequenceEqual(obj.IntercourseAnal) &&
				IntercourseVaginal.SequenceEqual(obj.IntercourseVaginal) &&
				InternalInjuries.SequenceEqual(obj.InternalInjuries) &&
				Masturbation.SequenceEqual(obj.Masturbation) &&
				Oral.SequenceEqual(obj.Oral) &&
				OtherOffense.SequenceEqual(obj.OtherOffense) &&
				OtherOffenseAgPerson.SequenceEqual(obj.OtherOffenseAgPerson) &&
				PenetrationDigital.SequenceEqual(obj.PenetrationDigital) &&
				PenetrationObjectile.SequenceEqual(obj.PenetrationObjectile) &&
				PhysicalDomesticViolence.SequenceEqual(obj.PhysicalDomesticViolence) &&
				PhysicalOther.SequenceEqual(obj.PhysicalOther) &&
				Poison.SequenceEqual(obj.Poison) &&
				RapeOrSexualAssault.SequenceEqual(obj.RapeOrSexualAssault) &&
				Robbery.SequenceEqual(obj.Robbery) &&
				SexualDomesticViolence.SequenceEqual(obj.SexualDomesticViolence) &&
				SexualOther.SequenceEqual(obj.SexualOther) &&
				Shaken.SequenceEqual(obj.Shaken) &&
				Solicitation.SequenceEqual(obj.Solicitation) &&
				Sprains.SequenceEqual(obj.Sprains) &&
				Stalking.SequenceEqual(obj.Stalking) &&
				SubduralHematoma.SequenceEqual(obj.SubduralHematoma) &&
				Torture.SequenceEqual(obj.Torture) &&
				UnknownOffense.SequenceEqual(obj.UnknownOffense) &&
				ViolationOfOOP.SequenceEqual(obj.ViolationOfOOP) &&
				Wounds.SequenceEqual(obj.Wounds) &&
#pragma warning disable 612
				WhatOther == obj.WhatOther &&
#pragma warning restore 612
                HumanLaborTrafficking.SequenceEqual(obj.HumanLaborTrafficking) &&
                HumanSexTrafficking.SequenceEqual(obj.HumanSexTrafficking) &&
                FinancialAbuse.SequenceEqual(obj.FinancialAbuse) &&
                SpiritualAbuse.SequenceEqual(obj.SpiritualAbuse) &&
                CityID == obj.CityID &&
				Comment == obj.Comment &&
				CountyID == obj.CountyID &&
				DateOfPrimOffense == obj.DateOfPrimOffense &&
				EndDateOfAbuse == obj.EndDateOfAbuse &&
				LocOfPrimOffenseID == obj.LocOfPrimOffenseID &&
				PhysicalComment == obj.PhysicalComment &&
				PrimaryPresentingIssueID == obj.PrimaryPresentingIssueID &&
				SexualComment == obj.SexualComment &&
				StateID == obj.StateID &&
				TownshipID == obj.TownshipID;
		}
	}
}