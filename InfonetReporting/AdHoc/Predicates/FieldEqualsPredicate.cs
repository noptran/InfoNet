namespace Infonet.Reporting.AdHoc.Predicates {
	//KMS DO rename this somehow?
	public class FieldEqualsPredicate : FieldPredicate {
		private object _value = null;

		public FieldEqualsPredicate(Field field) : base(field) { }

		public object Value {
			get { return _value; }
			set { _value = Field.Type.Convert(value); }
		}

		public override void WriteOn(QueryWriter sql) {
			Field.WriteToPredicate(sql);
			if (Value == null) {
				sql.Write(Not ? " IS NOT NULL" : " IS NULL");
			} else {
				sql.Write(Not ? " != " : " = ");
				sql.WriteParameter(Value, Field, "equals");
			}
		}
	}
}