namespace TestChatApplication
{
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

    public partial class Server : Form
    {
        /// <summary>
        /// creating an byte array for storing the incoming data
        /// </summary>
        byte[] Buffer = new byte[1024];

        /// <summary>
        /// creating the Socket for server
        /// </summary>
        public Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// storing the Client Socket information
        /// </summary>
        private List<StreamSocket> ClientSockets;
        public Server()
        {
            InitializeComponent();
            //// CheckForIllegalCrossThreadCalls = false;
            ClientSockets = new List<StreamSocket>();
        }

        /// <summary>
        /// setting up some UI requirements while loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Server_Load(object sender, EventArgs e)
        {
            try
            {
                Label.Text = "Please Wait while Setting up the Server...";
                //// Binding the Address to the Server Socket Object
                ServerSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1111));

                //// Starts listening to the request after binding
                ServerSocket.Listen(1);
                Label.Text = "Server Listening";

                //// Accepts requests
                ServerSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Standard Methods gets called when accepting request starts
        /// </summary>
        /// <param name="result"></param>
        private void AcceptCallBack(IAsyncResult result)
        {
            //// creating the socket to hold the client sockets information
            var ClientSocket = ServerSocket.EndAccept(result);

            //// addng the socket to the client Socket list
            ClientSockets.Add(new StreamSocket(ClientSocket));

            //// client socket starts recieving the request after the connection establishes
            ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), ClientSocket);

            //// after Recieving the data again starts to accept new data
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        /// <summary>
        /// Standard method gets called by while recieving the data
        /// </summary>
        /// <param name="result"></param>
        private void RecieveCallBack(IAsyncResult result)
        {
            var ClientSocket = (Socket)result.AsyncState;/// gettin error here

            //// proceeding if the connection is established
            if (ClientSocket.Connected)
            {
                //// storing the size of the data recieved
                int RecievedSize = ClientSocket.EndReceive(result);

                //// if recieved size not equal to zero than data is not recieved
                if (RecievedSize != 0)
                {
                    var TempBuffer = new Byte[RecievedSize];
                    Array.Copy(Buffer, TempBuffer, RecievedSize);
                    var Text = Encoding.ASCII.GetString(TempBuffer);
                    string ClientNames = "@";

                    //// if the string starts with the @ then it contains the new connection details
                    if (Text.StartsWith("@"))
                    {
                        Text = Text.TrimStart('@');
                        for (int i = 0; i < ClientSockets.Count; i++)
                        {
                            if (ClientSockets[i].socket.RemoteEndPoint.ToString().Equals(ClientSocket.RemoteEndPoint.ToString()))
                            {
                                //// if the connection is new the socket will not have the name so giving the socket its name here
                                if (ClientSockets[i].Name == null)
                                {
                                    ClientSockets[i].Name = Text;
                                    ClientListBox.Items.Add(ClientSockets[i].Name);
                                }
                            }
                            ClientNames += ClientSockets[i].Name + "#";
                        }
                        ClientNames = ClientNames.TrimEnd('#');
                        //// if te clientNames.length has more then 2 charactors then proceeding
                        if (ClientNames.Length > 2)
                        {
                            var ClientListBuffer = Encoding.ASCII.GetBytes(ClientNames);
                            foreach (var socket in ClientSockets)
                            {
                                //// sending the clientList that are connected to the Server Socket to every Client Sockets
                                socket.socket.BeginSend(ClientListBuffer, 0, ClientListBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket.socket);
                                ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), ClientSocket);
                            }
                            return;
                        }
                    }

                    //// recieved data string starts with # means it contains the Message
                    Text = Text.TrimStart('#');
                    var Sender = ClientSockets.Where(q => q.socket.RemoteEndPoint.ToString().Equals(ClientSocket.RemoteEndPoint.ToString())).FirstOrDefault().Name;
                    var To = Text.Substring(0, Text.IndexOf('#'));
                    var Message = Text.Substring(Text.IndexOf('#') + 1);
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
        /// <summary>
        /// Standard method which gets called while sendin the message
        /// </summary>
        /// <param name="result"></param>
        private void SendCallBack(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;
            socket.EndSend(result);
        }

        /// <summary>
        /// will send the Message to the clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSend_Click(object sender, EventArgs e)
        {
            //// creating an empty array of type string
            var NamesArray = new String[ClientListBox.Items.Count];
            //// Copyng the Names from the ListBox to the Array
            ClientListBox.Items.CopyTo(NamesArray, 0);
            //// Converting the Array to List so that we can work with linq
            var ListNames = NamesArray.ToList();
            //// Converting string to Bytes array
            var TempBuffer = Encoding.ASCII.GetBytes("Server : " + MessagetextBox.Text);
            //// using linq to iterate the socket list and names to send message to matched sockets
            ClientSockets.Where(q => ListNames.Any(y => y.Equals(q.Name))).ToList().ForEach(socket =>
            socket.socket.BeginSend(TempBuffer, 0, TempBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket.socket));
        }

        /// <summary>
        /// this will be called when sending the Message
        /// </summary>
        /// <param name="result"></param>
        private void SendCallback(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;
            socket.EndSend(result);
        }

    }
}
