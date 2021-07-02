using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infonet.Usps.Data.Models {
	public class StateCounties {
		[Key]
		[Column(Order = 0)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int StateID { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(32)]
		public string StateName { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(2)]
		public string StateAbbreviation { get; set; }

		[Key]
		[Column(Order = 3)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int CountyID { get; set; }

		[Key]
		[Column(Order = 4)]
		[StringLength(80)]
		public string CountyName { get; set; }
	}
}