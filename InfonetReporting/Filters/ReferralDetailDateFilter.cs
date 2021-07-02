using System;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ReferralDetailDateFilter : DateFilter {
		public ReferralDetailDateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Referral Date";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ClientReferralDetail.Predicates.Add(ClientReferralDetail.ReferralDateBetween(From, To));
		}
	}
}