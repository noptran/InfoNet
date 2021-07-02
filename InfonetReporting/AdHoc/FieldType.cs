using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Infonet.Core;
using Infonet.Core.Collections;
using Infonet.Data.Looking;

namespace Infonet.Reporting.AdHoc {
	#region suppressed
	[SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
	[SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
	[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
	[SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
	#endregion

	public class FieldType {
		#region comparers
		private static readonly IComparer _DefaultComparer = Comparers.NullsLast(System.Collections.Comparer.Default);
		private static readonly IComparer _CurrentCultureIgnoreCaseComparer = Comparers.NullsLast<string>(StringComparer.CurrentCultureIgnoreCase);
		private static readonly IComparer _ReverseComparer = Comparers.Reverse(System.Collections.Comparer.Default);
		#endregion

		public static readonly FieldType Checkbox = new FieldType(
			name: nameof(Checkbox),
			clrType: TypeCode.Boolean,
			dbType: SqlDbType.Bit,
			align: Alignment.Left,
			formatter: value => value == null ? null : (System.Convert.ToBoolean(value) ? "Yes" : "No"),
			conditions: new[] { Condition.IsTrue, Condition.IsFalse });

		public static readonly FieldType Date = new FieldType(
			name: nameof(Date),
			clrType: TypeCode.DateTime,
			dbType: SqlDbType.DateTime,
			align: Alignment.Center,
			format: "{0:MM/dd/yyyy}",
			conditions: new[] { Condition.Between, Condition.NotBetween, Condition.GreaterThan, Condition.GreaterThanOrEqual, Condition.LessThan, Condition.LessThanOrEqual, Condition.IsEqualTo, Condition.NotEqualTo });

		//KMS DO conditions fail because DateTime compares date as well as time
		public static readonly FieldType Time = new FieldType(
			name: nameof(Time),
			clrType: TypeCode.DateTime,
			dbType: SqlDbType.DateTime,
			align: Alignment.Center,
			format: "{0:hh:mm tt}");

		public static readonly FieldType Dollars = new FieldType(
			name: nameof(Dollars),
			clrType: TypeCode.Decimal,
			dbType: SqlDbType.Money,
			numeric: true,
			step: "0.01",
			align: Alignment.Right,
			format: "{0:c}",
			conditions: new[] { Condition.Between, Condition.NotBetween, Condition.GreaterThan, Condition.GreaterThanOrEqual, Condition.LessThan, Condition.LessThanOrEqual, Condition.IsEqualTo, Condition.NotEqualTo });

		public static readonly FieldType Hours = new FieldType(
			name: nameof(Hours),
			clrType: TypeCode.Single,
			dbType: SqlDbType.Float,
			numeric: true,
			step: "0.25",
			align: Alignment.Right,
			format: "{0:#,0.##}",
			conditions: new[] { Condition.Between, Condition.NotBetween, Condition.GreaterThan, Condition.GreaterThanOrEqual, Condition.LessThan, Condition.LessThanOrEqual, Condition.IsEqualTo, Condition.NotEqualTo });

		public static readonly FieldType Minutes = new FieldType(
			name: nameof(Minutes),
			clrType: TypeCode.Single,
			dbType: SqlDbType.Float,
			numeric: true,
			step: "0.25",
			align: Alignment.Right,
			format: "{0:#,0.##}",
			conditions: new[] { Condition.Between, Condition.NotBetween, Condition.GreaterThan, Condition.GreaterThanOrEqual, Condition.LessThan, Condition.LessThanOrEqual, Condition.IsEqualTo, Condition.NotEqualTo });

		public static readonly FieldType Id = new FieldType(
			name: nameof(Id),
			clrType: TypeCode.Int32,
			dbType: SqlDbType.Int,
			label: "Identifier");

		public static readonly FieldType NVarCharId = new FieldType(
			name: nameof(NVarCharId),
			clrType: TypeCode.String,
			dbType: SqlDbType.NVarChar,
			label: "Identifier");

		public static readonly FieldType Lookup = new FieldType(
			name: nameof(Lookup),
			clrType: TypeCode.Int32,
			dbType: SqlDbType.Int,
			formatter: value => (value as LookupCode)?.Description);

		public static readonly FieldType NVarChar = new FieldType(
			name: nameof(NVarChar),
			clrType: TypeCode.String,
			dbType: SqlDbType.NVarChar,
			label: "Text",
			conditions: new[] { Condition.StartsWith, Condition.NotStartsWith, Condition.EndsWith, Condition.NotEndsWith, Condition.Contains, Condition.NotContains, Condition.IsEqualTo, Condition.NotEqualTo, Condition.IsLike, Condition.NotLike });

		public static readonly FieldType Percent = new FieldType(
			name: nameof(Percent),
			clrType: TypeCode.Int32,
			dbType: SqlDbType.SmallInt,
			numeric: true,
			step: "1",
			align: Alignment.Right,
			format: "{0:0}%",
			conditions: new[] { Condition.Between, Condition.NotBetween, Condition.GreaterThan, Condition.GreaterThanOrEqual, Condition.LessThan, Condition.LessThanOrEqual, Condition.IsEqualTo, Condition.NotEqualTo });

		public static readonly FieldType Quantity = new FieldType(
			name: nameof(Quantity),
			clrType: TypeCode.Int32,
			dbType: SqlDbType.Int,
			numeric: true,
			step: "1",
			label: "Number",
			align: Alignment.Right,
			format: "{0:#,0}",
			conditions: new[] { Condition.Between, Condition.NotBetween, Condition.GreaterThan, Condition.GreaterThanOrEqual, Condition.LessThan, Condition.LessThanOrEqual, Condition.IsEqualTo, Condition.NotEqualTo });

		public static readonly FieldType BigQuantity = new FieldType(
			name: nameof(BigQuantity),
			clrType: TypeCode.Int64,
			dbType: SqlDbType.BigInt,
			numeric: true,
			step: "1",
			label: "Number",
			align: Alignment.Right,
			format: "{0:#,0}",
			conditions: new[] { Condition.Between, Condition.NotBetween, Condition.GreaterThan, Condition.GreaterThanOrEqual, Condition.LessThan, Condition.LessThanOrEqual, Condition.IsEqualTo, Condition.NotEqualTo });

		public static readonly FieldType Sequential = new FieldType(
			name: nameof(Sequential),
			clrType: TypeCode.Int32,
			dbType: SqlDbType.Int,
			numeric: true,
			step: "1",
			label: "Number",
			conditions: new[] { Condition.Between, Condition.NotBetween, Condition.GreaterThan, Condition.GreaterThanOrEqual, Condition.LessThan, Condition.LessThanOrEqual, Condition.IsEqualTo, Condition.NotEqualTo });

		#region implementation
		private FieldType(string name, TypeCode clrType, SqlDbType dbType, bool numeric = false, string step = null, string label = null, Alignment align = Alignment.Left, string format = null, Func<object, string> formatter = null, Condition[] conditions = null) {
			if (format != null && formatter != null)
				throw new ArgumentException($"FieldType may not have both {nameof(format)} and {nameof(formatter)}");

			Name = name;
			ClrType = clrType;
			DbType = dbType;
			Numeric = numeric;
			Step = step;
			Label = label ?? name;
			Align = align;
			Formatter = format == null ? formatter : value => value == null ? null : string.Format(format, value);
			Comparer = GetComparer(clrType);
			Conditions = Array.AsReadOnly(conditions ?? Array.Empty<Condition>());
		}

		public string Name { get; }

		public TypeCode ClrType { get; }

		public SqlDbType DbType { get; }

		public bool Numeric { get; }

		public string Step { get; }

		public string Label { get; }

		public Alignment Align { get; }

		public Func<object, string> Formatter { get; }

		public IComparer Comparer { get; }

		public IReadOnlyCollection<Condition> Conditions { get; }

		public object Convert(object value) {
			return value == null ? null : System.Convert.ChangeType(value, ClrType);
		}

		public string Format(object value) {
			return Formatter != null ? Formatter(value) : ConvertNull.ToString(value);
		}

		public override string ToString() {
			return Name;
		}

		public static IComparer GetComparer(TypeCode tc) {
			switch (tc) {
				case TypeCode.String:
					return _CurrentCultureIgnoreCaseComparer;
				case TypeCode.Boolean:
					return _ReverseComparer;
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.DateTime:
					return _DefaultComparer;
				default:
					throw new NotSupportedException("No Comparer defined for TypeCode." + tc);
			}
		}
		#endregion

		#region inner
		public enum Alignment {
			Left,
			Center,
			Right
		}
		#endregion
	}
}