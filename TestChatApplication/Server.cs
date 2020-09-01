using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestChatApplication
{
    public partial class Server : Form
    {
        byte[] Buffer = new byte[1024];
        public Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private List<StreamSocket> ClientSockets;
        public Server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            ClientSockets = new List<StreamSocket>();
        }

        private void Server_Load(object sender, EventArgs e)
        {
            try
            {
                Label.Text = "Please Wait while Setting up the Server...";
                ServerSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"),1111));
                ServerSocket.Listen(1);
                Label.Text = "Server Listening";
                ServerSocket.BeginAccept(new AsyncCallback(AcceptCallBack),null);
            }
            catch(Exception ex)
            {

            }
        }
        private void AcceptCallBack(IAsyncResult result)
        {
            var ClientSocket = ServerSocket.EndAccept(result);
            ClientSockets.Add(new StreamSocket(ClientSocket));

            ClientSocket.BeginReceive(Buffer,0,Buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), ClientSocket);
            
            //// after Recieving the data again starts to accept new data
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }
        private void RecieveCallBack(IAsyncResult result)
        {
            var ClientSocket = (Socket)result.AsyncState;/// gettin error here
           if(ClientSocket.Connected)
            {
                int RecievedSize = ClientSocket.EndReceive(result);

                //// if recieved size not equal to zero than data is recieved
                if (RecievedSize != 0)
                {
                    var TempBuffer = new Byte[RecievedSize];
                    Array.Copy(Buffer, TempBuffer, RecievedSize);
                    var Text = Encoding.ASCII.GetString(TempBuffer);
                    string ClientNames = "@";
                    if (Text.StartsWith("@"))
                    {
                        Text=Text.TrimStart('@');
                        for (int i = 0; i < ClientSockets.Count; i++)
                        {
                            if (ClientSockets[i].socket.RemoteEndPoint.ToString().Equals(ClientSocket.RemoteEndPoint.ToString()))
                            {
                                if (ClientSockets[i].Name == null)
                                {
                                    ClientSockets[i].Name = Text;
                                    ClientListBox.Items.Add(ClientSockets[i].Name);
                                }
                            }
                            ClientNames += ClientSockets[i].Name + "#";
                        }
                        ClientNames=ClientNames.TrimEnd('#');
                        if (ClientNames.Length > 2)
                        {
                            var ClientListBuffer = Encoding.ASCII.GetBytes(ClientNames);
                            foreach (var socket in ClientSockets)
                            {
                                socket.socket.BeginSend(ClientListBuffer, 0, ClientListBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket.socket);
                                ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), ClientSocket);
                            }
                            return;
                        }
                    }
                    Text=Text.TrimStart('#');
                    var Sender = ClientSockets.Where(q => q.socket.RemoteEndPoint.ToString().Equals(ClientSocket.RemoteEndPoint.ToString())).FirstOrDefault().Name;
                    var To = Text.Substring(0, Text.IndexOf('#'));
                    var Message = Text.Substring(Text.IndexOf('#')+1);
                    MessagelistBox.Items.Add(Sender + " sent Message : " + Message + " to :" + To);
                    foreach (var socket in ClientSockets)
                    {
                        if (socket.Name.Equals(To))
                        {
                            TempBuffer = Encoding.ASCII.GetBytes(Sender + "#" + Message);
                            socket.socket.BeginSend(TempBuffer, 0, TempBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket.socket);
                            ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), ClientSocket);
                        }
                    }
                }
            }
        }
       private void SendCallBack(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;
            socket.EndSend(result);
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            var arrayNames = new String[ClientListBox.Items.Count];
            ClientListBox.Items.CopyTo(arrayNames,0);
            var ListNames=arrayNames.ToList();
            var TempBuffer = Encoding.ASCII.GetBytes("Server : "+MessagetextBox.Text);
            ClientSockets.Where(q => ListNames.Any(y => y.Equals(q.Name))).ToList().ForEach(socket =>
            socket.socket.BeginSend(TempBuffer, 0, TempBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket.socket));
        }
        private void SendCallback(IAsyncResult result)
        {
            var socket=(Socket)result.AsyncState;
            socket.EndSend(result);
        }

    }
}
