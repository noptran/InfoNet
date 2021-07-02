using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;

namespace Infonet.Data.Looking {
	/*
	 * LookupCodes are indexed in arrays by their CodeIds.  This assumes
	 * negligible gaps exist between CodeIds.  Arrays are allocated of
	 * size sufficient to place minimum CodeId at [0] and maximum CodeId 
	 * at [Length -1].  As such gaps between CodeIds will waste memory at
	 * a rate of 1 integer per missing CodeId. NOTE: LookupCodes not
	 * starting at CodeId 0 have no impact as the CodeIds are offset such 
	 * that the minimum is always placed at [0].
	 */
	public class Lookup : ILookupIndex {
		public const double DEFAULT_MIN_LOAD_FACTOR = 0.25;

		private readonly LookupCode[] _codes;
		private readonly int _offset;
		private readonly ProviderMap<LookupGroup> _groups;
		private volatile IEnumerable<LookupCode> _sorted = null;

		internal Lookup(LookupBuilder lb, double minLoadFactor = DEFAULT_MIN_LOAD_FACTOR) {
			TableId = lb.TableId;
			TableName = lb.TableName;
			DisplayName = lb.DisplayName;
			Description = lb.Description;
			int codeCount = lb.Codes.Count;
			if (codeCount == 0) {
				_offset = 0;
				_codes = new LookupCode[0];
				return;
			}

			int minCodeId = lb.Codes.Min(c => c.CodeId);
			int maxCodeId = lb.Codes.Max(c => c.CodeId);
			int arraySize = maxCodeId - minCodeId + 1;
			if (codeCount < arraySize * minLoadFactor)
				throw new Exception(string.Format("Assertion failed: {0} for {1} underloaded: {2} codes in {3} slots with {4} {5}", GetType().Name, TableName, codeCount, arraySize, nameof(minLoadFactor), minLoadFactor));

			_codes = new LookupCode[arraySize];
			_offset = -minCodeId;
			foreach (var each in lb.Codes) {
				if (_codes[each.CodeId + _offset] != null)
					throw new Exception(GetType().Name + this + " cannot have multiple " + typeof(LookupCode).Name + "s with duplicate CodeIds");

				_codes[each.CodeId + _offset] = each.ToLookupCode(this);
			}

			var groups = new Dictionary<Provider, LookupGroup>();
			foreach (var eachProvider in OrdinalEnum<Provider>.Values)
				groups[eachProvider] = new LookupGroup(this, eachProvider);
			_groups = new ProviderMap<LookupGroup>(groups);
		}

		public int TableId { get; }

		public string TableName { get; }

		// ReSharper disable once MemberCanBePrivate.Global
		public string DisplayName { get; }

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public string Description { get; }

		public override string ToString() {
			return $"[{DisplayName}]";
		}

		public LookupGroup this[Provider provider] {
			get { return _groups[provider]; }
		}

		public LookupCode this[int? codeId] {
			get { return codeId == null ? null : this[codeId.Value]; }
		}

		public LookupCode this[int codeId] {
			get {
				var code = _codes[codeId + _offset];
				if (code == null)
					throw new IndexOutOfRangeException($"{GetType().Name}{this} does not include {nameof(codeId)} {codeId}");
				return code;
			}
		}

		public bool Contains(int codeId) {
			return this[codeId] != null;
		}

		public IEnumerator<LookupCode> GetEnumerator() {
			if (_sorted == null)
				_sorted = _codes.Where(c => c != null).OrderBy(c => c.Description).ToArray();
			return _sorted.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public bool IsEmpty {
			get { return _codes.Length == 0; }
		}
	}
}