using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Data;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;

namespace Infonet.Web.ViewModels.Case {
	public class CaseOutline {
		private CaseOutline() {
			Pages = new List<Page>();
		}

		public IList<Page> Pages { get; }

		public static CaseOutline CreateFor(ClientCase model, bool hasShelter, bool allRelatedHasShelter) {
			var outline = new CaseOutline();
			outline.Pages.Add(CreateIntakePageFor(model, hasShelter));
			outline.Pages.Add(CreateMedicalPage());
			outline.Pages.Add(CreateOffendersPage(model));
			outline.Pages.Add(CreateInvestigationPage());
			outline.Pages.Add(CreateServicesPage(hasShelter, allRelatedHasShelter));
			outline.Pages.Add(CreateCloseoutPage());
			return outline;
		}

		private static Page CreateIntakePageFor(ClientCase model, bool hasShelter) {
			var page = new Page("Edit", CaseType.Any) {
				Heading = model == null ? "Intake" : Lookups.ClientType[model.Client.ClientTypeId].Description + " Intake",
				NewCaseAllowed = true
			};
			page.Panels.Add(new Panel("demographics", CaseType.Any) { Script = new[] { "_Demographics.js" } });
			page.Panels.Add(new Panel("presentingIssues", CaseType.Adult | CaseType.Victim | CaseType.ChildVictim) { Name = "Presenting Issues", Script = new[] { "_PresentingIssues.js" } });
			page.Panels.Add(new Panel("income", CaseType.Adult | CaseType.SA) { Script = new[] { "_Income.js" } });
			page.Panels.Add(new Panel("benefits", CaseType.DV) { Script = new[] { "_Benefits.js" } });
			page.Panels.Add(new Panel("referredBy", CaseType.Adult | CaseType.SA) { Name = "Referrals", Script = new[] { "_ReferredBy.js" } });
			page.Panels.Add(new Panel("referredBy", CaseType.ChildVictim) { Name = "Referred From", Script = new[] { "_ReferredBy.js" } });
			page.Panels.Add(new Panel("specialNeeds", CaseType.DV | CaseType.CAC) { Name = "Special Needs", Script = new[] { "_SpecialNeeds.js" } });
			page.Panels.Add(new Panel("specialNeeds", CaseType.SA) { Name = "Language & Disability Needs", Script = new[] { "_SpecialNeeds.js" } });
			page.Panels.Add(new Panel("servicesNeeded", CaseType.DV) { Name = "Services Needed", Partial = "_ServicesNeeded" });
			page.Panels.Add(new Panel("behaviors", CaseType.Child) { Script = new[] { "_Behaviors.js" } });
			page.Panels.Add(new Panel("residence", CaseType.Any) { Script = new[] { "_Residence.js" } });
			if (hasShelter)
				page.Panels.Add(new Panel("previousServiceUse", CaseType.DV) { Name = "Previous Service Use", Script = new[] { "_PreviousServiceUse.js" } });
			page.Panels.Add(new Panel("relationships", CaseType.CAC) { Name = "Relationships" });
			return page;
		}

		private static Page CreateMedicalPage() {
			var page = new Page("Medical", CaseType.Adult | CaseType.Victim) {
				Heading = "Medical/Criminal Justice"
			};
			page.Panels.Add(new Panel("medical", CaseType.Adult | CaseType.Victim) { Partial = "_Medical", Script = new[] { "_Medical.js" } });
			page.Panels.Add(new Panel("prosecution", CaseType.Adult | CaseType.Victim) { Partial = "_PoliceProsecution", Name = "Police/Prosecution", Script = new[] { "_PoliceProsecution.js" } });
			page.Panels.Add(new Panel("orders", CaseType.Adult) { Partial = "_OrdersOfProtection", Name = "Orders of Protection", Script = new[] { "_OrdersOfProtection.js" } });
			return page;
		}

		private static Page CreateOffendersPage(ClientCase model) {
			var page = new Page("Offenders", CaseType.Adult | CaseType.Victim | CaseType.ChildVictim) { Script = new[] { "_Offenders.js" }, FooterPartial = "_OffendersFooter" };
			if (model == null)
				return page;

			var offenders = model.OffendersById;
			foreach (var each in offenders.KeysFor(offenders.Values.IncludingRestorable.OrderBy(o => o.OffenderId, true).ThenBy(o => offenders.KeyFor(o).Occurrence))) {
				bool isNew = each.Components[0] == null;
				bool isDeleted = !offenders.ContainsKey(each);
				var relationshipId = offenders[each].RelationshipToClientId;
				string prefix = !isDeleted ? (isNew ? "+" : "=") : (isNew ? "~" : "-");
				var panel = new Panel(("offender" + each).Replace(':', '_'), CaseType.Any) {
					Name = Lookups.RelationshipToClient[relationshipId]?.Description ?? "Someone",
					Partial = "_Offender",
					IsCollapsed = isDeleted,
					IsDeleted = isDeleted,
					ViewData = new Dictionary<string, object> { ["offenderKey"] = each, ["offenderKeyPrefix"] = prefix }
				};
				page.Panels.Add(panel);
			}

			return page;
		}

