using System.Data;
using System.IO;
using System.Linq;
using Infonet.Core.IO;

namespace Infonet.Core.Data {
	public static class DataTableExtensions {
		public static void WriteCsv(this DataTable sourceTable, TextWriter writer) {
			using (var csv = CsvWriter.WriteHeaders(writer, sourceTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray(), false))
				foreach (DataRow row in sourceTable.Rows)
					csv.WriteLine(row.ItemArray);
		}
	}
}