using System;
using System.Linq.Expressions;
using Infonet.Core.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using LinqKit;

namespace Infonet.Data.Models.Services {
	public class PhoneHotline : IRevisable {
		public int? PH_ID { get; set; }

		public int? SVID { get; set; }

		public int CenterID { get; set; }

		public DateTime? Date { get; set; }

		#region obsolete
		[Obsolete]
		public string Staff_Volunteer { get; set; }
		#endregion

		[Lookup("HotlineCallType")]
		public int? CallTypeID { get; set; }

		public int? NumberOfContacts { get; set; }

		public int? FundDateID { get; set; }

		public string Town { get; set; }

		public string Township { get; set; }

		public string ZipCode { get; set; }

		public int? CountyID { get; set; }

		public DateTime? TimeOfDay { get; set; }

		public double? TotalTime { get; set; }

		[Lookup("ReferralSource")]
		public int? ReferralFromID { get; set; }

		[Lookup("ReferralSource")]
		public int? ReferralToID { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public int? Age { get; set; }

		[Lookup("Sex")]
		public int? SexID { get; set; }

		[Lookup("Race")]
		public int? RaceID { get; set; }

		[Lookup("ClientType")]
		public int? ClientTypeID { get; set; }

		public virtual Center Center { get; set; }

		public virtual StaffVolunteer StaffVolunteer { get; set; }

		public virtual FundingDate FundingDate { get; set; }

		#region predicates
		public static Expression<Func<PhoneHotline, bool>> DateBetween(DateTime? minDate, DateTime? maxDate) {
			var predicate = PredicateBuilder.New<PhoneHotline>(true);
			if (minDate != null)
				predicate.And(ph => ph.Date >= minDate);
			if (maxDate != null)
				predicate.And(ph => ph.Date <= maxDate);
			return predicate;
		}
		#endregion
	}
}