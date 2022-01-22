using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Kaede.Server
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            var port = ConfigurationManager.AppSettings["allowedPort"];
            var cors = new EnableCorsAttribute($@"http://localhost:{port}", "*", "*");
            config.EnableCors(cors);

            const string endpointRoot = "api";
            config.Routes.MapHttpRoute(
                name: "NameApi",
                routeTemplate: endpointRoot + "/name/{id}",
                defaults: new { controller = "Name" }
            );
            config.Routes.MapHttpRoute(
                name: "IdsApi",
                routeTemplate: endpointRoot + "/ids/{name}",
                defaults: new { controller = "Ids" }
            );
            config.Routes.MapHttpRoute(
                name: "ExtractApi",
                routeTemplate: endpointRoot + "/extract/{id}",
                defaults: new { controller = "Extract" }
            );
        }
    }
}
