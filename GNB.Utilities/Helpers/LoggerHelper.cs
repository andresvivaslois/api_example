using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace GNB.Utilities.Helpers
{
    public static class LoggerHelper
    {

        private static readonly string LOG_CONFIG_FILE = @"log4net.config";

        private static readonly log4net.ILog _log = GetLogger(typeof(LoggerHelper));

        public static ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }

        public static void LogError(object message)
        {
            SetLog4NetConfiguration();
            _log.Error(message);
        }

        private static void SetLog4NetConfiguration()
        {
            XmlDocument log4netConfig = new XmlDocument();

            var prueba = File.OpenRead(LOG_CONFIG_FILE);
            log4netConfig.Load(File.OpenRead(LOG_CONFIG_FILE));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }
    }
}
