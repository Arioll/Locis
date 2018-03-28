using log4net;
using log4net.Config;

namespace WebApplication1.Logs
{
    public static class Logger
    {
        private static ILog log = LogManager.GetLogger("LocisLogger");


        public static ILog Log
        {
            get { return log; }
        }

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}