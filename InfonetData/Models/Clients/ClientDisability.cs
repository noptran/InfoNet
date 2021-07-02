using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "NoSpecialNeeds, UnknownSpecialNeeds, NotReported, Deaf, ADLProblem, MedsAdministered, VisualProblem, LimitedEnglish, PrimaryLanguageID, WheelChair, Immobil, MentalDisability, SpecialDiet, OtherDisability, WhatOther, DevelopmentallyDisabled")]
	[RemoveIfTrue("IsEmpty")]
	public class ClientDisability : IRevisable {
		public int ClientId { get; set; }

		public int CaseId { get; set; }

		[Display(Name = "Has developmental disability, requires assistance")]
		public bool DevelopmentallyDisabled { get; set; }

		[Display(Name = "Requires special diet")]
		public bool SpecialDiet { get; set; }

		[Display(Name = "Has immobility, requires assistance")]
		public bool Immobil { get; set; }

		[Display(Name = "Requires wheelchair accessibility")]
		public bool WheelChair { get; set; }

		[Display(Name = "Must have medication administered")]
		public bool MedsAdministered { get; set; }

		[Display(Name = "Has hearing impairment, requires assistance")]
		public bool Deaf { get; set; }

		[Display(Name = "Has a visual impairment, requires assistance")]
		public bool VisualProblem { get; set; }

		#region obsolete
		[Obsolete]
		public string PrimaryLanguage { get; set; }
		#endregion

		[Display(Name = "Has limited English proficiency, requires interpreter")]
		public bool LimitedEnglish { get; set; }

		[Display(Name = "Requires assistance in feeding, dressing, toileting, or other ADL")]
		public bool ADLProblem { get; set; }

		[Display(Name = "Other special needs")]
		public bool OtherDisability { get; set; }

        [MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Other special needs")]
		public string WhatOther { get; set; }

		public DateTime? RevisionStamp { get; set; }

		[Display(Name = "Mental/emotional disability")]
		public bool MentalDisability { get; set; }

		[Lookup("Language")]
		[Display(Name = "Primary Language")]
		public int? PrimaryLanguageID { get; set; }

		[Display(Name = "No Special Needs Indicated")]
		public bool NoSpecialNeeds { get; set; }

		[Display(Name = "Unknown")]
		public bool UnknownSpecialNeeds { get; set; }

		[Display(Name = "Not Reported")]
		public bool NotReported { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		[NotMapped]
		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public bool IsEmpty {
			get {
				return !DevelopmentallyDisabled &&
						!SpecialDiet &&
						!Immobil &&
						!WheelChair &&
						!MedsAdministered &&
						!Deaf &&
						!VisualProblem &&
#pragma warning disable 612
				       string.IsNullOrWhiteSpace(PrimaryLanguage) &&
#pragma warning restore 612
						!LimitedEnglish &&
						!ADLProblem &&
						!OtherDisability &&
						string.IsNullOrWhiteSpace(WhatOther) &&
						!MentalDisability &&
						PrimaryLanguageID == null &&
						!NoSpecialNeeds &&
						!UnknownSpecialNeeds &&
						!NotReported;
			}
		}
	}
}