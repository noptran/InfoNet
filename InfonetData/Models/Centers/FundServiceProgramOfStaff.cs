using System;
using Infonet.Core.Entity;
using Infonet.Data.Models._TLU;

namespace Infonet.Data.Models.Centers {
	public class FundServiceProgramOfStaff : IRevisable {
		public int FundDateID { get; set; }
		public int SVID { get; set; }
		public int ServiceProgramID { get; set; }
		public int FundingSourceID { get; set; }
		public short PercentFund { get; set; }
		public DateTime? RevisionStamp { get; set; }
		public int ID { get; set; }
		public virtual FundingDate FundingDate { get; set; }
		public virtual StaffVolunteer StaffVolunteer { get; set; }
		public virtual TLU_Codes_FundingSource TLU_Codes_FundingSource { get; set; }
		public virtual TLU_Codes_ProgramsAndServices TLU_Codes_ProgramsAndServices { get; set; }
	}
}