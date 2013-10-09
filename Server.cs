using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WindowsFormsApplication2
{
    public delegate void OnTextChangedHandler(object source, OnTextChanged e);

    public class OnTextChanged : EventArgs
    {
        private readonly string _text;

        public OnTextChanged(string text)
        {
            this._text = text;
        }

        public string GetText()
        {
            return this._text;
        }
    }

    class Server
    {
        public event OnTextChangedHandler OnTextChangedEvent;

        readonly TcpListener ServerSocket = new TcpListener(IPAddress.Any, 9000);
        TcpClient clientSocket = default(TcpClient);
        static List<TcpClient>   clients = new List<TcpClient>();
        int counter = 0;
        private string clNo;

        public void Listening()
        {
            ServerSocket.Start();
            Debug.WriteLine("Listening");
            if (this.OnTextChangedEvent != null) OnTextChangedEvent(this, new OnTextChanged("Server Started listening...."));
            counter = 0;
            while (true)
            {
                counter += 1;
                clientSocket = ServerSocket.AcceptTcpClient();
                clients.Add(clientSocket);
                HandleClient(clientSocket, Convert.ToString(counter)); 
            }
        }

        public void HandleClient(TcpClient inClientSocket, string clineNo)
        {
            clNo = clineNo;
            Thread ctThread = new Thread(ReceiveData);
            ctThread.Start();
        }
        private void ReceiveData()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    Debug.WriteLine(" >> " + "From client-" + clNo + dataFromClient);
                    if (this.OnTextChangedEvent != null) OnTextChangedEvent(this, new OnTextChanged("Client " + clNo + " says: " + dataFromClient));
                    Send(dataFromClient, clientSocket);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(" >> " + ex.ToString());
                }
            }
        }



        public void Start()
        {
            var t = new Thread(Listening);
            t.Start();
          
        }

        public void Send(String message,TcpClient currentTcpClient= null)
        {
            Debug.WriteLine("Sending to clients");
            foreach (var client in clients)
            {
                if (client.Connected && client != currentTcpClient)
                {
                    var stringtosend = message;
                    var bytestosend = Encoding.ASCII.GetBytes(stringtosend);
                    var thisStream = client.GetStream();
                    thisStream.Write(bytestosend, 0, bytestosend.Length);
                    //thisStream.Flush();
                    //thisStream.Close();
                }
            }
        }
    }
}
