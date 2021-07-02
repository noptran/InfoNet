using System;
using System.Collections.Generic;
using Infonet.Core.Collections;

namespace Infonet.Data.Looking {
	public class LookupCode : IEquatable<LookupCode> {
		internal LookupCode(Lookup owner, LookupBuilder.Code b) {
			Owner = owner;
			CodeId = b.CodeId;
			Description = b.Description;
			var entries = new Dictionary<Provider, Entry>();
			foreach (var each in b.Entries) {
				if (entries.ContainsKey(each.Provider))
					throw new Exception(GetType().Name + this + " cannot have multiple " + nameof(Entries) + " per " + typeof(Provider).Name);
				entries[each.Provider] = each.ToEntry(this);
			}
			Entries = new ProviderMap<Entry>(entries);
		}

		private Lookup Owner { get; }

		public int CodeId { get; }

		public string Description { get; }

		public ProviderMap<Entry> Entries { get; }

		public override string ToString() {
			return Description;
			//KMS DO return $"[{Owner}:{Description}]";
		}

		#region equality
		public override bool Equals(object other) {
			return Equals(other as LookupCode);
		}

		public bool Equals(LookupCode other) {
			return other != null && other.Owner.TableName == Owner.TableName && other.CodeId == CodeId;
		}

		public override int GetHashCode() {
			return HashCode.Compute(Owner.TableName, CodeId);
		}

		public static bool operator ==(LookupCode a, LookupCode b) {
			return ReferenceEquals(a, b) || !ReferenceEquals(a, null) && a.Equals(b);
		}

		public static bool operator !=(LookupCode a, LookupCode b) {
			return !(a == b);
		}
		#endregion

		#region inner Entry
		public class Entry : IComparable, IComparable<Entry> {
			internal Entry(LookupCode owner, LookupBuilder.Entry b) {
				Owner = owner;
				DisplayOrder = b.DisplayOrder;
				IsActive = b.IsActive;
			}

			private LookupCode Owner { get; }

			// ReSharper disable once MemberCanBePrivate.Global
			public int DisplayOrder { get; }

			public bool IsActive { get; }

			public int CompareTo(object other) {
				return CompareTo((Entry)other);
			}

			public int CompareTo(Entry other) {
				int result = DisplayOrder.CompareTo(other.DisplayOrder);
				if (result == 0)
					result = string.Compare(Owner.Description, other.Owner.Description, StringComparison.CurrentCultureIgnoreCase);
				return result;
			}
		}
		#endregion
	}
}