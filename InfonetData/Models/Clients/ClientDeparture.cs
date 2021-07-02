using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	[DeleteIfNulled("ClientID,CaseID")]
	[BindHint(Include = "DepartureID,ClientID,CaseID,DestinationID,DestinationTenureID,DestinationSubsidyID,ReasonForLeavingID,DepartureDate,IsDeleted")]
	public class ClientDeparture : IRevisable {
		public int DepartureID { get; set; }
		public int? ClientID { get; set; }
		public int? CaseID { get; set; }

		[Required]
		[Lookup("Destination")]
		[Display(Name = "Destination")]
		public int? DestinationID { get; set; }

		[Lookup("DestinationTenure")]
		[Display(Name = "Destination Tenure")]
		public int? DestinationTenureID { get; set; }

		[Lookup("DestinationSubsidy")]
		[Display(Name = "Destination Subsidy")]
		public int? DestinationSubsidyID { get; set; }

		[Lookup("ReasonForLeaving")]
		[Display(Name = "Reason for Leaving")]
		public int? ReasonForLeavingID { get; set; }

		[Required]
		[NotGreaterThanToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Departure Date")]
		public DateTime? DepartureDate { get; set; }

		public DateTime? RevisionStamp { get; set; }
		public virtual ClientCase ClientCase { get; set; }

		[NotMapped]
		public bool IsDeleted { get; set; }

		public bool IsUnchanged(ClientDeparture departure) {
			return departure != null &&
					DestinationID == departure.DestinationID &&
					DestinationTenureID == departure.DestinationTenureID &&
					DestinationSubsidyID == departure.DestinationSubsidyID &&
					ReasonForLeavingID == departure.ReasonForLeavingID &&
					DepartureDate == departure.DepartureDate &&
					ClientID == departure.ClientID &&
					CaseID == departure.CaseID;
		}
	}
}