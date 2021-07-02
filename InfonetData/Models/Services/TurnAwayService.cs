using System;
using System.Linq.Expressions;
using Infonet.Core.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using LinqKit;

namespace Infonet.Data.Models.Services {
	public class TurnAwayService : IRevisable {
		public int? Id { get; set; }

		public int? AdultsNo { get; set; }

		public int? ChildrenNo { get; set; }

		public int? LocationId { get; set; }

		#region obsolete
		[Obsolete]
		[Lookup("TurnAwayReason")]
		public int? ReasonId { get; set; }
		#endregion

		[Lookup("YesNo")]
		public int? ReferralMadeId { get; set; }

		public DateTime? TurnAwayDate { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual Center Center { get; set; }

		public bool IsUnchanged(TurnAwayService obj) {
			return obj != null &&
					Id == obj.Id &&
					AdultsNo == obj.AdultsNo &&
					ChildrenNo == obj.ChildrenNo &&
					LocationId == obj.LocationId &&
					TurnAwayDate == obj.TurnAwayDate &&
					ReferralMadeId == obj.ReferralMadeId;
		}

		#region predicates
		public static Expression<Func<TurnAwayService, bool>> TurnAwayDateBetween(DateTime? minTurnAwayDate, DateTime? maxTurnAwayDate) {
			var predicate = PredicateBuilder.New<TurnAwayService>(true);
			if (minTurnAwayDate != null)
				predicate.And(s => s.TurnAwayDate >= minTurnAwayDate);
			if (maxTurnAwayDate != null)
				predicate.And(s => s.TurnAwayDate <= maxTurnAwayDate);
			return predicate;
		}
		#endregion
	}
}