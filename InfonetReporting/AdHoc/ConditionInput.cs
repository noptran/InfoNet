using System;
using System.Collections.Generic;
using Infonet.Core.Collections;

namespace Infonet.Reporting.AdHoc {
	public enum ConditionInput {
		None,
		Value,
		Range,
		Select,
		MultiSelect,
		Custom
	}

	public static class ConditionInputEnum {
		public static IEnumerable<string> Parameters(this ConditionInput ci) {
			return ConditionInputDef.For(ci).Parameters;
		}

		public static void AssertValid(this ConditionInput ci, IDictionary<string, object> arguments) {
			ConditionInputDef.For(ci).AssertValid(arguments);
		}
	}

	internal class ConditionInputDef {
		private static readonly OrdinalEnumMap<ConditionInput, ConditionInputDef> _Instances = new OrdinalEnumMap<ConditionInput, ConditionInputDef>(new Dictionary<ConditionInput, ConditionInputDef> {
			[ConditionInput.None] = new ConditionInputDef(),
			[ConditionInput.Value] = new ConditionInputDef("value"),
			[ConditionInput.Range] = new ConditionInputDef("min", "max"),
			[ConditionInput.Select] = new ConditionInputDef("value"),
			[ConditionInput.MultiSelect] = new ConditionInputDef("list"),
			[ConditionInput.Custom] = new ConditionInputDef(null)
		});

		private ConditionInputDef(params string[] parameters) {
			Parameters = parameters == null ? null : Array.AsReadOnly(parameters);
		}

		internal IEnumerable<string> Parameters { get; }

		internal void AssertValid(IDictionary<string, object> arguments) {
			if (Parameters == null)
				return;

			int count = 0;
			foreach (string each in Parameters) {
				if (arguments == null || !arguments.ContainsKey(each))
					throw new ArgumentException($"ConditionInput argument missing: \'{each}\'");
				count++;
			}
			if (arguments != null && arguments.Count > count)
				throw new ArgumentException($"ConditionInput includes {arguments.Count - count} unexpected arguments");
		}

		internal static ConditionInputDef For(ConditionInput ci) {
			return _Instances[ci];
		}
	}
}