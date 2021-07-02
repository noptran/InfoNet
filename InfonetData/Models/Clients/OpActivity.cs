using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "OpActivityCodeID,OpActivityDate,NewExpirationDate")]
	[DeleteIfNulled("OP_ID")]
	public class OpActivity : IRevisable {
		public int? OpActivityID { get; set; }

		public int? OP_ID { get; set; }

		[Required]
		[Lookup("OrderOfProtectionActivity")]
		[Display(Name = "Activity")]
		public int? OpActivityCodeID { get; set; }

		[Required]
		[Display(Name = "Activity Date")]
		[BetweenNineteenSeventyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? OpActivityDate { get; set; }

		[NotLessThanNineteenSeventy]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "New Expiration Date")]
		public DateTime? NewExpirationDate { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual OrderOfProtection OrderOfProtection { get; set; }
	}
}