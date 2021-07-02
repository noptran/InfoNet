using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Core.Collections;
using Infonet.Core.IO;

namespace Infonet.Reporting.AdHoc {
	public class Entity {
		public Entity(Fragment tableSql) : this(null, tableSql) { }

		public Entity(string id, Fragment tableSql) {
			if (tableSql == null)
				throw new ArgumentNullException(nameof(tableSql));
			if (id == null && tableSql.Ids.Count == 0)
				throw new ArgumentException($"Unable to determine {GetType().Name}.{nameof(Id)}: {nameof(id)} not specified and {nameof(tableSql)} contains no {{#}} tags: \"{tableSql.Source}\"");
			if (id == null && tableSql.Ids.Count > 1)
				throw new ArgumentException($"Unable to determine {GetType().Name}.{nameof(Id)} for: {nameof(id)} not specified and {nameof(tableSql)} contains multiple distinct {{#}} tags: \"{tableSql.Source}\"");

			Fields = new KeyedCollection<string, Field>(f => f.LocalId);
			Relationships = new List<Relationship>();
			Filters = new KeyedCollection<string, Filter>(f => f.LocalId);
			RequiredEntityIds = new HashSet<string>(tableSql.Tags);

			Id = id ?? CamelCase.FromPascal(tableSql.Ids.Single());
			TableSql = tableSql.Text;
			Label = CamelCase.ToProper(id ?? tableSql.Ids.Single());
		}

		public string Id { get; }

		public string TableSql { get; }

		public string Label { get; set; }

		public KeyedCollection<string, Field> Fields { get; }

		public ICollection<Relationship> Relationships { get; }

		public KeyedCollection<string, Filter> Filters { get; }

		public Field[] Key { get; set; }

		/**
		 * Ids of Entities that must be joined into Queries whenever Fields
		 * requiring this Entity are included in results or predicates. These
		 * Entities must be joined but not necessarily before this Entity.
		 * NOTE: These Entities are not joined when this Entity is only joined
		 * to itself.
		 */
		public ISet<string> RequiredEntityIds { get; }

		public override string ToString() {
			return $"{{{Id}}}";
		}

		public void AddRequiredEntityIdsTo(ISet<string> entityIds) {
			entityIds.AddRange(RequiredEntityIds);
		}

		public void WriteOn(QueryWriter sql) {
			sql.Write(TableSql);
			sql.Write(" ");
			sql.Write(Id);
		}

		public Field Add(Field result) {
			result.SetParentId(Id);
			Fields.Add(result);
			return result;
		}

		public Relationship RelateOneTo(Cardinal rightCardinality, Entity right, string predicateSql) {
			var result = new Relationship(this, right, predicateSql, Cardinal.One.To(rightCardinality));
			Relationships.Add(result);
			right.Relationships.Add(result);
			return result;
		}

		public Relationship RelateZeroTo(Cardinal rightCardinality, Entity right, string predicateSql) {
			var result = new Relationship(this, right, predicateSql, Cardinal.ZeroOrOne.To(rightCardinality));
			Relationships.Add(result);
			right.Relationships.Add(result);
			return result;
		}

		public Filter AddFilter(string id, string expressionSql, string label) {
			var result = new Filter(Id, id, expressionSql, label);
			Filters.Add(result);
			return result;
		}
		
		public IEnumerable<Join> Join(ISet<Entity> needed) {
			var result = new List<Join>();
			var visited = new HashSet<Entity>();
			AppendJoinsTo(result, null, needed, visited);
			if (!needed.IsSubsetOf(visited)) {
				var missing = new HashSet<Entity>(needed);
				missing.ExceptWith(visited);
				throw new NotSupportedException($"Disconnect detected in Model: Entities not visited: {missing.ToConjoinedString("and")}");
			}

			//nest inner joins within earliest, preceding, allowed outer join
			result.Insert(0, new Join(null, this)); //placeholder outer join for initial inner joins
			for (int i = 1; i < result.Count; i++) {
				var each = result[i];
				if (each.Relationship.Cardinality.AllowsZero)
					continue;
				for (int j = 0; j < i; j++)
					try {
						result[j].Nested.Add(each); //fails when nesting each not allowed
						result.RemoveAt(i--);
						each = null;
						break;
					} catch (ArgumentException) { }
				if (each != null)
					throw new Exception($"Inner Join has Origin ({each.Origin}) unreachable by its predecessors");
			}
			result.InsertRange(1, result[0].Nested);
			result.RemoveAt(0);

			return result;
		}

		internal void AppendJoinsTo(List<Join> joins, Relationship caller, ISet<Entity> needed, ISet<Entity> visited) {
			if (visited.Contains(this))
				throw new NotSupportedException($"Cycle detected in Model: {this} already visited");

			visited.Add(this);
			foreach (var each in Relationships)
				if (each != caller)
					each.AppendJoinsTo(joins, this, needed, visited);
		}

		#region inner
		public abstract class Child {
			private string _parentId = null;
			private string _id = null;

			protected Child(string localId, Fragment expressionSql) {
				if (expressionSql == null)
					throw new ArgumentNullException(nameof(expressionSql));
				if (localId == null && expressionSql.Ids.Count == 0)
					throw new ArgumentException($"Unable to determine {GetType().Name}.{nameof(LocalId)}: {nameof(localId)} not specified and {nameof(expressionSql)} contains no {{#}} tags: \"{expressionSql.Source}\"");
				if (localId == null && expressionSql.Ids.Count > 1)
					throw new ArgumentException($"Unable to determine {GetType().Name}.{nameof(LocalId)}: {nameof(localId)} not specified and {nameof(expressionSql)} contains multiple distinct {{#}} tags: \"{expressionSql.Source}\"");

				LocalId = localId ?? CamelCase.FromPascal(expressionSql.Ids.Single());
				if (LocalId.Contains("_"))
					throw new ArgumentException($"Invalid {GetType().Name}.{nameof(LocalId)}: {nameof(LocalId)} may not contain underscores: '{LocalId}'");
				ExpressionSql = expressionSql.Text;
				RequiredEntityIds = new HashSet<string>(expressionSql.Tags);
				Label = CamelCase.ToProper(localId ?? expressionSql.Ids.Single());
				if (Label == "Id") //KMS DO expand this greatly?
					Label = "ID";
			}

			internal void SetParentId(string parentId, bool skipParentTagCheck = false) {
				if (!skipParentTagCheck && !RequiredEntityIds.Contains(parentId))
					throw new ArgumentException($"Parent {{{ParentId}}} not found in {nameof(ExpressionSql)} for {this}");

				_parentId = parentId;
				_id = parentId + Model.ID_SEPARATOR + LocalId;
			}

			public string ExpressionSql { get; }

			public string LocalId { get; }

			public string ParentId {
				get { return _parentId; }
			}

			public string Id {
				get { return _id; }
			}

			public string Label { get; set; }

			/**
			 * Ids of Entities that must be joined whenever this Field is
			 * included in results or predicates.  Additionally, when this
			 * Field is included in results, these Entities' Keys are included
			 * in the results to preserve distinct records AND in the
			 * predicates to eliminate extra (blank) records due to other
			 * Entities joined but without Fields in the results which require
			 * them.
			 */
			public ISet<string> RequiredEntityIds { get; }

			public override string ToString() {
				return $"{{{Id}}}";
			}

			public void AddRequiredEntityIdsTo(ISet<string> entityIds) {
				entityIds.AddRange(RequiredEntityIds);
			}
		}
		#endregion
	}
}