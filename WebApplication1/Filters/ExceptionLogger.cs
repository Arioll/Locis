using System;
using System.Diagnostics;
using System.Web.Http.Filters;
using WebApplication1.Logs;

namespace WebApplication1.Filters
{
    public class ExceptionLogger : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string logInfo =
                $"{DateTime.Now}: " +
                $"EXCEPTION: {actionExecutedContext.Exception.Message} " +
                $"CONTENT: {actionExecutedContext.Request} " +
                $"STACKTRACE: {actionExecutedContext.Exception.StackTrace}";

            Trace.WriteLine(logInfo);
            Logger.Log.Info(logInfo);
        }
    }
}