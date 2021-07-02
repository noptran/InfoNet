using System.ComponentModel.DataAnnotations;

namespace Infonet.Usps.Data.Models {
	public class IllinoisGovernments {
		public int ID { get; set; }

		[Required]
		[StringLength(30)]
		public string Name { get; set; }

		[Required]
		[StringLength(10)]
		public string Type { get; set; }

		[Required]
		[StringLength(15)]
		public string County { get; set; }

		[StringLength(15)]
		public string CountySeat { get; set; }

		[StringLength(50)]
		public string Comment { get; set; }

		public int? TownshipID { get; set; }

		public int? CountyID { get; set; }

		public int? CityID { get; set; }
	}
}