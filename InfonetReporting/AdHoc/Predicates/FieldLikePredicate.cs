using System.Text;

namespace Infonet.Reporting.AdHoc.Predicates {
	public class FieldLikePredicate : FieldPredicate {
		private object _like = null;

		public FieldLikePredicate(Field field) : base(field) { }

		public bool Raw { get; set; }

		public string Prefix { get; set; }

		public string Suffix { get; set; }

		public object Like {
			get { return _like; }
			set {
				object o = Field.Type.Convert(value);
				if (o is string) {
					var sb = new StringBuilder((string)o);
					if (!Raw)
						sb.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_").Replace("[", @"\[");
					if (Prefix != null)
						sb.Insert(0, Prefix);
					if (Suffix != null)
						sb.Append(Suffix);
					o = sb.ToString();
				}
				_like = o;
			}
		}

		public override void WriteOn(QueryWriter sql) {
			Field.WriteToPredicate(sql);
			if (Not)
				sql.Write(" NOT");
			sql.Write(" LIKE ");
			sql.WriteParameter(Like, Field, "like");
			sql.Write(@" ESCAPE '\'");
		}
	}
}