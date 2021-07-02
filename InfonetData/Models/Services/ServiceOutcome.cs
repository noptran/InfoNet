using System;
using Infonet.Core.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models._TLU;
using System.Linq.Expressions;
using LinqKit;

namespace Infonet.Data.Models.Services {
	public class ServiceOutcome : IRevisable {
		public int ID { get; set; }

		public int? LocationID { get; set; }

		public DateTime? OutcomeDate { get; set; }

		[Lookup("ServiceCategory")]
		public int? ServiceID { get; set; }

		[Lookup("ServiceOutcome")]
		public int? OutcomeID { get; set; }

		public int? ResponseYes { get; set; }

		public int? ResponseNo { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual Center Center { get; set; }

		public virtual TLU_Codes_ServiceCategory TLU_Codes_ServiceCategory { get; set; }

		public virtual TLU_Codes_ServiceOutcome TLU_Codes_ServiceOutcome { get; set; }

		public bool IsUnchanged(ServiceOutcome outcome) {
			return ID == outcome.ID && LocationID == outcome.LocationID && ServiceID == outcome.ServiceID && OutcomeDate == outcome.OutcomeDate && OutcomeID == outcome.OutcomeID && ResponseYes == outcome.ResponseYes && ResponseNo == outcome.ResponseNo;
		}

		#region predicates
		public static Expression<Func<ServiceOutcome, bool>> OutcomeDateBetween(DateTime? minOutcomeDate, DateTime? maxOutcomeDate) {
			var predicate = PredicateBuilder.New<ServiceOutcome>(true);
			if (minOutcomeDate != null)
				predicate.And(o => o.OutcomeDate >= minOutcomeDate);
			if (maxOutcomeDate != null)
				predicate.And(o => o.OutcomeDate <= maxOutcomeDate);
			return predicate;
		}
		#endregion
	}
}