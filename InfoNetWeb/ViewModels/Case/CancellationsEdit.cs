using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Case {
	public class CancellationsEdit {

		public int ID { get; set; }
		public int ServiceCancellationID { get; set; }
		public int? ClientID { get; set; }
		public int? CaseID { get; set; }

		[Required]
		[Display(Name = "Service")]
		[Lookup("DirectOrGroupServices")]
		public int? ServiceID { get; set; }

		[Required]
		[NotGreaterThanToday]
		[Display(Name = "Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? Date { get; set; }

		[Required]
		[Display(Name = "Staff")]
		public int? SVID { get; set; }

		public int? LocationID { get; set; }

		[Required]
		[Display(Name = "Reason")]
		public int? ReasonID { get; set; }

		public int? Index { get; set; }

		[Display(Name = "Edit")]
		public bool IsEdited { get; set; }

		[Display(Name = "Delete")]
		public bool IsDeleted { get; set; }
	}
}