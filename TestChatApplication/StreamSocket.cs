namespace TestChatApplication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// class to bind the socket with a Name
    /// </summary>
    public class StreamSocket
    {
        /// <summary>
        /// name for the socket
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// stream socket
        /// </summary>
        public Socket socket { get; set; }
        public StreamSocket(Socket skt)
        {
            this.socket = skt;
        }
    }
}
