using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infonet.Data;
using Infonet.Data.Helpers;
using Infonet.Data.Looking;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Views;
using Infonet.Usps.Data;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace Infonet.Reporting.Core {
	public class ReportContainer : IDisposable {
		#region static fields
		private static IRazorEngineService _Razor = null;
		#endregion

		#region fields
		private int? _commandTimeout = 600;
		private InfonetServerContext _infonetContext = null;
		private UspsContext _uspsContext = null;
		private int?[] _centerIds = new int?[0];
		private IEnumerable<CenterInfo> _centers = null;
		#endregion

		#region constructing/disposing
		public ReportContainer(string title) {
			Title = title;
			Reports = new List<IReportQuery>();
			GroupedSubReports = new Dictionary<SubReportSelection, List<IReportTable>>();
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (_infonetContext != null) {
					_infonetContext.Dispose();
					_infonetContext = null;
				}
				if (_uspsContext != null) {
					_uspsContext.Dispose();
					_uspsContext = null;
				}
			}
		}

		public void Dispose() {
			Dispose(true);
		}
		#endregion

		public int? CommandTimeout {
			get { return _commandTimeout; }
			set {
				_commandTimeout = value;
				if (_infonetContext != null)
					_infonetContext.CommandTimeout = _commandTimeout;
				if (_uspsContext != null)
					_uspsContext.CommandTimeout = _commandTimeout;
			}
		}

		public InfonetServerContext InfonetContext {
			get { return _infonetContext ?? (_infonetContext = new InfonetServerContext { CommandTimeout = _commandTimeout }); }
		}

		public UspsContext UspsContext {
			get { return _uspsContext ?? (_uspsContext = new UspsContext { CommandTimeout = _commandTimeout }); }
		}

		public string Title { get; set; }
        public string TitleNote { get; set; }
		public List<IReportQuery> Reports { get; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public Provider Provider { get; set; }
		public Dictionary<SubReportSelection, List<IReportTable>> GroupedSubReports { get; }
		public DateTime ReportRanTimestamp { get; private set; }

		public int?[] CenterIds {
			get { return _centerIds; }
			set {
				_centerIds = value;
				_centers = null;
			}
		}

		public IEnumerable<CenterInfo> Centers {
			get { return _centers ?? (_centers = _centerIds.Select(id => id == null ? null : InfonetContext.Helpers.Center.GetCenterById(id.Value)).ToArray()); }
		}

		public void Write(TextWriter html, TextWriter csv) {
			ReportRanTimestamp = DateTime.Now;
			var outputs = html == null ? null : new List<OrderedTextOutput>();
			foreach (var each in Reports) {
				each.ReportContainer = this;
				each.Write(outputs, csv);
			}
			if (html == null)
				return;

			outputs.Sort();
			foreach (var each in outputs)
				html.Write(each.Output);
		}

		#region static razor engine
		public static IRazorEngineService Razor {
			get {
				if (_Razor == null)
					throw new Exception("ReportContainer.Razor has not been initialized");

				return _Razor;
			}
		}

		/* not threadsafe */
		public static void InitializeRazor() {
			if (_Razor != null)
				throw new Exception("ReportContainer.Razor already initialized");

			_Razor = RazorEngineService.Create(new TemplateServiceConfiguration {
				TemplateManager = new EmbeddedResourceTemplateManager(typeof(ViewResources)),
				DisableTempFileLocking = true
			});
		}

		/* not threadsafe */
		public static void DisposeRazor() {
			if (_Razor != null) {
				_Razor.Dispose();
				_Razor = null;
			}
		}
		#endregion
	}
}