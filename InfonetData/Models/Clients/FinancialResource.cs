using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "ID,ClientID,CaseID,IncomeSource2ID,Amount")]
	public class FinancialResource : IRevisable {
		public int ID { get; set; }

		public int ClientID { get; set; }

		public int CaseID { get; set; }

		[Lookup("IncomeSource2")]
		public int? IncomeSource2ID { get; set; }

        [OnBindException("This field should be a number, either -1 or greater than 0.", typeof(FormatException))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F2}")]
        [RegularExpression(@"-1|^\d*[1-9]\d*\.?\d*$|^0*\.0?[1-9]\d*$", ErrorMessage = "This field should be a number, either -1 or greater than 0")]
		[Range(-1, 99999, ErrorMessage = "The Amount should be more than -1 and less than 99,999")]
		public decimal? Amount { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		public bool IsUnchanged(FinancialResource obj) {
			return obj != null &&
					ID == obj.ID &&
					Amount == obj.Amount &&
					CaseID == obj.CaseID &&
					ClientID == obj.ClientID &&
					IncomeSource2ID == obj.IncomeSource2ID;
		}

		#region predicates
		public static Expression<Func<FinancialResource, bool>> HasAmount() {
			return fr => fr.Amount != -1 && fr.IncomeSource2ID != -1;
		}
		#endregion
	}
}