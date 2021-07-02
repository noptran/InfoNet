using System;
using System.Collections.Generic;
using System.IO;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class SubcontextFilter<TEntity> : ReportFilter {
		private readonly List<ReportFilter> _innerFilters = new List<ReportFilter>();

		public SubcontextFilter(Func<FilterContext, Vertex<TEntity>> vertexSelector, params ReportFilter[] innerFilters) {
			Visible = false;
			VertexSelector = vertexSelector;
			if (innerFilters != null)
				_innerFilters.AddRange(innerFilters);
		}

		public Func<FilterContext, Vertex<TEntity>> VertexSelector { get; set; }

		public ReportFilter Add(ReportFilter innerFilter) {
			_innerFilters.Add(innerFilter);
			return innerFilter;
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var subcontext = new FilterContext();
			foreach (var each in _innerFilters)
				each.ApplyTo(subcontext, container);
			var expression = VertexSelector(subcontext).Build();
			if (expression != null)
				VertexSelector(context).Predicates.Add(expression);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) { }

		public override void AddVisibleTo(ISet<ReportFilter> visible) {
			base.AddVisibleTo(visible);
			foreach (var each in _innerFilters)
				each.AddVisibleTo(visible);
		}
	}
}