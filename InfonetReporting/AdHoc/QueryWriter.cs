using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Infonet.Reporting.AdHoc {
	//KMS DO should this extend TextWriter?
	public class QueryWriter : IDisposable {
		private TextWriter _writer;
		private int _tabDepth = 0;
		private bool _lineEmpty = true;

		#region constructing/disposing
		public QueryWriter(IDictionary<string, object> properties = null) : this(new StringWriter(), properties) { }

		public QueryWriter(TextWriter sql, IDictionary<string, object> properties = null) {
			if (properties == null)
				properties = new Dictionary<string, object>(0);

			_writer = sql;
			Properties = new ReadOnlyDictionary<string, object>(properties);
			//KMS DO hide this implementation and create via KeyedCollection extension
			Parameters = new Infonet.Core.Collections.KeyedCollection<string, SqlParameter>(parameter => parameter.ParameterName);
		}

		private void Dispose(bool disposing) {
			if (disposing)
				if (_writer != null) {
					_writer.Dispose();
					_writer = null;
				}
		}

		public void Dispose() {
			Dispose(true);
		}
		#endregion

		public IReadOnlyDictionary<string, object> Properties { get; }

		public KeyedCollection<string, SqlParameter> Parameters { get; }

		public override string ToString() {
			return _writer is StringWriter ? _writer.ToString() : base.ToString();
		}

		public void Write(string s) {
			if (string.IsNullOrEmpty(s))
				return;

			if (_lineEmpty) {
				for (int i = 0; i < _tabDepth; i++)
					_writer.Write('\t');
				_lineEmpty = false;
			}
			_writer.Write(s);
		}

		public void WriteLine(string s = null) {
			Write(s);
			_writer.WriteLine();
			_lineEmpty = true;
		}

		public void Write(string template, params object[] args) {
			Write(string.Format(template, args));
		}

		public void IndentMore() {
			_tabDepth++;
		}

		public void IndentLess() {
			if (_tabDepth > 0)
				_tabDepth--;
		}

		/* Returns unique SqlParameter.ParameterName. */
		//KMS DO should type really allow null?
		//KMS DO should these parameters be re-ordered now that id is required
		public string AddParameter(object value, SqlDbType? type, string id) {
			if (id == null)
				throw new ArgumentNullException(nameof(id));

			var sb = new StringBuilder();
			sb.Append("@p");
			sb.Append(Parameters.Count);
			sb.Append(Model.ID_SEPARATOR);
			sb.Append(id);
			var sqlParameter = type == null ? new SqlParameter(sb.ToString(), value) : new SqlParameter(sb.ToString(), type) { Value = value };
			Parameters.Add(sqlParameter);
			return sb.ToString();
		}

		public void WriteParameter(object value, SqlDbType type, string id) {
			Write(AddParameter(value, type, id));
		}

		//KMS DO use stringbuilder?  pass to AddParameter and use its StringBuilder?
		public void WriteParameter(object value, Field field, string name, bool neverInline = false) {
			if (!neverInline && (value is int || value is bool))
				Write(ToSql(value));
			else
				WriteParameter(value, field.Type.DbType, field.Id + Model.ID_SEPARATOR + name);
		}

		internal static string ToSql(object literal) {
			if (literal == null)
				return "NULL";
			if (literal is bool)
				return (bool)literal ? "1" : "0";
			if (literal is DateTime || literal is string)
				return "'" + literal + "'";
			return literal.ToString();
		}
	}
}