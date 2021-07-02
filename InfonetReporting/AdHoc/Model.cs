using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Reporting.AdHoc.Predicates;

//KMS DO verify everyone has a key or error when one is changed?
//KMS DO check all ids are alphanumeric only ....no underscores or spaces
//KMS DO should Keys take IEnumerables?
namespace Infonet.Reporting.AdHoc {
	public class Model {
		public const char ID_SEPARATOR = '_';

		public Model() {
			Entities = new KeyedCollection<string, Entity>(e => e.Id);
			Fields = new EntityChildDictionary<Field>(this, e => e.Fields);
			Filters = new EntityChildDictionary<Filter>(this, e => e.Filters);
		}

		public KeyedCollection<string, Entity> Entities { get; }

		public IReadOnlyDictionary<string, Field> Fields { get; }

		public IReadOnlyDictionary<string, Filter> Filters { get; }

		public Entity AddEntity(string id, string tableSql, string label) {
			return Add(new Entity(id, tableSql) { Label = label });
		}

		//KMS DO return it?
		public Entity Add(Entity entity) {
			Entities.Add(entity);
			return entity;
		}

		#region query building
		//KMS DO move these to a Context or Builder or something?
		public IEnumerable<Field> Select(IEnumerable<string> fieldIds) {
			return fieldIds.Select(Field);
		}

		public IEnumerable<Field> Select(params string[] fieldIds) {
			return fieldIds.Select(Field);
		}

		public Field Field(string fieldId) {
			return Fields[fieldId];
		}

		public FilterPredicate Filter(string filterId, IDictionary<string, object> criteria = null) {
			return Filters[filterId].ToPredicate(criteria);
		}

		public ConjunctionPredicate And(IEnumerable<IPredicate> operands) {
			return new ConjunctionPredicate(PredicateOperator.And, operands);
		}

		public ConjunctionPredicate And(params IPredicate[] operands) {
			return new ConjunctionPredicate(PredicateOperator.And, operands);
		}

		public ConjunctionPredicate Or(IEnumerable<IPredicate> operands) {
			return new ConjunctionPredicate(PredicateOperator.Or, operands);
		}

		public ConjunctionPredicate Or(params IPredicate[] operands) {
			return new ConjunctionPredicate(PredicateOperator.Or, operands);
		}
		#endregion

		#region inner
		private class EntityChildDictionary<TChild> : IReadOnlyDictionary<string, TChild> where TChild : Entity.Child {
			private readonly Model _model;
			private readonly Func<Entity, KeyedCollection<string, TChild>> _selector;

			internal EntityChildDictionary(Model model, Func<Entity, KeyedCollection<string, TChild>> selector) {
				_model = model;
				_selector = selector;
			}

			public IEnumerator<KeyValuePair<string, TChild>> GetEnumerator() {
				return new Enumerator(Values.GetEnumerator());
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			public int Count {
				get { return _model.Entities.Sum(e => _selector.Invoke(e).Count); }
			}

			public bool ContainsKey(string key) {
				TChild value;
				return TryGetValue(key, out value);
			}

			public bool TryGetValue(string key, out TChild value) {
				if (key == null)
					throw new ArgumentNullException(nameof(key));

				var ids = key.Split(ID_SEPARATOR);
				if (ids.Length != 2)
					throw new ArgumentException($"{nameof(key)} must be formatted as {{Entity.Id}}{ID_SEPARATOR}{{{typeof(TChild).Name}.LocalId}}");

				Entity entity;
				if (!_model.Entities.TryGetValue(ids[0], out entity)) {
					value = default(TChild);
					return false;
				}
				return _selector.Invoke(entity).TryGetValue(ids[1], out value);
			}

			public TChild this[string key] {
				get {
					TChild result;
					if (!TryGetValue(key, out result))
						throw new KeyNotFoundException($"{typeof(TChild).Name} {{{key}}} not found");
					return result;
				}
			}

			public IEnumerable<string> Keys {
				get { return Values.Select(f => f.Id); }
			}

			public IEnumerable<TChild> Values {
				get { return _model.Entities.SelectMany(e => _selector.Invoke(e)); }
			}

			private class Enumerator : EnumeratorDecorator<TChild, KeyValuePair<string, TChild>> {
				internal Enumerator(IEnumerator<TChild> inner) {
					Inner = inner;
				}

				protected override IEnumerator<TChild> Inner { get; }

				public override KeyValuePair<string, TChild> Current {
					get {
						var current = Inner.Current;
						return new KeyValuePair<string, TChild>(current.Id, current);
					}
				}
			}
		}
		#endregion
	}
}