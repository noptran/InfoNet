using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Core.Data;

namespace Infonet.Reporting.AdHoc {
	public class Query {
		public Query(Model model) {
			Distinct = false;
			Model = model;
		}

		public Model Model { get; }

		public bool Distinct { get; set; }

		public int? Top { get; set; }

		public IEnumerable<Field> Select { get; set; }

		public IPredicate Where { get; set; }

		private ISet<Entity> RequiredEntities {
			get {
				var entityIds = new HashSet<string>();
				if (Select != null)
					foreach (var each in Select)
						each.AddRequiredEntityIdsTo(entityIds);
				Where?.AddRequiredEntityIdsTo(entityIds);

				var result = new HashSet<Entity>();
				result.AddRange(entityIds.Select(s => Model.Entities[s]));

				while (true) {
					var additionalEntityIds = new HashSet<string>();
					foreach (var each in result)
						each.AddRequiredEntityIdsTo(additionalEntityIds);
					additionalEntityIds.ExceptWith(entityIds);
					if (additionalEntityIds.Count == 0)
						return result;

					result.AddRange(additionalEntityIds.Select(s => Model.Entities[s]));
					entityIds.AddRange(additionalEntityIds);
				}
			}
		}

		private Field[] RequiredKeys {
			get {
				var entityIds = new HashSet<string>();
				foreach (var each in Select)
					each.AddRequiredEntityIdsTo(entityIds);
				var requiredKeys = new List<Field>();
				foreach (string eachEntity in entityIds)
					foreach (var eachField in Model.Entities[eachEntity].Key) {
						if (!eachField.NotEmpty)
							throw new NotSupportedException($"Entity {eachEntity} has an invalid Key field: {eachField.LocalId} must be NotEmpty");
						requiredKeys.Add(eachField);
					}
				return requiredKeys.ToArray();
			}
		}

		//KMS DO ban full and invalid left to right combos?
		//KMS DO ...what if we forbid RIGHT and FULL...if we can't use only left, we're done!
		//KMS DO what if we pass through entities that have required entities????
		//KMS DO implement Filters here?
		public SqlCommand ToCommand(SqlConnection connection = null, IEnumerable<SqlParameter> externalParameters = null, int? timeout = null) {
			//KMS DO check that all fields have entities?

			var entities = RequiredEntities;
			var requiredKeys = RequiredKeys;
			var startingEntity = Model.Entities.FirstOrDefault(e => entities.Contains(e));

			using (var sql = new QueryWriter()) {
				sql.Write("SELECT");
				if (Distinct)
					sql.Write(" DISTINCT");
				if (Top != null) {
					sql.Write(" TOP ");
					sql.Write(Top.ToString());
				}
				sql.WriteLine();
				sql.IndentMore();
				var en = Lookahead.New(Select.Concat(requiredKeys.Except(Select)));
				if (!en.HasNext)
					sql.Write("COUNT(*) AS _count");
				while (en.MoveNext()) {
					if (!en.IsFirst)
						sql.WriteLine(",");
					en.Current.WriteToSelect(sql);
				}
				sql.IndentLess();

				if (startingEntity != null) {
					sql.WriteLine();
					sql.Write("FROM ");
					startingEntity.WriteOn(sql);
					sql.IndentMore();
					foreach (var each in startingEntity.Join(entities)) {
						sql.WriteLine();
						each.WriteOn(sql);
					}
					sql.IndentLess();
				}

				var from = Model.And();
				var requiredKeysNotEmpty = Model.Or(requiredKeys.Select(f => Condition.NotEmpty.ToPredicate(f)));
				if (requiredKeysNotEmpty.Count > 0)
					from.Add(requiredKeysNotEmpty);
				if (Where != null)
					from.Add(Where);
				if (from.Count > 0) {
					sql.WriteLine();
					sql.Write("WHERE ");
					sql.IndentMore();
					from.WriteOn(sql);
					sql.IndentLess();
				}

				var command = new SqlCommand(sql.ToString());
				command.Parameters.AddRange(sql.Parameters);
				if (externalParameters != null)
					command.Parameters.AddRange(externalParameters);
				if (timeout != null)
					command.CommandTimeout = (int)timeout;
				if (connection != null)
					command.Connection = connection;
				return command;
			}
		}

		//KMS DO this is great but it doesn't work for IN clauses...maybe just search and replace them all...for debugging only obviously???
		public string ToSql(IEnumerable<SqlParameter> externalParameters = null) {
			var command = ToCommand(null, externalParameters);
			using (var w = new StringWriter()) {
				foreach (SqlParameter each in command.Parameters)
					w.WriteLine($"DECLARE {each.ParameterName} {each.SqlDbType};");
				foreach (SqlParameter each in command.Parameters)
					w.WriteLine($"SET {each.ParameterName} = CAST({QueryWriter.ToSql(each.Value)} AS {each.SqlDbType});");
				w.Write(command.CommandText);
				return w.ToString();
			}
		}
	}
}