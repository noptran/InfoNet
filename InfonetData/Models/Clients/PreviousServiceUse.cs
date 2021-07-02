using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "PrevShelterUseId,PrevShelterDate,PrevServiceUseId,PrevServiceDate")]
	public class PreviousServiceUse : IRevisable {
		public int ClientId { get; set; }

		public int CaseId { get; set; }

		[Lookup("YesNo2")]
		[Display(Name = "Have you used another DV shelter in the service area in the last year?")]
		public int? PrevShelterUseId { get; set; }

		[BetweenNineteenSeventyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "If yes, about what date? (mm/dd/yyyy)")]
		public DateTime? PrevShelterDate { get; set; }

		[Display(Name = "Have you used another homeless service in the service area in the last year?")]
		[Lookup("YesNo2")]
		public int? PrevServiceUseId { get; set; }

		[BetweenNineteenSeventyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "If yes, about what date? (mm/dd/yyyy)")]
		public DateTime? PrevServiceDate { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual ClientCase ClientCase { get; set; }
	}
}