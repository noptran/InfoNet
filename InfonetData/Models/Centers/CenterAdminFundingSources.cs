using Infonet.Data.Looking;
using Infonet.Data.Models._TLU;

namespace Infonet.Data.Models.Centers {
	public class CenterAdminFundingSources {
		public int ID { get; set; }

		[Lookup("CenterAdministrator")]
		public int CenterAdminID { get; set; }

		public int CodeID { get; set; }

		public virtual TLU_Codes_FundingSource TLU_Codes_FundingSource { get; set; }
	}
}