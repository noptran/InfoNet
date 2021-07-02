using System.ServiceProcess;

namespace Infonet.Reporting.Service {
	public static class Program {
		public static void Main(string[] args) {
			if (args.Length > 0 && args[0] == "--debug")
				new ReportService().Debug();
			else if (args.Length > 0 && args[0] == "--flush")
				new ReportService().Flush();
			else
				ServiceBase.Run(new ServiceBase[] { new ReportService() });
		}
	}
}