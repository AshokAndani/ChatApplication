namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Policy;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class Client : Form
    {
        /// <summary>
        /// Creating the Socket which represents Client Socekt
        /// </summary>
        Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// an Buffer Array of bytes to store the incoming Data
        /// </summary>
        byte[] recivedBuffer = new byte[1024];

        /// <summary>
        /// to store other Client Socket Names 
        /// </summary>
        HashSet<string> ClientNames;
        public Client()
        {
            InitializeComponent();
            ClientNames = new HashSet<string>();
        }

        /// <summary>
        /// this method connects the client socket with the Server Socket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            Label.Text = "Please wait while connecting to the server....";

            //// Connecting to the Server Socket by specifying the its Address
            ClientSocket.Connect(IPAddress.Parse("127.0.0.1"), 1111);
            Label.Text = "Connected to Server....!";

            //// after the Connection Beginning to recieve the data from the other Socket
            ClientSocket.BeginReceive(recivedBuffer, 0, recivedBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), ClientSocket);

            //// Taking the Name of the user to bind it with the socket
            var TempBuffer = Encoding.ASCII.GetBytes("@" + NameTextBox.Text);

            //// Now sending the Data to the Server Socket
            ClientSocket.Send(TempBuffer);
        }

        /// <summary>
        /// Standard method which is called when a socket sends the data
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveCallBack(IAsyncResult result)
        {
            try
            {
                var socket = (Socket)result.AsyncState;

                //// taking the recieved data size
                int Recieved = socket.EndReceive(result);
                var TempBuffer = new Byte[Recieved];

                //// Copying the data to a new array
                Array.Copy(recivedBuffer, TempBuffer, Recieved);

                //// converting bytes to string format
                string Message = Encoding.ASCII.GetString(TempBuffer);

                //// if the string starts with the @ then it is a new connection
                if (Message.StartsWith("@"))
                {
                    //// Extracting the Name and adding it to the ClientList
                    Message = Message.TrimStart('@');
                    Message.Split('#').ToList().ForEach(q => ClientNames.Add(q));
                    checkedListBox.Items.Clear();

                    //// to display the clients added to the list
                    foreach (var name in ClientNames)
                    {
                        checkedListBox.Items.Add(name);
                    }

                    //// starts to recieve the data again
                    ClientSocket.BeginReceive(recivedBuffer, 0, recivedBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), ClientSocket);
                    return;
                }

                //// false in if condition which means the data contains the text message
                Message = Message.TrimStart('#');
                MessageListBox.Items.Add(Message.Substring(0, Message.IndexOf("#")) + " Sent : " + Message.Substring(Message.IndexOf("#") + 1));
                ClientSocket.BeginReceive(recivedBuffer, 0, recivedBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), ClientSocket);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// this methos is to send the Message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            //// Making the string for sending
            string Content = "#" + checkedListBox.SelectedItem + "#" + MessageTextBox.Text;
            var TempBuffer = Encoding.ASCII.GetBytes(Content);

            //// Sending the Message
            ClientSocket.Send(TempBuffer);
            MessageListBox.Items.Add("Me : " + MessageTextBox.Text);
            MessageListBox.Items.Add(Environment.NewLine);
            MessageTextBox.Text = String.Empty;
        }
    }
}
