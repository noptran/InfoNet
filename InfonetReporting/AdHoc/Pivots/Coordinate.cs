using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;

namespace Infonet.Reporting.AdHoc.Pivots {
	//KMS DO IEnumerable?
	public sealed class Coordinate : IComparable, IComparable<Coordinate>, IEquatable<Coordinate> {
		private readonly object[] _components;
		private readonly int _hashCode;

		public Coordinate(IEnumerable<object> components) : this(components?.ToArray()) { }

		private Coordinate(object[] components) {
			if (components == null)
				throw new ArgumentNullException(nameof(components));

			_components = components;
			_hashCode = HashCode.Compute(_components) + HashCode.PRIME + GetType().GetHashCode();
		}

		public int Rank {
			get { return _components.Length; }
		}

		public object this[int i] {
			get { return _components[i]; }
		}

		public override string ToString() {
			return $"{nameof(Coordinate)}[{string.Join(", ", _components)}]";
		}

		#region equality and comparison
		public override bool Equals(object other) {
			var otherCoordinate = other as Coordinate;
			return otherCoordinate != null && Equals(otherCoordinate);
		}

		public bool Equals(Coordinate other) {
			int rank = Rank;
			return other != null && rank == other.Rank && ComponentsEqual(other, rank);
		}

		public bool EqualsThru(Coordinate other, int thruRank) {
			int comparisonRank = Math.Min(thruRank, Rank);
			return other != null && comparisonRank == Math.Min(thruRank, other.Rank) && ComponentsEqual(other, comparisonRank);
		}

		private bool ComponentsEqual(Coordinate other, int thruRank) {
			for (int i = 0; i < thruRank; i++)
				if (!_components[i].SafeEquals(other._components[i]))
					return false;
			return true;
		}

		public override int GetHashCode() {
			return _hashCode;
		}

		public static bool operator ==(Coordinate a, Coordinate b) {
			return ReferenceEquals(a, b) || !ReferenceEquals(a, null) && a.Equals(b);
		}

		public static bool operator !=(Coordinate a, Coordinate b) {
			return !(a == b);
		}

		public int CompareTo(object other) {
			return CompareTo((Coordinate)other);
		}

		public int CompareTo(Coordinate other) {
			return CoordinateComparer.Default.Compare(this, other);
		}
		#endregion
	}
}