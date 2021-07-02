using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infonet.Usps.Data.Models {
	public class CityZipcodes {
		[Key]
		[Column(Order = 0)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int CityID { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(80)]
		public string CityName { get; set; }

		[Key]
		[Column(Order = 2)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Zipcode { get; set; }
	}
}