using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Infonet.Core;
using Infonet.Core.Collections;
using Infonet.Core.Data;
using Infonet.Core.IO;
using Infonet.Data.Models.Reporting;
using Infonet.Reporting.AdHoc;
using Infonet.Reporting.AdHoc.Pivots;
using Infonet.Reporting.AdHoc.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rotativa;
using Rotativa.Options;

//KMS DO pdf options before running?
namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "CACADMIN, DVADMIN, SAADMIN")]
	public class AdHocController : InfonetControllerBase {
		private const string CSV_CONTENT_TYPE = "text/csv";

		private static readonly string _WkHtmlToPdfOptions = ConfigurationManager.AppSettings["Reporting:WkHtmlToPdf:Options"];

		private IEnumerable<SqlParameter> SessionParameters {
			get {
				yield return new SqlParameter("@locationId", SqlDbType.Int) { Value = Session.Center().Id };
				yield return new SqlParameter("@centerId", SqlDbType.Int) { Value = Session.Center().Top.Id };
				yield return new SqlParameter("@providerId", SqlDbType.Int) { Value = Session.Center().ProviderId };
			}
		}

		public ActionResult Index(int? id = null, Perspective perspective = Perspective.Client) {
			AdHocQuery model;
			if (id != null) {
				model = db.RPT_AdHocQueries.Single(q => q.Id == id && q.UserId == UserId);
				//perspective parameter is ignored when loading a saved query
			} else {
				var json = new JObject { ["perspective"] = perspective.ToString() };
				model = new AdHocQuery { Name = "New Query", Json = json.ToString() };
			}
			return View(model);
		}

		#region page building
		public ActionResult Conjunction(Perspective perspective, string conjunctionJson, bool isRoot = false) {
			ViewBag.Perspective = perspective;
			ViewBag.Json = JObject.Parse(conjunctionJson);
			ViewBag.IsRoot = isRoot;
			return PartialView("_Conjunction", DataModel.For(Session.Center().Provider, perspective));
		}

		public ActionResult Predicate(Perspective perspective, string predicateJson) {
			ViewBag.Perspective = perspective;
			ViewBag.Json = JObject.Parse(predicateJson);
			return PartialView("_Predicate", DataModel.For(Session.Center().Provider, perspective));
		}

		public ActionResult PredicateCondition(Perspective perspective, string predicateJson) {
			ViewBag.Perspective = perspective;
			ViewBag.Json = JObject.Parse(predicateJson);
			return PartialView("_Predicate_Condition", DataModel.For(Session.Center().Provider, perspective));
		}

		public ActionResult PredicateInput(Perspective perspective, string predicateJson) {
			Func<Field, IEnumerable<Option>> options = f => f.Options ?? (f.OptionsSql == null ? null : QueryOptionsFor(f));

			ViewBag.Json = JObject.Parse(predicateJson);
			ViewBag.Options = options;
			return PartialView("_Predicate_Condition_Input", DataModel.For(Session.Center().Provider, perspective));
		}

		private IEnumerable<Option> QueryOptionsFor(Field field) {
			var result = new List<Option>();
			using (var command = new SqlCommand(field.OptionsSql, db.Database.Connection as SqlConnection)) {
				command.Parameters.AddRange(SessionParameters);
				command.Parameters.Add(new SqlParameter("@effectiveDate", SqlDbType.DateTime) { Value = DateTime.Now });
				using (var reader = command.ExecuteReader(CommandBehavior.SingleResult, true)) {
					int iValue = reader.OrdinalOf(nameof(Option.Value), 0);
					int iLabel = reader.OrdinalOf(nameof(Option.Label), 1);
					int iGroup = reader.OrdinalOf(nameof(Option.Group), 2);
					while (reader.Read()) {
						var each = new Option();
						if (iValue != -1)
							each.Value = ConvertNull.ToObject(reader[iValue]);
						if (iLabel != -1)
							each.Label = ConvertNull.ToString(reader[iLabel]);
						if (iGroup != -1)
							each.Group = ConvertNull.ToString(reader[iGroup]);
						result.Add(each);
					}
				}
			}
			return result;
		}
		#endregion

		#region run menu
		[HttpPost]
		public ActionResult Run(string queryJson, bool isPdf = false, int? id = null) {
			var jo = JObject.Parse(queryJson);
			QueryPivot pivot;
			var query = ParseQuery(jo, out pivot);
			var model = new ReportViewModel {
				Perspective = (Perspective)Enum.Parse(typeof(Perspective), jo["perspective"].Value<string>(), true),
				Title = jo["title"]?.Value<string>() ?? "Ad Hoc Report",
				IsPdf = isPdf
			};
			if (pivot == null)
				model.Renderer = w => Output.WriteData(w, db.Database.Connection as SqlConnection, query, SessionParameters);
			else
				model.Renderer = w => Output.WritePivot(w, db.Database.Connection as SqlConnection, query, pivot, SessionParameters);

			db.RPT_AdHocTracking.Add(new AdHocTracking { Json = queryJson, OutputId = (isPdf ? ReportOutputType.Pdf : ReportOutputType.Html).ToInt32(), UserId = UserId, RunDate = DateTime.Now, QueryId = id });
			db.SaveChanges();

			if (isPdf) {
				var pdfView = new ViewAsPdf("Report", model) {
					PageSize = Size.Letter,
					PageOrientation = Orientation.Portrait,
					ContentDisposition = ContentDisposition.Attachment,
					FileName = $"{model.Title.Replace(" ", "")}_{DateTime.Now:yyyy-MM-dd-HHmm}.pdf"
					//KMS DO spaces probably not enough
				};
				if (_WkHtmlToPdfOptions != null)
					pdfView.CustomSwitches = _WkHtmlToPdfOptions;
				return pdfView;
			}
			return View("Report", model);
		}

		[HttpPost]
		public ActionResult ExportCsv(string queryJson, int? id = null) {
			var jo = JObject.Parse(queryJson);
			QueryPivot pivot;
			var query = ParseQuery(jo, out pivot);

			db.RPT_AdHocTracking.Add(new AdHocTracking { Json = queryJson, OutputId = ReportOutputType.Csv.ToInt32(), UserId = UserId, RunDate = DateTime.Now, QueryId = id });
			db.SaveChanges();

			Response.ClearHeaders();
			Response.ContentType = CSV_CONTENT_TYPE;
			//KMS DO replacing spaces might not be enough
			//KMS DO "Ad Hoc Report" const?
			Response.AddHeader("Content-Disposition", $"attachment;filename={(jo["title"]?.Value<string>() ?? "Ad Hoc Report").Replace(" ", "")}_{DateTime.Now:yyyy-MM-dd-HHmm}.csv");
			using (var sw = new StreamWriter(Response.OutputStream, Encoding.UTF8, BufferHelper.DEFAULT_STREAMWRITER_BUFFER_SIZE, true))
			using (var command = query.ToCommand(db.Database.Connection as SqlConnection, SessionParameters, Output.CommandTimeout))
			using (var reader = command.ExecuteReader(CommandBehavior.SingleResult, true))
			using (var csv = CsvWriter.WriteHeaders(sw, query.Select.Select(f => f.Label))) {
				var fields = query.Select.Select(f => f.CreateReader()).ToArray();
				while (reader.Read()) {
					foreach (var each in fields)
						csv.WriteField(each.Field.Format(each.Read(reader)));
					csv.WriteEol();
				}
			}
			return new EmptyResult();
		}

		[HttpPost]
		public ActionResult OutputSql(string queryJson) {
			QueryPivot pivot;
			var jo = JObject.Parse(queryJson);

			ViewBag.Title = "Query SQL";
			ViewBag.Text = ParseQuery(jo, out pivot).ToSql(SessionParameters);

			return PartialView("_OutputModal");
		}

		[HttpPost]
		public ActionResult OutputJson(string queryJson) {
			ViewBag.Title = "Report JSON";
			ViewBag.Text = JObject.Parse(queryJson).ToString(Formatting.Indented);
			return PartialView("_OutputModal");
		}
		#endregion

		#region query menu
		public ActionResult Open() {
			return PartialView("_OpenModal", db.Database.SqlQuery<QueryViewModel>("SELECT q.ID as QueryID, q.Name, q.RevisionStamp, MAX(RunDate) AS LastRunDate, COUNT(DISTINCT t.ID) AS RunCount FROM Rpt_AdHocQueries q LEFT JOIN Rpt_AdHocTracking t ON q.ID = t.QueryID WHERE q.UserID = @p0 GROUP BY q.ID, q.Name, q.RevisionStamp ORDER BY q.Name", UserId));
		}

		[HttpPost]
		public JsonResult Save(int? id, string name, string queryJson) {
			var query = id != null
				? db.RPT_AdHocQueries.Single(q => q.Id == id && q.UserId == UserId)
				: db.RPT_AdHocQueries.Add(new AdHocQuery { UserId = UserId });

			if (name != query.Name) {
				string original = name;
				int i = 0;
				while (db.RPT_AdHocQueries.Count(q => q.UserId == UserId && q.Name == name) > 0)
					name = $"{original}({++i})";
			}

			query.Name = name;
			query.Json = queryJson;
			db.SaveChanges();

			return Json(new { id = query.Id, name = query.Name });
		}

		[HttpPost]
		public JsonResult Delete(int id) {
			var query = db.RPT_AdHocQueries.Single(q => q.Id == id && q.UserId == UserId);
			db.RPT_AdHocQueries.Remove(query);
			db.SaveChanges();

			return Json(new { id = query.Id, name = query.Name });
		}
		#endregion

		#region json query parsing
		private Query ParseQuery(JObject jsonQuery, out QueryPivot pivot) {
			pivot = null;
			var model = DataModel.For(Session.Center().Provider, Enums.Parse<Perspective>(jsonQuery["perspective"].Value<string>()));

			IEnumerable<Field> select = null;
			var jsonQuerySelect = jsonQuery["select"];
			if (jsonQuerySelect?.Type == JTokenType.Array) {
				select = model.Select(jsonQuerySelect.Values<string>());
			} else if (jsonQuerySelect?.Type == JTokenType.Object) {
				var columns = model.Select(jsonQuerySelect["columns"]?.Values<string>() ?? Array.Empty<string>()).ToArray();
				var rows = model.Select(jsonQuerySelect["rows"]?.Values<string>() ?? Array.Empty<string>()).ToArray();
				select = columns.Concat(rows);
				string aggregateId = jsonQuerySelect["aggregate"]?.Value<string>();
				if (aggregateId == null) {
					pivot = new QueryPivot(rows, columns) { Caption = "Record Counts" };
				} else {
					var aggregate = model.Field(aggregateId);
					select = select.Concat(new[] { aggregate });
					pivot = new QueryPivot(rows, columns, () => new CountDistinct(aggregate)) { Caption = $"Unique {model.Entities[aggregate.ParentId].Label} {aggregate.Label} Counts" };
				}
			} else if (jsonQuerySelect != null) {
				throw new ArgumentException($"{nameof(jsonQuery)}.select must be an Array, Object or null");
			}

			var result = new Query(model) {
				Select = select,
				Distinct = true,
				Where = jsonQuery["where"] == null ? null : ParseConjunction(model, jsonQuery["where"])
			};
			return result;
		}

		private static FieldPredicate ParsePredicate(Model model, JToken predicate) {
			string field = predicate["field"].Value<string>();
			var condition = Enums.Parse<Condition>(predicate["condition"].Value<string>());
			var arguments = predicate.Children<JProperty>().Where(jp => jp.Name != "field" && jp.Name != "condition").ToDictionary(jp => jp.Name, jp => jp.Value.HasValues ? (object)jp.Value.Values<string>() : jp.Value.Value<string>());
			return condition.ToPredicate(model.Field(field), arguments);
		}

		private static ConjunctionPredicate ParseConjunction(Model model, JToken conjunction) {
			var op = Enums.Parse<PredicateOperator>(conjunction["operator"].Value<string>());
			var predicates = conjunction["predicates"].Select(jt => jt["predicates"] == null ? ParsePredicate(model, jt) : (IPredicate)ParseConjunction(model, jt));
			return new ConjunctionPredicate(op, predicates);
		}
		#endregion

		#region inner
		public class ReportViewModel {
			public Perspective Perspective { get; set; }
			public string Title { get; set; }
			public bool IsPdf { get; set; }
			public Func<TextWriter, int> Renderer { get; set; }
		}

		public class QueryViewModel {
			public int QueryId { get; set; }
			public string Name { get; set; }
			public DateTime? RevisionStamp { get; set; }
			public DateTime? LastRunDate { get; set; }
			public int RunCount { get; set; }
		}
		#endregion
	}
}