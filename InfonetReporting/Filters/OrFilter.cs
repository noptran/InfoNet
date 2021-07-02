using System;
using System.Collections.Generic;
using System.IO;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using LinqKit;

namespace Infonet.Reporting.Filters {
	public class OrFilter<TEntity> : ReportFilter {
		private readonly List<ReportFilter> _operands = new List<ReportFilter>();

		public OrFilter(Func<FilterContext, Vertex<TEntity>> vertexSelector, params ReportFilter[] operands) {
			Visible = false;
			VertexSelector = vertexSelector;
			if (operands != null)
				_operands.AddRange(operands);
		}

		public Func<FilterContext, Vertex<TEntity>> VertexSelector { get; set; }

		public ReportFilter Add(ReportFilter operand) {
			_operands.Add(operand);
			return operand;
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var predicate = PredicateBuilder.New<TEntity>(false);
			foreach (var each in _operands) {
				var subcontext = new FilterContext();
				each.ApplyTo(subcontext, container);
				var expression = VertexSelector(subcontext).Build();
				if (expression != null)
					predicate.Or(expression);
			}
			if (predicate.IsStarted)
				VertexSelector(context).Predicates.Add(predicate);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) { }

		public override void AddVisibleTo(ISet<ReportFilter> visible) {
			base.AddVisibleTo(visible);
			foreach (var each in _operands)
				each.AddVisibleTo(visible);
		}
	}
}