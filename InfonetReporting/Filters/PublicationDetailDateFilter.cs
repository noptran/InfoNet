using System;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class PublicationDetailDateFilter : DateFilter {
		public PublicationDetailDateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Publication Date";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PublicationDetail.Predicates.Add(PublicationDetail.PDateBetween(From, To));
		}
	}
}