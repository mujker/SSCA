using System;
using System.Configuration;
using StackExchange.Redis;

//using StackExchange.Redis;

namespace SSCA.Common
{
    public class Settings
    {
        private Settings()
        {
        }

        static Settings()
        {
            RmiIp = ConfigurationManager.AppSettings["rmi-ip"];
            RmiPort = Convert.ToInt32(ConfigurationManager.AppSettings["rmi-port"]);
            CryptKey = ConfigurationManager.AppSettings["CryptKey"];
            JkdSql = ConfigurationManager.AppSettings["JkdSql"];
            DelayTime = Convert.ToInt32(ConfigurationManager.AppSettings["DelayTime"]);
            ConnectStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            ConOption = new ConfigurationOptions()
            {
                EndPoints =
                {
                    {
                        ConfigurationManager.AppSettings["redis_ip"],
                        Convert.ToInt32(ConfigurationManager.AppSettings["redis_port"])
                    }
                },
                Password = ConfigurationManager.AppSettings["redis_pw"]
            };
            RedisIp = ConfigurationManager.AppSettings["redis_ip"];
            RedisPort = ConfigurationManager.AppSettings["redis_port"];
            RedisPw = ConfigurationManager.AppSettings["redis_pw"];
            RedisFlag = Convert.ToBoolean(ConfigurationManager.AppSettings["redis_flag"]);
        }

        /// <summary>
        /// rmi ip
        /// </summary>
        public static string RmiIp;

        /// <summary>
        /// rmi port
        /// </summary>
        public static int RmiPort;

        /// <summary>
        /// 加密/解密字符串
        /// </summary>
        public static string CryptKey;

        /// <summary>
        /// 查询换热站sql
        /// </summary>
        public static string JkdSql;

        /// <summary>
        /// 线程Delay Time(毫秒)
        /// </summary>
        public static int DelayTime;

        /// <summary>
        /// Oracle数据库连接字符串
        /// </summary>
        public static string ConnectStr;

        /// <summary>
        /// redis连接config
        /// </summary>
        public static ConfigurationOptions ConOption;

        /// <summary>
        /// redis ip
        /// </summary>
        public static string RedisIp;

        /// <summary>
        /// redis 端口
        /// </summary>
        public static string RedisPort;

        /// <summary>
        /// redis 密码
        /// </summary>
        public static string RedisPw;

        /// <summary>
        /// 是否转存redis标识
        /// </summary>
        public static bool RedisFlag;
    }
}