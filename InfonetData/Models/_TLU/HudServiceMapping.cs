namespace Infonet.Data.Models._TLU {
	public class HudServiceMapping {
		public int ServiceProgramId { get; set; }
		public int HudServiceId { get; set; }
		public virtual TLU_Codes_HUDService TLU_Codes_HUDService { get; set; }
		public virtual TLU_Codes_ProgramsAndServices TLU_Codes_ProgramsAndServices { get; set; }
	}
}