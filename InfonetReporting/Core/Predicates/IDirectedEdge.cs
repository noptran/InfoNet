using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infonet.Reporting.Core.Predicates {
	public interface IDirectedEdge {
		IDirectedEdge Reverse { get; }
	}

	public interface IDirectedEdge<TEntityOrigin> : IDirectedEdge {
		Expression<Func<TEntityOrigin, bool>> Build(ISet<IDirectedEdge> visited);
	}
}