using System;
using System.Collections.Generic;

namespace Infonet.Reporting.AdHoc {
	public class Relationship {
		internal Relationship(Entity left, Entity right, Fragment conditionSql, CardinalityExpression cardinality) {
			if (left == null)
				throw new ArgumentNullException(nameof(left));
			if (right == null)
				throw new ArgumentNullException(nameof(right));
			if (conditionSql == null)
				throw new ArgumentNullException(nameof(conditionSql));
			if (left == right)
				throw new NotSupportedException($"Self-referencing relationships ({left}->{right}) are not allowed");
			if (cardinality.Left == Cardinal.Zero || cardinality.Right == Cardinal.Zero)
				throw new NotSupportedException("Cardinality.Zero not allowed");
			if ((cardinality.Left & cardinality.Right & Cardinal.Many) > 0)
				throw new NotSupportedException("Many-to-many relationships are not allowed");

			Left = left;
			Right = right;
			ConditionSql = conditionSql.Text;
			Cardinality = cardinality;

			//KMS DO should Relationship have RequiredEntityIds? Should we allow skipping this check?
			foreach (string each in conditionSql.Tags)
				if (each != Left.Id && each != Right.Id)
					throw new NotSupportedException($"Entity {{{each}}} found in {nameof(conditionSql)}[{conditionSql}] is neither Left[{Left}] nor Right[{Right}]");
		}

		public Entity Left { get; }

		public Entity Right { get; }

		public string ConditionSql { get; }

		public CardinalityExpression Cardinality { get; }

		public CardinalityExpression CardinalityTo(Entity destination) {
			if (destination == Right)
				return Cardinality;
			if (destination == Left)
				return Cardinality.Reverse();
			throw new ArgumentException($"{destination} has no cardinality defined for this relationship ({Left}->{Right})");
		}

		internal void AppendJoinsTo(List<Join> joins, Entity caller, ISet<Entity> needed, ISet<Entity> visited) {
			int preCount = joins.Count;
			var destination = Left == caller ? Right : Left;
			destination.AppendJoinsTo(joins, this, needed, visited);
			if (joins.Count > preCount || needed.Contains(destination))
				joins.Insert(preCount, new Join(this, destination));
		}
	}
}