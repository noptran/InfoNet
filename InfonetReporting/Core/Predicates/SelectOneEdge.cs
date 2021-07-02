using System;
using System.Linq.Expressions;
using LinqKit;

namespace Infonet.Reporting.Core.Predicates {
	public class SelectOneEdge<TEntityOrigin, TEntityDestination> : DirectedEdge<TEntityOrigin, TEntityDestination> {
		public SelectOneEdge(Vertex<TEntityDestination> destination, Expression<Func<TEntityOrigin, TEntityDestination>> selector) : base(destination) {
			Selector = selector;
		}

		public Expression<Func<TEntityOrigin, TEntityDestination>> Selector { get; }

		protected override Expression<Func<TEntityOrigin, bool>> BuildOn(Expression<Func<TEntityDestination, bool>> destinationPredicate) {
			var selector = Selector;
			return q => destinationPredicate.Invoke(selector.Invoke(q));
		}
	}
}