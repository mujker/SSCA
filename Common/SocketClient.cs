using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.ClientEngine;

namespace SSCA.Common
{
    public class SocketClient
    {
        public async void TestConnectRepeat()
        {
            var client = new EasyClient();

//            client.Initialize(new FakeReceiveFilter(), (p) =>
//            {
//                // do nothing
//            });

            client.Error += (s, e) => { Console.WriteLine("Error:" + e.Exception.Message); };
            var connected =
                await client.ConnectAsync(new IPEndPoint(IPAddress.Parse(Settings.RmiIp), Settings.RmiPort));

            if (connected)
            {
                // Send data to the server
                client.Send(Encoding.ASCII.GetBytes("LOGIN kerry"));
            }
        }
    }
}