using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestChatApplication
{
    public class StreamSocket
    {
        public string Name { get; set; }
        public Socket socket { get; set; }
        public StreamSocket(Socket skt)
        {
            this.socket = skt;
        }
    }
}
