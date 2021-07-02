using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using Infonet.Web.Mvc;
using Infonet.Web.ViewModels.Admin;
using PagedList;

namespace Infonet.Web.Controllers {
	[Authorize(Roles = "DATAEXPORTER")]
	public class ExportStaffInfoController : InfonetControllerBase {
		#region constants
		private const string CSV = ".csv";
		private const string CSV_CONTENT_TYPE = "text/csv";
		#endregion

		public ActionResult Search(ExportStaffInfoViewModel model, int? page, bool download = false) {
			if (!download) {
				model.SearchResults = GetStaffQuery(model).ToPagedList(page ?? 1, model.PageSize == -1 ? int.MaxValue : model.PageSize);
				return View(model);
			}

			Response.ClearHeaders();
			Response.ContentType = CSV_CONTENT_TYPE;
			Response.AddHeader("Content-Disposition", $"attachment;filename=ExportedStaff_{DateTime.Now:yyyy-MM-dd-HHmmss}{CSV}");
			using (var sw = new StreamWriter(Response.OutputStream, Encoding.UTF8, BufferHelper.DEFAULT_STREAMWRITER_BUFFER_SIZE, true))
				WriteCsv(sw, model);
			return new EmptyResult();
		}

		private IOrderedQueryable<StaffVolunteer> GetStaffQuery(ExportStaffInfoViewModel model) {
			int centerId = Session.Center().Id;
			var query = db.T_StaffVolunteer.Where(sv => sv.CenterId == centerId);

			if (model.Status == StaffStatus.Active)
				query = query.Where(sv => (sv.StartDate == null || sv.StartDate <= DateTime.Today) && (sv.TerminationDate == null || sv.TerminationDate > DateTime.Today));
			if (model.Status == StaffStatus.Inactive)
				query = query.Where(sv => sv.StartDate != null && sv.StartDate > DateTime.Today || sv.TerminationDate != null && sv.TerminationDate <= DateTime.Today);

			if (model.TypeOfStaff != null)
				query = query.Where(sv => sv.TypeId == (int)model.TypeOfStaff);

			if (model.PersonnelTypeId != null)
				query = query.Where(sv => sv.PersonnelTypeId == model.PersonnelTypeId);

			return query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);
		}

		private void WriteCsv(TextWriter w, ExportStaffInfoViewModel model) {
			var clients = GetStaffQuery(model);
			using (var csv = CsvWriter.WriteHeaders(w, new[] { "Staff ID", "Last Name", "First name", "Staff Type", "Personnel Type", "Start Date", "Termination Date" }, false))
				foreach (var each in clients) {
					csv.WriteField(each.SvId);
					csv.WriteField(each.LastName);
					csv.WriteField(each.FirstName);
					csv.WriteField(each.TypeOfStaff.ToString().Substring(0, 1));
					csv.WriteField(Lookups.PersonnelType[each.PersonnelTypeId]?.Description);
					csv.WriteField(each.StartDate, "MM/dd/yyyy");
					csv.WriteField(each.TerminationDate, "MM/dd/yyyy");
					csv.WriteEol();
				}
		}
	}
}