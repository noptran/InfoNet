using System;
using System.Collections.Generic;
using Infonet.Core.Collections;

namespace Infonet.Reporting.AdHoc.Predicates {
	public class ConjunctionPredicate : CollectionDecorator<IPredicate>, IPredicate {
		private readonly List<IPredicate> _inner;

		public ConjunctionPredicate(PredicateOperator conjunction) {
			_inner = new List<IPredicate>();
			Conjunction = conjunction;
		}

		public ConjunctionPredicate(PredicateOperator conjunction, IEnumerable<IPredicate> operands) {
			_inner = new List<IPredicate>(operands);
			Conjunction = conjunction;
		}

		protected override ICollection<IPredicate> Inner {
			get { return _inner; }
		}

		public PredicateOperator Conjunction { get; set; }

		public PredicateOperator Precedence {
			get { return Count > 1 ? Conjunction : (Count == 1 ? _inner[0].Precedence : PredicateOperator.Comparison); }
		}

		public void AddRequiredEntityIdsTo(ISet<string> entityIds) {
			foreach (var each in this)
				each.AddRequiredEntityIdsTo(entityIds);
		}

		public void WriteOn(QueryWriter sql) {
			if (Count == 0) {
				sql.Write(EmptySql);
				return;
			}
			var thisPrecedence = Precedence;
			bool lastParenthesized = false;
			for (var en = Lookahead.New(this); en.MoveNext();) {
				if (!en.IsFirst) {
					if (lastParenthesized)
						sql.Write(" ");
					else
						sql.WriteLine();
					sql.Write(OperatorSql);
				}
				var eachPrecedence = en.Current.Precedence;
				bool parenthesize = eachPrecedence != PredicateOperator.Comparison && eachPrecedence != thisPrecedence;
				if (parenthesize) {
					sql.WriteLine("(");
					sql.IndentMore();
				}
				en.Current.WriteOn(sql);
				if (parenthesize) {
					sql.IndentLess();
					sql.WriteLine();
					sql.Write(")");
				}
				lastParenthesized = parenthesize;
			}
		}

		private string OperatorSql {
			get {
				switch (Conjunction) {
					case PredicateOperator.And:
						return "AND ";
					case PredicateOperator.Or:
						return "OR ";
					default:
						throw new NotSupportedException($"PredicateOperator.{Conjunction} not supported");
				}
			}
		}

		private string EmptySql {
			get {
				switch (Conjunction) {
					case PredicateOperator.And:
						return "1 = 1";
					case PredicateOperator.Or:
						return "1 = 2";
					default:
						throw new NotSupportedException($"PredicateOperator.{Conjunction} not supported");
				}
			}
		}
	}
}