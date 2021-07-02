using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infonet.Usps.Data.Models {
	public class CountyZipcodes {
		[Key]
		[Column(Order = 0)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int CountyID { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(80)]
		public string CountyName { get; set; }

		[Key]
		[Column(Order = 2)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Zipcode { get; set; }
	}
}