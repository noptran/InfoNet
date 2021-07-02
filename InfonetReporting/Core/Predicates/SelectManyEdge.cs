using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;

namespace Infonet.Reporting.Core.Predicates {
	public class SelectManyEdge<TEntityOrigin, TEntityDestination> : DirectedEdge<TEntityOrigin, TEntityDestination> {
		public SelectManyEdge(Vertex<TEntityDestination> destination, Expression<Func<TEntityOrigin, IEnumerable<TEntityDestination>>> selector) : base(destination) {
			Selector = selector;
		}

		public Expression<Func<TEntityOrigin, IEnumerable<TEntityDestination>>> Selector { get; }

		protected override Expression<Func<TEntityOrigin, bool>> BuildOn(Expression<Func<TEntityDestination, bool>> destinationPredicate) {
			var selector = Selector;
			return q => selector.Invoke(q).Any(destinationPredicate.Compile());
		}
	}
}