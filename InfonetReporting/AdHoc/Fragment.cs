using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Infonet.Reporting.AdHoc {
	public class Fragment {
		private static readonly Regex _Regex = new Regex(@"{([^{}]*)}|{({[^{}]*})}");

		private Fragment(string source, string text, IEnumerable<string> ids, IEnumerable<string> parameters, IEnumerable<string> tags) {
			Source = source;
			Text = text;
			Ids = Array.AsReadOnly(ids.ToArray());
			Parameters = Array.AsReadOnly(parameters.ToArray());
			Tags = Array.AsReadOnly(tags.ToArray());
		}

		public string Source { get; }

		public string Text { get; }

		public IReadOnlyCollection<string> Ids { get; }

		public IReadOnlyCollection<string> Parameters { get; }

		public IReadOnlyCollection<string> Tags { get; }

		public override string ToString() {
			return Source;
		}

		public static Fragment New(string source) {
			var text = new StringBuilder(source);
			var ids = new List<string>();
			var parameters = new List<string>();
			var tags = new List<string>();

			int indexShift = 0;
			foreach (Match each in _Regex.Matches(source)) {
				string groupValue = each.Groups.Cast<Group>().Skip(1).SingleOrDefault(g => g.Success)?.Value;
				if (groupValue.Length == 0) {
					tags.Add(groupValue);
				} else {
					char firstChar = groupValue[0];
					if (firstChar == '#') {
						groupValue = groupValue.Substring(1);
						ids.Add(groupValue);
					} else if (firstChar == '@') {
						groupValue = groupValue.Substring(1);
						int i = parameters.IndexOf(groupValue);
						if (i < 0) {
							parameters.Add(groupValue);
							i = parameters.Count - 1;
						}
						groupValue = "{" + i + "}";
					} else if (firstChar != '{' || groupValue[groupValue.Length - 1] != '}') {
						tags.Add(groupValue);
					}
				}
				text.Remove(each.Index + indexShift, each.Length);
				text.Insert(each.Index + indexShift, groupValue);
				indexShift += groupValue.Length - each.Length;
			}
			return new Fragment(source, text.ToString(), ids.Distinct(), parameters.Distinct(), tags.Distinct());
		}

		public static implicit operator Fragment(string source) {
			return New(source);
		}
	}
}