using System.Web;
using System.Web.Http;
using DataAccess.NHibernate;
using WebApplication1.Logs;

namespace WebApplication1
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var bootstraper = new Bootstraper();
            bootstraper.Setup();
            Logger.InitLogger();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_BeginRequest()
        {
            var sessionProvider = GlobalConfiguration.Configuration.DependencyResolver.GetService(
                typeof(SessionProvider)) as SessionProvider;
            sessionProvider.OpenSession();
        }

        protected void Application_EndRequest()
        {
            var sessionProvider = GlobalConfiguration.Configuration.DependencyResolver.GetService(
                typeof(SessionProvider)) as SessionProvider;
            sessionProvider.CloseSession();
        }
    }
}