using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Case {
	public class CancellationsAdd {
		public CancellationsAdd() {
			IsEmpty = true;
		}

		public int ID { get; set; }
		public int? ClientID { get; set; }
		public int? CaseID { get; set; }

		[Display(Name = "Service")]
		[Lookup("DirectOrGroupServices")]
		public int? ServiceID { get; set; }

		[NotGreaterThanToday]
		[Display(Name = "Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? Date { get; set; }

		[Display(Name = "Staff")]
		public int? SVID { get; set; }

		public int? LocationID { get; set; }

		[Display(Name = "Reason")]
		public int? ReasonID { get; set; }

		public int? Index { get; set; }
		public bool IsAdded { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsEmpty { get; set; }
	}
}