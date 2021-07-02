using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Web.Mvc;
using Infonet.Web.ViewModels.Admin;
using PagedList;

namespace Infonet.Web.Controllers {
	[Authorize(Roles = "DATAEXPORTER")]
	public class ExportClientInfoController : InfonetControllerBase {
		#region constants
		private const string CSV = ".csv";
		private const string CSV_CONTENT_TYPE = "text/csv";
		#endregion

		public ActionResult Search(ExportClientInfoViewModel model, int? page, bool download = false) {
			if (!download) {
				model.SearchResults = GetClientsQuery(model).ToPagedList(page ?? 1, model.PageSize == -1 ? int.MaxValue : model.PageSize);
				return View(model);
			}
			Response.ClearHeaders();
			Response.ContentType = CSV_CONTENT_TYPE;
			Response.AddHeader("Content-Disposition", $"attachment;filename=ExportedClients_{DateTime.Now:yyyy-MM-dd-HHmmss}{CSV}");
			using (var sw = new StreamWriter(Response.OutputStream, Encoding.UTF8, BufferHelper.DEFAULT_STREAMWRITER_BUFFER_SIZE, true))
				WriteCsv(sw, model);
			return new EmptyResult();
		}

		private IOrderedQueryable<ClientCase> GetClientsQuery(ExportClientInfoViewModel model) {
			int centerId = Session.Center().Id;
			var query = db.T_ClientCases.Include(cs => cs.Client.ClientRaces).Where(cs => cs.Client.CenterId == centerId);
			if (model.StartDate != null)
				query = query.Where(c => c.FirstContactDate >= model.StartDate);
			if (model.EndDate != null)
				query = query.Where(c => c.FirstContactDate <= model.EndDate);
			if (model.ClientCode != null)
				query = query.Where(c => c.Client.ClientCode.Contains(model.ClientCode));
			return query.OrderBy(cs => cs.Client.ClientCode).ThenByDescending(cs => cs.CaseId);
		}

		private void WriteCsv(TextWriter w, ExportClientInfoViewModel model) {
			var clients = GetClientsQuery(model);
			using (var csv = CsvWriter.WriteHeaders(w, new[] { "Client ID", "Client Code", "Sex", "Ethnicity", "Race", "First Contact Date" }, false))
				foreach (var each in clients) {
					csv.WriteField(each.ClientId);
					csv.WriteField(each.Client.ClientCode);
					csv.WriteField(Lookups.GenderIdentity[each.Client.GenderIdentityId]?.Description);
					csv.WriteField(Lookups.Ethnicity[each.Client.EthnicityId]?.Description);
					if (each.Provider == Provider.CAC)
						csv.WriteField(Lookups.Race[each.Client.RaceId]?.Description);
					else
						csv.WriteField(string.Join(",", each.Client.RaceHudIds.Select(r => Lookups.RaceHud[r].Description)));
					csv.WriteField(each.FirstContactDate, "MM/dd/yyyy");
					csv.WriteEol();
				}
		}
	}
}