using System;
using System.Linq.Expressions;
using Infonet.Core.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using LinqKit;

namespace Infonet.Data.Models.Clients {
	public class ClientReferralDetail : IRevisable {
		public int ReferralDetailID { get; set; }

		public int ClientID { get; set; }

		public int CaseID { get; set; }

		public DateTime? ReferralDate { get; set; }

		[Lookup("ReferralType")]
		public int? ReferralTypeID { get; set; }

		public int? AgencyID { get; set; }

		[Lookup("ReferralResponse")]
		public int? ResponseID { get; set; }

		public int? LocationID { get; set; }

		public int? CityTownTownshpID { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual Agency Agency { get; set; }

		public virtual Center Center { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		public virtual TwnTshipCounty TwnTshipCounty { get; set; }
		
		#region predicates
		public static Expression<Func<ClientReferralDetail, bool>> ReferralDateBetween(DateTime? minServiceDate, DateTime? maxServiceDate) {
			var predicate = PredicateBuilder.New<ClientReferralDetail>(true);
			if (minServiceDate != null)
				predicate.And(s => s.ReferralDate >= minServiceDate);
			if (maxServiceDate != null)
				predicate.And(s => s.ReferralDate <= maxServiceDate);
			return predicate;
		}

		#endregion
	}
}