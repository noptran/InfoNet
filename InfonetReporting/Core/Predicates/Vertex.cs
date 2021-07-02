using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using LinqKit;

namespace Infonet.Reporting.Core.Predicates {
	public class Vertex<TEntity> {
		public Vertex() {
			Predicates = new List<Expression<Func<TEntity, bool>>>();
			Edges = new List<IDirectedEdge<TEntity>>();
		}

		public ICollection<Expression<Func<TEntity, bool>>> Predicates { get; }

		public ICollection<IDirectedEdge<TEntity>> Edges { get; }

		[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
		public SelectOneEdge<TEntity, TEntityDestination> AddEdge<TEntityDestination>(Vertex<TEntityDestination> destination, Expression<Func<TEntity, TEntityDestination>> selector, Expression<Func<TEntityDestination, TEntity>> reverseSelector = null) {
			var result = new SelectOneEdge<TEntity, TEntityDestination>(destination, selector);
			Edges.Add(result);
			if (reverseSelector != null) {
				var reverse = new SelectOneEdge<TEntityDestination, TEntity>(this, reverseSelector) { Reverse = result };
				result.Reverse = reverse;
				destination.Edges.Add(reverse);
			}
			return result;
		}

		[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
		public SelectOneEdge<TEntity, TEntityDestination> AddEdge<TEntityDestination>(Vertex<TEntityDestination> destination, Expression<Func<TEntity, TEntityDestination>> selector, Expression<Func<TEntityDestination, IEnumerable<TEntity>>> reverseSelector) {
			var result = new SelectOneEdge<TEntity, TEntityDestination>(destination, selector);
			Edges.Add(result);
			if (reverseSelector != null) {
				var reverse = new SelectManyEdge<TEntityDestination, TEntity>(this, reverseSelector) { Reverse = result };
				result.Reverse = reverse;
				destination.Edges.Add(reverse);
			}
			return result;
		}

		[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
		public SelectManyEdge<TEntity, TEntityDestination> AddEdge<TEntityDestination>(Vertex<TEntityDestination> destination, Expression<Func<TEntity, IEnumerable<TEntityDestination>>> selector, Expression<Func<TEntityDestination, TEntity>> reverseSelector = null) {
			var result = new SelectManyEdge<TEntity, TEntityDestination>(destination, selector);
			Edges.Add(result);
			if (reverseSelector != null) {
				var reverse = new SelectOneEdge<TEntityDestination, TEntity>(this, reverseSelector) { Reverse = result };
				result.Reverse = reverse;
				destination.Edges.Add(reverse);
			}
			return result;
		}

		/** returns null if no predicates are found **/
		public Expression<Func<TEntity, bool>> Build() {
			return Build(null, new HashSet<IDirectedEdge>());
		}

		internal Expression<Func<TEntity, bool>> Build(IDirectedEdge caller, ISet<IDirectedEdge> visited) {
			var result = BuildVertex();

			foreach (var each in Edges) {
				if (each == caller?.Reverse)
					continue;

				var edgePredicate = each.Build(visited);
				if (edgePredicate != null)
					result = result.And(edgePredicate);
			}

			return result.IsStarted ? result : null;
		}

		public ExpressionStarter<TEntity> BuildVertex() {
			var result = PredicateBuilder.New<TEntity>(true);

			foreach (var each in Predicates)
				result = result.And(each);
			return result;
		}
	}
}