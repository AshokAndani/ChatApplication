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

namespace Client
{
    public partial class Client : Form
    {
        Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byte[] recivedBuffer = new byte[1024];
        HashSet<string> ClientNames;
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            ClientNames = new HashSet<string>();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Label.Text = "Please wait while connecting to the server....";
            ClientSocket.Connect(IPAddress.Parse("127.0.0.1"), 1111);
            Label.Text = "Connected to Server....!";
            ClientSocket.BeginReceive(recivedBuffer, 0, recivedBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), ClientSocket);
            var TempBuffer = Encoding.ASCII.GetBytes("@" + NameTextBox.Text);
            ClientSocket.Send(TempBuffer);

        }
        private void ReceiveCallBack(IAsyncResult result)
        {
            try
            {
                var socket = (Socket)result.AsyncState;
                int Recieved = socket.EndReceive(result);
                var TempBuffer = new Byte[Recieved];
                Array.Copy(recivedBuffer, TempBuffer, Recieved);
                string Message = Encoding.ASCII.GetString(TempBuffer);
                if (Message.StartsWith("@"))
                {
                    Message=Message.TrimStart('@');
                    Message.Split('#').ToList().ForEach(q => ClientNames.Add(q));
                    checkedListBox.Items.Clear();
                    foreach(var name in ClientNames)
                    {
                        checkedListBox.Items.Add(name);
                    }
                    ClientSocket.BeginReceive(recivedBuffer, 0, recivedBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), ClientSocket);
                    return;
                }
                Message=Message.TrimStart('#') ; 
                MessageListBox.Items.Add(Message.Substring(0,Message.IndexOf("#"))+" Sent : "+Message.Substring(Message.IndexOf("#")+1));
                ClientSocket.BeginReceive(recivedBuffer, 0, recivedBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), ClientSocket) ;
                
            }
            catch (Exception ex)
            {

            }
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            string Content = "#" + checkedListBox.SelectedItem + "#" + MessageTextBox.Text;
            var TempBuffer = Encoding.ASCII.GetBytes(Content);
            ClientSocket.Send(TempBuffer);
            MessageListBox.Items.Add("Me : " + MessageTextBox.Text);
            MessageListBox.Items.Add(Environment.NewLine);
            MessageTextBox.Text = String.Empty;

        }
    }
}
