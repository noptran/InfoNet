using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infonet.Core.Collections;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	[DeleteIfNulled("ClientId,CaseId")]
	public class AbuseNeglectPetition : IRevisable {
		public AbuseNeglectPetition() {
			Respondents = new List<AbuseNeglectPetitionRespondent>();
			RespondentsById = new DerivedDictionary<AbuseNeglectPetitionRespondent>(() => Respondents, true, e => e.Id?.ToString()) { Template = () => new AbuseNeglectPetitionRespondent() };
		}

		public int? Id { get; set; }

		public int ClientId { get; set; }

		public int CaseId { get; set; }

		[Required]
		[Display(Name = "Petition")]
		[Lookup("AbuseNeglectPetition")]
		public int? AbuseNeglectPetitionId { get; set; }

		[BetweenNineteenSeventyToday]
		[Display(Name = "Date of Petition")]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? PetitionDate { get; set; }

		[Lookup("PetitionAdjudication")]
		[Display(Name = "Adjudicated")]
		public int? AdjudicatedId { get; set; }

		[BetweenNineteenSeventyToday]
		[Display(Name = "Date Adjudicated")]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? AdjudicatedDate { get; set; }

		#region obsolete
		[Obsolete]
		[Lookup("Disposition")]
		[Display(Name = "Disposition")]
		public int? DispositionId { get; set; }

		[Obsolete]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Date of Disposition")]
		public DateTime? DispositionDate { get; set; }
		#endregion

		public DateTime? RevisionStamp { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		public virtual ICollection<AbuseNeglectPetitionRespondent> Respondents { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<AbuseNeglectPetitionRespondent> RespondentsById { get; }

		[NotMapped]
		[Display(Name = "Respondents")]
		[Required]
		public string[] RespondentArray { get; set; }
		
		//KMS DO clear RespondentsById
	}
}