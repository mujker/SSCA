using System;
using System.Configuration;

//using StackExchange.Redis;

namespace SSCA.Common
{
    public class Settings
    {
        /// <summary>
        /// rmi ip
        /// </summary>
        public static string RmiIp = ConfigurationManager.AppSettings["rmi-ip"];

        /// <summary>
        /// rmi port
        /// </summary>
        public static int RmiPort = Convert.ToInt32(ConfigurationManager.AppSettings["rmi-port"]);

        /// <summary>
        /// 加密/解密字符串
        /// </summary>
        public static string CryptKey = ConfigurationManager.AppSettings["CryptKey"];

        /// <summary>
        /// 查询换热站sql
        /// </summary>
        public static string JkdSql = ConfigurationManager.AppSettings["JkdSql"];

        /// <summary>
        /// redis连接config
        /// </summary>
        //        public static ConfigurationOptions ConOption = new ConfigurationOptions()
        //        {
        //            EndPoints =
        //            {
        //                { ConfigurationManager.AppSettings["redis_ip"], Convert.ToInt32(ConfigurationManager.AppSettings["redis_port"]) }
        //            },
        //            Password = ConfigurationManager.AppSettings["redis_pw"]
        //        };
    }
}