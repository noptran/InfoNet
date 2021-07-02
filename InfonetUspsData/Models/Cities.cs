using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Usps.Data.Models {
	public class Cities {
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Cities() {
			ZipCodes = new HashSet<ZipCodes>();
			Counties = new HashSet<Counties>();
			States = new HashSet<States>();
			Townships = new HashSet<Townships>();
		}

		public int ID { get; set; }

		[Required]
		[StringLength(80)]
		public string CityName { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<ZipCodes> ZipCodes { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Counties> Counties { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<States> States { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Townships> Townships { get; set; }
	}
}