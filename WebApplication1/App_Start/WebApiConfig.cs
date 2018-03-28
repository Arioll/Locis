using System;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Cors;
using UserMangment.Application;
using WebApplication1.Filters;

namespace WebApplication1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );

            var authorizer = config.DependencyResolver.GetService(typeof(IAuthorization)) as IAuthorization;
            config.Filters.Add(new AuthenticationFilter(authorizer));
            config.Filters.Add(new ExceptionLogger());
        }
    }
}