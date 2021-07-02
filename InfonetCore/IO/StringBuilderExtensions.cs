using System.Text;

//KMS DO eliminate this
namespace Infonet.Core.IO {
	public static class StringBuilderExtensions {
		public static void AppendQuotedCSVData(this StringBuilder sb, object value) {
			sb.Append(string.Format(@"""{0}""", value));
		}
	}
}