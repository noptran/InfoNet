namespace Infonet.Reporting.AdHoc.Predicates {
	public class FieldEmptyPredicate : FieldPredicate {
		public FieldEmptyPredicate(Field field) : base(field) { }

		public override void WriteOn(QueryWriter sql) {
			if (Field.Type == FieldType.NVarChar)
				sql.Write("NULLIF(");
			Field.WriteToPredicate(sql);
			if (Field.Type == FieldType.NVarChar)
				sql.Write(", '')");
			sql.Write(Not ? " IS NOT NULL" : " IS NULL");
		}
	}
}