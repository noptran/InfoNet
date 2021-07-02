using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;

namespace Infonet.Reporting.AdHoc.Pivots {
	public class CoordinateComparer : IComparer, IComparer<Coordinate> {
		public static readonly CoordinateComparer Default = new CoordinateComparer();
		public static readonly IComparer DefaultComponent = Comparers.NullsLast(Comparer.Default);

		private readonly IComparer[] _components;

		public CoordinateComparer(params IComparer[] components) : this((IEnumerable<IComparer>)components) { }

		public CoordinateComparer(IEnumerable<IComparer> components) {
			_components = components?.ToArray() ?? Array.Empty<IComparer>();
		}

		public int Compare(object a, object b) {
			return Compare((Coordinate)a, (Coordinate)b);
		}

		public int Compare(Coordinate a, Coordinate b) {
			int rank = Math.Min(a.Rank, b.Rank);
			for (int i = 0; i < rank; i++) {
				var component = i < _components.Length ? _components[i] : null;
				int result = (component ?? DefaultComponent).Compare(a[i], b[i]);
				if (result != 0)
					return result;
			}
			return a.Rank.CompareTo(b.Rank);
		}
	}
}