using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "CourtDate, CourtContinuanceID")]
	[DeleteIfNulled("ClientId,CaseId")]
	public class ClientCourtAppearance : IRevisable {
		public int? ID { get; set; }

		public int? ClientId { get; set; }

		public int? CaseId { get; set; }

		[Lookup("CourtContinuance")]
		[Display(Name = "Court Progress")]
		public int? CourtContinuanceID { get; set; }

		[Required]
		[NotLessThanNineteenSeventy]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Date of Court Appearance")]
		public DateTime? CourtDate { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual ClientCase ClientCase { get; set; }
	}
}