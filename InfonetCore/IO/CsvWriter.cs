using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Infonet.Core.IO {
	public class CsvWriter : IDisposable {
		#region constants
		public const bool DEFAULT_DEEP_DISPOSE = true;
		private const char COMMA = ',';
		private const char QUOTE = '"';

		// ReSharper disable once InconsistentNaming
		private static readonly char[] QUOTED_CHARS = { COMMA, QUOTE, '\n', '\r' };
		#endregion

		private TextWriter _inner;
		private readonly int _columns;
		private readonly bool _deepDispose;
		private long _currentRow = 0;
		private int _currentColumn = 0;

		public CsvWriter(TextWriter inner, int columns, bool deepDispose = DEFAULT_DEEP_DISPOSE) {
			_inner = inner;
			_columns = columns;
			_deepDispose = deepDispose;
		}

		public CsvWriter(int columns) : this(new StringWriter(), columns) { }

		public int Columns {
			get { return _columns; }
		}

		public static CsvWriter WriteHeaders(TextWriter inner, IEnumerable<string> headers, bool deepDispose = DEFAULT_DEEP_DISPOSE) {
			var headerArray = headers as string[] ?? headers.ToArray();
			var csv = new CsvWriter(inner, headerArray.Length, deepDispose);
			foreach (string each in headerArray)
				csv.WriteField(each);
			csv.WriteEol();
			csv._currentRow = 0;
			return csv;
		}

		public void WriteField(string value) {
			if (_currentColumn >= _columns)
				throw new InvalidOperationException($"{_ToString()}: failed writing field: current row is full");

			if (_currentColumn > 0)
				_inner.Write(COMMA);
			if (value == null || value.IndexOfAny(QUOTED_CHARS) < 0) {
				_inner.Write(value);
			} else {
				_inner.Write(QUOTE);
				foreach (char c in value) {
					_inner.Write(c);
					if (c == QUOTE)
						_inner.Write(QUOTE);
				}
				_inner.Write(QUOTE);
			}
			_currentColumn++;
		}

		public void WriteField(object value) {
			var formattable = value as IFormattable;
			WriteField(formattable?.ToString(null, _inner.FormatProvider) ?? value?.ToString());
		}

		public void WriteField(IFormattable value, string format = null) {
			WriteField(value?.ToString(format, _inner.FormatProvider));
		}

		#region write overloads
		public void WriteField(char value) {
			throw new NotImplementedException("Support for char not yet implemented"); //KMS DO support this and char[]?
		}

		public void WriteField(bool value) {
			WriteField(value.ToString(_inner.FormatProvider));
		}

		public void WriteField(int value) {
			WriteField(value.ToString(_inner.FormatProvider));
		}

		public void WriteField(uint value) {
			WriteField(value.ToString(_inner.FormatProvider));
		}

		public void WriteField(long value) {
			WriteField(value.ToString(_inner.FormatProvider));
		}

		public void WriteField(ulong value) {
			WriteField(value.ToString(_inner.FormatProvider));
		}

		public void WriteField(float value) {
			WriteField(value.ToString(_inner.FormatProvider));
		}

		public void WriteField(double value) {
			WriteField(value.ToString(_inner.FormatProvider));
		}

		public void WriteField(decimal value) {
			WriteField(value.ToString(_inner.FormatProvider));
		}
		#endregion

		public void WriteEmptyFields(int count) {
			for (int i = 0; i < count; i++)
				WriteField(null);
		}

		// ReSharper disable once UnusedParameter.Global
		public void AssertEol(int emptyFieldsRemaining = 0) {
			if (_currentColumn + emptyFieldsRemaining < _columns)
				throw new InvalidOperationException($"{_ToString()}: assertion failed: current row is not full");
		}

		public void WriteEol(int emptyFieldsToWrite = 0) {
			AssertEol(emptyFieldsToWrite);
			WriteEmptyFields(emptyFieldsToWrite);
			_inner.WriteLine();
			_currentRow++;
			_currentColumn = 0;
		}

		public void WriteFields(object[] values) {
			foreach (var each in values)
				WriteField(each);
		}

		public void WriteLine(object[] values, int emptyFieldsToWrite = 0) {
			WriteFields(values);
			WriteEol(emptyFieldsToWrite);
		}

		private string _ToString() {
			return $"CSV[row={_currentRow},column={(_currentColumn < _columns ? _currentColumn.ToString() : "EOL")}]";
		}

		public override string ToString() {
			return _inner is StringWriter ? _inner.ToString() : _ToString();
		}

		public string ToString(bool assertEol, int emptyFieldsToWrite = 0) {
			WriteEmptyFields(emptyFieldsToWrite);
			if (assertEol)
				AssertEol();
			return ToString();
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing && _inner != null) {
				if (_deepDispose)
					_inner.Dispose();
				_inner = null;
			}
		}

		public void Dispose() {
			Dispose(true);
		}
	}
}