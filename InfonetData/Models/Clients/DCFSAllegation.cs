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
	public class DCFSAllegation : IRevisable {
		public DCFSAllegation() {
			Respondents = new List<DCFSAllegationRespondent>();
			RespondentsById = new DerivedDictionary<DCFSAllegationRespondent>(() => Respondents, true, e => e.Id?.ToString()) { Template = () => new DCFSAllegationRespondent() };
		}

		public int? Id { get; set; }

		public int? ClientId { get; set; }

		public int? CaseId { get; set; }

		[Lookup("AbuseAllegation")]
		[Required]
		[Display(Name = "DCFS Allegation")]
		public int? AbuseAllegationId { get; set; }

		[Lookup("AbuseAllegationFinding")]
		[Display(Name = "Finding")]
		public int? FindingId { get; set; }

		[BetweenNineteenSeventyToday]
		[Display(Name = "Date of Finding")]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? FindingDate { get; set; }

		#region obsolete
		[Obsolete]
		public bool? Indicated { get; set; }
		#endregion

		public DateTime? RevisionStamp { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		public virtual ICollection<DCFSAllegationRespondent> Respondents { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<DCFSAllegationRespondent> RespondentsById { get; }

		[NotMapped]
		[Display(Name = "Respondents")]
		[Required]
		public string[] RespondentArray { get; set; }
	}
}