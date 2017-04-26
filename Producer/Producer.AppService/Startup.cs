using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Producer.AppService.Startup))]
namespace Producer.AppService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}