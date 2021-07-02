using System;
using System.IO;
using System.Linq.Expressions;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class PredicateFilter<TEntity> : ReportFilter {
		public PredicateFilter(Func<FilterContext, Vertex<TEntity>> vertexSelector, Expression<Func<TEntity, bool>> predicate) {
			Visible = false;
			VertexSelector = vertexSelector;
			Predicate = predicate;
		}

		public Func<FilterContext, Vertex<TEntity>> VertexSelector { get; set; }

		public Expression<Func<TEntity, bool>> Predicate { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			VertexSelector(context).Predicates.Add(Predicate);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) { }
	}
}