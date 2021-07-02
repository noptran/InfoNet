using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models._TLU;
using System.Linq.Expressions;
using LinqKit;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "ID,ClientID,CaseID,ServiceID,Date,SVID,LocationID,ReasonID,IsDeleted,IsEdited")]
	public class Cancellation {
		public int ID { get; set; }

		public int? ClientID { get; set; }

		public int? CaseID { get; set; }

		[Display(Name = "Service")]
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
		[Lookup("CancellationReason")]
		public int? ReasonID { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		public virtual StaffVolunteer StaffVolunteer { get; set; }

		public virtual TLU_Codes_ProgramsAndServices TLU_Codes_ProgramsAndServices { get; set; }

		public bool IsUnchanged(Cancellation cancellation) {
			return cancellation != null &&
					ServiceID == cancellation.ServiceID &&
					Date == cancellation.Date &&
					SVID == cancellation.SVID &&
					LocationID == cancellation.LocationID &&
					ReasonID == cancellation.ReasonID &&
					ClientID == cancellation.ClientID &&
					CaseID == cancellation.CaseID;
		}

		#region predicates
		public static Expression<Func<Cancellation, bool>> DateBetween(DateTime? minDate, DateTime? maxDate) {
			var predicate = PredicateBuilder.New<Cancellation>(true);
			if (minDate != null)
				predicate.And(c => c.Date >= minDate);
			if (maxDate != null)
				predicate.And(c => c.Date <= maxDate);
			return predicate;
		}
		#endregion

	}
}