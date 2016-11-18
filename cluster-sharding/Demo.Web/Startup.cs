using System.IO;
using System.Web.Http;
using Demo.Web.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace Demo.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.EnableCors();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //app.UseFileServer(new FileServerOptions
            //{
            //    EnableDirectoryBrowsing = false,
            //    FileSystem = new PhysicalFileSystem("/")
            //});
            app.UseFileServer(new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = new PhysicalFileSystem(Path.Combine(Directory.GetCurrentDirectory()))
            });
            app.UseWebApi(config);
            app.Map("/signalr", map =>
            {
                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                    EnableDetailedErrors = true,
                    EnableJavaScriptProxies = true,
                    EnableJSONP = true
                };
                 
                // camelcase contract resolver
                var serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    ContractResolver = new SignalRContractResolver()
                });

                GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch is already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}