using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        TcpClient tcp = new TcpClient();
        NetworkStream stream;
        Thread thread;

        delegate void SetTextCallBack(string message);

        private void SetText(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                SetTextCallBack set = SetText;
                Invoke(set, new object[] { text });
            }

            else
            {
                richTextBox1.AppendText("\n" + text);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var readbytes = Encoding.ASCII.GetBytes(textBox2.Text);
            stream = tcp.GetStream();
            
            stream.Write(readbytes, 0, readbytes.Length);
            SetText(" << "+textBox2.Text);
        }

        //listening server
        private void button1_Click(object sender, EventArgs e)
        {
            
            Thread listenerThread = new Thread(new ThreadStart(Listening));
            listenerThread.Start();
        }

         void Listening()
        {
            var listener = new TcpListener(IPAddress.Any, 9000);
            listener.Start();
             SetText("Listening for clients");
             tcp = listener.AcceptTcpClient();
             if (tcp.Connected)
                {
                    SetText("client connected");
                    thread = new Thread(ReceiveData);
                    thread.Start();
                }

        }

        //connecting client
        private void button2_Click(object sender, EventArgs e)
        {
            SetText("Connecting ...");
            var a = IPAddress.Parse("127.0.0.1");
            tcp.Connect(a, 9000);
            thread = new Thread(ReceiveData);
            thread.Start();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ReceiveData()
        {
            string message = "";
            int bytesRead;
            byte[] readbuffer = new byte[1];

            stream = tcp.GetStream();
            SetText("ready to receive data ...");
            string substring = "";

            while (stream.CanRead)
            {
                bytesRead = stream.Read(readbuffer, 0, readbuffer.Length);

                if (readbuffer.Length == bytesRead)
                {
                    message = Encoding.ASCII.GetString(readbuffer, 0, bytesRead);
                }

                substring = substring + message;
            

            switch (substring)
            {
                case "bye":
                {
                    var bytesToSend = Encoding.ASCII.GetBytes("bye");
                    stream.Write(bytesToSend, 0, bytesToSend.Length);
                    SetText("Connection closed..");
                    stream.Close();
                    tcp.Close();
                    break;
                }
                default:
                SetText(message);
                    break;
            }
        }
    }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
