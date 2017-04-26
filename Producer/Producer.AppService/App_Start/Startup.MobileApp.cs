using System.Configuration;
using System.Data.Entity;
using System.Web.Http;

using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Tables.Config;

using Owin;

using Producer.AppService.Models;

namespace Producer.AppService
{
	public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            // Info on Web API tracing: http://go.microsoft.com/fwlink/?LinkId=620686
            config.EnableSystemDiagnosticsTracing();

            new MobileAppConfiguration ()
                .AddMobileAppHomeController ()
                .MapApiControllers ()
                //.AddPushNotifications () // obsolete but may still be needed
                .AddTables (new MobileAppTableConfiguration ().MapTableControllers ().AddEntityFramework ())
                .ApplyTo (config);


            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new ProducerInitializer());

            // To prevent Entity Framework from modifying your database schema, use a null database initializer
            // Database.SetInitializer<ProducerContext>(null);

            var settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                // This middleware is intended to be used locally for debugging. By default, HostName will
                // only have a value when running in an App Service application.
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }

            app.UseWebApi(config);
        }
    }

    public class ProducerInitializer : CreateDatabaseIfNotExists<ProducerContext>
    {
        protected override void Seed(ProducerContext context)
        {
			//foreach (var item in TempData.PublicAvContent)
			//{
			//	context.Set<AvContent> ().Add (item);
			//}

            base.Seed(context);
        }
    }
}

