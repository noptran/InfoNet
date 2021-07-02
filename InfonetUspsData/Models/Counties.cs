using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Usps.Data.Models {
	public class Counties {
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Counties() {
			Cities = new HashSet<Cities>();
			Townships = new HashSet<Townships>();
			ZipCodes = new HashSet<ZipCodes>();
			States = new HashSet<States>();
		}

		public int ID { get; set; }

		[Required]
		[StringLength(80)]
		public string CountyName { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Cities> Cities { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Townships> Townships { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<ZipCodes> ZipCodes { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<States> States { get; set; }
	}
}