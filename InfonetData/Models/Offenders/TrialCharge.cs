using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Infonet.Core.Collections;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Offenders {
	[BindHint(Include = "StatuteId,ChargeDate,DispositionId,DispositionDate,ChargesFiledId,SentencesById,ChargeTypeId,ChargeCounts")]
	[DeleteIfNulled("OffenderId")]
	public class TrialCharge : IRevisable, INotifyContextSavedChanges {
		public TrialCharge() {
			Sentences = new List<Sentence>();
			SentencesById = new DerivedDictionary<Sentence>(() => Sentences, true, e => e.Id?.ToString()) { Template = () => new Sentence() };
		}

		public int? TrialChargeId { get; set; }

		public int? OffenderId { get; set; }

		[Lookup("Statute")]
		[Display(Name = "State's Attorney Charge")]
		public int? StatuteId { get; set; }

		[BetweenNineteenSeventyToday, CompareWithDate("DispositionDate", CompareType.LessThanEqualTo)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Charge Date")]
		public DateTime? ChargeDate { get; set; }

		#region obsolete
		[Obsolete]
		[Lookup("Sentence")]
		public int? Sentence1Id { get; set; }

		[Obsolete]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? SentenceDate { get; set; }

		[Obsolete]
		[Lookup("Sentence")]
		public int? Sentence2Id { get; set; }

		[Obsolete]
		[Lookup("Sentence")]
		public int? Sentence3Id { get; set; }
		#endregion

		[Lookup("Disposition")]
		[Display(Name = "Disposition")]
		public int? DispositionId { get; set; }

		[BetweenNineteenSeventyToday, CompareWithDate("ChargeDate", CompareType.GreaterThanEqualTo)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Disposition Date")]
		public DateTime? DispositionDate { get; set; }

		#region obsolete
		[Obsolete]
		public int? YearsSentenced { get; set; }

		[Obsolete]
		public int? MonthsSentenced { get; set; }

		[Obsolete]
		public int? DaysSentenced { get; set; }
		#endregion

		public DateTime? RevisionStamp { get; set; }

		[Display(Name = "Counts")]
		[Range(0, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
		[WholeNumber]
		public int? ChargeCounts { get; set; }

		[Required]
		[Lookup("TrialChargeFiled")]
		[Display(Name = "Charges Filed?")]
		public int? ChargesFiledId { get; set; }

		[Lookup("CrimeClass")]
		[Display(Name = "Class")]
		public int? ChargeTypeId { get; set; }

		public virtual Offender Offender { get; set; }

		public virtual ICollection<Sentence> Sentences { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<Sentence> SentencesById { get; }

		public void OnContextSavedChanges(EntityState prior) {
			SentencesById.RestorableKeys.Clear();
		}
	}
}