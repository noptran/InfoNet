using System;
using Infonet.Core.Entity;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models._TLU;
using System.Linq.Expressions;
using LinqKit;

namespace Infonet.Data.Models.Services {
	public class OtherStaffActivity : IRevisable {
		public int? OsaID { get; set; }
		public int? SVID { get; set; }
		public int? OtherStaffActivityID { get; set; }
		public float? ConductingHours { get; set; }
		public float? TravelHours { get; set; }
		public float? PrepareHours { get; set; }
		public DateTime? OsaDate { get; set; }

		#region obsolete
		[Obsolete]
		public string ServiceType { get; set; }
		#endregion

		public DateTime? RevisionStamp { get; set; }
		public virtual StaffVolunteer StaffVolunteer { get; set; }
		public virtual TLU_Codes_OtherStaffActivity TLU_Codes_OtherStaffActivity { get; set; }

		#region predicates
		public static Expression<Func<OtherStaffActivity, bool>> OsaDateBetween(DateTime? minOsaDate, DateTime? maxOsaDate) {
			var predicate = PredicateBuilder.New<OtherStaffActivity>(true);
			if (minOsaDate != null)
				predicate.And(osa => osa.OsaDate >= minOsaDate);
			if (maxOsaDate != null)
				predicate.And(osa => osa.OsaDate <= maxOsaDate);
			return predicate;
		}
		#endregion

	}
}