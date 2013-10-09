using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    delegate void SetTextCallBack(string text);

    public partial class Form1 : Form
    {
        private Client _client;
        private Server _server;

        public void SettingText(Object o,OnTextChanged e)
        {
            SetText(e.GetText());
            Debug.WriteLine("event fired................");
        }
      
        public void SetText(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                SetTextCallBack setText = SetText;
                Invoke(setText, new object[] { text });
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
            if (_client != null)
            {
                _client.Send(textBox2.Text);
            }
            else if (_server != null)
            {
                _server.Send(textBox2.Text);
            }
        }

        //listening server
        private void button1_Click(object sender, EventArgs e)
        {
            _server = new Server();
            _server.Start();
        }
        //connecting client
        private void button2_Click(object sender, EventArgs e)
        {
            _client = new Client();
            _client.Connect();
            _client.ReceiveData();
        }
    }
}
