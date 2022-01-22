using Kaede.Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Kaede.Server {
    public class WebApiApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            // jsonで返す
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            // lowerCamel
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var mapleDir = ConfigurationManager.AppSettings["mapleDir"];
            var target = ConfigurationManager.AppSettings["target"];
        }
    }
}