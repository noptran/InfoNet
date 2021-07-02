using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Infonet.Data.Looking {
	public class LookupGroup : ILookupSelection, ILookupIndex {
		private readonly Lookup _source;
		private readonly Provider _provider;
		private readonly LookupSelection _all;
		private readonly LookupSelection _active;
		private readonly LookupComparer _comparer;

		internal LookupGroup(Lookup source, Provider provider) {
			_source = source;
			_provider = provider;
			_all = new LookupSelection(this, _source.Where(c => c.Entries[provider] != null));
			_active = new LookupSelection(this, _all.Where(c => c.Entries[provider].IsActive));
			_comparer = new LookupComparer(_provider);
		}

		public Lookup Source {
			get { return _source; }
		}

		public Provider Provider {
			get { return _provider; }
		}

		public LookupCode this[int? codeId] {
			get { return codeId == null ? null : this[codeId.Value]; }
		}

		public LookupCode this[int codeId] {
			get {
				var code = _source[codeId];
				if (code.Entries[_provider] == null)
					throw new IndexOutOfRangeException(GetType().Name + this + " does not include " + typeof(LookupCode).Name + code);
				return code;
			}
		}

		public IEnumerator<LookupCode> GetEnumerator() {
			return _active.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public bool IsEmpty {
			get { return _active.IsEmpty; }
		}

		public IComparer Comparer {
			get { return _comparer; }
		}

		public IComparer<LookupCode> LookupComparer {
			get { return _comparer; }
		}

		public override string ToString() {
			return $"[{_source}/{_provider}]";
		}

		// ReSharper disable once UnusedMember.Global
		public ILookupSelection IncludeInactive {
			get { return _all; }
		}

		public ILookupSelection Exclude(params int[] codeIds) {
			return _active.Exclude(codeIds);
		}

		public ILookupSelection Include(params int[] codeIds) {
			return _active.Include(codeIds);
		}

		public ILookupSelection ExcludeFor(Provider provider, params int[] codeIds) {
			return Provider != provider ? this : Exclude(codeIds);
		}

		public ILookupSelection IncludeFor(Provider provider, params int[] codeIds) {
			return Provider != provider ? this : Include(codeIds);
		}
	}
}