using System;
using System.Collections.Generic;
using Infonet.Core.Collections;
using Infonet.Reporting.AdHoc.Predicates;

namespace Infonet.Reporting.AdHoc {
	public enum Condition {
		IsEmpty,
		NotEmpty,
		IsEqualTo,
		NotEqualTo,
		IsTrue,
		IsFalse,
		IsAnyOf,
		NotAnyOf,
		Between,
		NotBetween,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual,
		StartsWith,
		NotStartsWith,
		EndsWith,
		NotEndsWith,
		Contains,
		NotContains,
		IsLike,
		NotLike
	}

	public static class ConditionEnum {
		public static string Label(this Condition c) {
			return ConditionDef.For(c).Label;
		}

		public static ConditionInput Input(this Condition c) {
			return ConditionDef.For(c).Input;
		}

		public static FieldPredicate ToPredicate(this Condition c, Field field, IDictionary<string, object> values = null) {
			return ConditionDef.For(c).ToPredicate(field, values);
		}
	}

	internal class ConditionDef {
		private static readonly OrdinalEnumMap<Condition, ConditionDef> _Instances = new OrdinalEnumMap<Condition, ConditionDef>(new Dictionary<Condition, ConditionDef> {
			[Condition.IsEmpty] = new ConditionDef("is empty", ConditionInput.None, (f, v) => new FieldEmptyPredicate(f)),
			[Condition.NotEmpty] = new ConditionDef("is not empty", ConditionInput.None, (f, v) => new FieldEmptyPredicate(f) { Not = true }),
			[Condition.IsEqualTo] = new ConditionDef("is equal to", ConditionInput.Value, (f, v) => new FieldEqualsPredicate(f) { Value = v["value"] }),
			[Condition.NotEqualTo] = new ConditionDef("is not equal to", ConditionInput.Value, (f, v) => new FieldEqualsPredicate(f) { Not = true, Value = v["value"] }),
			[Condition.IsTrue] = new ConditionDef("is checked", ConditionInput.None, (f, v) => new FieldEqualsPredicate(f) { Value = true }),
			[Condition.IsFalse] = new ConditionDef("is not checked", ConditionInput.None, (f, v) => new FieldEqualsPredicate(f) { Value = false }),
			[Condition.IsAnyOf] = new ConditionDef("is any of", ConditionInput.MultiSelect, (f, v) => new FieldInPredicate(f) { In = (IEnumerable<object>)v["list"] }),
			[Condition.NotAnyOf] = new ConditionDef("is not any of", ConditionInput.MultiSelect, (f, v) => new FieldInPredicate(f) { Not = true, In = (IEnumerable<object>)v["list"] }),
			[Condition.Between] = new ConditionDef("is between", ConditionInput.Range, (f, v) => new FieldBetweenPredicate(f) { Min = v["min"], Max = v["max"] }),
			[Condition.NotBetween] = new ConditionDef("is not between", ConditionInput.Range, (f, v) => new FieldBetweenPredicate(f) { Not = true, Min = v["min"], Max = v["max"] }),
			[Condition.GreaterThan] = new ConditionDef("greater than", ConditionInput.Value, (f, v) => new FieldBetweenPredicate(f) { MinExcluded = true, Min = v["value"] }),
			[Condition.GreaterThanOrEqual] = new ConditionDef("greater than or equal to", ConditionInput.Value, (f, v) => new FieldBetweenPredicate(f) { Min = v["value"] }),
			[Condition.LessThan] = new ConditionDef("less than", ConditionInput.Value, (f, v) => new FieldBetweenPredicate(f) { MaxExcluded = true, Max = v["value"] }),
			[Condition.LessThanOrEqual] = new ConditionDef("less than or equal to", ConditionInput.Value, (f, v) => new FieldBetweenPredicate(f) { Max = v["value"] }),
			[Condition.StartsWith] = new ConditionDef("starts with", ConditionInput.Value, (f, v) => new FieldLikePredicate(f) { Suffix = "%", Like = v["value"] }),
			[Condition.NotStartsWith] = new ConditionDef("does not start with", ConditionInput.Value, (f, v) => new FieldLikePredicate(f) { Suffix = "%", Not = true, Like = v["value"] }),
			[Condition.EndsWith] = new ConditionDef("ends with", ConditionInput.Value, (f, v) => new FieldLikePredicate(f) { Prefix = "%", Like = v["value"] }),
			[Condition.NotEndsWith] = new ConditionDef("does not end with", ConditionInput.Value, (f, v) => new FieldLikePredicate(f) { Prefix = "%", Not = true, Like = v["value"] }),
			[Condition.Contains] = new ConditionDef("contains", ConditionInput.Value, (f, v) => new FieldLikePredicate(f) { Prefix = "%", Suffix = "%", Like = v["value"] }),
			[Condition.NotContains] = new ConditionDef("does not contain", ConditionInput.Value, (f, v) => new FieldLikePredicate(f) { Prefix = "%", Suffix = "%", Not = true, Like = v["value"] }),
			[Condition.IsLike] = new ConditionDef("is like", ConditionInput.Value, (f, v) => new FieldLikePredicate(f) { Raw = true, Like = v["value"] }),
			[Condition.NotLike] = new ConditionDef("is not like", ConditionInput.Value, (f, v) => new FieldLikePredicate(f) { Raw = true, Not = true, Like = v["value"] })
		});

		private readonly Func<Field, IDictionary<string, object>, FieldPredicate> _toPredicate;

		private ConditionDef(string label, ConditionInput input, Func<Field, IDictionary<string, object>, FieldPredicate> toPredicate) {
			Label = label;
			Input = input;
			_toPredicate = toPredicate;
		}

		internal string Label { get; }

		internal ConditionInput Input { get; }

		internal FieldPredicate ToPredicate(Field field, IDictionary<string, object> values) {
			Input.AssertValid(values);
			return _toPredicate(field, values);
		}

		internal static ConditionDef For(Condition c) {
			return _Instances[c];
		}
	}
}