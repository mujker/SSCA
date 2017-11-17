using System;
using System.Net;
using System.Text;
using SSCA.Common;
using SuperSocket.ClientEngine;

namespace SSCA.Socket
{
    /// <summary>
    /// 获取/发送  RMI 数据类
    /// </summary>
    public class RmiSc
    {
        private EasyClient client;

        public RmiSc()
        {
            client = new EasyClient();
        }
    }
}