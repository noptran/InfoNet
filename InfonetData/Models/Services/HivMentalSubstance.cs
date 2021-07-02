using System;
using Infonet.Core.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models._TLU;
using System.Linq.Expressions;
using LinqKit;

namespace Infonet.Data.Models.Services {
	public class HivMentalSubstance : IRevisable {
		public int ID { get; set; }

		public int LocationID { get; set; }

		[Lookup("HivMentalSubstance")]
		public int TypeID { get; set; }

		public DateTime HMSDate { get; set; }

		public int? AdultsNo { get; set; }

		public int? ChildrenNo { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual Center Center { get; set; }

		public virtual TLU_Codes_HivMentalSubstance TLU_Codes_HivMentalSubstance { get; set; }

		public bool IsUnchanged(HivMentalSubstance obj) {
			return TypeID == obj.TypeID && HMSDate == obj.HMSDate && AdultsNo == obj.AdultsNo && ChildrenNo == obj.ChildrenNo;
		}
	
		#region predicates
		public static Expression<Func<HivMentalSubstance, bool>> HMSDateBetween(DateTime? minDate, DateTime? maxDate) {
			var predicate = PredicateBuilder.New<HivMentalSubstance>(true);
			if (minDate != null)
				predicate.And(s => s.HMSDate >= minDate);
			if (maxDate != null)
				predicate.And(s => s.HMSDate <= maxDate);
			return predicate;
		}
		#endregion
	}
}