using System;
using System.Collections.Generic;
using Infonet.Core.Collections;

namespace Infonet.Reporting.AdHoc {
	public class Join {
		private readonly ICollection<Join> _nested;
		private readonly ISet<Entity> _nestedDestinations = new HashSet<Entity>();

		public Join(Relationship relationship, Entity destination) {
			_nested = new ExposableCollection<Join>(j => {
				if (!_nestedDestinations.Contains(j.Origin))
					throw new ArgumentException("Join may not be nested because its Origin is unreachable");
				_nestedDestinations.Add(j.Destination);
			}, onRemoving: j => { throw new NotSupportedException(GetType().FullName + ".Inner does not support removal of Joins"); });

			Relationship = relationship;
			if (relationship != null)
				Origin = relationship.Right == destination ? relationship.Left : relationship.Right;
			Destination = destination;

			_nestedDestinations.Add(Destination);
		}

		public Relationship Relationship { get; }

		public Entity Origin { get; }

		public Entity Destination { get; }

		public ICollection<Join> Nested {
			get { return _nested; }
		}

		public void WriteOn(QueryWriter sql) {
			var cardinality = Relationship.CardinalityTo(Destination);
			string descriptor = "";
			if ((cardinality.Left & cardinality.Right & Cardinal.Zero) > 0)
				descriptor = "FULL ";
			else if ((cardinality.Right & Cardinal.Zero) > 0)
				descriptor = "LEFT ";
			else if ((cardinality.Left & Cardinal.Zero) > 0)
				descriptor = "RIGHT ";

			sql.Write(descriptor);
			sql.Write("JOIN ");

			if (Nested.Count > 0) {
				sql.IndentMore();
				sql.WriteLine("(");
			}
			Destination.WriteOn(sql);
			if (Nested.Count > 0) {
				foreach (var each in Nested) {
					sql.WriteLine();
					each.WriteOn(sql);
				}
				sql.IndentLess();
				sql.WriteLine();
				sql.Write(")");
			}
			sql.Write(" ON ");
			sql.Write(Relationship.ConditionSql);
		}
	}
}