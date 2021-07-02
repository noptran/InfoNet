using System.Collections.Generic;
using Infonet.Core.Collections;

namespace Infonet.Data.Looking {
	public class ProviderMap<TValue> : OrdinalEnumMap<Provider, TValue> {
		public ProviderMap(IDictionary<Provider, TValue> values) : base(values) { }
	}
}