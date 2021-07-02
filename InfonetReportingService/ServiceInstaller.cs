using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Infonet.Reporting.Service {
	[RunInstaller(true)]
	public class ServiceInstaller : Installer {
		public ServiceInstaller() {
			Installers.AddRange(new Installer[] {
				new ServiceProcessInstaller {
					Account = ServiceAccount.LocalSystem
				},
				new System.ServiceProcess.ServiceInstaller {
					Description = "Runs scheduled reports, notifies approvers, and cleans up after expiration.",
					DisplayName = "ICJIA InfoNet Reporting Service",
					ServiceName = "InfonetReportingService",
					StartType = ServiceStartMode.Automatic
				}
			});
		}
	}
}