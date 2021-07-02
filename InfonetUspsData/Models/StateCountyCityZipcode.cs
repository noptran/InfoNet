using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infonet.Usps.Data.Models {
	[Table("StateCountyCityZipcode")]
	public class StateCountyCityZipcode {
		[Key]
		[Column(Order = 0)]
		[StringLength(2)]
		public string StateAbbreviation { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(80)]
		public string CountyName { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(80)]
		public string CityName { get; set; }

		[Key]
		[Column(Order = 3)]
		[StringLength(5)]
		public string Zipcode { get; set; }
	}
}