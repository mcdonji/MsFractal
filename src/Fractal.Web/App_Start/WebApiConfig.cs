using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace Fractal.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            JsonMediaTypeFormatter jsonMediaTypeFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            jsonMediaTypeFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
