using System.Diagnostics.CodeAnalysis;

namespace Infonet.Reporting.AdHoc {
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	public class Option {
		private string _label = null;

		public object Value { get; set; }

		public string Label {
			get { return _label ?? Value?.ToString(); }
			set { _label = value; }
		}

		public string Group { get; set; }

		//KMS DO IsActive?
	}
}