using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.Offender {
	public class MedicalCJOffendersVictimizedMultipleReportTable : ReportTable<MedicalCJOffendersLineItem> {
		private readonly List<int> _offenderIds = new List<int>();
		private readonly ICollection<ClientOffender> _offendersWithClients = new Collection<ClientOffender>();

		public MedicalCJOffendersVictimizedMultipleReportTable(string title, int displayOrder) : base(title, displayOrder) { }
		
		//KMS DO this runs lots of extra queries
		public override void PostCheckAndApply(ReportContainer reportContainer) {
			var offenderListQ = from offender in reportContainer.InfonetContext.T_OffenderList where reportContainer.CenterIds.Contains(offender.ParentCenterId) select offender.OffenderListingId;
			var offenderQ = from offender in reportContainer.InfonetContext.T_Offender where offenderListQ.Contains(offender.OffenderListingId.Value) select offender;
			offenderQ = from offender in offenderQ where _offenderIds.Contains(offender.OffenderListingId.Value) select offender;

			var lineItems = new List<MedicalCJOffendersLineItem>();
			foreach (var i in offenderQ) {
				var current = new MedicalCJOffendersLineItem {
					OffenderRecordID = i.OffenderId,
					OffenderID = i.OffenderListingId,
					ClientID = i.ClientId,
					CaseID = i.CaseId,
					ClientStatus = i.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).First().Value >= reportContainer.StartDate && i.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= reportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
					RaceID = i.RaceId,
					Age = i.Age,
					VisitationID = i.VisitationId,
					RelationshipToVictimID = i.RelationshipToClientId,
					GenderID = i.SexId,
					RegisteredID = i.RegisteredId
				};

				lineItems.Add(current);
			}

			foreach (var item in lineItems) {
				var currentOffenderWithClients = _offendersWithClients.FirstOrDefault(a => item.OffenderID == a.OffenderId);
				if (currentOffenderWithClients == null) {
					currentOffenderWithClients = new ClientOffender { OffenderId = item.OffenderID };
					_offendersWithClients.Add(currentOffenderWithClients);
				}
				currentOffenderWithClients.MedicalCJOffendersLineItems.Add(item);
			}

			var allOffendersWithMultipleClients = _offendersWithClients.Where(x => x.MedicalCJOffendersLineItems.Count > 1);
			foreach (var offender in allOffendersWithMultipleClients) {
				foreach (ReportRow row in Rows) {
					if (offender.MedicalCJOffendersLineItems.Any(o => o.ClientStatus == ReportTableHeaderEnum.New))
						row.Counts["New"]["Total"] += 1;
					else if (offender.MedicalCJOffendersLineItems.Any(o => o.ClientStatus == ReportTableHeaderEnum.Ongoing))
						row.Counts["Ongoing"]["Total"] += 1;

					row.Counts["Total"]["Total"] += 1;
				}
			}
		}

		public override void CheckAndApply(MedicalCJOffendersLineItem item) {
			_offenderIds.Add(item.OffenderID.Value);
		}
	}

	internal class ClientOffender {
		public ClientOffender() {
			MedicalCJOffendersLineItems = new List<MedicalCJOffendersLineItem>();
		}

		public int? OffenderId { get; set; }
		public List<MedicalCJOffendersLineItem> MedicalCJOffendersLineItems { get; }
	}
}