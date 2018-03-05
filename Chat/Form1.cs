using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Chat
{
    public partial class Form1 : Form
    {
        Socket suck;
        EndPoint eLocal, eRemote;

        public Form1()
        {
            InitializeComponent();
            suck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            suck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            txtOneIP.Text = GetLocalIP();
            txtTwoIP.Text = GetLocalIP();

            btnSend.Enabled = false;
        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach(IPAddress ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }

        private void MessageCallBack(IAsyncResult result)
        {
            try
            {
                int size = suck.EndReceiveFrom(result, ref eRemote);

                if (size > 0)
                {
                    byte[] receiveData = new Byte[1464];
                    receiveData = (byte[])result.AsyncState;

                    ASCIIEncoding e = new ASCIIEncoding();
                    string recievedMessage = e.GetString(receiveData);
                    listConvo.Items.Add("Friend: " + recievedMessage);
                }

                byte[] buffer = new byte[1500];
                suck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref eRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
            }
            catch
            {

            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                eLocal = new IPEndPoint(IPAddress.Parse(txtOneIP.Text), Convert.ToInt32(txtOnePort));
                suck.Connect(eLocal);

                eRemote = new IPEndPoint(IPAddress.Parse(txtTwoIP.Text), Convert.ToInt32(txtTwoPort));
                suck.Connect(eRemote);

                byte[] buffer = new byte[1500];
                suck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref eRemote, new AsyncCallback(MessageCallBack), buffer);

                btnStart.Text = "Connected";
                btnStart.Enabled = false;
                btnSend.Enabled = true;
                txtType.Focus();
            }
            catch(Exception x)
            {
                MessageBox.Show(x.ToString());
            }
        }
    }
}
