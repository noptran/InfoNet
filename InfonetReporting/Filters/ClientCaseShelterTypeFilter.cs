using System;
using System.IO;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using LinqKit;

namespace Infonet.Reporting.Filters {
	/** In the context of this filter, Walk-in means not Sheltered.  In other words, to be a Walk-in, a ClientCase need not receive any service at all. **/
	public class ClientCaseShelterTypeFilter : ReportFilter {
		public ClientCaseShelterTypeFilter(int?[] shelterTypes, DateTime? from, DateTime? to) : this(shelterTypes.Cast<ShelterServiceEnum>().ToArray(), from, to) { }

		public ClientCaseShelterTypeFilter(ShelterServiceEnum[] shelterTypes, DateTime? from, DateTime? to) {
			Label = "Client Type";
			ShelterTypes = shelterTypes;
			From = from;
			To = to;
		}

		public ShelterServiceEnum[] ShelterTypes { get; set; }

		public DateTime? From { get; set; }

		public DateTime? To { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var isShelter = ServiceDetailOfClient.IsShelter();
			var datesIntersect = ServiceDetailOfClient.ShelterDatesIntersect(From, To);
			var selectedShelterIds = ShelterTypes.Where(t => t != ShelterServiceEnum.Walkin).Cast<int>().ToArray();
			bool selectedWalkin = ShelterTypes.Contains(ShelterServiceEnum.Walkin);

			var predicate = PredicateBuilder.New<ClientCase>(false);
			if (selectedShelterIds.Length > 0)
				predicate.Or(cc => cc.ServiceDetailsOfClient.Any(sd => selectedShelterIds.Contains(sd.ServiceID) && datesIntersect.Invoke(sd)));
			if (selectedWalkin)
				predicate.Or(cc => !cc.ServiceDetailsOfClient.Any(sd => isShelter.Invoke(sd) && datesIntersect.Invoke(sd)));
			context.ClientCase.Predicates.Add(predicate);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.WriteConjoined("or", null, ShelterTypes.Select(t => t.GetDisplayName()));
		}
	}
}