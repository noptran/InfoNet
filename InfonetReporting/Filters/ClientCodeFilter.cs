using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using LinqKit;

namespace Infonet.Reporting.Filters {
	public class ClientCodeFilter : ReportFilter {
		public ClientCodeFilter(string pattern) {
			Label = "Client ID";
			Pattern = pattern;
		}

		public string Pattern { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var sb = new StringBuilder(Pattern);
			sb.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_").Replace("[", @"\[");

			var rawPatterns = sb.ToString().Split(',');
			var patterns = new Dictionary<string, bool>(rawPatterns.Length);
			foreach (string each in rawPatterns) {
				string trimmed = each.Trim();
				if (trimmed.Length == 0 || trimmed.All(c => c == '*'))
					continue;

				bool needsLike = trimmed.IndexOf('*') >= 0;
				if (needsLike)
					trimmed = trimmed.Replace('*', '%');
				patterns[trimmed] = needsLike;
			}
			if (patterns.Count == 0)
				return;

			var predicate = PredicateBuilder.New<Client>(false);
			foreach (var each in patterns)
				if (each.Value)
					predicate.Or(c => DbFunctions.Like(c.ClientCode, each.Key, @"\"));
				else
					predicate.Or(c => c.ClientCode == each.Key);
			context.Client.Predicates.Add(predicate);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.Write(Pattern);
		}
	}
}