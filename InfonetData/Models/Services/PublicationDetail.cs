using System;
using System.Collections.Generic;
using Infonet.Core.Entity;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models._TLU;
using System.Linq.Expressions;
using LinqKit;

namespace Infonet.Data.Models.Services {
	public class PublicationDetail : IRevisable {
		public PublicationDetail() {
			PublicationDetailStaff = new List<PublicationDetailStaff>();
		}

		public int ICS_ID { get; set; }
		public int CenterID { get; set; }
		public int ProgramID { get; set; }
		public DateTime? PDate { get; set; }
		public int? FundDateID { get; set; }
		public string Title { get; set; }

		#region obsolete
		[Obsolete]
		public int? NumStaff { get; set; }
		#endregion

		public double? PrepareHours { get; set; }
		public int? NumOfBrochure { get; set; }
		public string Comment_Pub { get; set; }
		public DateTime? RevisionStamp { get; set; }
		public virtual Center Center { get; set; }
		public virtual TLU_Codes_ProgramsAndServices TLU_Codes_ProgramsAndServices { get; set; }
		public virtual IList<PublicationDetailStaff> PublicationDetailStaff { get; set; }
		public virtual FundingDate FundingDate { get; set; }

		#region predicates
		public static Expression<Func<PublicationDetail, bool>> PDateBetween(DateTime? minPDate, DateTime? maxPDate) {
			var predicate = PredicateBuilder.New<PublicationDetail>(true);
			if (minPDate != null)
				predicate.And(s => s.PDate >= minPDate);
			if (maxPDate != null)
				predicate.And(s => s.PDate <= maxPDate);
			return predicate;
		}
		#endregion
	}
}