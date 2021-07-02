using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Infonet.Core;
using Infonet.Core.Data;
using Infonet.Reporting.AdHoc.Pivots;
using Infonet.Reporting.Core;
using RazorEngine.Templating;

namespace Infonet.Reporting.AdHoc {
	public static class Output {
		public const string EMPTY = "\u00A0\u2014\u00A0";

		public static readonly int? CommandTimeout = ConvertNull.ToInt32(ConfigurationManager.AppSettings["Reporting:AdHoc:TimeoutSeconds"]);

		public static int WritePivot(TextWriter html, SqlConnection connection, Query query, QueryPivot pivot, IEnumerable<SqlParameter> externalParameters = null, int? timeout = null) {
			int count = 0;
			using (var command = query.ToCommand(connection, externalParameters, timeout ?? CommandTimeout))
			using (var reader = command.ExecuteReader(CommandBehavior.SingleResult, true))
				while (reader.Read()) {
					pivot.Ingest(reader);
					count++;
				}
			ReportContainer.Razor.RunCompile("AdHoc._Pivot", html, typeof(QueryPivot), pivot);
			return count;
		}

		public static int WriteData(TextWriter html, SqlConnection connection, Query query, IEnumerable<SqlParameter> externalParameters = null, int? timeout = null) {
			using (var command = query.ToCommand(connection, externalParameters, timeout ?? CommandTimeout))
			using (var reader = command.ExecuteReader(CommandBehavior.SingleResult, true)) {
				var model = new DataViewModel { Reader = reader, Query = query };
				ReportContainer.Razor.RunCompile("AdHoc._Data", html, typeof(DataViewModel), model);
				return model.RowCount;
			}
		}

		public class DataViewModel {
			public SqlDataReader Reader { get; set; }
			public Query Query { get; set; }
			public int RowCount { get; set; }
		}
	}
}