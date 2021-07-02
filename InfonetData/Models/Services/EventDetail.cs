using System;
using System.Collections.Generic;
using Infonet.Core.Entity;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models._TLU;
using System.Linq.Expressions;
using LinqKit;

namespace Infonet.Data.Models.Services {
	public class EventDetail : IRevisable {
		public EventDetail() {
			EventDetailStaff = new List<EventDetailStaff>();
		}

		public int ICS_ID { get; set; }
		public int CenterID { get; set; }
		public int ProgramID { get; set; }
		public string EventName { get; set; }
		public double? EventHours { get; set; }
		public int? NumPeopleReached { get; set; }
		public DateTime? EventDate { get; set; }
		public string Comment { get; set; }
		public string Location { get; set; }
		public int? FundDateID { get; set; }
		public int? CountyID { get; set; }
		public int? StateID { get; set; }
		public DateTime? RevisionStamp { get; set; }
		public virtual Center Center { get; set; }
		public virtual TLU_Codes_ProgramsAndServices TLU_Codes_ProgramsAndServices { get; set; }
		public virtual ICollection<EventDetailStaff> EventDetailStaff { get; set; }
		public virtual FundingDate FundingDate { get; set; }
		
		#region predicates
		public static Expression<Func<EventDetail, bool>> EventDateBetween(DateTime? minEventDate, DateTime? maxEventDate) {
			var predicate = PredicateBuilder.New<EventDetail>(true);
			if (minEventDate != null)
				predicate.And(s => s.EventDate >= minEventDate);
			if (maxEventDate != null)
				predicate.And(s => s.EventDate <= maxEventDate);
			return predicate;
		}
		#endregion
	}
}