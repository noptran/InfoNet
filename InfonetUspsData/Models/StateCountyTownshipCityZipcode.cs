using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infonet.Usps.Data.Models {
	[Table("StateCountyTownshipCityZipcode")]
	public class StateCountyTownshipCityZipcode {
		[Key]
		[Column(Order = 0)]
		[StringLength(32)]
		public string StateName { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(80)]
		public string CountyName { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(80)]
		public string TownshipName { get; set; }

		[Key]
		[Column(Order = 3)]
		[StringLength(80)]
		public string CityName { get; set; }

		[Key]
		[Column(Order = 4)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Zipcode { get; set; }
	}
}