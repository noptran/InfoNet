using System;
using System.Collections.Generic;
using Infonet.Core.Entity;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models._TLU;
using System.Linq.Expressions;
using LinqKit;

namespace Infonet.Data.Models.Services {
	public class ProgramDetail : IRevisable {
		public ProgramDetail() {
			ServiceDetailsOfClient = new List<ServiceDetailOfClient>();
			ProgramDetailStaff = new List<ProgramDetailStaff>();
		}

		public int? ICS_ID { get; set; }
		public int CenterID { get; set; }
		public int ProgramID { get; set; }
		public int? NumOfSession { get; set; }
		public double? Hours { get; set; }
		public int? ParticipantsNum { get; set; }
		public DateTime? PDate { get; set; }

		#region obsolete
		[Obsolete]
		public int? ConductStaffNum { get; set; }
		#endregion

		public string Comment_Act { get; set; }
		public int? FundDateID { get; set; }

		#region obsolete
		[Obsolete]
		public bool ChildSpecific { get; set; }

		[Obsolete]
		public string AgencyName { get; set; }
		#endregion

		public string Location { get; set; }
		public DateTime? RevisionStamp { get; set; }
		public int? AgencyID { get; set; }
		public int? Agency_ICS_ID { get; set; }
		public int? CountyID { get; set; }
		public int? StateID { get; set; }
		public virtual Agency Agency { get; set; }
		public virtual Center Center { get; set; }
		public virtual TLU_Codes_ProgramsAndServices TLU_Codes_ProgramsAndServices { get; set; }
		public virtual IList<ServiceDetailOfClient> ServiceDetailsOfClient { get; set; }
		public virtual IList<ProgramDetailStaff> ProgramDetailStaff { get; set; }
		public virtual FundingDate FundingDate { get; set; }

		#region predicates
		public static Expression<Func<ProgramDetail, bool>> PDateBetween(DateTime? minPDate, DateTime? maxPDate) {
			var predicate = PredicateBuilder.New<ProgramDetail>(true);
			if (minPDate != null)
				predicate.And(s => s.PDate >= minPDate);
			if (maxPDate != null)
				predicate.And(s => s.PDate <= maxPDate);
			return predicate;
		}
		#endregion
	}
}