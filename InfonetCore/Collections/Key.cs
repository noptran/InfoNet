using System;
using System.Collections.Generic;
using System.Text;

namespace Infonet.Core.Collections {
	//KMS DO first pass, contructor takes nullable<int>s and components returns nullable<int>s
	//KMS DO second pass, store components nullable<int> array so parsing is not needed
	//KMS DO third pass, make Components return something that is Equatable, Comparable, and so on SO it can 
	//KMS DO fourth pass, replace direct references to _components with Components
	//KMS DO fifth pass, drop _components if makes sense
	public sealed class Key : IComparable, IComparable<Key>, IEquatable<Key> {
		#region constants
		// ReSharper disable once InconsistentNaming
		private static readonly string[] NO_COMPONENTS = new string[0];

		private const int NEW_OCCURRENCE = -1;
		private const char SEPARATOR = ':';
		#endregion

		// ReSharper disable once InconsistentNaming
		internal readonly string _components;

		private readonly int _occurrence;

		// ReSharper disable once MemberCanBePrivate.Global
		public Key(IEnumerable<string> components, int occurrence) : this(Compose(components), occurrence) { }

		internal Key(string components, int occurrence) {
			_components = components;
			_occurrence = occurrence;
		}

		//KMS DO should probably store this also?
		public string[] Components {
			get {
				if (_components == null)
					return NO_COMPONENTS;
				var split = _components.Split(SEPARATOR);
				for (int i = 0; i < split.Length; i++)
					if (split[i].Length == 0)
						split[i] = null;
				return split;
			}
		}

		public int Occurrence {
			get { return _occurrence; }
		}

		public bool IsTemplate {
			get { return _occurrence == NEW_OCCURRENCE; }
		}

		public override string ToString() {
			if (IsTemplate)
				return _components ?? "";

			if (_components == null)
				return _occurrence.ToString();

			return _components + SEPARATOR + _occurrence;
		}

		#region equality and comparison
		public override bool Equals(object other) {
			if (!(other is Key))
				return false;

			return Equals((Key)other);
		}

		public bool Equals(Key other) {
			if (other == null)
				return false;

			return other._components == _components && other._occurrence == _occurrence;
		}

		public override int GetHashCode() {
			return HashCode.Compute(_components, _occurrence);
		}

		public static bool operator ==(Key a, Key b) {
			if (ReferenceEquals(a, b))
				return true;

			return !ReferenceEquals(a, null) && a.Equals(b);
		}

		public static bool operator !=(Key a, Key b) {
			return !(a == b);
		}

		public int CompareTo(object other) {
			return CompareTo((Key)other);
		}

		//KMS DO is this parsing worth the cost?  i don't think so...
		//KMS DO this could put the nulls last for us?
		public int CompareTo(Key other) {
			string[] components = Components;
			string[] otherComponents = other.Components;
			int result;
			for (int i = 0; i < components.Length && i < otherComponents.Length; i++) {
				result = components[i].SafeCompareTo(otherComponents[i]);
				if (result != 0)
					return result;
			}
			result = components.Length.CompareTo(otherComponents.Length);
			if (result != 0)
				return result;

			return Occurrence.CompareTo(other.Occurrence);
		}
		#endregion

		#region static utils
		internal static string Compose(IEnumerable<string> components) {
			int componentsCount = 0;
			var sb = new StringBuilder();
			foreach (string eachKey in components) {
				if (componentsCount++ > 0)
					sb.Append(SEPARATOR);

				if (eachKey == null)
					continue;

				if (eachKey.Length == 0)
					/* should probably allow this but adds complexity for now */
					throw new ArgumentException("components may not contain empty strings");

				foreach (char eachChar in eachKey)
					if (eachChar == SEPARATOR)
						/* should probably allow this but adds complexity for now */
						throw new ArgumentException("components may not contain colons");
					else
						sb.Append(eachChar);
			}
			return componentsCount == 0 ? null : sb.ToString();
		}

		public static Key Parse(string s) {
			int index = s.LastIndexOf(SEPARATOR);
			if (index == -1)
				return new Key((string)null, int.Parse(s));

			return new Key(s.Substring(0, index), int.Parse(s.Substring(index + 1)));
		}

		public static Key Template(params string[] any) {
			if (any == null)
				any = new string[] { null };
			return new Key(any, NEW_OCCURRENCE);
		}
		#endregion
	}
}