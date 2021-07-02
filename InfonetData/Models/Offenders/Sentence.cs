using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Offenders {
	[BindHint(Include = "SentenceDate,SentenceId,YearsSentenced,MonthsSentenced,DaysSentenced")]
	[DeleteIfNulled("TrialChargeId")]
	public class Sentence : IRevisable {
		public int? Id { get; set; }

		public int? TrialChargeId { get; set; }

		[Lookup("Sentence")]
		[Display(Name = "Sentence")]
		public int? SentenceId { get; set; }

		[Required]
		[BetweenNineteenSeventyToday]
		[Display(Name = "Sentence Date")]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? SentenceDate { get; set; }

		[WholeNumber]
		[Range(0, 150)]
		[Display(Name = "Years")]
		public int? YearsSentenced { get; set; }

		[WholeNumber]
		[Range(0, 11)]
		[Display(Name = "Months")]
		public int? MonthsSentenced { get; set; }

		[WholeNumber]
		[Range(0, 364)]
		[Display(Name = "Days")]
		public int? DaysSentenced { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual TrialCharge TrialCharge { get; set; }
	}
}