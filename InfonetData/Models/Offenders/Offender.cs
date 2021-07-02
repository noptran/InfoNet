using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Infonet.Core.Collections;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Models.Offenders {
	[BindHint(Include = "SexId,RaceId,CountyId,RelationshipToClientId,Age,VisitationId,StateId,PoliceChargesById,TrialChargesById,RegisteredId,OffenderListing,OffenderListingId")]
	[DeleteIfNulled("ClientId,CaseId")]
	public class Offender : IRevisable, INotifyContextSavedChanges {
		public Offender() {
			PoliceCharges = new List<PoliceCharge>();
			PoliceChargesById = new DerivedDictionary<PoliceCharge>(() => PoliceCharges, true, e => e.PoliceChargeId?.ToString()) { Template = () => new PoliceCharge() };
			TrialCharges = new List<TrialCharge>();
			TrialChargesById = new DerivedDictionary<TrialCharge>(() => TrialCharges, true, e => e.TrialChargeId?.ToString()) { Template = () => new TrialCharge() };
		}

		[Display(Name = "Offender ID")]
		public int? OffenderId { get; set; }

		public int? ClientId { get; set; }

		public int? CaseId { get; set; }

		[Required]
		[Help("Select the offender's Gender Identity from the drop-down menu.  If this information is not known or client does not want to provide this information, select Unknown.")]
		[Help(Provider.SA, "Select the offender's Gender identity from the drop-down menu. If this information is not known, select Unknown. If client does not want to provide this information, select Not Reported.")]
		[Lookup("GenderIdentity")]
		[Display(Name = "Gender Identity")]
		public int? SexId { get; set; }

		[Required]
		[Lookup("Race")]
		[Display(Name = "Race/Ethnicity")]
		public int? RaceId { get; set; }

		[Display(Name = "County of Residence")]
		public int? CountyId { get; set; }

		[Required]
		[Lookup("RelationshipToClient")]
		[Display(Name = "Relationship to Victim")]
		public int? RelationshipToClientId { get; set; }

		[Required]
		[Range(-1, 120)]
		[WholeNumber]
		[OnBindException("The field {1} must be a whole number.", typeof(FormatException))]
		[Display(Name = "Age at Victim Intake")]
		public short? Age { get; set; }

		//[Required]
		[Lookup("Visitation")]
		[Display(Name = "Visitation")]
		public int? VisitationId { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public int? OffenderListingId { get; set; }

		[Display(Name = "State of Residence")]
		public int? StateId { get; set; }

		[Lookup("YesNo")]
		[Display(Name = "Registered Offender")]
		public int? RegisteredId { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		public virtual OffenderListing OffenderListing { get; set; }

		public virtual ICollection<PoliceCharge> PoliceCharges { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<PoliceCharge> PoliceChargesById { get; }

		public virtual ICollection<TrialCharge> TrialCharges { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<TrialCharge> TrialChargesById { get; }

		public void OnContextSavedChanges(EntityState prior) {
			PoliceChargesById.RestorableKeys.Clear();
			TrialChargesById.RestorableKeys.Clear();
		}
	}
}