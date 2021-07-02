using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Entity;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Offenders {
	[BindHint(Include = "ArrestDate,StatuteId,ChargeDate,ChargeTypeId,ArrestMadeId,ChargeCounts")]
	[DeleteIfNulled("OffenderId")]
	public class PoliceCharge : IRevisable {
		public int? PoliceChargeId { get; set; }

		public int? OffenderId { get; set; }

		[BetweenNineteenSeventyToday]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Date of Arrest")]
		public DateTime? ArrestDate { get; set; }

		[Help("Select if there was a police charge (offense for which the offender was/will be arrested) from the drop-down menu. The charges are grouped by sex crimes, physical/violent crimes, and other crimes. The charges are sorted alphabetically for each group.")]
		[Help(Provider.CAC, "Select if there was a Police Charge from the drop-down menu.  If there are multiple Police Charges on this Offender, then you can add more Police Charge records.  The charges are grouped by Physical/Violent Crimes, Sex Crimes, and Other Crimes.  The charges are sorted alphabetically for each group.  If the charge is not listed, select Other Charge.")]
		[Lookup("Statute")]
		[Display(Name = "Police Charge")]
		public int? StatuteId { get; set; }

		[BetweenNineteenSeventyToday]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Date of Charge")]
		public DateTime? ChargeDate { get; set; }

		[Help("Select the type of police charge (offense for which offender was/will be arrested) from the drop-down menu.  If there are multiple police charges on this offender, you can add more police charge records.")]
		[Help(Provider.CAC, "Select the class of police charge (offense for which offender was/will be arrested) from the drop-down menu.")]
		[Lookup("CrimeClass")]
		[Display(Name = "Charge Type")]
		public int? ChargeTypeId { get; set; }

		public DateTime? RevisionStamp { get; set; }

		#region obsolete
		[Obsolete]
		public int? AR_Id { get; set; }
		#endregion

		[Display(Name = "Charge Counts")]
		[Range(0, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
		[WholeNumber]
		public int? ChargeCounts { get; set; }

		[Required]
		[Help("Select if this offender was arrested from the drop-down menu. If this information is unknown, select Unknown.")]
		[Help(Provider.CAC, "Select to indicate if this offender was arrested from the drop-down menu. If this information is unknown, select Unknown.")]
		[Lookup("ArrestMade")]
		[Display(Name = "Arrest Made?")]
		public int? ArrestMadeId { get; set; }

		public virtual Offender Offender { get; set; }
	}
}