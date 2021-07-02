using Infonet.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Infonet.Web
{
	public partial class Startup {
		public void Configuration(IAppBuilder app) {
			ConfigureAuth(app);
		}

		//public void ConfigureServices(ServiceCollection services) { }
	}
}