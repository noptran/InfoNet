using System;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using LinqKit;

namespace Infonet.Reporting.Filters {
	public class ServiceDetailDateFilter : DateFilter {
		public ServiceDetailDateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Service Date";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var predicate = ServiceDetailOfClient.IsNotShelter().And(ServiceDetailOfClient.ServiceDateBetween(From, To));
			if (container.Provider == Provider.DV)
				predicate = predicate.Or(ServiceDetailOfClient.IsShelter().And(ServiceDetailOfClient.ShelterDatesIntersect(From, To)));
			context.ServiceDetailOfClient.Predicates.Add(predicate);
		}
	}
}