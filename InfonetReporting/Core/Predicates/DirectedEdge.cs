using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infonet.Reporting.Core.Predicates {
	public abstract class DirectedEdge<TEntityOrigin, TEntityDestination> : IDirectedEdge<TEntityOrigin> {
		protected DirectedEdge(Vertex<TEntityDestination> destination) {
			Destination = destination;
		}

		public IDirectedEdge Reverse { get; set; }

		public Vertex<TEntityDestination> Destination { get; }

		public Expression<Func<TEntityOrigin, bool>> Build(ISet<IDirectedEdge> visited) {
			if (visited.Contains(this))
				throw new NotSupportedException("Directed Cycle detected: this edge already visited");

			visited.Add(this);

			var destinationPredicate = Destination.Build(this, visited);
			return destinationPredicate == null ? null : BuildOn(destinationPredicate);
		}

		protected abstract Expression<Func<TEntityOrigin, bool>> BuildOn(Expression<Func<TEntityDestination, bool>> destinationPredicate);
	}
}