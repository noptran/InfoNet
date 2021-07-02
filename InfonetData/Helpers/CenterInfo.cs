using Infonet.Data.Looking;

namespace Infonet.Data.Helpers {
	//KMS DO rename this
	public class CenterInfo {
		public int CenterId { get; set; }

		public string CenterName { get; set; }

		public Provider Provider { get; set; }

		public bool HasShelter { get; set; }

		public bool isReal { get; set; } //KMS DO fix name

		public bool isSatellite { get; set; } //KMS DO fix name

		public int ParentCenterId { get; set; }

		public bool isChecked { get; set; } //KMS DO this needs to live elsewhere
	}
}