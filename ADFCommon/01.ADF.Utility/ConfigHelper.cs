using System;
using Microsoft.Extensions.Configuration;

namespace ADF.Utility
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public static class ConfigHelper
    {
        static IConfiguration _config;
        static object _lock = new object();
        public static IConfiguration Configuration
        {
            get
            {
                if (null == _config)
                {
                    lock (_lock)
                    {
                        if (null == _config)
                        {
                            var builder = new ConfigurationBuilder()
                            .SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("appsettings.json");
                            _config = builder.Build();
                        }
                    }
                }
                return _config;
            }
            set { _config = value; }
        }

        public static string GetValue(string appName)
        {
            return Configuration[appName];
        }

        public static string GetConnectionStr(string dbName)
        {
            return Configuration.GetConnectionString(dbName);
        }
    }
}