using System;
using System.Linq;
using System.Linq.Expressions;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using LinqKit;

namespace Infonet.Reporting.Filters {
	public class OrderOfProtectionDateExpiredFilter : DateFilter {
		private static readonly Expression<Func<OrderOfProtection, DateTime?>> _LastExpiration = oop =>
			oop.OrderOfProtectionActivities.Any(op => op.NewExpirationDate != null)
				? oop.OrderOfProtectionActivities.Max(op => op.NewExpirationDate)
				: oop.OriginalExpirationDate;

		public OrderOfProtectionDateExpiredFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Date Expired";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var predicate = PredicateBuilder.New<OrderOfProtection>(true);
			if (From != null)
				predicate.And(op => _LastExpiration.Invoke(op) >= From);
			if (To != null)
				predicate.And(op => _LastExpiration.Invoke(op) <= To);
			if (predicate.IsStarted)
				context.OrderOfProtection.Predicates.Add(predicate);
		}
	}
}