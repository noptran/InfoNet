using System;
using System.Text.RegularExpressions;
using Infonet.Core.Collections;

namespace Infonet.Reporting.AdHoc {
	public class DateField : Field {
		private static readonly Condition[] _NoConditions = Array.Empty<Condition>();
		private static readonly Regex _DateInLabel = new Regex(@"(?<!\S)Date(?!\S)");

		public DateField(Fragment expressionSql, Fragment selectSql = null) : base(expressionSql, FieldType.Date, selectSql) { }

		public DateField(string localId, Fragment expressionSql, Fragment selectSql = null) : base(localId, expressionSql, FieldType.Date, selectSql) { }

		public void AddTo(Entity entity, bool includeDerivedFields) {
			entity.Add(this);
			if (includeDerivedFields) {
				entity.Add(ToYear());
				entity.Add(ToQuarter());
				entity.Add(ToMonth());
			}
		}

		public Field ToYear() {
			var result = new Field(LocalId + "Year", "YEAR(" + ExpressionSql + ")", FieldType.Id) { NotEmpty = NotEmpty, AvailableConditions = _NoConditions, Label = DeriveLabel("Year") };
			result.RequiredEntityIds.AddRange(RequiredEntityIds);
			return result;
		}

		public Field ToQuarter() {
			var result = new Field(LocalId + "Quarter", "(MONTH(" + ExpressionSql + ") - 1) / 3 + 1", FieldType.Id) { NotEmpty = NotEmpty, AvailableConditions = _NoConditions, Label = DeriveLabel("Quarter") };
			result.RequiredEntityIds.AddRange(RequiredEntityIds);
			return result;
		}

		public Field ToMonth() {
			var result = new Field(LocalId + "Month", "MONTH(" + ExpressionSql + ")", FieldType.Id) { NotEmpty = NotEmpty, AvailableConditions = _NoConditions, Label = DeriveLabel("Month") };
			result.RequiredEntityIds.AddRange(RequiredEntityIds);
			return result;
		}

		#region private
		private string DeriveLabel(string ammendment) {
			return _DateInLabel.IsMatch(Label) ? _DateInLabel.Replace(Label, ammendment) : Label + " " + ammendment;
		}
		#endregion
	}
}