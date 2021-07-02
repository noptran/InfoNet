using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Infonet.Core.Collections {
	public class OrdinalEnum<TEnum> : IEnumerable<TEnum> where TEnum : struct, IConvertible {
		private static readonly OrdinalEnum<TEnum> _Instance = new OrdinalEnum<TEnum>();
		private readonly TEnum[] _values;
		private readonly int[] _ordinalMap;
		private readonly int _ordinalMapOffset;

		private OrdinalEnum() {
			if (!typeof(TEnum).IsEnum)
				throw new ArgumentException("TEnum must be an enum type");

			_values = Enums.GetValues<TEnum>().ToArray();
			_ordinalMapOffset = -Convert.ToInt32(_values.Min());
			int ordinalMapSpan = Convert.ToInt32(_values.Max()) + _ordinalMapOffset + 1;

			//KMS LATER create an attribute that alters this constraint
			if (_values.Length * 2 < ordinalMapSpan)
				throw new Exception("Assertion Failed: " + GetType().Name + " count(TEnum) must be at least half of max(TEnum)");

			_ordinalMap = new int[ordinalMapSpan];
			for (int i = 0; i < _ordinalMap.Length; i++)
				_ordinalMap[i] = -1;
			for (int j = 0; j < _values.Length; j++)
				_ordinalMap[Convert.ToInt32(_values[j]) + _ordinalMapOffset] = j;
		}

		public static OrdinalEnum<TEnum> Values {
			get { return _Instance; }
		}

		public static int Length {
			get { return _Instance._values.Length; }
		}

		public static int OrdinalOf(TEnum e) {
			int result = _Instance._ordinalMap[Convert.ToInt32(e) + _Instance._ordinalMapOffset];
			if (result < 0)
				throw new ArgumentOutOfRangeException();
			return result;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public TEnum this[int ordinal] {
			get { return _values[ordinal]; }
		}

		public IEnumerator<TEnum> GetEnumerator() {
			return ((IEnumerable<TEnum>)_values).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}