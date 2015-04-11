using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace SideStream.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            foreach (MediaTypeFormatter f in GlobalConfiguration.Configuration.Formatters)
            {
                JsonMediaTypeFormatter jf = f as JsonMediaTypeFormatter;
                if (jf != null)
                {
                    jf.SerializerSettings.Converters.Add(new SideStream.API.GeoJson.MapLayersResultConverter());
                }
            }

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
