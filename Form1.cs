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
            var listener = new TcpListener(IPAddress.Any, 9000);
            listener.Start();
            SetText("Listening for a client ...");
            tcp = listener.AcceptTcpClient();
            if (tcp.Connected)
            {
                thread = new Thread(ReceiveData);
                thread.Start();
            }
        }

        //connecting client
        private void button2_Click(object sender, EventArgs e)
        {
            SetText("Connecting ...");
            var a = IPAddress.Parse(textBox1.Text);
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
            string message;
            int bytesRead;
            var readbuffer = new byte[1024];

            stream = tcp.GetStream();
            SetText("ready to receive data ...");

            while (true)
            {
                bytesRead = stream.Read(readbuffer, 0, readbuffer.Length);
                message = Encoding.ASCII.GetString(readbuffer, 0, bytesRead);

                switch (message)
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
                        SetText(" >> " + message);
                        stream.Close();
                        break;
                }
            }  
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
