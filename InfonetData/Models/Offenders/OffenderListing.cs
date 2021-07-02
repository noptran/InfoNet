using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Offenders {
	[BindHint(Include = "OffenderCode,SexId,RaceId,BirthYear")]
	public class OffenderListing : IRevisable {
		public OffenderListing() {
			Offender = new List<Offender>();
		}

		public int? OffenderListingId { get; set; }

		[StringTrim]
		[Required]
        [MaxLength(20, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Offender ID")]
		public string OffenderCode { get; set; }

		public int ParentCenterId { get; set; }

		[Lookup("GenderIdentity")]
		[Display(Name = "Gender Identity")]
		public int? SexId { get; set; }

		[Lookup("Race")]
		[Display(Name = "Race/Ethnicity")]
		public int? RaceId { get; set; }

		public int? BirthYear { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual ICollection<Offender> Offender { get; set; }

		//KMS DO this is viewmodel stuff forced into the entity
		[NotMapped]
		public virtual ICollection<ClientCaseTied> CasesTiedToThisOffender { get; set; }

		[NotMapped]
		public class ClientCaseTied {
			public int? ClientId { get; set; }

			public int? CaseId { get; set; }

			public string ClientCode { get; set; }

			[DataType(DataType.Date)]
			[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
			public DateTime? FirstContactDate { get; set; }

			public int? ClientTypeId { get; set; }
		}
	}
}