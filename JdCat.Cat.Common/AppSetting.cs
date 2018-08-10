
using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common
{
    public class AppSetting
    {
        public static ILoggerRepository LogRepository;
        static AppSetting()
        {
            LogRepository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(LogRepository, new FileInfo("log4net.config"));
        }
    }
}