		private static Page CreateInvestigationPage() {
			var page = new Page("Investigation", CaseType.ChildVictim | CaseType.ChildNonVictim) { Script = new[] { "Investigation.js" } };
			page.Panels.Add(new Panel("dcfsAllegations", CaseType.ChildVictim) { Name = "DCFS Allegations", Script = new[] { "_DcfsAllegations.js" } });
			page.Panels.Add(new Panel("petitions", CaseType.ChildVictim) { Name = "Abuse/Neglect Petitions", Script = new[] { "_Petitions.js" } });
			page.Panels.Add(new Panel("team", CaseType.ChildVictim) { Name = "Multidisciplinary Team", ShortName = "MDT", Script = new[] { "_Team.js" } });
			page.Panels.Add(new Panel("interview", CaseType.ChildVictim | CaseType.ChildNonVictim) { Name = "Victim Sensitive Interviews", ShortName = "VSI", Partial = "_VictimSensitiveInterviews", Script = new[] { "_VictimSensitiveInterviews.js" } });
			page.Panels.Add(new Panel("medicalVisit", CaseType.ChildVictim) { Name = "Medical Visits", Partial = "_MedicalVisits", Script = new[] { "_MedicalVisits.js" } });
			return page;
		}

		private static Page CreateServicesPage(bool hasShelter, bool allRelatedHasShelter) {
			var page = new Page("Services", CaseType.Any) { Script = new[] { "_Services.js" } };
			page.Panels.Add(new Panel("directService", CaseType.Any) { Name = "Direct Services", Partial = "_DirectServices", Script = new[] { "_DirectServices.js" } });

			if (allRelatedHasShelter)
				page.Panels.Add(new Panel("housingServices", CaseType.DV) { Name = "Housing Services", Partial = "_HousingServices", Script = new[] { "_HousingServices.js" } });

			if (hasShelter)
				page.Panels.Add(new Panel("departures", CaseType.DV) { Name = "Departure Information", Partial = "_Departures", Script = new[] { "_Departures.js" } });

			page.Panels.Add(new Panel("cancellations", CaseType.DV | CaseType.SA) { Name = "Cancellation/No Show", Partial = "_Cancellations", Script = new[] { "_Cancellations.js" } });
			page.Panels.Add(new Panel("referral", CaseType.CAC) { Name = "Referrals", Partial = "_Referral", Script = new[] { "_Referral.js" } });
			page.Panels.Add(new Panel("servicesReceived", CaseType.DV) { Name = "Services Received", Partial = "_ServicesReceived" });
			return page;
		}

		private static Page CreateCloseoutPage() {
			var page = new Page("Closeout", CaseType.Any) {
				Heading = "Case Closeout"
			};
			page.Panels.Add(new Panel("caseStatus", CaseType.Any) { Name = "Case Status" });
			page.Panels.Add(new Panel("behaviors", CaseType.Child) { Script = new[] { "_Behaviors.js" } });
			page.Panels.Add(new Panel("referredBy", CaseType.Adult) { Name = "Referrals", Script = new[] { "_ReferredBy.js" } });
			return page;
		}

		#region inner structs
		public struct Page {
			public Page(string action, CaseType visibility) {
				Action = action;
				Visibility = visibility;
				Heading = action;
				NewCaseAllowed = false;
				Panels = new List<Panel>();
				Script = null;
				FooterPartial = null;
			}

			public string Action { get; set; }
			public string Heading { get; set; }
			public CaseType Visibility { get; set; }
			public bool NewCaseAllowed { get; set; }
			public IList<Panel> Panels { get; }
			public string[] Script { get; set; }
			public string FooterPartial { get; set; }
		}

		public struct Panel {
			private string _shortName;

			public Panel(string id, CaseType visibility) {
				Id = id;
				Visibility = visibility;
				Name = id.Length == 0 ? "" : id.Substring(0, 1).ToUpper() + id.Substring(1);
				Partial = "_" + Name;
				Script = null;
				IsCollapsed = false;
				IsDeleted = false;
				ViewData = null;
				_shortName = null;
			}

			public string Id { get; set; }
			public CaseType Visibility { get; set; }
			public string Name { get; set; }
			public string Partial { get; set; }
			public string[] Script { get; set; }
			public bool IsCollapsed { get; set; }
			public bool IsDeleted { get; set; }
			public IDictionary<string, object> ViewData { get; set; }

			public string ShortName {
				get { return _shortName ?? Name; }
				set { _shortName = value; }
			}
		}
		#endregion
	}
}