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
                    string get = decrypt(recievedMessage);
                    listConvo.Items.Add("Friend (encrypted): " + recievedMessage);
                    listConvo.Items.Add("Friend (decrypted): " + get);
                }

                byte[] buffer = new byte[1500];
                suck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref eRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        private void sendMessage()
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];

                string pass = encrypt(txtType.Text);
                msg = enc.GetBytes(pass);

                suck.Send(msg);
                
                listConvo.Items.Add("You (encrypted): " + pass);
                listConvo.Items.Add("You (decrypted): " + txtType.Text);
                txtType.Clear();
            }
            catch (Exception x)
            {
                //MessageBox.Show(x.ToString());
            }
        }

        private void txtType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendMessage();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                eLocal = new IPEndPoint(IPAddress.Parse(txtOneIP.Text), Convert.ToInt32(txtOnePort.Text));
                suck.Bind(eLocal);

                eRemote = new IPEndPoint(IPAddress.Parse(txtTwoIP.Text), Convert.ToInt32(txtTwoPort.Text));
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
                //MessageBox.Show(x.ToString());
            }
        }

        private string encrypt(string message)
        {
            string encrypted = "";
            int length = message.Length;
            char[] encrypted_message = new char[length];
            char c;
            bool flag = true;
            int key = 2;

            for (int i = 0; i < length; i++)
            {
                c = message[i];

                if (c >= 'a' && c <= 'z')
                {
                    c = (char)(c + key);
                    if (c > 'z')
                    {
                        c = (char)(c - 'z' + 'a' - 1);
                    }

                    encrypted_message[i] = c;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    c = (char)(c + key);
                    if (c > 'Z')
                    {
                        c = (char)(c - 'Z' + 'A' - 1);
                    }

                    encrypted_message[i] = c;
                }
                else if (c == ' ')
                {
                    c = (char)(c + key);
                    encrypted_message[i] = c;
                }
                else
                {
                    MessageBox.Show("No numbers or symbols allowed!", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    flag = false;
                    break;
                }
            }
            if (flag == true)
            {
                encrypted = new String(encrypted_message);
            }
            else
            {
                encrypted = "";
            }

            return encrypted;
        }

        private string decrypt(string message)
        {
            string decrypted = "";
            int length = message.Length;
            char[] decrypted_message = new char[length];
            char c;
            int key = 2;

            for (int i = 0; i < length; i++)
            {
                c = message[i];
                c = (char)(c - key);

                if (!(Char.IsLetter(c)) && !(c.Equals(' ')))
                {
                    c = (char)(c + 26);
                }
                decrypted_message[i] = c;
            }

            decrypted = new String(decrypted_message);

            return decrypted;
        }

        private string hash(string s)
        {
            string hashed = "";
            return hashed;
        }
    }
}
